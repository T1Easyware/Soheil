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

			//Add event handlers
			BlockDataService.BlockAdded += (s, e) =>
			{
				PPItems.AddItem(e.NewModel);
			};
			BlockDataService.BlockUpdated += (s, e) =>
			{
				PPItems.RemoveItem(e.OldModel);
				PPItems.AddItem(e.NewModel);
			};
			//useless???
			JobDataService.JobAdded += (s, e) =>
			{
				var jobVm = new PPJobVm(e.NewModel);
				foreach (var block in e.NewModel.Blocks)
				{
					var blockVm = PPItems.FindItem(block);
					if (blockVm != null) blockVm.Job = jobVm;
					else PPItems.AddItem(block);
				}
			};
		}
		/// <summary>
		/// Initializes TaskEditor, JobEditor and JobList
		/// </summary>
		void initializeEditors()
		{
			TaskEditor = new PPTaskEditorVm();
			TaskEditor.RefreshPPItems += () => UpdateRange(true);
			JobEditor = new PPJobEditorVm();
			JobList = new JobListVm(JobDataService);

			//Add an inline event handler for JobSelected event of JobList
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
					UpdateRange(true);
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
		/// Gets ViewModel for TaskEditor
		/// </summary>
		public PPTaskEditorVm TaskEditor
		{
			get { return (PPTaskEditorVm)GetValue(TaskEditorProperty); }
			private set { SetValue(TaskEditorProperty, value); }
		}
		public static readonly DependencyProperty TaskEditorProperty =
			DependencyProperty.Register("TaskEditor", typeof(PPTaskEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets ViewModel for JobEditor
		/// </summary>
		public PPJobEditorVm JobEditor
		{
			get { return (PPJobEditorVm)GetValue(JobEditorProperty); }
			private set { SetValue(JobEditorProperty, value); }
		}
		public static readonly DependencyProperty JobEditorProperty =
			DependencyProperty.Register("JobEditor", typeof(PPJobEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets ViewModel for JobList
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
					try
					{
						BlockDataService.DeleteModelById(block.Id);
						station.Blocks.Remove(block);
					}
					catch(Soheil.Common.SoheilException.RoutedException exp)
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
					catch(Exception exp)
					{
						block.Message.AddEmbeddedException(exp.Message);
					}
				}
			}
		}
		#endregion

		#region TimeRoll and Shifts
		//ShiftsAndBreaks Observable Collection
		private ObservableCollection<OrganizationCalendar.WorkTimeRangeVm> _shiftsAndBreaks = new ObservableCollection<OrganizationCalendar.WorkTimeRangeVm>();
		public ObservableCollection<OrganizationCalendar.WorkTimeRangeVm> ShiftsAndBreaks { get { return _shiftsAndBreaks; } }

		//Hours Dependency Property
		public HourCollection Hours
		{
			get { return (HourCollection)GetValue(HoursProperty); }
			set { SetValue(HoursProperty, value); }
		}
		public static readonly DependencyProperty HoursProperty =
			DependencyProperty.Register("Hours", typeof(HourCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		//Days Dependency Property
		public DayCollection Days
		{
			get { return (DayCollection)GetValue(DaysProperty); }
			set { SetValue(DaysProperty, value); }
		}
		public static readonly DependencyProperty DaysProperty =
			DependencyProperty.Register("Days", typeof(DayCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		//Months Dependency Property
		public MonthCollection Months
		{
			get { return (MonthCollection)GetValue(MonthsProperty); }
			set { SetValue(MonthsProperty, value); }
		}
		public static readonly DependencyProperty MonthsProperty =
			DependencyProperty.Register("Months", typeof(MonthCollection), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Hours Passed (measured from the edge of the screen)
		/// </summary>
		public double HoursPassed
		{
			get { return (double)GetValue(HoursPassedProperty); }
			set { SetValue(HoursPassedProperty, value); }
		}
		public static readonly DependencyProperty HoursPassedProperty =
			DependencyProperty.Register("HoursPassed", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(0d));
		/// <summary>
		/// A value indicating the Width of one month
		/// <para>GridWidth divided by the Number of days in SelectedMonth</para>
		/// <para>Default value = 40</para>
		/// </summary>
		public double DayZoom
		{
			get { return (double)GetValue(DayZoomProperty); }
			set { SetValue(DayZoomProperty, value); }
		}
		public static readonly DependencyProperty DayZoomProperty =
			DependencyProperty.Register("DayZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(40d));
		/// <summary>
		/// Width of OneHour in HoursBar or in pptable
		/// </summary>
		public double HourZoom
		{
			get { return (double)GetValue(HourZoomProperty); }
			set { SetValue(HourZoomProperty, value); }
		}
		public static readonly DependencyProperty HourZoomProperty =
			DependencyProperty.Register("HourZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(36d, (d, e) => { ((PPTableVm)d).UpdateRange(false); }));
		/// <summary>
		/// Number of days in currect active year
		/// </summary>
		public int DaysInYear
		{
			get { return (int)GetValue(DaysInYearProperty); }
			set { SetValue(DaysInYearProperty, value); }
		}
		public static readonly DependencyProperty DaysInYearProperty =
			DependencyProperty.Register("DaysInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(365));
		/// <summary>
		/// Total number of hours in current active year
		/// </summary>
		public int HoursInYear
		{
			get { return (int)GetValue(HoursInYearProperty); }
			set { SetValue(HoursInYearProperty, value); }
		}
		public static readonly DependencyProperty HoursInYearProperty =
			DependencyProperty.Register("HoursInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(8760));
		/// <summary>
		/// Gets or sets the Selected Month and Updates the timeline
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
		/// Width of screen
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
		/// A value indicating the vertical offset of the PPTable's scrollviewer
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
		/// Number of active stations in PPTable
		/// </summary>
		public int NumberOfStations
		{
			get { return (int)GetValue(NumberOfStationsProperty); }
			set { SetValue(NumberOfStationsProperty, value); }
		}
		public static readonly DependencyProperty NumberOfStationsProperty =
			DependencyProperty.Register("NumberOfStations", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(0));
		/// <summary>
		/// A collection of Stations, in each a collection of PPTable Items that are visible
		/// <para>Each item can be either an NPT or a Block</para>
		/// </summary>
		public PPItemCollection PPItems
		{
			get { return (PPItemCollection)GetValue(PPItemsProperty); }
			set { SetValue(PPItemsProperty, value); }
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
		public TaskReportHolderVm CurrentTaskReportBuilder
		{
			get { return (TaskReportHolderVm)GetValue(CurrentTaskReportBuilderProperty); }
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
			DependencyProperty.Register("CurrentTaskReportBuilder", typeof(TaskReportHolderVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the current NPTReportBuilder
		/// <para>Automatically deselects other ReportBuilders if set (to an object)</para>
		/// <para>Does not do that via binding</para>
		/// </summary>
		public NPTReportVm CurrentNPTReportBuilder
		{
			get { return (NPTReportVm)GetValue(CurrentNPTReportBuilderProperty); }
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
			DependencyProperty.Register("CurrentNPTReportBuilder", typeof(NPTReportVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the current ProcessReportBuilder
		/// <para>Automatically deselects other ReportBuilders if set (to an object)</para>
		/// <para>Does not do that via binding</para>
		/// </summary>
		public ProcessReportCellVm CurrentProcessReportBuilder
		{
			get { return (ProcessReportCellVm)GetValue(CurrentProcessReportBuilderProperty); }
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
			DependencyProperty.Register("CurrentProcessReportBuilder", typeof(ProcessReportCellVm), typeof(PPTableVm), new UIPropertyMetadata(null));
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
			GoToNow = new Commands.Command(o =>
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
		public Commands.Command ToggleTaskEditorCommand
		{
			get { return (Commands.Command)GetValue(ToggleTaskEditorCommandProperty); }
			set { SetValue(ToggleTaskEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty ToggleTaskEditorCommandProperty =
			DependencyProperty.Register("ToggleTaskEditorCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		//CleanAddBlockCommand Dependency Property
		public Commands.Command CleanAddBlockCommand
		{
			get { return (Commands.Command)GetValue(CleanAddBlockCommandProperty); }
			set { SetValue(CleanAddBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty CleanAddBlockCommandProperty =
			DependencyProperty.Register("CleanAddBlockCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		//Job commands

		//ToggleJobEditorCommand Dependency Property
		public Commands.Command ToggleJobEditorCommand
		{
			get { return (Commands.Command)GetValue(ToggleJobEditorCommandProperty); }
			set { SetValue(ToggleJobEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty ToggleJobEditorCommandProperty =
			DependencyProperty.Register("ToggleJobEditorCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		//CleanAddJobCommand Dependency Property
		public Commands.Command CleanAddJobCommand
		{
			get { return (Commands.Command)GetValue(CleanAddJobCommandProperty); }
			set { SetValue(CleanAddJobCommandProperty, value); }
		}
		public static readonly DependencyProperty CleanAddJobCommandProperty =
			DependencyProperty.Register("CleanAddJobCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		//Zoom commands

		//GoToNow Command
		public Commands.Command GoToNow
		{
			get { return (Commands.Command)GetValue(GoToNowProperty); }
			set { SetValue(GoToNowProperty, value); }
		}
		public static readonly DependencyProperty GoToNowProperty =
			DependencyProperty.Register("GoToNow", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		//ZoomStartedCommand Dependency Property
		public Commands.Command ZoomStartedCommand
		{
			get { return (Commands.Command)GetValue(ZoomStartedCommandProperty); }
			set { SetValue(ZoomStartedCommandProperty, value); }
		}
		public static readonly DependencyProperty ZoomStartedCommandProperty =
			DependencyProperty.Register("ZoomStartedCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		//UndoZoomCommand Dependency Property
		public Commands.Command UndoZoomCommand
		{
			get { return (Commands.Command)GetValue(UndoZoomCommandProperty); }
			set { SetValue(UndoZoomCommandProperty, value); }
		}
		public static readonly DependencyProperty UndoZoomCommandProperty =
			DependencyProperty.Register("UndoZoomCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		#endregion

		#region Toolbar extra items
		/// <summary>
		/// If set to true PPTable shows Product Codes, else it shows Product Names
		/// </summary>
		public bool ShowProductCodes
		{
			get { return (bool)GetValue(ShowProductCodesProperty); }
			set { SetValue(ShowProductCodesProperty, value); }
		}
		public static readonly DependencyProperty ShowProductCodesProperty =
			DependencyProperty.Register("ShowProductCodes", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(true));
		/// <summary>
		/// Gets or sets a value indicating whether "Insert Setup Button" should be visible in PPTable between Blocks
		/// </summary>
		public bool ShowInsertSetupButton
		{
			get { return (bool)GetValue(ShowInsertSetupButtonProperty); }
			set { SetValue(ShowInsertSetupButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowInsertSetupButtonProperty =
			DependencyProperty.Register("ShowInsertSetupButton", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(true));
		#endregion

	}
}
