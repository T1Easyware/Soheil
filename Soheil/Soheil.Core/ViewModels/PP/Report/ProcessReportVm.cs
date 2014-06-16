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
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ProcessReportVm : DependencyObject
	{
		/// <summary>
		/// Occurs when the ViewModel needs to reload reports
		/// </summary>
		public event Action LayoutChanged;
		/// <summary>
		/// Occurs when the ViewModel is selected
		/// </summary>
		public event Action<ProcessReportVm> ProcessReportSelected;
	
		#region Members
		/// <summary>
		/// Gets the unit of work
		/// </summary>
		public Dal.SoheilEdmContext UOW { get; protected set; }
		DataServices.ProcessReportDataService _processReportDataService;
		DataServices.TaskReportDataService _taskReportDataService;

		/// <summary>
		/// Gets ProcessReport Model
		/// </summary>
		public Model.ProcessReport Model { get; private set; }



		/// <summary>
		/// Gets ProcessReport Id
		/// </summary>
		public int Id { get { return Model == null ? 0 : Model.Id; } }
		/// <summary>
		/// Gets Process Id
		/// </summary>
		public int ProcessId { get { return Model.Process.Id; } }
		/// <summary>
		/// Gets or sets a value that indicates whether user is dragging thumbs to change datetimes
		/// </summary>
		public bool IsUserDrag { get; set; }

		internal bool _isInInitializingPhase = true;
		#endregion

		#region Ctor and Methods
		/// <summary>
		/// Creates a ViewModel for the given ProcessReport with given row and column
		/// </summary>
		/// <param name="model">if null, it automatically assign unreported process space</param>
		public ProcessReportVm(Model.ProcessReport model, Dal.SoheilEdmContext uow)
		{
			Model = model;
			Message = new EmbeddedException();
			try
			{
				ActivityName = model.Process.StateStationActivity.Activity.Name;
				ProductName = model.Process.StateStationActivity.StateStation.State.FPC.Product.Name;
				ProductColor = model.Process.StateStationActivity.StateStation.State.FPC.Product.Color;
			}
			catch { }

			//uow
			UOW = uow;
			_processReportDataService = new DataServices.ProcessReportDataService(UOW);
			_taskReportDataService = new DataServices.TaskReportDataService(UOW);

			//properties
			//Model.ProducedG1 = Model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);//??? can be different than sum
			ProducedG1 = Model.ProducedG1;
			Timing = new TimingSet(this);
			Timing.Saved += () => Save();
			Timing.DurationChanged += v => Model.DurationSeconds = v;
			Timing.StartChanged += v => Model.StartDateTime = v;
			Timing.EndChanged += v => Model.EndDateTime = v;
			Timing.TargetPointChanged += tp =>
			{
				TargetPointForOperator = Model.OperatorProcessReports.Any() ?
					string.Format("{0:F2}", (float)tp / Model.OperatorProcessReports.Count) :
					"---";
				Model.ProcessReportTargetPoint = tp;
			};
			TargetPointForOperator = Model.OperatorProcessReports.Any() ?
				string.Format("{0:F2}", (float)Model.ProcessReportTargetPoint / Model.OperatorProcessReports.Count) :
				"---";
			//reports
			OperatorReports = new OperatorReportCollection(this);
			DefectionReports = new DefectionReportCollection(this);
			DefectionCount = (int)Model.DefectionReports.Sum(x => x.CountEquivalence);
			StoppageReports = new StoppageReportCollection(this);
			StoppageCount = (int)Model.StoppageReports.Sum(x => x.CountEquivalence);

			IsUserDrag = false;
			_isInInitializingPhase = false;
			initializeCommands();
		}



		/// <summary>
		/// Loads defection reports + stoppage reports + (loads and corrects) operator reports 
		/// </summary>
		public void LoadInnerData()
		{
			var model = _processReportDataService.GetSingleFull(Id);
			if (model != null) Model = model;
			_processReportDataService.CorrectOperatorReports(Model);
			OperatorReports.Reset();
			foreach (var opr in Model.OperatorProcessReports)
			{
				OperatorReports.Add(new OperatorReportVm(opr));
			}
			if (Model.OperatorProcessReports.Any())
				SumOfProducedG1 = Model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);

			DefectionReports.Reset();
			foreach (var def in Model.DefectionReports)
			{
				DefectionReports.List.Add(new DefectionReportVm(DefectionReports, def));
			}
			DefectionCount = (int)Model.DefectionReports.Sum(x => x.CountEquivalence);

			StoppageReports.Reset();
			foreach (var stp in Model.StoppageReports)
			{
				StoppageReports.List.Add(new StoppageReportVm(StoppageReports, stp));
			}
			StoppageCount = (int)Model.StoppageReports.Sum(x => x.CountEquivalence);
		}
		/// <summary>
		/// Saves this viewModel
		/// </summary>
		public void Save(bool onlyCommit = true)
		{
			if (_isInInitializingPhase || IsUserDrag) return;
			
			//attach
			if (Model.Id == 0)
				_processReportDataService.AddModel(Model);
			
			//saving scope
			if (onlyCommit)
				UOW.Commit();
			else
				_processReportDataService.Save(Model);
		}
		#endregion


		#region DepProperties and callbacks
		//Time Dependency Property
		public TimingSet Timing
		{
			get { return (TimingSet)GetValue(TimingProperty); }
			set { SetValue(TimingProperty, value); }
		}
		public static readonly DependencyProperty TimingProperty =
			DependencyProperty.Register("Timing", typeof(TimingSet), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		//ActivityName Dependency Property
		public string ActivityName
		{
			get { return (string)GetValue(ActivityNameProperty); }
			set { SetValue(ActivityNameProperty, value); }
		}
		public static readonly DependencyProperty ActivityNameProperty =
			DependencyProperty.Register("ActivityName", typeof(string), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		//ProductName Dependency Property
		public string ProductName
		{
			get { return (string)GetValue(ProductNameProperty); }
			set { SetValue(ProductNameProperty, value); }
		}
		public static readonly DependencyProperty ProductNameProperty =
			DependencyProperty.Register("ProductName", typeof(string), typeof(ProcessReportVm), new UIPropertyMetadata(null));
		//ProductColor Dependency Property
		public Color ProductColor
		{
			get { return (Color)GetValue(ProductColorProperty); }
			set { SetValue(ProductColorProperty, value); }
		}
		public static readonly DependencyProperty ProductColorProperty =
			DependencyProperty.Register("ProductColor", typeof(Color), typeof(ProcessReportVm), new UIPropertyMetadata(Colors.White));
		/// <summary>
		/// Gets or sets the bindable number of produced Grade 1 products for this process report
		/// </summary>
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(ProcessReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessReportVm)d;
				vm.Model.ProducedG1 = (int)e.NewValue;
				vm.Save();
			}));
		//SumOfProducedG1 Dependency Property
		public int SumOfProducedG1
		{
			get { return (int)GetValue(SumOfProducedG1Property); }
			set { SetValue(SumOfProducedG1Property, value); }
		}
		public static readonly DependencyProperty SumOfProducedG1Property =
			DependencyProperty.Register("SumOfProducedG1", typeof(int), typeof(ProcessReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessReportVm)d;
				if ((int)e.NewValue > vm.ProducedG1) 
					vm.ProducedG1 = (int)e.NewValue;
				vm.Model.ProducedG1 = (int)e.NewValue;
				vm.Save();
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates the TargetPoint for each operator
		/// </summary>
		public string TargetPointForOperator
		{
			get { return (string)GetValue(TargetPointForOperatorProperty); }
			set { SetValue(TargetPointForOperatorProperty, value); }
		}
		public static readonly DependencyProperty TargetPointForOperatorProperty =
			DependencyProperty.Register("TargetPointForOperator", typeof(string), typeof(ProcessReportVm), new UIPropertyMetadata("0"));


		#endregion

		#region Reports
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
		/// Gets or sets the bindable Stoppage reports
		/// </summary>
		public StoppageReportCollection StoppageReports
		{
			get { return (StoppageReportCollection)GetValue(StoppageReportsProperty); }
			set { SetValue(StoppageReportsProperty, value); }
		}
		public static readonly DependencyProperty StoppageReportsProperty =
			DependencyProperty.Register("StoppageReports", typeof(StoppageReportCollection), typeof(ProcessReportVm), new UIPropertyMetadata(null));
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

		#region Other Members
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
		/// <summary>
		/// Gets or sets the bindable error message
		/// </summary>
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(ProcessReportVm), new UIPropertyMetadata(null));

		#endregion


		#region Commands
		void initializeCommands()
		{
			OpenCommand = new Commands.Command(o => IsSelected = true);
			CloseCommand = new Commands.Command(o => IsSelected = false);
			SaveCommand = new Commands.Command(o =>
			{
				Save(o != null);
				IsSelected = false;
				//reload all process reports for the block
				if (LayoutChanged != null) 
					LayoutChanged();
			});
			DeleteCommand = new Commands.Command(o =>
			{
				try
				{
					IsSelected = false;
					_processReportDataService.DeleteModel(Model);
					//reload all process reports for the block
					if (LayoutChanged != null)
						LayoutChanged();
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
		public Commands.Command OpenCommand
		{
			get { return (Commands.Command)GetValue(OpenCommandProperty); }
			set { SetValue(OpenCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenCommandProperty =
			DependencyProperty.Register("OpenCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));
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
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(ProcessReportVm), new UIPropertyMetadata(null));

		#endregion
	}
}