using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ProcessReportVm : PPItemVm
	{
		/// <summary>
		/// Occurs when the ViewModel needs to reload reports
		/// </summary>
		public event Action Refresh;
		/// <summary>
		/// Occurs when the ViewModel is selected
		/// </summary>
		public event Action<ProcessReportVm> ProcessReportSelected;

		DataServices.ProcessReportDataService _processReportDataService;
		DataServices.TaskReportDataService _taskReportDataService;
		/// <summary>
		/// Gets ProcessReport Model
		/// </summary>
		public Model.ProcessReport Model { get; private set; }
		/// <summary>
		/// Gets ProcessReport Id
		/// </summary>
		public override int Id { get { return Model == null ? 0 : Model.Id; } }
		/// <summary>
		/// Gets Process Id
		/// </summary>
		public int ProcessId { get { return Model.Process.Id; } }
		/// <summary>
		/// Gets Parent row : ProcessReportRowVm
		/// </summary>
		public ProcessRowVm ParentRow { get; private set; }
		public bool IsUserDrag { get; set; }

		#region Ctor
		/// <summary>
		/// Creates a ViewModel for the given ProcessReport with given row and column
		/// </summary>
		/// <param name="model">if null, it automatically assign unreported process space</param>
		/// <param name="processReportRow">row of the viewModel cell within the report grid</param>
		public ProcessReportVm(Model.ProcessReport model, ProcessRowVm processReportRow)
			:base()
		{
			ParentRow = processReportRow;
			Model = model;

			UOW = new Dal.SoheilEdmContext();
			_processReportDataService = new DataServices.ProcessReportDataService(UOW);
			_taskReportDataService = new DataServices.TaskReportDataService(UOW);

			if (Model == null)
			{
				ViewMode = PPViewMode.Empty;
                ProcessReportTargetPoint = 0;
                    //- processReportRow.ProcessReportCells.Where(y => y.Id > 0).Sum(x => x.ProcessReportTargetPoint);
			}
			else
			{
				ViewMode = PPViewMode.Simple;
				ProducedG1 = Model.ProducedG1;
				ProcessReportTargetPoint = Model.ProcessReportTargetPoint;
				DefectionCount = (int)Model.DefectionReports.Sum(x => x.CountEquivalence);
				StoppageCount = (int)Model.StoppageReports.Sum(x => x.CountEquivalence);
			}

			//DateTimes and Durations
			DurationSeconds = Model.DurationSeconds;
			//by updating StartDateTime (while DurationSeconds is updated before) EndDateTime gets updated correctly
			StartDateTimeChanged += newVal =>
			{
				StartDate = newVal.Date;
				StartTime = newVal.TimeOfDay;
				SetValue(StartDateTimeProperty, newVal);
				SetValue(EndDateTimeProperty, newVal.AddSeconds(DurationSeconds));
			};
			StartDateTime = Model.StartDateTime;
			Model.ProducedG1 = Model.ProcessOperatorReports.Sum(x => x.OperatorProducedG1);//??? can be different than sum
			ProducedG1 = Model.ProducedG1;

			OperatorReports = new OperatorReportCollection(this);
			DefectionReports = new DefectionReportCollection(this);
			StoppageReports = new StoppageReportCollection(this);

			IsUserDrag = false;
			initializeCommands();
		}

		#endregion

		/// <summary>
		/// Loads defection reports + stoppage reports + (loads and corrects) operator reports 
		/// </summary>
		public void LoadInnerData()
		{
			Model = _processReportDataService.GetSingleFull(Id);
			_processReportDataService.CorrectOperatorReports(Model);
			OperatorReports.Reset();
			foreach (var opr in Model.ProcessOperatorReports)
			{
				OperatorReports.Add(new OperatorReportVm(opr));
			}

			DefectionReports.Reset();
			foreach (var def in Model.DefectionReports)
			{
				DefectionReports.List.Add(new DefectionReportVm(DefectionReports, def));
			}

			StoppageReports.Reset();
			foreach (var stp in Model.StoppageReports)
			{
				StoppageReports.List.Add(new StoppageReportVm(StoppageReports, stp));
			}
		}
		/// <summary>
		/// Saves this viewModel
		/// </summary>
		public void Save()
		{
			_processReportDataService.Save(this);
		}


		#region Count
		/// <summary>
		/// Gets or sets the bindable number of target point for this process report
		/// </summary>
		public int ProcessReportTargetPoint
		{
			get { return (int)GetValue(ProcessReportTargetPointProperty); }
			set { SetValue(ProcessReportTargetPointProperty, value); }
		}
		public static readonly DependencyProperty ProcessReportTargetPointProperty =
			DependencyProperty.Register("ProcessReportTargetPoint", typeof(int), typeof(ProcessReportVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets or sets the bindable number of produced Grade 1 products for this process report
		/// </summary>
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(ProcessReportVm), new UIPropertyMetadata(0));

		//the following 2 props are different than DefectionReports & StoppageReports
		//they show the overall equivalent for the progressbar view, but the 2 lists show every detail
		/// <summary>
		/// Gets or sets the bindable total equivalent lost number of defections
		/// </summary>
		public int DefectionCount
		{
			get { return (int)GetValue(DefectionCountProperty); }
			set { SetValue(DefectionCountProperty, value); }
		}
		public static readonly DependencyProperty DefectionCountProperty =
			DependencyProperty.Register("DefectionCount", typeof(int), typeof(ProcessReportVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets or sets the bindable total equivalent lost number of stoppages
		/// </summary>
		public int StoppageCount
		{
			get { return (int)GetValue(StoppageCountProperty); }
			set { SetValue(StoppageCountProperty, value); }
		}
		public static readonly DependencyProperty StoppageCountProperty =
			DependencyProperty.Register("StoppageCount", typeof(int), typeof(ProcessReportVm), new UIPropertyMetadata(0));
		#endregion

		#region DateTime
		/// <summary>
		/// Gets or sets the bindable StartDate of the process report
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(ProcessReportVm), new UIPropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets the bindable StartTime of the process report
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(ProcessReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		//EndDateTime Dependency Property
		public DateTime EndDateTime
		{
			get { return (DateTime)GetValue(EndDateTimeProperty); }
			set { SetValue(EndDateTimeProperty, value); }
		}
		public static readonly DependencyProperty EndDateTimeProperty =
			DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(ProcessReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as ProcessReportVm;
				var val = (DateTime)e.NewValue;
				if (val == null) return;
			}));
		#endregion

		#region Operator,Defection,Stoppage
		/// <summary>
		/// Gets or sets the bindable Operator process reports
		/// </summary>
		public OperatorReportCollection OperatorReports
		{
			get { return (OperatorReportCollection)GetValue(OperatorReportsProperty); }
			set { SetValue(OperatorReportsProperty, value); }
		}
		public static readonly DependencyProperty OperatorReportsProperty =
			DependencyProperty.Register("OperatorReports", typeof(OperatorReportCollection), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable Defection reports
		/// </summary>
		public DefectionReportCollection DefectionReports
		{
			get { return (DefectionReportCollection)GetValue(DefectionReportsProperty); }
			set { SetValue(DefectionReportsProperty, value); }
		}
		public static readonly DependencyProperty DefectionReportsProperty =
			DependencyProperty.Register("DefectionReports", typeof(DefectionReportCollection), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable Stoppage reports
		/// </summary>
		public StoppageReportCollection StoppageReports
		{
			get { return (StoppageReportCollection)GetValue(StoppageReportsProperty); }
			set { SetValue(StoppageReportsProperty, value); }
		}
		public static readonly DependencyProperty StoppageReportsProperty =
			DependencyProperty.Register("StoppageReports", typeof(StoppageReportCollection), typeof(ProcessReportVm), new UIPropertyMetadata(null));

		#endregion

		#region Other Members
		public bool ByEndDate
		{
			get { return (bool)GetValue(ByEndDateProperty); }
			set { SetValue(ByEndDateProperty, value); }
		}
		public static readonly DependencyProperty ByEndDateProperty =
			DependencyProperty.Register("ByEndDate", typeof(bool), typeof(ProcessReportVm), new UIPropertyMetadata(false));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this process report is selected
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProcessReportVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (ProcessReportVm)d;
				var val = (bool)e.NewValue;
				if (vm.ProcessReportSelected != null)
					vm.ProcessReportSelected(val ? vm : null);
				if (val) vm.LoadInnerData();
			}));
		#endregion


		#region Commands
		void initializeCommands()
		{
			OpenReportCommand = new Commands.Command(o => IsSelected = true);
			CloseCommand = new Commands.Command(o => IsSelected = false);
			SaveCommand = new Commands.Command(o =>
			{
				Save();
				IsSelected = false;
				//reload all process reports for the block
				if (Refresh != null) 
					Refresh();
			});
			DeleteProcessReportCommand = new Commands.Command(o =>
			{
				try
				{
					IsSelected = false;
					_processReportDataService.DeleteModel(Model);
					//reload all process reports for the block
					if (Refresh != null)
						Refresh();
				}
				catch(Exception ex)
				{
					Message.AddEmbeddedException(ex);
				}
			});
		}

		/// <summary>
		/// Gets or sets the bindable command to open this report
		/// </summary>
		public Commands.Command OpenReportCommand
		{
			get { return (Commands.Command)GetValue(OpenReportCommandProperty); }
			set { SetValue(OpenReportCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenReportCommandProperty =
			DependencyProperty.Register("OpenReportCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable command to save this report
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable command to close this report
		/// </summary>
		public Commands.Command CloseCommand
		{
			get { return (Commands.Command)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}
		public static readonly DependencyProperty CloseCommandProperty =
			DependencyProperty.Register("CloseCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable command to delete this report
		/// </summary>
		public Commands.Command DeleteProcessReportCommand
		{
			get { return (Commands.Command)GetValue(DeleteProcessReportCommandProperty); }
			set { SetValue(DeleteProcessReportCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteProcessReportCommandProperty =
			DependencyProperty.Register("DeleteProcessReportCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		#endregion
	}
}