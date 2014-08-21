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
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for PPTable
	/// </summary>
	public class PPTableVm : DependencyObject, ISingularList, IDisposable
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; private set; }

		private bool _suppressUpdateRange = true;


		#region Ctor, Init and Load
		/// <summary>
		/// Instantiates and Initializes a new Instance of PPTable with given access level
		/// </summary>
		/// <param name="access">Access level used for the this instance of PPTable</param>
		public PPTableVm(AccessType access)
		{
			Access = access;
			initializeCommands();
			initializeEditors();
		}
		public void Dispose()
		{
			if (PPItems != null)
				PPItems.Dispose();
		}

		/// <summary>
		/// Initializes TaskEditor, JobEditor and JobList
		/// </summary>
		void initializeEditors()
		{
			TaskEditor = new PlanEditorVm();
			TaskEditor.RefreshPPItems += () =>
			{
				PPItems.Manager.ForceReload();

				if (ShowBlockReport)
				{
					SelectedBlock.BlockReport = new Report.BlockReportVm(SelectedBlock);
					SelectedBlock.BlockReport.ProcessReportBuilderChanged += val => CurrentProcessReportBuilder = val;
				}
			};
			/*refresh is enough.
			 * TaskEditor.BlockAdded += model => PPItems.AddItem(model);
			TaskEditor.BlockUpdated += (oldModel, newModel) =>
			{
				PPItems.RemoveItem(oldModel);
				var newVm = PPItems.AddItem(newModel);
				if (SelectedBlock.Id == oldModel.Id)
					SelectedBlock = newVm;
			};*/

			JobEditor = new JobEditorVm();
			JobEditor.RefreshPPItems += () => PPItems.Manager.ForceReload();
			/*refresh is enough.
			 *JobEditor.JobAdded += model =>
			{
				var jobVm = new JobVm(model);
				foreach (var block in model.Blocks)
				{
					var blockVm = PPItems.FindItem(block);
					if (blockVm != null) blockVm.Job = jobVm;
					else PPItems.AddItem(block);
				}
			};*/

			JobList = new JobListVm();
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
		/// A constant value (40) for Delay of _initialTimer
		/// </summary>
		const int _initialTimerInterval = 40;
		/// <summary>
		/// Resets the timeline to Now, loads PPItems in range and loads Stations
		/// <para>This method is using _initialTimer so its body will be run with a 10ms delay</para>
		/// </summary>
		public void ResetTimeLine()
		{
			//exit if loaded once
			if (_initialTimer != null) return;

			_initialTimer = new System.Threading.Timer(new System.Threading.TimerCallback
				(t => Dispatcher.Invoke(() =>
				{
					_suppressUpdateRange = true;
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
					PPItems.Manager.DayColorsUpdated += (startingDate, colors) =>
					{
						//refetch it, if selected month is changed after fetch
						if (SelectedMonth.Data != startingDate)
						{
							PPItems.Manager.InitMonth(SelectedMonth.Data);
							return;
						}
						//colorize days
						for (int i = 0; i < colors.Length; i++)
						{
							Days[i].Color = colors[i];
						}
					};
					PPItems.Manager.WorkTimeAdded += item =>
					{
						var vms = Soheil.Core.ViewModels.OrganizationCalendar.WorkTimeRangeVm.CreateAuto(item);
						foreach (var vm in vms)
						{
							ShiftsAndBreaks.Add(vm);
						}
					};
					PPItems.Manager.WorkTimesRemoved += () =>
					{
						ShiftsAndBreaks.Clear();
					};
					PPItems.BlockAdded += vm =>
					{
						initializeCommands(vm);
					};
					PPItems.BlockRemoved += id =>
					{
						if (SelectedBlock != null && SelectedBlock.Id == id)
							SelectedBlock = null;
					};
					PPItems.NptAdded += vm =>
					{
						initializeCommands(vm);
					};
					PPItems.NptRemoved += id =>
					{
						if (SelectedNPT != null && SelectedNPT.Id == id)
							SelectedNPT = null;
					};


					//Initialize stations
					var stationModels = new DataServices.StationDataService().FixAndGetActives();
					NumberOfStations = stationModels.Count();
					foreach (var stationModel in stationModels)
					{
						PPItems.Add(new StationVm { Text = stationModel.Name });
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
					_suppressUpdateRange = false;
					GoToNowCommand.Execute(null);
				})), null, _initialTimerInterval, System.Threading.Timeout.Infinite);
		}


		/// <summary>
		/// Updates current TimeRange of items (hours, shifts, tasks...) which are visible in PPTable
		/// </summary>
		/// <remarks>
		/// This function uses a Monitor to waits 2 secs if local lock is on
		/// </remarks>
		/// <param name="loadTasksAsWell">Load PP Items (setups, tasks, ...) while loading timeline</param>
		public void UpdateRange(bool loadItemsAsWell)
		{
			//finds start and end of the visible range in PPTable
			if (double.IsInfinity(HoursPassed)) return;

			var start = SelectedMonth.Data.AddHours(HoursPassed);
			var end = start.AddHours(GridWidth / HourZoom);

			//Loads hours items
			Hours.FetchRange(start, end);

			//Loads PPItems
			if (loadItemsAsWell)
				PPItems.Manager.AutoFetchRange(start, end);
		}
		#endregion

		#region Editors
		/// <summary>
		/// Gets bindable ViewModel for PlanEditor
		/// </summary>
		public PlanEditorVm TaskEditor
		{
			get { return (PlanEditorVm)GetValue(TaskEditorProperty); }
			private set { SetValue(TaskEditorProperty, value); }
		}
		public static readonly DependencyProperty TaskEditorProperty =
			DependencyProperty.Register("TaskEditor", typeof(PlanEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets bindable ViewModel for JobEditor
		/// </summary>
		public JobEditorVm JobEditor
		{
			get { return (JobEditorVm)GetValue(JobEditorProperty); }
			private set { SetValue(JobEditorProperty, value); }
		}
		public static readonly DependencyProperty JobEditorProperty =
			DependencyProperty.Register("JobEditor", typeof(JobEditorVm), typeof(PPTableVm), new UIPropertyMetadata(null));

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
		public void RemoveBlocks(JobVm job)
		{
			foreach (var station in PPItems.ToArray())
			{
				foreach (var block in station.Blocks.ToArray())
				{
					//check if this block in this station is part of the given Job
					if (block.Job != null && block.Job.Id == job.Id)
					{
						block.DeleteItemCommand.Execute(null);
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
			DependencyProperty.Register("HourZoom", typeof(double), typeof(PPTableVm), new UIPropertyMetadata(100d, (d, e) => { ((PPTableVm)d).UpdateRange(false); }, (d, v) =>
			{
				if ((double)v < 20) return 20d;
				if ((double)v > 2000) return 2000d;
				return v;
			}));
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
				vm.PPItems.Manager.InitMonth(val.Data);

				//changes the DayZoom to match the number of days in the current month
				vm.DayZoom = vm.GridWidth / val.NumOfDays;
				if (!vm._suppressUpdateRange)
					vm.UpdateRange(true);
			}));

		#endregion

		#region Zoom and Pan
		/// <summary>
		/// Gets or sets Width of screen
		/// </summary>
		public double GridWidth { get; set; }

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
			var start = blockVm.StartDateTime;
			HoursPassed = start.GetPersianDayOfMonth() * 24 + start.Hour + start.Minute / 60d + start.Second / 3600d - 24;
			HourZoom = (GridWidth * 3600) / blockVm.DurationSeconds;
		}
		/// <summary>
		/// Zooms to fit the given range in screen
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void ZoomToRange(DateTime start, DateTime end)
		{
			HoursPassed = start.GetPersianDayOfMonth() * 24 + start.Hour + start.Minute / 60d + start.Second / 3600d - 24;
			HourZoom = (GridWidth * 3600) / end.Subtract(start).TotalSeconds;
		}
		/// <summary>
		/// Restores values of HoursPassed, HourZoom and VerticalScreenOffset to their backup values
		/// </summary>
		public void RestoreZoom()
		{
			HourZoom = (double)HourZoomProperty.DefaultMetadata.DefaultValue;
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
		/// Gets or sets the Selected Block (if same block is set again, a reset to null happens before setting the value to ensure callback)
		/// <para>sets the ViewMode of previous and current SelectedBlock</para>
		/// <para>Restores zoom if null and Zooms to block if otherwise</para>
		/// </summary>
		public BlockVm SelectedBlock
		{
			get { return (BlockVm)GetValue(SelectedBlockProperty); }
			set { SetValue(SelectedBlockProperty, null); SetValue(SelectedBlockProperty, value); }
		}
		public static readonly DependencyProperty SelectedBlockProperty =
			DependencyProperty.Register("SelectedBlock", typeof(BlockVm), typeof(PPTableVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PPTableVm)d;
				
				//set the ViewMode of the previously selected block to Simple
				var old = (BlockVm)e.OldValue;
				if (old != null) old.ViewMode = PPViewMode.Simple;

				//don't zoom or anything if copy/cut
				if (vm.ShowItemEditing)
				{
					return;
				}

				var val = (BlockVm)e.NewValue;
				//if no block is selected go back to normal
				if (val == null)
				{
					vm.ShowBlockReport = false;
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
					vm.RestoreZoom();
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
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
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
		public Report.ProcessReportVm CurrentProcessReportBuilder
		{
			get { return (Report.ProcessReportVm)GetValue(CurrentProcessReportBuilderProperty); }
			set
			{
				if (value != null)
				{
					if (CurrentNPTReportBuilder != null)
						CurrentNPTReportBuilder.IsSelected = false;
					if(CurrentProcessReportBuilder != null)
						CurrentProcessReportBuilder.IsSelected = false;
				}
				SetValue(CurrentProcessReportBuilderProperty, value);
			}
		}
		public static readonly DependencyProperty CurrentProcessReportBuilderProperty =
			DependencyProperty.Register("CurrentProcessReportBuilder", typeof(Report.ProcessReportVm), typeof(PPTableVm), new UIPropertyMetadata(null));
		#endregion

		#region Commands
		/// <summary>
		/// Initializes PPTableVm commands that can be assigned in this class
		/// </summary>
		void initializeCommands()
		{
			//command
			ToggleTaskEditorCommand = new Commands.Command(o =>
			{
				JobEditor.IsVisible = false;
				TaskEditor.IsVisible = !TaskEditor.IsVisible;
				PPItems.Manager.Pause = TaskEditor.IsVisible;
			});
			CleanAddBlockCommand = new Commands.Command(o =>
			{
				PPItems.Manager.Pause = true;
				TaskEditor.IsVisible = true;
				JobEditor.IsVisible = false;
				TaskEditor.Reset();
			});
			ToggleJobEditorCommand = new Commands.Command(o =>
			{
				TaskEditor.IsVisible = false;
				JobEditor.IsVisible = !JobEditor.IsVisible;
				PPItems.Manager.Pause = JobEditor.IsVisible;
			});
			CleanAddJobCommand = new Commands.Command(o =>
			{
				PPItems.Manager.Pause = true;
				JobEditor.IsVisible = true;
				TaskEditor.IsVisible = false;
				JobEditor.Reset();
			});
			GoToNowCommand = new Commands.Command(o =>
			{
				DateTime now = DateTime.Now;
				int month = (int)now.GetPersianMonth() - 1;
				SelectedMonth = Months.First(x => x.ColumnIndex == month);
				HoursPassed = now.GetPersianDayOfMonth() * 24 + now.Hour + now.Minute / 60d + now.Second / 3600d - 24;
				UpdateRange(true);
			});
			ZoomStartedCommand = new Commands.Command(o => { });
			UndoZoomCommand = new Commands.Command(o => RestoreZoom());
			RefreshCommand = new Commands.Command(o => PPItems.Manager.ForceReload(true));
			CloseBlockReportCommand = new Commands.Command(o => ShowBlockReport = false);

			//copy/cut
			CopyToCommand = new Commands.Command(o =>
			{
				SelectedBlock.CopyTo(ItemEditingDate.Date.Add(ItemEditingTime));
				PPItems.Manager.ForceReload(true);
			}, () => SelectedBlock != null);
			MoveToCommand = new Commands.Command(o =>
			{
				SelectedBlock.MoveTo(ItemEditingDate.Date.Add(ItemEditingTime));
				PPItems.Manager.ForceReload(true);
			}, () => SelectedBlock != null);
			CloseItemEditing = new Commands.Command(o =>
			{
				ShowItemEditing = false;
			});
		}
		/// <summary>
		/// Initializes BlockVm commands of vm that can be assigned in this class
		/// </summary>
		/// <param name="vm"></param>
		void initializeCommands(BlockVm vm)
		{
			//Task editor
			vm.AddBlockToEditorCommand = new Commands.Command(o =>
			{
				try
				{
					var ppeBlock = new Editor.BlockEditorVm(vm.Model);
					TaskEditor.BlockList.Add(ppeBlock);
					TaskEditor.SelectedBlock = ppeBlock;
				}
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			}, () => vm.Model != null);
			vm.EditItemCommand = new Commands.Command(o =>
			{
				try
				{
					TaskEditor.Reset();
					TaskEditor.IsVisible = true;
					JobEditor.IsVisible = false;
					vm.AddBlockToEditorCommand.Execute(o);
				}
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			});

			vm.MarkForCopyCommand = new Commands.Command(o =>
			{
				ShowItemEditing = true;
				SelectedBlock = vm;
			});

			//Job editor
			vm.AddJobToEditorCommand = new Commands.Command(o =>
			{
				try { JobEditor.Append(vm.Job); }
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (vm.Job == null) return false;
				if (vm.Job.Id == 0) return false;
				return true;
			});
			vm.EditJobCommand = new Commands.Command(o =>
			{
				try
				{
					TaskEditor.IsVisible = false;
					JobEditor.IsVisible = true;
					JobEditor.Reset();
					vm.AddJobToEditorCommand.Execute(o);
				}
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (vm.Job == null) return false;
				if (vm.Job.Id == 0) return false;
				return true;
			});


			vm.DeleteJobCommand = new Commands.Command(o =>
			{
				lock (PPItems.Manager)
				{
					try
					{
						new DataServices.JobDataService().DeleteModel(vm.Job.Model);
						RemoveBlocks(vm.Job);
					}
					catch (RoutedException exp)
					{
						if (exp.Target is TaskVm)
							(exp.Target as TaskVm).Message.AddEmbeddedException(exp.Message);
						else //if(exp.Target is BlockVm)
							vm.Message.AddEmbeddedException(exp.Message);
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}
			}, () => { return vm.Job != null; });

			//report
			vm.EditReportCommand = new Commands.Command(o =>
			{
				try
				{
					ShowItemEditing = false;
					vm.BlockReport = new Report.BlockReportVm(vm);
					vm.BlockReport.ProcessReportBuilderChanged += val => CurrentProcessReportBuilder = val;
					SelectedBlock = vm;
				}
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			});
			//EditReportCommand reloads *ALL* reports for its block 
			//vm.TaskList.CollectionChanged += (s, e) =>
			//{
			//	if (e.NewItems != null)
			//		foreach (var task in e.NewItems.OfType<TaskVm>())
			//		{
			//			if (task != null)
			//				task.EditReportCommand = new Commands.Command(o =>
			//				{
			//					SelectedBlock = vm;
			//					vm.ReloadReports();
			//				});
			//		}
			//};
		}
		void initializeCommands(NPTVm vm)
		{
			vm.SelectionChanged += (nptVm, isSelected) =>
			{
				if (!isSelected)
					if (SelectedNPT == nptVm) 
						SelectedNPT = null;
			};
			vm.EditItemCommand = new Commands.Command(o =>
			{
				if (SelectedNPT != null)
					SelectedNPT.IsEditMode = false;
				SelectedNPT = vm;
				vm.IsEditMode = true;
			});
		}
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

		/// <summary>
		/// Gets or sets a bindable value that indicates RefreshCommand
		/// </summary>
		public Commands.Command RefreshCommand
		{
			get { return (Commands.Command)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshCommandProperty =
			DependencyProperty.Register("RefreshCommand", typeof(Commands.Command), typeof(PPTableVm), new PropertyMetadata(null));


		//CloseBlockReportCommand Dependency Property
		public Commands.Command CloseBlockReportCommand
		{
			get { return (Commands.Command)GetValue(CloseBlockReportCommandProperty); }
			set { SetValue(CloseBlockReportCommandProperty, value); }
		}
		public static readonly DependencyProperty CloseBlockReportCommandProperty =
			DependencyProperty.Register("CloseBlockReportCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));


		/// <summary>
		/// Gets or sets a bindable value that indicates CopyToCommand
		/// </summary>
		public Commands.Command CopyToCommand
		{
			get { return (Commands.Command)GetValue(CopyToCommandProperty); }
			set { SetValue(CopyToCommandProperty, value); }
		}
		public static readonly DependencyProperty CopyToCommandProperty =
			DependencyProperty.Register("CopyToCommand", typeof(Commands.Command), typeof(PPTableVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates MoveToCommand
		/// </summary>
		public Commands.Command MoveToCommand
		{
			get { return (Commands.Command)GetValue(MoveToCommandProperty); }
			set { SetValue(MoveToCommandProperty, value); }
		}
		public static readonly DependencyProperty MoveToCommandProperty =
			DependencyProperty.Register("MoveToCommand", typeof(Commands.Command), typeof(PPTableVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates CloseItemEditing
		/// </summary>
		public Commands.Command CloseItemEditing
		{
			get { return (Commands.Command)GetValue(CloseItemEditingProperty); }
			set { SetValue(CloseItemEditingProperty, value); }
		}
		public static readonly DependencyProperty CloseItemEditingProperty =
			DependencyProperty.Register("CloseItemEditing", typeof(Commands.Command), typeof(PPTableVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates ShowItemEditing
		/// </summary>
		public bool ShowItemEditing
		{
			get { return (bool)GetValue(ShowItemEditingProperty); }
			set { SetValue(ShowItemEditingProperty, value); }
		}
		public static readonly DependencyProperty ShowItemEditingProperty =
			DependencyProperty.Register("ShowItemEditing", typeof(bool), typeof(PPTableVm), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a bindable value that indicates ItemEditingDate
		/// </summary>
		public DateTime ItemEditingDate
		{
			get { return (DateTime)GetValue(ItemEditingDateProperty); }
			set { SetValue(ItemEditingDateProperty, value); }
		}
		public static readonly DependencyProperty ItemEditingDateProperty =
			DependencyProperty.Register("ItemEditingDate", typeof(DateTime), typeof(PPTableVm), new PropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets a bindable value that indicates ItemEditingTime
		/// </summary>
		public TimeSpan ItemEditingTime
		{
			get { return (TimeSpan)GetValue(ItemEditingTimeProperty); }
			set { SetValue(ItemEditingTimeProperty, value); }
		}
		public static readonly DependencyProperty ItemEditingTimeProperty =
			DependencyProperty.Register("ItemEditingTime", typeof(TimeSpan), typeof(PPTableVm), new PropertyMetadata(TimeSpan.Zero));

		/// <summary>
		/// Gets or sets a bindable value that indicates DailyStationPlanCommand
		/// </summary>
		public Commands.Command DailyStationPlanCommand
		{
			get { return (Commands.Command)GetValue(DailyStationPlanCommandProperty); }
			set { SetValue(DailyStationPlanCommandProperty, value); }
		}
		public static readonly DependencyProperty DailyStationPlanCommandProperty =
			DependencyProperty.Register("DailyStationPlanCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DailyReportCommand
		/// </summary>
		public Commands.Command DailyReportCommand
		{
			get { return (Commands.Command)GetValue(DailyReportCommandProperty); }
			set { SetValue(DailyReportCommandProperty, value); }
		}
		public static readonly DependencyProperty DailyReportCommandProperty =
			DependencyProperty.Register("DailyReportCommand", typeof(Commands.Command), typeof(PPTableVm), new UIPropertyMetadata(null));

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
			DependencyProperty.Register("ShowProductCodes", typeof(bool), typeof(PPTableVm), new UIPropertyMetadata(false));
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
