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


		#region Members
		DataServices.BlockDataService _blockDataService;
		DataServices.NPTDataService _nptDataService;

		//block
		Action<object> _actionLoadBlock;//load blocks within the modified range as ActionData (<object>)
		Action<object> _actionUpdateBlock;
		Action<object> _actionDeleteBlock;

		//npt
		Action<object> _actionLoadNpt;
		Action<object> _actionUpdateNpt;
		Action<object> _actionDeleteNpt;

		//worktimes
		Action<object> _actionLoadWorkTimes;
		Action<object> _actionDeleteWorkTimes;

		//range
		object _rangeLock = new object();
		TimeSpan _rangeMargin = TimeSpan.FromHours(3);//additional margin to range
		DateTime _start = DateTime.MinValue;//last value AutoFetchRange taken (after margin is added)
		DateTime _end = DateTime.MinValue;

		//cache
		object _cacheLock = new object();
		int _stations = 0;
		List<PPItemBlock>[] _blocks;
		List<PPItemNpt> _npts;
		List<PPItemWorkTime> _shifts;

		#endregion



		#region Ctor
		public PPItemManager(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			IsAutoRefresh = true;
			_instances.Add(this);

			//initialize dataservices
			_blockDataService = new DataServices.BlockDataService();
			_nptDataService = new DataServices.NPTDataService();

			_stations = new DataServices.StationDataService().GetActives().Max(x => x.Index);
			_blocks = new List<PPItemBlock>[_stations];
			for (int i = 0; i < _stations; i++)
			{
				_blocks[i] = new List<PPItemBlock>();
			}
			_npts = new List<PPItemNpt>();
			_shifts = new List<PPItemWorkTime>();

			//initialize qTimer and actions
			initializeActions();

			//start the thread
			Pause = false;
		}
		#endregion

		#region Thread
		
		/// <summary>
		/// Gets or sets a value that indicates whether PPItemManager keep all items updated when idle
		/// </summary>
		public bool IsAutoRefresh { get; set; }

		//private members
		Thread _qThread;
		bool _qThreadAlive = false;
		bool _pause = true;
		bool _isIdle = true;
		static List<PPItemManager> _instances = new List<PPItemManager>();
		Dispatcher _dispatcher;

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
		void _threadStart()
		{
			while (_qThreadAlive)
			{
				//control the thread
				Thread.Sleep(200);
				while (Pause) Thread.Sleep(200);

				//retreive thread-unsafe values
				PPRange range;
				bool isIdle;
				lock(_rangeLock)
				{
					isIdle = _isIdle;
					range = new PPRange(_start, _end);
					//apply margin
					if (range.IsValidRange())
						range.ApplyMargin(_rangeMargin);
				}

				//decision making
				if (!isIdle)
				{
					//Load blocks in range (add/update newer blocks)
					_actionLoadBlock.Invoke(range);
					_actionLoadNpt.Invoke(range);
				}
				else
				{
					//Idle => GC & Refresh
					_actionUpdateBlock.Invoke(range);
					_actionUpdateNpt.Invoke(range);
				}
			}
		}

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
			lock (_rangeLock)
			{
				_start = rangeStart.Add(-_rangeMargin);
				_end = rangeEnd.Add(_rangeMargin);
				_isIdle = false;
			}
		}
		/// <summary>
		/// Reloads day colors and shifts of the selected month
		/// </summary>
		/// <param name="startOfMonth"></param>
		public void FetchWorkTimes(DateTime startOfMonth)
		{
			var end = startOfMonth.AddDays(startOfMonth.GetPersianMonthDays());
			var data = new PPRange(startOfMonth, end);
			Task.Factory.StartNew(_actionLoadWorkTimes, data);
		}
		public void ForceReload()
		{
			Pause = true;

			//sync load
			var data = new PPRange(_start, _end);
			_actionLoadBlock.Invoke(data);
			_actionLoadWorkTimes.Invoke(data);
			_actionLoadNpt.Invoke(data);

			Pause = false;
		}
		#endregion

		#region Actions
		private void initializeActions()
		{
			#region Load Actions
			//-----------------------	ADD BLOCK	-------------------------------
			_actionLoadBlock = (object data) =>
			{
				var actionData = data as PPRange;
				if (!actionData.IsValidRange()) return;

				//Load blockInfos
				var blockInfos = _blockDataService.GetInRange(actionData.Start, actionData.End).ToArray();

				//add each blockInfo to cache
				foreach (var blockInfo in blockInfos)
				{
					//find b in cache
					PPItemBlock b;
					lock (_cacheLock)
						b = _blocks[blockInfo.StationIndex].FirstOrDefault(x => x.Id == blockInfo.Id);

					if (b == null)
					{
						//new block (not in cache) is found
						//create the PPItemBlock and load full data
						//set HasVm to false
						b = new PPItemBlock(blockInfo.Id);
						lock (_cacheLock)
						{
							//add to cache
							_blocks[blockInfo.StationIndex].Add(b);
						}
					}
					else
					{
						//newer version (of an existing block in cache) is found
						lock (_cacheLock)
						{
							if (b.Model.ModifiedDate != blockInfo.ModifiedDate)
							{
								//reload b
								//set HasVm to false
								b.Reload();
							}
						}
					}
					Thread.Sleep(10);
				}

				//find VIndex of each block in cache
				for (int st = 0; st < _stations; st++)
				{
					lock(_cacheLock)
					{
						if(_blocks[st].Any())
						{
							//find VIndex of station[st]
							_blocks[st].Sort((x, y) =>
							{
								return x.Start < y.Start ? 1 : -1;
							});

							int maxV = 1;
							for (int i = 0; i < _blocks[st].Count; i++)
							{
								//block (b)
								var b = _blocks[st][i];

								//find VIndex of block (b)
								if (i == 0)
								{
									b.VIndex = 0;
								}
								else
								{
									//search through previous blocks (pblocks)
									var pblocks = _blocks[st].Take(i);
									var nominates = pblocks.Where(x => b.Start >= x.End).OrderByDescending(x => x.End);
									var nom = pblocks.FirstOrDefault();

									if (nom == null)
									{
										//non of rows has space
										b.VIndex = maxV;
										maxV++;
									}
									else
									{
										//nom is the row
										b.VIndex = nom.VIndex;
									}
								}

								//fire event to add it
								if (!PPItemBlock.IsNull(b))
								{
									_dispatcher.Invoke(() =>
									{
										//set HasVm to true
										if (BlockAdded != null)
											BlockAdded(b);
									});
								}
							}
							
							//sleep if station[st] contains some block
							Thread.Sleep(50);
						}
					}
				}
			};

			//-----------------------	ADD NPT	-------------------------------
			_actionLoadNpt = (object data) =>
			{
				//retreive data
				var actionData = data as PPRange;
				if (!actionData.IsValidRange()) return;

				//find npt Ids within the range
				var nptIds = _nptDataService.GetIdsInRange(actionData.Start, actionData.End).ToArray();

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
				var actionData = data as PPRange;
				if (!actionData.IsValidRange()) return;
				var _workProfilePlanDataService = new DataServices.WorkProfilePlanDataService();

				//find workprofiles
				var workTimes = _workProfilePlanDataService.GetShiftsInRange(actionData.Start, actionData.End)
					.Where(x => x.Item1.IsOpen);

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

				//find day colors
				var dayColors = _workProfilePlanDataService.GetBusinessDayColorsInRange(actionData.Start, actionData.End).ToArray();

				//change colors of Days
				_dispatcher.Invoke(() =>
				{
					if (DayColorsUpdated != null)
						DayColorsUpdated(actionData.Start, dayColors);
				});

				_workProfilePlanDataService.Dispose();
			};
			#endregion

			#region Delete Actions
			//-----------------------	REMOVE BLOCK	-------------------------------
			_actionDeleteBlock = (object data) =>
			{
				//retreive data
				/*var actionData = data as ActionData;
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
				}*/
			};
			//-----------------------	REMOVE NPT	-------------------------------
			_actionDeleteNpt = (object data) =>
			{
				//retreive data
				var actionData = data as PPRange;
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
				var actionData = data as PPRange;
				if (!actionData.IsValidRange()) return;
				var correctedActionData = new PPRange(actionData.Start.Date.AddDays(-1), actionData.End.Date.AddDays(1));

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
				/*var actionData = data as ActionData;
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
				}*/
			};
			_actionUpdateNpt = (object data) =>
			{
				//retreive data
				var actionData = data as PPRange;
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
