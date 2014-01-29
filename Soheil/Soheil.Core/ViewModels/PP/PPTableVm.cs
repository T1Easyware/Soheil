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
using Soheil.Core.PP;
using Soheil.Core.ViewModels.PP.Editor;

namespace Soheil.Core.ViewModels.PP
{
	public class PPTableVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; private set; }

		public DataServices.BlockDataService BlockDataService { get; private set; }
		public DataServices.NPTDataService NPTDataService { get; private set; }
		public DataServices.TaskDataService TaskDataService { get; private set; }
		public DataServices.JobDataService JobDataService { get; private set; }
		public DataServices.TaskReportDataService TaskReportDataService { get; private set; }
		public DataServices.ProcessReportDataService ProcessReportDataService { get; private set; }
		public Dal.SoheilEdmContext UOW { get; private set; }

		#region Ctor, Init and Load
		public PPTableVm(AccessType access)
		{
			Access = access;

			initializeCommands();
			initializeDataServices();

			TaskEditor = new PPTaskEditorVm();
			JobEditor = new PPJobEditorVm();

			ToggleTaskEditorCommand.Execute(null);
			TaskEditor.SelectedProduct = TaskEditor.AllProductGroups.First().Products.First();
			var block = new PPEditorBlock(TaskEditor.FpcViewer.States.First(x=>x.StateType == StateType.Mid).Model);
			TaskEditor.BlockList.Add(block);
			TaskEditor.SelectedBlock = TaskEditor.BlockList.First();
		}

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
		}

		public void InitializeViewModel()
		{
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

		//Starting Timer
		System.Threading.Timer _initialTimer;
		public void ResetTimeLine()
		{
			if (_initialTimer != null) _initialTimer.Dispose();
			_initialTimer = new System.Threading.Timer(new System.Threading.TimerCallback
				(o => Dispatcher.Invoke(() =>
				{
					var currentDate = Arash.PersianDate.Today.ToDateTime();
					var startDate = currentDate.GetNorooz();

					//initialize providers
					Days = new DayCollection();
					Months = new MonthCollection(startDate, Days, this);
					Hours = new HourCollection();
					PPItems = new PPItemCollection(this);
					//Add stations
					var stationModels = new DataServices.StationDataService(UOW).GetActives().OrderBy(x => x.Index);
					NumberOfStations = stationModels.Count();
					foreach (var stationModel in stationModels)
					{
						PPItems.Add(new PPStationVm { Text = stationModel.Name });
					}

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

					UpdateRange(true);
					BackupZoom();
				})), null, 10, System.Threading.Timeout.Infinite);
		}

		//AlwaysLoadTasks Dependency Property
		System.Timers.Timer _periodicTimer;
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
					if (vm._periodicTimer == null)
					{
						vm._periodicTimer = new System.Timers.Timer(5000);
						vm._periodicTimer.Elapsed += ((a, b) =>
						{
							try
							{
								vm.Dispatcher.Invoke(() => { vm.PPItems.Reload(); });
							}
							catch { }
						});
						vm._periodicTimer.Start();
					}
					vm._periodicTimer.Start();
				}
				else if (vm._periodicTimer != null) vm._periodicTimer.Stop();
			}));
		//ShowProductCodes Dependency Property
		public bool ShowProductCodes
		{
			get { return (bool)GetValue(ShowProductCodesProperty); }
			set { SetValue(ShowProductCodesProperty, value); }
		}
		public static readonly DependencyProperty ShowProductCodesProperty =
			DependencyProperty.Register("ShowProductCodes", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Updates current TimeRange of items (hours, shifts, tasks...) which are visible in PPTable
		/// </summary>
		/// <param name="loadTasksAsWell">Load PP Items (setups, tasks, ...) while loading timeline</param>
		public void UpdateRange(bool loadItemsAsWell)
		{
			var start = SelectedMonth.Data.AddHours(HoursPassed);
			var end = start.AddHours(GridWidth / HourZoom);
			Hours.FetchRange(start, end);
			updateShiftsAndBreaks(start, end);

			if ((loadItemsAsWell || AlwaysLoadTasks) && (ViewMode == PPViewMode.Simple))
				PPItems.FetchRange(start, end);
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

		#region Task&Block
		//TaskEditor Dependency Property
		public PPTaskEditorVm TaskEditor
		{
			get { return (PPTaskEditorVm)GetValue(TaskEditorProperty); }
			private set { SetValue(TaskEditorProperty, value); }
		}
		public static readonly DependencyProperty TaskEditorProperty =
			DependencyProperty.Register("TaskEditor", typeof(PPTaskEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

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

		//Opens the PPEditor for a new Task
		public void ResetTaskEditor()
		{
			TaskEditor.Reset();
		}
		//Opens the PPEditor for existing Tasks
		public void ResetTaskEditor(Model.Block blockToEdit)
		{
			TaskEditor.Reset();
			TaskEditor.BlockList.Add(new PPEditorBlock(blockToEdit));
			TaskEditor.SelectedBlock = TaskEditor.BlockList.Last();
		}
		//Add existing Tasks to the PPEditor
		public void AppendToTaskEditor(Model.Block blockToAppend)
		{
			if (blockToAppend == null) return;
			TaskEditor.BlockList.Add(new PPEditorBlock(blockToAppend));
			TaskEditor.SelectedBlock = TaskEditor.BlockList.Last();
		}

		public void RemoveBlock(BlockVm block)
		{
			
		}
		#endregion

		#region Job
		//JobEditor Dependency Property
		public PPJobEditorVm JobEditor
		{
			get { return (PPJobEditorVm)GetValue(JobEditorProperty); }
			set { SetValue(JobEditorProperty, value); }
		}
		public static readonly DependencyProperty JobEditorProperty =
			DependencyProperty.Register("JobEditor", typeof(PPJobEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

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

		//Opens the PPEditor for a new Job
		public void ResetJobEditor()
		{
			JobEditor.Reset();
		}
		//Add existing Tasks to the PPEditor
		public void AppendToJobEditor(Model.Job jobToAppend)
		{
			JobEditor.JobList.Add(new PPEditorJob(jobToAppend));
		}
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

		#region Setup
		//ShowInsertSetupButton Dependency Property
		public bool ShowInsertSetupButton
		{
			get { return (bool)GetValue(ShowInsertSetupButtonProperty); }
			set { SetValue(ShowInsertSetupButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowInsertSetupButtonProperty =
			DependencyProperty.Register("ShowInsertSetupButton", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(true));
		#endregion

		#region TimeRoll
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


		//Hours Passed from the edge of the screen in days bar 
		public double HoursPassed
		{
			get { return (double)GetValue(HoursPassedProperty); }
			set { SetValue(HoursPassedProperty, value); }
		}
		public static readonly DependencyProperty HoursPassedProperty =
			DependencyProperty.Register("HoursPassed", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(0d));
		//DayZoom Dependency Property
		public double DayZoom
		{
			get { return (double)GetValue(DayZoomProperty); }
			set { SetValue(DayZoomProperty, value); }
		}
		public static readonly DependencyProperty DayZoomProperty =
			DependencyProperty.Register("DayZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(40d));
		//HourZoom Dependency Property (Width of OneHour in HoursBar or in pptable)
		public double HourZoom
		{
			get { return (double)GetValue(HourZoomProperty); }
			set { SetValue(HourZoomProperty, value); }
		}
		public static readonly DependencyProperty HourZoomProperty =
			DependencyProperty.Register("HourZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(36d, (d, e) => { ((PPTableVm)d).UpdateRange(false); }));
		//DaysInYear Dependency Property
		public int DaysInYear
		{
			get { return (int)GetValue(DaysInYearProperty); }
			set { SetValue(DaysInYearProperty, value); }
		}
		public static readonly DependencyProperty DaysInYearProperty =
			DependencyProperty.Register("DaysInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(365));
		public int HoursInYear
		{
			get { return (int)GetValue(HoursInYearProperty); }
			set { SetValue(HoursInYearProperty, value); }
		}
		public static readonly DependencyProperty HoursInYearProperty =
			DependencyProperty.Register("HoursInYear", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(8760));
		//SelectedMonth Dependency Property
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
				val.IsSelected = true;
				vm.DayZoom = vm.GridWidth / val.NumOfDays;
				vm.UpdateRange(true);
			}));

		#endregion

		#region Zoom and Pan
		//Width of screen
		public double GridWidth { get; set; }
		public void UpdateWidths()
		{
			if (SelectedMonth != null)
				DayZoom = GridWidth / SelectedMonth.NumOfDays;
		}
		//VerticalScreenOffset Dependency Property
		public double VerticalScreenOffset
		{
			get { return (double)GetValue(VerticalScreenOffsetProperty); }
			set { SetValue(VerticalScreenOffsetProperty, value); }
		}
		public static readonly DependencyProperty VerticalScreenOffsetProperty =
			DependencyProperty.Register("VerticalScreenOffset", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(0d));
		//GoToNow Command
		public Commands.Command GoToNow
		{
			get { return (Commands.Command)GetValue(GoToNowProperty); }
			set { SetValue(GoToNowProperty, value); }
		}
		public static readonly DependencyProperty GoToNowProperty =
			DependencyProperty.Register("GoToNow", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

		//Zoooooom
		private double _hoursPassedBackup = 0;
		private double _hourZoomBackup = 36;
		private double _verticalScreenOffset = 0;
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
		public void BackupZoom()
		{
			_hoursPassedBackup = HoursPassed;
			_hourZoomBackup = HourZoom;
			_verticalScreenOffset = VerticalScreenOffset;
		}
		public void RestoreZoom()
		{
			HourZoom = _hourZoomBackup;
			HoursPassed = _hoursPassedBackup;
			VerticalScreenOffset = _verticalScreenOffset;
		}
		#endregion

		#region Stations and their items
		//NumberOfStations Dependency Property
		public int NumberOfStations
		{
			get { return (int)GetValue(NumberOfStationsProperty); }
			set { SetValue(NumberOfStationsProperty, value); }
		}
		public static readonly DependencyProperty NumberOfStationsProperty =
			DependencyProperty.Register("NumberOfStations", typeof(int), typeof(PPTableVm), new UIPropertyMetadata(0));
		//Stations Dependency Property
		public PPItemCollection PPItems
		{
			get { return (PPItemCollection)GetValue(PPItemsProperty); }
			set { SetValue(PPItemsProperty, value); }
		}
		public static readonly DependencyProperty PPItemsProperty =
			DependencyProperty.Register("PPItems", typeof(PPItemCollection), typeof(PPTableVm), new UIPropertyMetadata(null));
		#endregion

		#region Report and Selected items
		//SelectedBlock Dependency Property
		public BlockVm SelectedBlock
		{
			get { return (BlockVm)GetValue(SelectedBlockProperty); }
			set { SetValue(SelectedBlockProperty, value); }
		}
		public static readonly DependencyProperty SelectedBlockProperty =
			DependencyProperty.Register("SelectedBlock", typeof(BlockVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		//SelectedNPT Dependency Property
		public NPTVm SelectedNPT
		{
			get { return (NPTVm)GetValue(SelectedNPTProperty); }
			set { SetValue(SelectedNPTProperty, value); }
		}
		public static readonly DependencyProperty SelectedNPTProperty =
			DependencyProperty.Register("SelectedNPT", typeof(NPTVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		//ViewMode Dependency Property
		public PPViewMode ViewMode
		{
			get { return (PPViewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public static readonly DependencyProperty ViewModeProperty =
			DependencyProperty.Register("ViewMode", typeof(PPViewMode), typeof(PPTableVm),
			new UIPropertyMetadata(PPViewMode.Simple, (d, e) =>
			{
				var vm = (PPTableVm)d;
				if ((PPViewMode)e.NewValue == (PPViewMode)e.OldValue) return;
				//exiting ProcessReport mode
				if ((PPViewMode)e.OldValue == PPViewMode.Report)
				{
					vm.RestoreZoom();
				}
				//entering new mode
				switch ((PPViewMode)e.NewValue)
				{
					case PPViewMode.Simple:
						vm.PPItems.ViewMode = PPViewMode.Simple;
						break;
					case PPViewMode.Report:
						if (vm.SelectedBlock != null)
						{
							vm.ZoomToBlock(vm.SelectedBlock);
						}
						vm.VerticalScreenOffset = 0;
						break;
					default:
						break;
				}
			}, (d, v) =>
			{
				var vm = (PPTableVm)d;
				if ((PPViewMode)v == PPViewMode.Report && vm.SelectedBlock == null) return PPViewMode.Simple;
				return (PPViewMode)v;
			}));

		//CurrentTaskReportBuilder Dependency Property
		public TaskReportHolderVm CurrentTaskReportBuilder
		{
			get { return (TaskReportHolderVm)GetValue(CurrentTaskReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
					if (CurrentTaskReportBuilderInProcess != null)
						CurrentTaskReportBuilderInProcess.IsSelected = false;
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
				}
				SetValue(CurrentTaskReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentTaskReportBuilderProperty =
			DependencyProperty.Register("CurrentTaskReportBuilder", typeof(TaskReportHolderVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		//CurrentNPTReportBuilder Dependency Property
		public NPTReportVm CurrentNPTReportBuilder
		{
			get { return (NPTReportVm)GetValue(CurrentNPTReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
					if (CurrentTaskReportBuilderInProcess != null)
						CurrentTaskReportBuilderInProcess.IsSelected = false;
					if (CurrentTaskReportBuilder != null)
						CurrentTaskReportBuilder.IsSelected = false;
				}
				SetValue(CurrentNPTReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentNPTReportBuilderProperty =
			DependencyProperty.Register("CurrentNPTReportBuilder", typeof(NPTReportVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		//CurrentTaskReportBuilderInProcess Dependency Property
		public ProcessReportCellTaskReportHolder CurrentTaskReportBuilderInProcess
		{
			get { return (ProcessReportCellTaskReportHolder)GetValue(CurrentTaskReportBuilderInProcessProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
					if (CurrentTaskReportBuilder != null)
						CurrentTaskReportBuilder.IsSelected = false;
				}
				SetValue(CurrentTaskReportBuilderInProcessProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentTaskReportBuilderInProcessProperty =
			DependencyProperty.Register("CurrentTaskReportBuilderInProcess", typeof(ProcessReportCellTaskReportHolder), typeof(PPTableVm), new UIPropertyMetadata(null));
		//CurrentProcessReportBuilder Dependency Property
		public ProcessReportCellVm CurrentProcessReportBuilder
		{
			get { return (ProcessReportCellVm)GetValue(CurrentProcessReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentTaskReportBuilderInProcess != null)
						CurrentTaskReportBuilderInProcess.IsSelected = false;
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

	}
}
