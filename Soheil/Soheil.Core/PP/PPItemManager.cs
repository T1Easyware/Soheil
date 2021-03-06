﻿using System;
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
		public event Action WorkTimesRemoved;

		/// <summary>
		/// Occurs when a block is added or updated
		/// </summary>
		public event Action<PPItemBlock> BlockAddedOrUpdated;
		public event Action<PPItemBlock> BlockRemoved;

		public event Action<PPItemNpt> NptAddedOrUpdated;
		public event Action<PPItemNpt> NptRemoved;

		/// <summary>
		/// Occurs when day colors are updated
		/// <para>first param is starting date, seconds param is array of colors</para>
		/// </summary>
		public event Action<DateTime, Color[]> DayColorsUpdated;
		#endregion


		#region Members
		//range
		object _rangeLock = new object();
		TimeSpan _rangeMargin = TimeSpan.FromHours(3);//additional margin to range
		DateTime _start = DateTime.MinValue;//last value AutoFetchRange taken (after margin is added)
		DateTime _end = DateTime.MinValue;

		//actions
		Action<object> _actionLoadItem;//load blocks/Npts within the modified range (which is given as (object)PPRange in parameter)
		Action<object> _actionLoadWorkTimes;

		//cache
		object _cacheLock = new object();
		int _stations = 0;
		List<PPItemBlock>[] _blocks;
		List<PPItemNpt>[] _npts;
		List<PPItemWorkTime> _shifts;

		#endregion



		#region Ctor
		public PPItemManager(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			IsAutoRefresh = true;
			_instances.Add(this);

			//initialize cache
			var stations = new DataServices.StationDataService().GetActives();
			if (stations.Any())
				_stations = stations.Max(x => x.Index)+1;
			else
				_stations = 0;
			_blocks = new List<PPItemBlock>[_stations];
			_npts = new List<PPItemNpt>[_stations];
			for (int i = 0; i < _stations; i++)
			{
				_blocks[i] = new List<PPItemBlock>();
				_npts[i] = new List<PPItemNpt>();
			}
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
		bool _force = false;
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
				while (Pause)
				{
					if (!_qThreadAlive) return;
					Thread.Sleep(200);
				}

				//retreive thread-unsafe values
				PPRange range;
				lock(_rangeLock)
				{
					range = new PPRange(_start, _end);
					//apply margin
					if (range.IsValidRange())
						range.ApplyMargin(_rangeMargin);
				}

				//Load blocks/Npts in range (add/update newer blocks)
				_actionLoadItem.Invoke(range);
				Thread.Sleep(500);
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
			}
		}
		/// <summary>
		/// Reloads day colors and shifts of the selected month
		/// <para>Also removes other items</para>
		/// </summary>
		/// <param name="startOfMonth"></param>
		public void InitMonth(DateTime startOfMonth)
		{
			//time range
			var end = startOfMonth.AddDays(startOfMonth.GetPersianMonthDays());
			var data = new PPRange(startOfMonth, end);

			//remove other items

			//add days and shifts
			Task.Factory.StartNew(_actionLoadWorkTimes, data);
		}
		public void ForceReload()
		{
			_qThreadAlive = false;
			Pause = true;
			while (_qThread.IsAlive) ;

			//reset
			for (int i = 0; i < _stations; i++)
			{
				_blocks[i].Clear();
				_npts[i].Clear();
			}

			//sync load
			_force = true;
			var data = new PPRange(_start, _end);
			Task.Factory.StartNew(_actionLoadItem, data);
		}
		#endregion

		private void initializeActions()
		{
			#region Items(Blocks/NPTs) actions
			//-----------------------	ADD BLOCK	-------------------------------
			_actionLoadItem = (object data) =>
			{
				try
				{
					var actionData = data as PPRange;
					if (!actionData.IsValidRange()) return;

					#region Load blockInfos
					var blockInfos = new DataServices.BlockDataService()
						.GetInRange(actionData.Start, actionData.End).ToArray();

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
						if (!_force && !_qThreadAlive) return;
						Thread.Sleep(10);
					}
					#endregion

					#region Load NptInfos
					//find npt Ids within the range
					var nptInfos = new DataServices.NPTDataService()
						.GetInRange(actionData.Start, actionData.End).ToArray();

					//add each nptInfo to cache
					foreach (var nptInfo in nptInfos)
					{
						//find n in cache
						PPItemNpt n;
						lock (_cacheLock)
							n = _npts[nptInfo.StationIndex].FirstOrDefault(x => x.Id == nptInfo.Id);

						if (n == null)
						{
							//new npt (not in cache) is found
							//create the PPItemNPT and load full data
							//set HasVm to false
							n = new PPItemNpt(nptInfo.Id);
							lock (_cacheLock)
							{
								//add to cache
								_npts[nptInfo.StationIndex].Add(n);
							}
						}
						else
						{
							//newer version (of an existing block in cache) is found
							//check for newer version???
						}

						if (!_force && !_qThreadAlive) return;
						Thread.Sleep(5);
					}
					#endregion

					#region find VIndex of each item in cache
					for (int st = 0; st < _stations; st++)
					{
						lock (_cacheLock)
						{
							if (_blocks[st].Any() || _npts[st].Any())
							{
								//find VIndexes for station[st] (row)
								var row = (_blocks[st].Concat<PPItemBase>(_npts[st]))
									.OrderBy(x => x.Start)
									.ToArray();

								int maxV = 1;
								for (int i = 0; i < row.Length; i++)
								{
									//block (b)
									var item = row[i];

									//find VIndex of block (b)
									if (i == 0)
									{
										item.VIndex = 0;
									}
									else
									{
										//search through previous blocks
										var targetRow = row
											.Take(i)
											.GroupBy(g => g.VIndex)
											.FirstOrDefault(g => g
												.All(x => (x.End - item.Start < TimeSpan.FromSeconds(1))));

										if (targetRow == null)
										{
											//non of rows has space
											item.VIndex = maxV;
											maxV++;
										}
										else
										{
											//nom is the row
											item.VIndex = targetRow.First().VIndex;
										}
									}

									//fire event to add it
									if (!PPItemBase.IsNull(item))
									{
										if (item is PPItemBlock)
											_dispatcher.Invoke(() =>
											{
												//set HasVm to true
												if (BlockAddedOrUpdated != null)
													BlockAddedOrUpdated(item as PPItemBlock);
											});
										else if (item is PPItemNpt)
											//fire event to add it
											_dispatcher.Invoke(() =>
											{
												if (NptAddedOrUpdated != null)
													NptAddedOrUpdated(item as PPItemNpt);
											});
									}
								}

								//sleep if station[st] contains some block
								if (!_force && !_qThreadAlive) return;
								Thread.Sleep(10);
							}
						}
					}
					#endregion

					#region Delete outside Items
					//widen the range
					var actionData2 = new PPRange(actionData.Start.Add(-_rangeMargin), actionData.End.Add(_rangeMargin));
					for (int st = 0; st < _stations; st++)
					{
						//find outside blocks
						PPItemBlock[] outsideBlocks;
						lock (this)
						{
							outsideBlocks = _blocks[st].Where(x => !actionData2.IsInRange(x) || x.Id == 0).ToArray();
						}
						foreach (var item in outsideBlocks)
						{
							//remove from list
							lock (this) { _blocks[st].Remove(item); }

							//remove from Vm
							_dispatcher.Invoke(() =>
							{
								if (BlockRemoved != null)
									BlockRemoved(item);
							});
						}

						if (!_force && !_qThreadAlive) return;
						Thread.Sleep(5);

						//find outside NPTs
						PPItemNpt[] outsideNpts;
						lock (this) { outsideNpts = _npts[st].Where(x => !actionData2.IsInRange(x)).ToArray(); }
						foreach (var item in outsideNpts)
						{
							//remove from list
							lock (this) { _npts[st].Remove(item); }

							//remove from Vm
							_dispatcher.Invoke(() =>
							{
								if (NptRemoved != null)
									NptRemoved(item);
							});
						}

						if (!_force && !_qThreadAlive) return;
						Thread.Sleep(5);
					}
					#endregion
				}
				catch (Exception e)
				{
					if (!LoginInfo.IsDebugMode) return;

					_qThreadAlive = false;
					using (var log = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Soheil"), "Exceptions.log"), append: true))
					{
						log.WriteLine(DateTime.Now.ToShortTimeString());
						log.Write("Ex = ");
						log.WriteLine(e.Message);
						if (e.InnerException != null)
						{
							log.Write("InnerEx = ");
							log.WriteLine(e.InnerException.Message);
						}
						log.Write("Source = ");
						log.WriteLine(e.Source);
						log.Write("Trace = ");
						log.WriteLine(e.StackTrace);
						log.WriteLine();
						log.Close();
					}
					System.Windows.MessageBox.Show("Fatal error.\nfor more info see %appdata%\\Soheil\\Exceptions.Log");
				}
				finally
				{
					if(_force)
					{
						_force = false;
						Pause = false;
					}
				}
			};
			#endregion

			#region WorkTimes actions
			//-----------------------	ADD WORK TIME	-------------------------------
			_actionLoadWorkTimes = (object data) =>
			{
				//retreive data
				var actionData = data as PPRange;
				if (!actionData.IsValidRange()) return;
				var _workProfilePlanDataService = new DataServices.WorkProfilePlanDataService();

				lock (this)
				{
					//remove all from Vm
					_dispatcher.Invoke(() =>
					{
						if (WorkTimesRemoved != null)
							WorkTimesRemoved();
					});
					_shifts.Clear();
				}


				//find workprofiles
				var workTimes = _workProfilePlanDataService.GetShiftsInRange(actionData.Start, actionData.End)
					.Where(x => x.Item1.IsOpen);

				foreach (var tuple in workTimes)
				{
					//keep updated with _shiftsDates
					//if (_shifts.Any(x => x.Id == tuple.Item1.Id && x.DayStart == tuple.Item2)) continue;

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
		}

	}
}
