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
		public event Action<PPItemBlock> BlockRemoved;

		public event Action<PPItemNpt> NptAdded;
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

		Stack<Task> _qLoad;
		Action<object> _actionLoadWorkTimes;
		Action<object> _actionLoadBlock;
		Action<object> _actionLoadNpt;
		Action<object> _actionLoadDayColors;

		Stack<Task> _qDelete;
		Action<object> _actionDeleteWorkTimes;
		Action<object> _actionDeleteBlock;
		Action<object> _actionDeleteNpt;

		Action<object> _actionUpdate;

		//_lastPerf: last values FetchRange performed upon
		//_perfMargin: min required ticks between a dt and its _lastPerf to perform upon that dt
		const long _perfMargin = 50000000000;//1hours and 32minutes
		DateTime _lastPerfStart = DateTime.MinValue;
		DateTime _lastPerfEnd = DateTime.MinValue;

		List<PPItemBlock> _blocks = new List<PPItemBlock>();
		List<PPItemNpt> _npts = new List<PPItemNpt>();
		List<PPItemWorkTime> _shifts = new List<PPItemWorkTime>();

		private object _lockObject;
		/// <summary>
		/// Number of milliseconds to sleep when loading each item
		/// </summary>
		private const int _workerSleepTime = 40; 
		#endregion

		public PPItemManager(Dispatcher dispatcher)
		{
			_lockObject = new Object();
			_dispatcher = dispatcher;

			_uow = new Dal.SoheilEdmContext();
			_workProfilePlanDataService = new DataServices.WorkProfilePlanDataService(_uow);
			_blockDataService = new DataServices.BlockDataService(_uow);
			_nptDataService = new DataServices.NPTDataService(_uow);

			//initialize qTimer and actions
			initializeActions();
		}

		public void ForceReload()
		{

		}

		public void Dispose()
		{
			_qLoad.Clear();
			_qDelete.Clear();
			_qThread.ForceQuit();
		}

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
			//findout how the range is changed
			bool reloadStart = false;
			bool deleteStart = false;
			bool reloadEnd = false;
			bool deleteEnd = false;

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
					if (rangeStart > _lastPerfStart.AddTicks(_perfMargin))
					{
						deleteStart = true;
					}
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
					if (_lastPerfEnd > rangeEnd.AddTicks(_perfMargin))
					{
						deleteEnd = true;
					}
				}
				//change datetime
				else if (Math.Abs(rangeStart.Ticks - _lastPerfStart.Ticks) > _perfMargin 
					|| Math.Abs(rangeEnd.Ticks - _lastPerfEnd.Ticks) > _perfMargin)
				{
					deleteRange(_lastPerfStart, _lastPerfEnd);
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
			if (deleteStart)
			{
				_lastPerfStart = rangeStart;
				deleteRange(DateTime.MinValue, rangeStart);
			}
			if (deleteEnd)
			{
				_lastPerfEnd = rangeEnd;
				deleteRange(rangeEnd, DateTime.MaxValue);
			}
		}
		public void FetchDayColorsOfMonth(DateTime startOfMonth)
		{
			var end = startOfMonth.AddDays(startOfMonth.GetPersianMonthDays());

			var data = new ActionData(startOfMonth, end);
			var loadDayColors = new Task(_actionLoadDayColors, data);
			_qLoad.Push(loadDayColors);
		}
		private void fetchRange(DateTime start, DateTime end)
		{
			var data = new ActionData(start, end);
			var loadWorkTimes = new Task(_actionLoadWorkTimes, data);
			_qLoad.Push(loadWorkTimes);
			var loadBlocks = new Task(_actionLoadBlock, data);
			_qLoad.Push(loadBlocks);
			var loadNpts = new Task(_actionLoadNpt, data);
			_qLoad.Push(loadNpts);
		}
		private void deleteRange(DateTime start, DateTime end)
		{
			var data = new ActionData(start, end);
			var deleteWorkTimes = new Task(_actionDeleteWorkTimes, data);
			_qLoad.Push(deleteWorkTimes);
			var deleteBlocks = new Task(_actionDeleteBlock, data);
			_qLoad.Push(deleteBlocks);
			var deleteNpts = new Task(_actionDeleteNpt, data);
			_qLoad.Push(deleteNpts);
		}

		private void initializeActions()
		{
			#region Queue Timer
			_qLoad = new Stack<Task>();
			_qDelete = new Stack<Task>();

			_qThread = new Thread(o =>
			{
				while (true)
				{
					Thread.Sleep(1000);
					while (_qLoad.Any())
					{
						var task = _qLoad.Pop();
						task.Start();
						task.Wait();
					}
					while (_qDelete.Any())
					{
						var task = _qDelete.Pop();
						task.Start();
						task.Wait();
					}
					while (IsAutoRefresh && !_qLoad.Any() && !_qDelete.Any())
					{
						var task = new Task(_actionUpdate, null);
						task.Start();
						task.Wait();
					}
				}
			});
			_qThread.IsBackground = true;
			_qThread.Start();
			#endregion

			#region Load Actions
			//load blocks within the modified range
			_actionLoadBlock = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find blocks in range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var blockIds = _blockDataService.GetIdsInRange(start, end).ToArray();

				//add each block to view model
				foreach (var blockId in blockIds)
				{
					//keep updated with _lastBlockIds
					if (_blocks.Any(x => x.Id == blockId)) continue;

					//create and add the view model
					//load full data
					var fullData = new PPItemBlock(blockId);
					//fire event to add it
					if (!PPItemBlock.IsNull(fullData))
					{
						_blocks.Add(fullData);
						_dispatcher.Invoke(() =>
						{
							if (BlockAdded != null)
								BlockAdded(fullData);
						});
					}
				}
			};

			_actionLoadNpt = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find npt Ids within the range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var nptIds = _nptDataService.GetIdsInRange(start, end).ToArray();

				//add each npt to view model
				foreach (var nptId in nptIds)
				{
					//keep updated with _lastNptIds
					if (_npts.Any(x => x.Id == nptId)) continue;

					//create and add the view model
					var item = new PPItemNpt(nptId);
					_npts.Add(item);
					//fire event to add it
					_dispatcher.Invoke(() =>
					{
						if (NptAdded != null)
							NptAdded(item);
					});
				}
			};

			_actionLoadWorkTimes = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find npt Ids within the range
				var start = actionData.Start.AddSeconds(-1);
				var end = actionData.End.AddSeconds(1);
				var workTimes = _workProfilePlanDataService.GetShiftsInRange(start, end);

				foreach (var tuple in workTimes)
				{
					//keep updated with _shiftsDates
					if (_shifts.Any(x => x.Id == tuple.Item1.Id && x.DayStart == tuple.Item2)) continue;

					//create and add view model
					var shiftItem = new PPItemWorkTime(tuple.Item1, tuple.Item2);
					_shifts.Add(shiftItem);
					//fire event to add it and its breaks

					_dispatcher.Invoke(() =>
					{
						if (WorkTimeAdded != null)
							WorkTimeAdded(shiftItem);
					});
				}
			};

			_actionLoadDayColors = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;
				
				Color[] colors;
				int length = actionData.Start.GetPersianMonthDays();
				colors = new Color[length];

				var dayColors = _workProfilePlanDataService.GetBusinessDayColorsInRange(actionData.Start, actionData.End);
				for (int i = 0; i < dayColors.Count; i++)
				{
					colors[i] = dayColors[i];
				}

				_dispatcher.Invoke(() =>
				{
					if (DayColorsUpdated != null)
						DayColorsUpdated(actionData.Start, colors);
				});
			};
			#endregion

			#region Delete Actions
			_actionDeleteBlock = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find outside blocks
				var outsideItems = _blocks.Where(x => actionData.IsInRange(x.Start, x.End)).ToArray();
				foreach (var item in outsideItems)
				{
					//remove from list
					_blocks.Remove(item);
					//remove from Vm
					_dispatcher.Invoke(() =>
					{
						if (BlockRemoved != null)
						BlockRemoved(item);
					});
				}
			};
			_actionDeleteNpt = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find outside NPTs
				var outsideItems = _npts.Where(x => actionData.IsInRange(x.Start, x.End)).ToArray();
				foreach (var item in outsideItems)
				{
					//remove from list
					_npts.Remove(item);
					//remove from Vm
					_dispatcher.Invoke(() =>
					{
						if (NptRemoved != null)
						NptRemoved(item);
					});
				}
			};
			_actionDeleteWorkTimes = (object data) =>
			{
				//retreive data
				var actionData = data as ActionData;

				//find outside worktimes
				var outsideItems = _shifts.Where(x => actionData.IsInRange(x.Start, x.End)).ToArray();
				foreach (var item in outsideItems)
				{
					//remove from list
					_shifts.Remove(item);
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
			_actionUpdate = (object data) =>
			{
				_dispatcher.Invoke(() =>
				{
				});
			};
			#endregion
		}

		#endregion

	}
}
