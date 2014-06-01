using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Soheil.Common;
using System.Windows.Media;

namespace Soheil.Core.PP
{
	/// <summary>
	/// PPItemManger version 3.
	/// Manager for loading, refreshing and removing PPItems of PPTable
	/// <para>See AutoFetchRange method of this class for more information</para>
	/// </summary>
	public class PPItemManager : IDisposable
	{
		#region Events
		public event Action<PPItemWorkTime> WorkTimeAdded;
		public event Action<PPItemWorkTime> WorkTimeRemoved;

		public event Action<PPItemBlock> BlockAdded;
		public event Action<PPItemBlock> BlockUpdated;
		public event Action<PPItemBlock> BlockRemoved;

		public event Action<PPItemNpt> NptAdded;
		public event Action<PPItemNpt> NptUpdated;
		public event Action<PPItemNpt> NptRemoved;

		/// <summary>
		/// Occurs when day colors are updated
		/// <para>first param is starting date, seconds param is array of colors</para>
		/// </summary>
		public event Action<DateTime, Color[]> DayColorsUpdated;
		#endregion

		/// <summary>
		/// Gets or sets a value that indicates whether PPItemManager keep all items updated when idle
		/// </summary>
		public bool IsAutoRefresh { get; set; }

		#region Members
		Dal.SoheilEdmContext _uow;
		DataServices.WorkProfilePlanDataService _workProfilePlanDataService;
		DataServices.BlockDataService _blockDataService;
		DataServices.NPTDataService _nptDataService;

		Thread _qThread;
		Dispatcher _dispatcher;
		bool _qThreadAlive = false;
		bool _pause = false;

		/// <summary>
		/// Pauses or unpauses the thread
		/// </summary>
		public bool Pause
		{
			get { return _pause; }
			set
			{
				if (!_qThreadAlive)
				{
					if (value)
					{
						if (_qThread != null)
							if (_qThread.IsAlive)
								_qThread.Abort();
					}
					else
					{
						_qThreadAlive = true;
						_qThread = new Thread(_threadStart);
						_qThread.IsBackground = true;
						_qThread.Start();
					}
				}
				_pause = value;
			}
		}

		Stack<Task> _qLoad;
		Action<object> _actionLoadWorkTimes;
		Action<object> _actionLoadBlock;//load blocks within the modified range as ActionData (<object>)
		Action<object> _actionLoadNpt;
		private object _lockObject;//used for load day colors
		Action<object> _actionLoadDayColors;

		Action<object> _actionDeleteWorkTimes;
		Action<object> _actionDeleteBlock;
		Action<object> _actionDeleteNpt;

		Action<object> _actionUpdateBlock;
		Action<object> _actionUpdateNpt;

		//_last: last values FetchRange taken (used for delete and update actions)
		//_lastPerf: last values FetchRange performed upon (used to determine needs for load actions)
		//_perfMargin: min required ticks between a dt and its _lastPerf to perform upon that dt
		const long _perfMargin = 50000000000;//1hours and 32minutes
		DateTime _lastStart = DateTime.MinValue;
		DateTime _lastEnd = DateTime.MinValue;
		DateTime _lastPerfStart = DateTime.MinValue;
		DateTime _lastPerfEnd = DateTime.MinValue;

		List<PPItemBlock> _blocks;
		List<PPItemNpt> _npts;
		List<PPItemWorkTime> _shifts;

		#endregion

		public PPItemManager(Dispatcher dispatcher)
		{
			_lockObject = new Object();
			_dispatcher = dispatcher;
			IsAutoRefresh = true;
			_instances.Add(this);

			_uow = new Dal.SoheilEdmContext();
			_workProfilePlanDataService = new DataServices.WorkProfilePlanDataService(_uow);
			_blockDataService = new DataServices.BlockDataService(_uow);
			_nptDataService = new DataServices.NPTDataService(_uow);

			_blocks = new List<PPItemBlock>();
			_npts = new List<PPItemNpt>();
			_shifts = new List<PPItemWorkTime>();

			//initialize qTimer and actions
			initializeActions();
		}

		public void ForceReload()
		{

		}

		void _threadStart()
		{
			while (_qThreadAlive)
			{
				Thread.Sleep(200);

				while (!Pause && _qThreadAlive)
				{
					while (true)
					{
						Thread.Sleep(10);

						lock (this)
							if (!_qLoad.Any()) break;

						//load
						var task = _qLoad.Pop();
						task.Start();
						task.Wait();
					}

					Thread.Sleep(50);

					//update/delete
					if (IsAutoRefresh)
					{
						Task.Factory.StartNew(_actionUpdateBlock, new ActionData(_lastStart, _lastEnd)).Wait();
						Thread.Sleep(50);
						Task.Factory.StartNew(_actionUpdateNpt, new ActionData(_lastStart, _lastEnd)).Wait();
						Thread.Sleep(50);
					}

					Task.Factory.StartNew(_actionDeleteBlock, new ActionData(_lastStart, _lastEnd)).Wait();
					Thread.Sleep(50);
					Task.Factory.StartNew(_actionDeleteNpt, new ActionData(_lastStart, _lastEnd)).Wait();
					Thread.Sleep(50);
					Task.Factory.StartNew(_actionDeleteWorkTimes, new ActionData(_lastStart, _lastEnd)).Wait();
					Thread.Sleep(50);
				}
			}
		}

		#region Exit
		static List<PPItemManager> _instances = new List<PPItemManager>();
		public static void Abort()
		{
			while(_instances.Any())
			{
				_instances.FirstOrDefault().Dispose();
			}
		}
		public void Dispose()
		{
			_qThreadAlive = false;
			_instances.Remove(this);
			_qLoad.Clear();
			if (_qThread != null)
			{
				int counter = 0;
				while (_qThread.IsAlive)
				{
					Thread.Sleep(20);
					if (counter++ > 100) Pause = true;
				}
			}
		} 
		#endregion

		#region Methods
		/// <summary>
		/// Automatically Reloads or deletes blocks, Npts and workShifts within the specified range
		/// <para>Any item that partially fall into the range is included</para>
		/// <para>This methods pushes actions to queues so that qTimer can perform them</para>
		/// <para>Results will be notified by firing the events of this class</para>
		/// </summary>
		/// <param name="rangeStart">start of the timeline range</param>
		/// <param name="rangeEnd">end of the timeline range</param>
		public void AutoFetchRange(DateTime rangeStart, DateTime rangeEnd)
		{
			_lastStart = rangeStart;
			_lastEnd = rangeEnd;

			//findout how the range is changed
			bool reloadStart = false;
			bool reloadEnd = false;

			//if it's simply zoomed
			if (rangeStart == _lastPerfStart && rangeEnd != _lastPerfEnd)
			{
				//zoom out from a fixed start
				if (rangeEnd > _lastPerfEnd.AddTicks(_perfMargin))
				{
					reloadEnd = true;
				}
			}
			else//if it's moved
			{
				//zoom out from both sides
				if (rangeStart.AddTicks(_perfMargin) < _lastPerfStart && rangeEnd > _lastPerfEnd.AddTicks(_perfMargin))
				{
					reloadStart = true;
					reloadEnd = true;
				}
				//moved forward but still overlapping with last range
				else if (rangeStart <= _lastPerfEnd && _lastPerfEnd < rangeEnd)
				{
					if (_lastPerfEnd.AddTicks(_perfMargin) < rangeEnd)
					{
						reloadEnd = true;
					}
				}
				//moved backward but still overlapping with last range
				else if (rangeStart < _lastPerfStart && _lastPerfStart < rangeEnd)
				{
					if (rangeStart.AddTicks(_perfMargin) < _lastPerfStart)
					{
						reloadStart = true;
					}
				}
				//change datetime
				else if (Math.Abs(rangeStart.Ticks - _lastPerfStart.Ticks) > _perfMargin 
					|| Math.Abs(rangeEnd.Ticks - _lastPerfEnd.Ticks) > _perfMargin)
				{
					_lastPerfStart = rangeStart;
					_lastPerfEnd = rangeEnd;
					fetchRange(rangeStart, rangeEnd);
				}
			}

			//then enqueue actions
			if (reloadStart)
			{
				_lastPerfStart = rangeStart;
				fetchRange(rangeStart, _lastPerfStart);
			}
			if (reloadEnd)
			{
				_lastPerfEnd = rangeEnd;
				fetchRange(_lastPerfEnd, rangeEnd);
			}
		}
		public void FetchDayColorsOfMonth(DateTime startOfMonth)
		{
			var end = startOfMonth.AddDays(startOfMonth.GetPersianMonthDays());

			var data = new ActionData(startOfMonth, end);
			var loadDayColors = new Task(_actionLoadDayColors, data);
			lock (this)
				_qLoad.Push(loadDayColors);
		}
		private void fetchRange(DateTime start, DateTime end)
		{
			var data = new ActionData(start, end);
			var loadWorkTimes = new Task(_actionLoadWorkTimes, data);
			lock (this)
				_qLoad.Push(loadWorkTimes);
			var loadBlocks = new Task(_actionLoadBlock, data);
			lock (this)
				_qLoad.Push(loadBlocks);
			var loadNpts = new Task(_actionLoadNpt, data);
			lock (this)
				_qLoad.Push(loadNpts);
		}
		#endregion

		#region Actions
		private void initializeActions()
		{
			#region Queue Timer
			_qLoad = new Stack<Task>();
			//start the thread
			Pause = false;
			#endregion

			#region Load Actions
			//-----------------------	ADD BLOCK	-------------------------------
			_actionLoadBlock = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find blocks in range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var blockIds = _blockDataService.GetIdsInRange(start, end).ToArray();

				//add each block to view model
				foreach (var blockId in blockIds)
				{
					//keep updated with _lastBlockIds
					lock (this) { if (_blocks.Any(x => x.Id == blockId)) continue; }

					//create the PPItemBlock and load full data
					var fullData = new PPItemBlock(blockId);

					//fire event to add it
					if (!PPItemBlock.IsNull(fullData))
					{
						_dispatcher.Invoke(() =>
						{
							if (BlockAdded != null)
								BlockAdded(fullData);
						});

						//add the PPItemBlock
						lock (this) { _blocks.Add(fullData); }
					}
				}
			};

			//-----------------------	ADD NPT	-------------------------------
			_actionLoadNpt = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find npt Ids within the range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var nptIds = _nptDataService.GetIdsInRange(start, end).ToArray();

				//add each npt to view model
				foreach (var nptId in nptIds)
				{
					//keep updated with _lastNptIds
					if (_npts.Any(x => x.Id == nptId)) continue;

					//create the PPItemNpt
					var item = new PPItemNpt(nptId);

					//fire event to add it
					_dispatcher.Invoke(() =>
					{
						if (NptAdded != null)
							NptAdded(item);
					});

					//add the PPItemNpt
					lock (this) { _npts.Add(item); }
				}
			};

			//-----------------------	ADD WORK TIME	-------------------------------
			_actionLoadWorkTimes = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find npt Ids within the range
				var start = actionData.Start.Date.AddDays(-1);
				var end = actionData.End.Date.AddDays(1);
				var workTimes = _workProfilePlanDataService.GetShiftsInRange(start, end).Where(x => x.Item1.IsOpen);

				foreach (var tuple in workTimes)
				{
					//keep updated with _shiftsDates
					if (_shifts.Any(x => x.Id == tuple.Item1.Id && x.DayStart == tuple.Item2)) continue;

					//create PPItemWorkTime
					var shiftItem = new PPItemWorkTime(tuple.Item1, tuple.Item2);

					//add PPItemWorkTime
					lock (this) { _shifts.Add(shiftItem); }

					//fire event to add it and its breaks
					_dispatcher.Invoke(() =>
					{
						if (WorkTimeAdded != null)
							WorkTimeAdded(shiftItem);
					});
				}
			};

			DateTime lastMonthUpdated = DateTime.MinValue;
			//-----------------------	ADD DAY COLOR	-------------------------------
			_actionLoadDayColors = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;
				lock (_lockObject)
				{
					if (actionData.Start == lastMonthUpdated) return;
					else lastMonthUpdated = actionData.Start;
				}
				
				//read colors from data service
				var dayColors = _workProfilePlanDataService.GetBusinessDayColorsInRange(actionData.Start, actionData.End).ToArray();

				//change colors of Days
				_dispatcher.Invoke(() =>
				{
					if (DayColorsUpdated != null)
						DayColorsUpdated(actionData.Start, dayColors);
				});
			};
			#endregion

			#region Delete Actions
			//-----------------------	REMOVE BLOCK	-------------------------------
			_actionDeleteBlock = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find outside blocks
				PPItemBlock[] outsideItems;
				lock (this) { outsideItems = _blocks.Where(x => !actionData.IsInRange(x)).ToArray(); }
				foreach (var item in outsideItems)
				{
					//remove from list
					lock (this) { _blocks.Remove(item); }

					//remove from Vm
					_dispatcher.Invoke(() =>
					{
						if (BlockRemoved != null)
						BlockRemoved(item);
					});
				}
			};
			//-----------------------	REMOVE NPT	-------------------------------
			_actionDeleteNpt = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find outside NPTs
				PPItemNpt[] outsideItems;
				lock (this) { outsideItems = _npts.Where(x => !actionData.IsInRange(x)).ToArray(); }
				foreach (var item in outsideItems)
				{
					//remove from list
					lock (this) { _npts.Remove(item); }

					//remove from Vm
					_dispatcher.Invoke(() =>
					{
						if (NptRemoved != null)
						NptRemoved(item);
					});
				}
			};
			//-----------------------	REMOVE WORK TIME	-------------------------------
			_actionDeleteWorkTimes = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;
				var correctedActionData = new ActionData(actionData.Start.Date.AddDays(-1), actionData.End.Date.AddDays(1));

				//find outside worktimes
				PPItemWorkTime[] outsideItems;
				lock (this) { outsideItems = _shifts.Where(x => !correctedActionData.IsInRange(x)).ToArray(); }
				foreach (var item in outsideItems)
				{
					//remove from list
					lock (this) { _shifts.Remove(item); }

					//remove from Vm
					_dispatcher.Invoke(() =>
					{
						if (WorkTimeRemoved != null)
						WorkTimeRemoved(item);
					});
				}
			};
			#endregion

			#region Update Action
			_actionUpdateBlock = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find blocks in range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var blockIds = _blockDataService.GetIdsInRange(start, end).ToArray();

				//remove garbage
				PPItemBlock[] tmp_blocks;
				lock (this) { tmp_blocks = _blocks.ToArray(); }
				foreach (var block in tmp_blocks)
				{
					//-----------------------	REMOVE BLOCK	-------------------------------
					if (!actionData.IsInRange(block))
					{
						//remove from list
						lock (this) { _blocks.Remove(block); }

						//remove from Vm
						_dispatcher.Invoke(() =>
						{
							if (BlockRemoved != null)
								BlockRemoved(block);
						});
					}
				}

				//add each block to view model
				foreach (var blockId in blockIds)
				{
					//check if it's an update or an add
					PPItemBlock block;
					lock (this) { block = _blocks.FirstOrDefault(x => x.Id == blockId); }

					//-----------------------	ADD BLOCK	-------------------------------
					if (block == null)
					{
						//create the PPItemBlock and load full data
						var fullData = new PPItemBlock(blockId);

						//fire event to add it
						if (!PPItemBlock.IsNull(fullData))
						{
							_dispatcher.Invoke(() =>
							{
								if (BlockAdded != null)
									BlockAdded(fullData);
							});

							//add the PPItemBlock
							lock (this) { _blocks.Add(fullData); }
						}
					}
					else//-----------------------	UPDATE BLOCK	-------------------------------
					{
						//create the PPItemBlock and load full data
						var fullData = new PPItemBlock(blockId);

						//fire event to update it
						_dispatcher.Invoke(() =>
						{
							if (BlockUpdated != null)
								BlockUpdated(fullData);
						});

						//update the PPItemBlock
						lock (this)
						{
							int idx = _blocks.IndexOf(block);
							if (idx != -1) _blocks[idx] = fullData;
						}
					}
				}
			};
			_actionUpdateNpt = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				if (!actionData.IsValidRange()) return;

				//find blocks in range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var nptIds = _nptDataService.GetIdsInRange(start, end).ToArray();

				//remove garbage
				PPItemNpt[] tmp_npts;
				lock (this) { tmp_npts = _npts.ToArray(); }
				foreach (var npt in tmp_npts)
				{
					//-----------------------	REMOVE NPT	-------------------------------
					if (!actionData.IsInRange(npt))
					{
						//remove from list
						lock (this) { _npts.Remove(npt); }

						//remove from Vm
						_dispatcher.Invoke(() =>
						{
							if (NptRemoved != null)
								NptRemoved(npt);
						});
					}
				}

				//add each npt to view model
				foreach (var nptId in nptIds)
				{
					//check if it's an update or an add
					PPItemNpt npt;
					lock (this) { npt = _npts.FirstOrDefault(x => x.Id == nptId); }

					//-----------------------	ADD NPT	-------------------------------
					if (npt == null)
					{
						//create the PPItemNpt
						var item = new PPItemNpt(nptId);

						//fire event to add it
						_dispatcher.Invoke(() =>
						{
							if (NptAdded != null)
								NptAdded(item);
						});

						//add the PPItemNpt
						lock (this) { _npts.Add(item); }
					}
					else//-----------------------	UPDATE NPT	-------------------------------
					{
						//create the PPItemNpt
						var item = new PPItemNpt(nptId);

						//fire event to update it
						_dispatcher.Invoke(() =>
						{
							if (NptUpdated != null)
								NptUpdated(item);
						});

						//update the PPItemNpt
						lock (this)
						{
							int idx = _npts.IndexOf(npt);
							if (idx != -1) _npts[idx] = item;
						}
					}
				}
			};
			#endregion
		}
		#endregion

	}
}
