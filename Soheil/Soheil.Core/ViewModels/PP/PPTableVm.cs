using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.PP.Timeline;
using Soheil.Core.ViewModels.PP.Editor;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for PPTable
	/// </summary>
	public class PPTableVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; private set; }
		private static readonly object _LOCK = new object();

		#region DataServices
		/// <summary>
		/// Gets BlockDataService (runs using local UOW)
		/// </summary>
		public DataServices.BlockDataService BlockDataService { get; private set; }
		/// <summary>
		/// Gets NPTDataService (runs using local UOW)
		/// </summary>
		public DataServices.NPTDataService NPTDataService { get; private set; }
		/// <summary>
		/// Gets TaskDataService (runs using local UOW)
		/// </summary>
		public DataServices.TaskDataService TaskDataService { get; private set; }
		/// <summary>
		/// Gets JobDataService (runs using local UOW)
		/// </summary>
		public DataServices.JobDataService JobDataService { get; private set; }
		/// <summary>
		/// Gets TaskReportDataService (runs using local UOW)
		/// </summary>
		public DataServices.TaskReportDataService TaskReportDataService { get; private set; }
		/// <summary>
		/// Gets ProcessReportDataService (runs using local UOW)
		/// </summary>
		public DataServices.ProcessReportDataService ProcessReportDataService { get; private set; }
		/// <summary>
		/// Gets Unit of Work for this View Model (local UOW)
		/// </summary>
		public Dal.SoheilEdmContext UOW { get; private set; } 
		#endregion

		#region Ctor, Init and Load
		/// <summary>
		/// Instantiates and Initializes a new Instance of PPTable with given access level
		/// </summary>
		/// <param name="access">Access level used for the this instance of PPTable</param>
		public PPTableVm(AccessType access)
		{
			Access = access;
			initializeCommands();
			initializeDataServices();
			initializeEditors();
		}
		/// <summary>
		/// Instantiates UOW and DataServies and add their EventHandlers
		/// </summary>
		void initializeDataServices()
		{
			UOW = new Dal.SoheilEdmContext();

			BlockDataService = new DataServices.BlockDataService(UOW);
			NPTDataService = new DataServices.NPTDataService(UOW);
			TaskDataService = new DataServices.TaskDataService(UOW);
			JobDataService = new DataServices.JobDataService(UOW);
			TaskReportDataService = new DataServices.TaskReportDataService(UOW);
			ProcessReportDataService = new DataServices.ProcessReportDataService(UOW);
		}
		/// <summary>
		/// Initializes TaskEditor, JobEditor and JobList
		/// </summary>
		void initializeEditors()
		{
			TaskEditor = new PPTaskEditorVm();
			TaskEditor.RefreshPPItems += () => UpdateRange(true);
			/*refresh is enough.
			 * TaskEditor.BlockAdded += model => PPItems.AddItem(model);
			TaskEditor.BlockUpdated += (oldModel, newModel) =>
			{
				PPItems.RemoveItem(oldModel);
				var newVm = PPItems.AddItem(newModel);
				if (SelectedBlock.Id == oldModel.Id)
					SelectedBlock = newVm;
			};*/

			JobEditor = new PPJobEditorVm();
			TaskEditor.RefreshPPItems += () => UpdateRange(true);
			/*refresh is enough.
			 *JobEditor.JobAdded += model =>
			{
				var jobVm = new PPJobVm(model);
				foreach (var block in model.Blocks)
				{
					var blockVm = PPItems.FindItem(block);
					if (blockVm != null) blockVm.Job = jobVm;
					else PPItems.AddItem(block);
				}
			};*/

			JobList = new JobListVm(JobDataService);
			JobList.JobSelected += job =>
			{
				if (job != null)
				{
					//prepare to focus new job
					if (job.BlocksCount > 0)
					{
						SelectedJobId = job.Id;
						//form now on any newly created block will check its jobId with this
						ZoomToRange(job.StartDT, job.StartDT.AddDays(1));
					}
					else
					{
						//if selected job contains no blocks, zooms to its ReleaseDate
						ZoomToRange(job.ReleaseDT, job.ReleaseDT.AddDays(1));
					}
					//when refresh completed, job will be selected automatically
					UpdateRange(true);
				}
				else
				{
					//If no job is selected set Id to -10
					SelectedJobId = -10;
				}
			};
		}

		/// <summary>
		/// This timer runs only (once) when timeline is changed
		/// </summary>
		System.Threading.Timer _initialTimer;
		/// <summary>
		/// A constant value (10) for Intervals of _initialTimer
		/// </summary>
		const int _initialTimerInterval = 10;
		/// <summary>
		/// Resets the timeline to Now, loads PPItems in range and loads Stations
		/// <para>This method is using _initialTimer so its body will be run with a 10ms delay</para>
		/// </summary>
		public void ResetTimeLine()
		{
			if (_initialTimer != null) _initialTimer.Dispose();
			_initialTimer = new System.Threading.Timer(new System.Threading.TimerCallback
				(o => Dispatcher.Invoke(() =>
				{
					//initialize DateTimes
					var currentDate = Arash.PersianDate.Today.ToDateTime();
					var startDate = currentDate.GetNorooz();

					//initialize Timeline components
					Days = new DayCollection();
					Months = new MonthCollection(startDate);
					Months.SelectedMonthChanged += month => SelectedMonth = month;
					Hours = new HourCollection();

					//initialize PPItems
					PPItems = new PPItemCollection(this);
					PPItems.BlockRemoved += id =>
					{
						if (SelectedBlock != null && SelectedBlock.Id == id)
							SelectedBlock = null;
						BlockDataService.DeleteModelById(id);
					};
					PPItems.NptRemoved += id =>
					{
						if (SelectedNPT != null && SelectedNPT.Id == id)
							SelectedNPT = null;
					};
					PPItems.JobRemoved += jobVm =>
					{
						JobDataService.DeleteModel(jobVm.Id);
						RemoveBlocks(jobVm);
					};
					PPItems.TaskEditorUpdated += ppeblock =>
					{
						TaskEditor.BlockList.Add(ppeblock);
						TaskEditor.SelectedBlock = ppeblock;
					};
					PPItems.TaskEditorReset += () =>
					{
						TaskEditor.Reset();
						TaskEditor.IsVisible = true;
						JobEditor.IsVisible = false;
					};
					PPItems.JobEditorUpdated += jobVm =>
					{
						JobEditor.Append(jobVm);
					};
					PPItems.JobEditorReset += () =>
					{
						TaskEditor.IsVisible = false;
						JobEditor.IsVisible = true;
						JobEditor.Reset();
					};
					PPItems.EditBlockReportStarted += blockVm =>
					{
						blockVm.BlockReport = new Report.BlockReportVm(blockVm);
						SelectedBlock = blockVm;
					};


					//Initialize stations
					var stationModels = new DataServices.StationDataService(UOW).GetActives().OrderBy(x => x.Index);
					NumberOfStations = stationModels.Count();
					foreach (var stationModel in stationModels)
					{
						PPItems.Add(new PPStationVm { Text = stationModel.Name });
					}

					//Set advanced timeline components
					SelectedMonth = Months[(int)currentDate.GetPersianMonth() - 1];
					HoursPassed = currentDate.Subtract(SelectedMonth.Data).TotalHours;
					if (currentDate.Year % 4 == 3)
					{
						HoursInYear = 8784;
						DaysInYear = 366;
					}
					else
					{
						HoursInYear = 8760;
						DaysInYear = 365;
					}

					//Loads PPItems
					GoToNowCommand.Execute(null);
					BackupZoom();
				})), null, _initialTimerInterval, System.Threading.Timeout.Infinite);
		}

		/// <summary>
		/// This timer only runs when AlwaysLoadTasks is set to true
		/// </summary>
		System.Timers.Timer _periodicTimer;
		/// <summary>
		/// A constant value (5000) for Intervals of _periodicTimer
		/// </summary>
		const int _periodicTimerInterval = 5000;
		/// <summary>
		/// When set to true, _periodicTimer will safely Reload PPItems in 5s intervals
		/// </summary>
		public bool AlwaysLoadTasks
		{
			get { return (bool)GetValue(AlwaysLoadTasksProperty); }
			set { SetValue(AlwaysLoadTasksProperty, value); }
		}
		public static readonly DependencyProperty AlwaysLoadTasksProperty =
			DependencyProperty.Register("AlwaysLoadTasks", typeof(bool), typeof(PPTableVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PPTableVm)d;
				if ((bool)e.NewValue)
				{
					//Initialize the timer
					if (vm._periodicTimer == null)
					{
						vm._periodicTimer = new System.Timers.Timer(_periodicTimerInterval);

						//add an inline event handler for the timer to safely Reload PPItems
						vm._periodicTimer.Elapsed += ((a, b) =>
						{
							try { vm.Dispatcher.Invoke(() => vm.PPItems.Reload()); }
							catch { }
						});
					}
					//Start the timer
					vm._periodicTimer.Start();
				}
				else if (vm._periodicTimer != null)
				{
					//Stops the timer
					vm._periodicTimer.Stop();
				}
			}));

		/// <summary>
		/// Updates current TimeRange of items (hours, shifts, tasks...) which are visible in PPTable
		/// </summary>
		/// <remarks>
		/// This function uses a Monitor to waits 2 secs if local lock is on
		/// </remarks>
		/// <param name="loadTasksAsWell">Load PP Items (setups, tasks, ...) while loading timeline</param>
		public void UpdateRange(bool loadItemsAsWell)
		{
			if (System.Threading.Monitor.TryEnter(_LOCK, 2000))
			{
				try
				{
					//finds start and end of the visible range in PPTable
					var start = SelectedMonth.Data.AddHours(HoursPassed);
					var end = start.AddHours(GridWidth / HourZoom);

					//Loads hours items
					Hours.FetchRange(start, end);

					//Loads work profile data
					updateShiftsAndBreaks(start, end);

					//Loads PPItems
					if (loadItemsAsWell || AlwaysLoadTasks)
						PPItems.FetchRange(start, end);
				}
				finally
				{
					System.Threading.Monitor.Exit(_LOCK);
				}
			}
		}
		/// <summary>
		/// updates all shifts and breaks within specified range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		private void updateShiftsAndBreaks(DateTime start, DateTime end)
		{
			ShiftsAndBreaks.Clear();
			using (var ds = new DataServices.WorkProfilePlanDataService())
			{
				var list = ds.GetShiftsAndBreaksInRange(start, end);
				foreach (var item in list)
				{
					var vm = Soheil.Core.ViewModels.OrganizationCalendar.WorkTimeRangeVm.CreateAuto(item);
					if (vm != null)
						ShiftsAndBreaks.Add(vm);
				}
			}
		}
		#endregion

		#region Editors
		/// <summary>
		/// Gets bindable ViewModel for TaskEditor
		/// </summary>
		public PPTaskEditorVm TaskEditor
		{
			get { return (PPTaskEditorVm)GetValue(TaskEditorProperty); }
			private set { SetValue(TaskEditorProperty, value); }
		}
		public static readonly DependencyProperty TaskEditorProperty =
			DependencyProperty.Register("TaskEditor", typeof(PPTaskEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets bindable ViewModel for JobEditor
		/// </summary>
		public PPJobEditorVm JobEditor
		{
			get { return (PPJobEditorVm)GetValue(JobEditorProperty); }
			private set { SetValue(JobEditorProperty, value); }
		}
		public static readonly DependencyProperty JobEditorProperty =
			DependencyProperty.Register("JobEditor", typeof(PPJobEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets bindable ViewModel for JobList
		/// </summary>
		public JobListVm JobList
		{
			get { return (JobListVm)GetValue(JobListProperty); }
			private set { SetValue(JobListProperty, value); }
		}
		public static readonly DependencyProperty JobListProperty =
			DependencyProperty.Register("JobList", typeof(JobListVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Removes all blocks of a job from PPTable and database
		/// </summary>
		/// <param name="job"></param>
		public void RemoveBlocks(PPJobVm job)
		{
			foreach (var station in PPItems.ToList())
			{
				foreach (var block in station.Blocks.ToArray())
				{
					//check if this block in this station is part of the given Job
					if (block.Job != null && block.Job.Id == job.Id)
					{
						try
						{
							//remove the block from database and ViewModel
							BlockDataService.DeleteModelById(block.Id);
							station.Blocks.Remove(block);
						}
						catch (Soheil.Common.SoheilException.RoutedException exp)
						{
							if (exp.Target is Model.Task)
							{
								block.TaskList.First(x => x.Id == ((Task)exp.Target).Id).Message.AddEmbeddedException(exp.Message);
							}
							else// if (exp.Target is Model.Block)
							{
								block.Message.AddEmbeddedException(exp.Message);
							}
						}
						catch (Exception exp)
						{
							block.Message.AddEmbeddedException(exp.Message);
						}
					}
				}
			}
		}
		#endregion

		#region TimeRoll and Shifts
		/// <summary>
		/// Gets a bindable collection of <see cref="OrganizationCalendar.WorkTimeRangeVm"/>s
		/// </summary>
		public ObservableCollection<OrganizationCalendar.WorkTimeRangeVm> ShiftsAndBreaks { get { return _shiftsAndBreaks; } }
		private ObservableCollection<OrganizationCalendar.WorkTimeRangeVm> _shiftsAndBreaks = new ObservableCollection<OrganizationCalendar.WorkTimeRangeVm>();

		/// <summary>
		/// Gets a bindable collection for timeline hours
		/// </summary>
		public HourCollection Hours
		{
			get { return (HourCollection)GetValue(HoursProperty); }
			protected set { SetValue(HoursProperty, value); }
		}
		public static readonly DependencyProperty HoursProperty =
			DependencyProperty.Register("Hours", typeof(HourCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable collection for timeline days
		/// </summary>
		public DayCollection Days
		{
			get { return (DayCollection)GetValue(DaysProperty); }
			protected set { SetValue(DaysProperty, value); }
		}
		public static readonly DependencyProperty DaysProperty =
			DependencyProperty.Register("Days", typeof(DayCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable collection for timeline months
		/// </summary>
		public MonthCollection Months
		{
			get { return (MonthCollection)GetValue(MonthsProperty); }
			protected set { SetValue(MonthsProperty, value); }
		}
		public static readonly DependencyProperty MonthsProperty =
			DependencyProperty.Register("Months", typeof(MonthCollection), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets bindable Hours Passed (measured from the edge of the screen)
		/// </summary>
		public double HoursPassed
		{
			get { return (double)GetValue(HoursPassedProperty); }
			set { SetValue(HoursPassedProperty, value); }
		}
		public static readonly DependencyProperty HoursPassedProperty =
			DependencyProperty.Register("HoursPassed", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(0d));
		/// <summary>
		/// Gets a bindable value indicating the Width of one month
		/// <para>GridWidth divided by the Number of days in SelectedMonth</para>
		/// <para>Default value = 40</para>
		/// </summary>
		public double DayZoom
		{
			get { return (double)GetValue(DayZoomProperty); }
			protected set { SetValue(DayZoomProperty, value); }
		}
		public static readonly DependencyProperty DayZoomProperty =
			DependencyProperty.Register("DayZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(40d));
		/// <summary>
		/// Gets bindable Width of OneHour in HoursBar or in pptable
		/// </summary>
		public double HourZoom
		{
			get { return (double)GetValue(HourZoomProperty); }
			protected set { SetValue(HourZoomProperty, value); }
		}
		public static readonly DependencyProperty HourZoomProperty =
			DependencyProperty.Register("HourZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(100d, (d, e) => { ((PPTableVm)d).UpdateRange(false); }));
		/// <summary>
		/// Gets bindable Number of days in currect active year
		/// </summary>
		public int DaysInYear
		{
			get { return (int)GetValue(DaysInYearProperty); }
			protected set { SetValue(DaysInYearProperty, value); }
		}
		public static readonly DependencyProperty DaysInYearProperty =
			DependencyProperty.Register("DaysInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(365));
		/// <summary>
		/// Gets bindable Total number of hours in current active year
		/// </summary>
		public int HoursInYear
		{
			get { return (int)GetValue(HoursInYearProperty); }
			protected set { SetValue(HoursInYearProperty, value); }
		}
		public static readonly DependencyProperty HoursInYearProperty =
			DependencyProperty.Register("HoursInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(8760));
		/// <summary>
		/// Gets or sets a bindable value for Selected Month that when changed updates the timeline
		/// </summary>
		public MonthSlideItemVm SelectedMonth
		{
			get { return (MonthSlideItemVm)GetValue(SelectedMonthProperty); }
			set { SetValue(SelectedMonthProperty, value); }
		}
		public static readonly DependencyProperty SelectedMonthProperty =
			DependencyProperty.Register("SelectedMonth", typeof(MonthSlideItemVm), typeof(PPTableVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PPTableVm)d;
				var val = (MonthSlideItemVm)e.NewValue;
				if (val == null) return;
				if(!val.IsSelected) val.IsSelected = true;

				//reloads the days collection according to DateTime of selected month
				vm.Days.Reload(val.Data);

				//changes the DayZoom to match the number of days in the current month
				vm.DayZoom = vm.GridWidth / val.NumOfDays;
				vm.UpdateRange(true);
			}));

		#endregion

		#region Zoom and Pan
		/// <summary>
		/// Gets or sets Width of screen
		/// </summary>
		public double GridWidth { get; set; }
		/// <summary>
		/// Backup value for HoursPassed (to restore to)
		/// </summary>
		private double _hoursPassedBackup = 0;
		/// <summary>
		/// Backup value for HourZoom (to restore to)
		/// </summary>
		private double _hourZoomBackup = 36;
		/// <summary>
		/// Backup value for VerticalScreenOffset (to restore to)
		/// </summary>
		private double _verticalScreenOffsetBackup = 0;

		/// <summary>
		/// Sets the DayZoom according to SelectedMonth
		/// </summary>
		public void UpdateWidths()
		{
			if (SelectedMonth != null)
				DayZoom = GridWidth / SelectedMonth.NumOfDays;
		}
		/// <summary>
		/// Gets or sets a bindable value indicating the vertical offset of the PPTable's scrollviewer
		/// </summary>
		public double VerticalScreenOffset
		{
			get { return (double)GetValue(VerticalScreenOffsetProperty); }
			set { SetValue(VerticalScreenOffsetProperty, value); }
		}
		public static readonly DependencyProperty VerticalScreenOffsetProperty =
			DependencyProperty.Register("VerticalScreenOffset", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(0d));

		/// <summary>
		/// Zooms to fit the given block in screen
		/// </summary>
		/// <param name="blockVm"></param>
		public void ZoomToBlock(BlockVm blockVm)
		{
			BackupZoom();

			var start = blockVm.StartDateTime;
			HoursPassed = start.GetPersianDayOfMonth() * 24 + start.Hour + start.Minute / 60d + start.Second / 3600d - 24;

			var tmp = (GridWidth * 3600) / blockVm.DurationSeconds;
			if (tmp < 20) HourZoom = 0;
			else if (tmp > 2000) HourZoom = 2000;
			else HourZoom = tmp;
		}
		/// <summary>
		/// Zooms to fit the given range in screen
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void ZoomToRange(DateTime start, DateTime end)
		{
			BackupZoom();

			HoursPassed = start.GetPersianDayOfMonth() * 24 + start.Hour + start.Minute / 60d + start.Second / 3600d - 24;

			var tmp = (GridWidth * 3600) / end.Subtract(start).TotalSeconds;
			if (tmp < 20) HourZoom = 0;
			else if (tmp > 2000) HourZoom = 2000;
			else HourZoom = tmp;
		}
		/// <summary>
		/// Backs up values of HoursPassed, HourZoom and VerticalScreenOffset into their backup values
		/// </summary>
		public void BackupZoom()
		{
			_hoursPassedBackup = HoursPassed;
			_hourZoomBackup = HourZoom;
			_verticalScreenOffsetBackup = VerticalScreenOffset;
		}
		/// <summary>
		/// Restores values of HoursPassed, HourZoom and VerticalScreenOffset to their backup values
		/// </summary>
		public void RestoreZoom()
		{
			HourZoom = _hourZoomBackup;
			HoursPassed = _hoursPassedBackup;
			VerticalScreenOffset = _verticalScreenOffsetBackup;
		}
		#endregion

		#region Stations and their items
		/// <summary>
		/// Gets bindable Number of active stations in PPTable
		/// </summary>
		public int NumberOfStations
		{
			get { return (int)GetValue(NumberOfStationsProperty); }
			protected set { SetValue(NumberOfStationsProperty, value); }
		}
		public static readonly DependencyProperty NumberOfStationsProperty =
			DependencyProperty.Register("NumberOfStations", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets a bindable collection of Stations, in each a collection of PPTable Items that are visible
		/// <para>Each item can be either an NPT or a Block</para>
		/// </summary>
		public PPItemCollection PPItems
		{
			get { return (PPItemCollection)GetValue(PPItemsProperty); }
			protected set { SetValue(PPItemsProperty, value); }
		}
		public static readonly DependencyProperty PPItemsProperty =
			DependencyProperty.Register("PPItems", typeof(PPItemCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		#endregion

		#region Report and Selected items
		/// <summary>
		/// Gets or sets the Id of Selected Job (a normal property)
		/// </summary>
		public int SelectedJobId { get; set; }

		/// <summary>
		/// Gets or sets the Selected Block
		/// <para>sets the ViewMode of previous and current SelectedBlock</para>
		/// <para>Restores zoom if null and Zooms to block if otherwise</para>
		/// </summary>
		public BlockVm SelectedBlock
		{
			get { return (BlockVm)GetValue(SelectedBlockProperty); }
			set { SetValue(SelectedBlockProperty, value); }
		}
		public static readonly DependencyProperty SelectedBlockProperty =
			DependencyProperty.Register("SelectedBlock", typeof(BlockVm), typeof(PPTableVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PPTableVm)d;
				
				//set the ViewMode of the previously selected block to Simple
				var old = (BlockVm)e.OldValue;
				if (old != null) old.ViewMode = PPViewMode.Simple;
				
				var val = (BlockVm)e.NewValue;
				//if no block is selected go back to normal
				if (val == null)
				{
					vm.ShowBlockReport = false;
					vm.RestoreZoom();
				}
				//if a block is selected zoom to it and show its reports
				else
				{
					vm.ShowBlockReport = true;
					val.ViewMode = PPViewMode.Report;
					vm.ZoomToBlock(val);
					vm.VerticalScreenOffset = 0;
				}
			}));
		/// <summary>
		/// Gets or sets a value indicating whether or not show the SelectedBlock's report
		/// <para>Automatically sets to false if SelectedBlock is null</para>
		/// <para>Automatically sets SelectedBlock to null if set to false</para>
		/// </summary>
		public bool ShowBlockReport
		{
			get { return (bool)GetValue(ShowBlockReportProperty); }
			set { SetValue(ShowBlockReportProperty, value); }
		}
		public static readonly DependencyProperty ShowBlockReportProperty =
			DependencyProperty.Register("ShowBlockReport", typeof(bool), typeof(PPTableVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
			}, (d, v) =>
			{
				var vm = d as PPTableVm;
				//if no block is selected for report
				if (vm.SelectedBlock == null)
				{
					return false;
				}
				//if intend to hide block report
				if (!(bool)v)
				{
					vm.SelectedBlock = null;
					return false;
				}
				//otherwise allow to show block report
				return true;
			}));

		/// <summary>
		/// Gets or sets the selected NPT item
		/// </summary>
		public NPTVm SelectedNPT
		{
			get { return (NPTVm)GetValue(SelectedNPTProperty); }
			set { SetValue(SelectedNPTProperty, value); }
		}
		public static readonly DependencyProperty SelectedNPTProperty =
			DependencyProperty.Register("SelectedNPT", typeof(NPTVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the current TaskReportBuilder
		/// <para>Automatically deselects other ReportBuilders if set (to an object)</para>
		/// <para>Does not do that via binding</para>
		/// </summary>
		public Report.TaskReportHolderVm CurrentTaskReportBuilder
		{
			get { return (Report.TaskReportHolderVm)GetValue(CurrentTaskReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
				}
				SetValue(CurrentTaskReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentTaskReportBuilderProperty =
			DependencyProperty.Register("CurrentTaskReportBuilder", typeof(Report.TaskReportHolderVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the current NPTReportBuilder
		/// <para>Automatically deselects other ReportBuilders if set (to an object)</para>
		/// <para>Does not do that via binding</para>
		/// </summary>
		public Report.NPTReportVm CurrentNPTReportBuilder
		{
			get { return (Report.NPTReportVm)GetValue(CurrentNPTReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
					if (CurrentTaskReportBuilder != null)
						CurrentTaskReportBuilder.IsSelected = false;
				}
				SetValue(CurrentNPTReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentNPTReportBuilderProperty =
			DependencyProperty.Register("CurrentNPTReportBuilder", typeof(Report.NPTReportVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the current ProcessReportBuilder
		/// <para>Automatically deselects other ReportBuilders if set (to an object)</para>
		/// <para>Does not do that via binding</para>
		/// </summary>
		public Report.ProcessReportCellVm CurrentProcessReportBuilder
		{
			get { return (Report.ProcessReportCellVm)GetValue(CurrentProcessReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
					if (CurrentTaskReportBuilder != null)
						CurrentTaskReportBuilder.IsSelected = false;
				}
				SetValue(CurrentProcessReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentProcessReportBuilderProperty =
			DependencyProperty.Register("CurrentProcessReportBuilder", typeof(Report.ProcessReportCellVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		#endregion

		#region Commands
		void initializeCommands()
		{
			//command
			ToggleTaskEditorCommand = new Commands.Command(o =>
			{
				JobEditor.IsVisible = false;
				TaskEditor.IsVisible = !TaskEditor.IsVisible;
			});
			CleanAddBlockCommand = new Commands.Command(o =>
			{
				TaskEditor.IsVisible = true;
				JobEditor.IsVisible = false;
				TaskEditor.Reset();
			});
			ToggleJobEditorCommand = new Commands.Command(o =>
			{
				TaskEditor.IsVisible = false;
				JobEditor.IsVisible = !JobEditor.IsVisible;
			});
			CleanAddJobCommand = new Commands.Command(o =>
			{
				JobEditor.IsVisible = true;
				TaskEditor.IsVisible = false;
				JobEditor.Reset();
			});
			GoToNowCommand = new Commands.Command(o =>
			{
				BackupZoom();
				DateTime now = DateTime.Now;
				int month = (int)now.GetPersianMonth() - 1;
				SelectedMonth = Months.First(x => x.ColumnIndex == month);
				HoursPassed = now.GetPersianDayOfMonth() * 24 + now.Hour + now.Minute / 60d + now.Second / 3600d - 24;
				UpdateRange(true);
			});
			ZoomStartedCommand = new Commands.Command(o => BackupZoom());
			UndoZoomCommand = new Commands.Command(o => RestoreZoom());
		}

		//Task commands

		//ToggleTaskEditorCommand Dependency Property
		/// <summary>
		/// Gets a bindable command that toggles task editor's visiblity
		/// </summary>
		public Commands.Command ToggleTaskEditorCommand
		{
			get { return (Commands.Command)GetValue(ToggleTaskEditorCommandProperty); }
			protected set { SetValue(ToggleTaskEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty ToggleTaskEditorCommandProperty =
			DependencyProperty.Register("ToggleTaskEditorCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable command that resets the task editor and opens it
		/// </summary>
		public Commands.Command CleanAddBlockCommand
		{
			get { return (Commands.Command)GetValue(CleanAddBlockCommandProperty); }
			protected set { SetValue(CleanAddBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty CleanAddBlockCommandProperty =
			DependencyProperty.Register("CleanAddBlockCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		//Job commands

		/// <summary>
		/// Gets a bindable command that toggles job editor's visiblity
		/// </summary>
		public Commands.Command ToggleJobEditorCommand
		{
			get { return (Commands.Command)GetValue(ToggleJobEditorCommandProperty); }
			protected set { SetValue(ToggleJobEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty ToggleJobEditorCommandProperty =
			DependencyProperty.Register("ToggleJobEditorCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable command that resets the job editor and opens it
		/// </summary>
		public Commands.Command CleanAddJobCommand
		{
			get { return (Commands.Command)GetValue(CleanAddJobCommandProperty); }
			protected set { SetValue(CleanAddJobCommandProperty, value); }
		}
		public static readonly DependencyProperty CleanAddJobCommandProperty =
			DependencyProperty.Register("CleanAddJobCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		//Zoom commands

		/// <summary>
		/// Gets a bindable command that makes the timeline to navigates to Now
		/// </summary>
		public Commands.Command GoToNowCommand
		{
			get { return (Commands.Command)GetValue(GoToNowProperty); }
			protected set { SetValue(GoToNowProperty, value); }
		}
		public static readonly DependencyProperty GoToNowProperty =
			DependencyProperty.Register("GoToNowCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable command to notify ViewModel when user starts dragging the zoom slider
		/// </summary>
		public Commands.Command ZoomStartedCommand
		{
			get { return (Commands.Command)GetValue(ZoomStartedCommandProperty); }
			protected set { SetValue(ZoomStartedCommandProperty, value); }
		}
		public static readonly DependencyProperty ZoomStartedCommandProperty =
			DependencyProperty.Register("ZoomStartedCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable command that restore the timeline zoom to its backup values
		/// </summary>
		public Commands.Command UndoZoomCommand
		{
			get { return (Commands.Command)GetValue(UndoZoomCommandProperty); }
			protected set { SetValue(UndoZoomCommandProperty, value); }
		}
		public static readonly DependencyProperty UndoZoomCommandProperty =
			DependencyProperty.Register("UndoZoomCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		#endregion

		#region Toolbar extra items
		/// <summary>
		/// Gets a value that indicates if PPTable shows Product Codes
		/// <para>If set to false it shows Product Names</para>
		/// </summary>
		public bool ShowProductCodes
		{
			get { return (bool)GetValue(ShowProductCodesProperty); }
			protected set { SetValue(ShowProductCodesProperty, value); }
		}
		public static readonly DependencyProperty ShowProductCodesProperty =
			DependencyProperty.Register("ShowProductCodes", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(true));
		/// <summary>
		/// Gets a value that indicates whether "Insert Setup Button" should be visible in PPTable between Blocks
		/// </summary>
		public bool ShowInsertSetupButton
		{
			get { return (bool)GetValue(ShowInsertSetupButtonProperty); }
			protected set { SetValue(ShowInsertSetupButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowInsertSetupButtonProperty =
			DependencyProperty.Register("ShowInsertSetupButton", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(false));
		#endregion

	}
}
