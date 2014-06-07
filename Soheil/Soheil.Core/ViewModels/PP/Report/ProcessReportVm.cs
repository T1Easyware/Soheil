﻿using System;
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

		public ProcessReportVm PreviousReport { get; set; }
		public ProcessReportVm NextReport { get; set; }
		public DateTime LowerBound { get { return PreviousReport == null ? StartDateTime : PreviousReport.EndDateTime; } }
		public DateTime UpperBound { get { return NextReport == null ? EndDateTime : NextReport.StartDateTime; } }


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

		protected bool _isInInitializingPhase = true;
		#endregion

		#region Ctor and Methods
		/// <summary>
		/// Creates a ViewModel for the given ProcessReport with given row and column
		/// </summary>
		/// <param name="model">if null, it automatically assign unreported process space</param>
		public ProcessReportVm(Model.ProcessReport model)
		{
			Model = model;
			Message = new EmbeddedException();

			//uow
			UOW = new Dal.SoheilEdmContext();
			_processReportDataService = new DataServices.ProcessReportDataService(UOW);
			_taskReportDataService = new DataServices.TaskReportDataService(UOW);

			//properties
			Model.ProducedG1 = Model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);//??? can be different than sum
			ProducedG1 = Model.ProducedG1;
			TargetPoint = Model.ProcessReportTargetPoint;
			DurationSeconds = Model.DurationSeconds;
			StartDateTime = Model.StartDateTime;
			EndDateTime = Model.EndDateTime;

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
			Model.ProducedG1 = Model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);//??? can be different than sum

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
		public void Save()
		{
			_processReportDataService.Save(this);
		}
		#endregion


		#region DpProperties and callbacks
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
			}));

		/// <summary>
		/// Gets or sets the bindable number of target point for this process report
		/// </summary>
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(ProcessReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessReportVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (int)e.NewValue;
				vm.Model.ProcessReportTargetPoint = val;
				vm.DurationSeconds = (int)vm.Model.Process.StateStationActivity.CycleTime * val;
			}, (d, v) =>
			{
				var vm = (ProcessReportVm)d;
				var val = (int)v;
				if (val < 1) return 1;
				return v;
			}));
		/// <summary>
		/// Gets or sets the bindable duration seconds
		/// </summary>
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(ProcessReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(ProcessReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));
				((ProcessReportVm)d).ProcessReportVm_DurationSecondsChanged((int)e.NewValue);
			}, (d, v) =>
			{
				var vm = (ProcessReportVm)d;
				var val = (TimeSpan)v;
				if (val.TotalSeconds < vm.Model.Process.StateStationActivity.CycleTime)
					return vm.Model.Process.StateStationActivity.CycleTime;
				if(vm.EndDateTime - vm.StartDateTime < val)
				{
					return vm.EndDateTime - vm.StartDateTime;
				}
				return v;
			}));
		void ProcessReportVm_DurationSecondsChanged(int newVal)
		{
			if (!_isInInitializingPhase)
			{
				var tp = (int)(newVal / Model.Process.StateStationActivity.CycleTime);
				var duration = (int)Model.Process.StateStationActivity.CycleTime * TargetPoint;
				Model.DurationSeconds = duration;

				TargetPoint = tp;
				EndDateTime = StartDateTime.AddSeconds(duration);
			}
		}

		#region Start
		/// <summary>
		/// Gets or sets the bindable Start dateTime
		/// </summary>
		public DateTime StartDateTime
		{
			get { return (DateTime)GetValue(StartDateTimeProperty); }
			set { SetValue(StartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty StartDateTimeProperty =
			DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(ProcessReportVm),
			new UIPropertyMetadata(DateTime.Now,
				(d, e) => ((ProcessReportVm)d).ProcessReportVm_StartDateTimeChanged((DateTime)e.NewValue),
				(d, v) =>
				{
					var vm = (ProcessReportVm)d;
					var val = (DateTime)v;
					if (val < vm.LowerBound)
						return vm.LowerBound;
					return v;
				}));
		void ProcessReportVm_StartDateTimeChanged(DateTime newVal)
		{
			if (!_isInInitializingPhase)
			{
				Model.StartDateTime = newVal;
				_isInInitializingPhase = true;
				EndDateTime = newVal.AddSeconds(DurationSeconds);
				_isInInitializingPhase = false;
			}

			//update only startTime & startDate
			_isInInitializingPhase = true;
			SetValue(StartTimeProperty, newVal.TimeOfDay);
			SetValue(StartDateProperty, newVal.Date);
			_isInInitializingPhase = false;
		}

		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(ProcessReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as ProcessReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (DateTime)e.NewValue;
				vm.StartDateTime = val.Add((TimeSpan)d.GetValue(StartTimeProperty));
				vm.DurationSeconds = (int)(vm.Model.EndDateTime - vm.Model.StartDateTime).TotalSeconds;
			}));
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(ProcessReportVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as ProcessReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (TimeSpan)e.NewValue;
				vm.StartDateTime = ((DateTime)d.GetValue(StartTimeProperty)).Add(val);
				vm.DurationSeconds = (int)(vm.Model.EndDateTime - vm.Model.StartDateTime).TotalSeconds;
			})); 
		#endregion

		#region End
		public DateTime EndDateTime
		{
			get
			{
				return Model.EndDateTime;
			}
			set
			{
				//coerce value
				if (value > UpperBound)
				{
					value = UpperBound;
				}

				//update model
				if (!_isInInitializingPhase)
					Model.EndDateTime = value;

				//update endTime & endDate
				_isInInitializingPhase = true;
				SetValue(EndTimeProperty, value.TimeOfDay);
				SetValue(EndDateProperty, value.Date);
				_isInInitializingPhase = false;

				//update duration
				DurationSeconds = (int)(value - Model.StartDateTime).TotalSeconds;
			}
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(ProcessReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as ProcessReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (DateTime)e.NewValue;
				vm.EndDateTime = val.Add((TimeSpan)d.GetValue(EndTimeProperty));
			}));
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(ProcessReportVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as ProcessReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (TimeSpan)e.NewValue;
				vm.EndDateTime = ((DateTime)d.GetValue(EndDateProperty)).Add(val);
			})); 
		#endregion

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
			OpenReportCommand = new Commands.Command(o => IsSelected = true);
			CloseCommand = new Commands.Command(o => IsSelected = false);
			SaveCommand = new Commands.Command(o =>
			{
				Save();
				IsSelected = false;
				//reload all process reports for the block
				if (LayoutChanged != null) 
					LayoutChanged();
			});
			DeleteProcessReportCommand = new Commands.Command(o =>
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