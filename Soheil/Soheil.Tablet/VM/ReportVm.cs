using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Common;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Tablet.VM
{
	public class ReportVm : DependencyObject
	{
		#region Properties and Events
		public event Action<ReportVm> Selected;
		public Dal.SoheilEdmContext UOW { get; set; }
		public Model.ProcessReport Model { get; set; }

		/// <summary>
		/// Gets or sets a bindable collection of Operators
		/// </summary>
		public ObservableCollection<OperatorVm> Operators { get { return _operators; } }
		private ObservableCollection<OperatorVm> _operators = new ObservableCollection<OperatorVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates TargetPointPerOperator
		/// </summary>
		public string TargetPointPerOperator
		{
			get { return (string)GetValue(TargetPointPerOperatorProperty); }
			set { SetValue(TargetPointPerOperatorProperty, value); }
		}
		public static readonly DependencyProperty TargetPointPerOperatorProperty =
			DependencyProperty.Register("TargetPointPerOperator", typeof(string), typeof(ReportVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates ProducedTime
		/// </summary>
		public string ProducedTime
		{
			get { return (string)GetValue(ProducedTimeProperty); }
			set { SetValue(ProducedTimeProperty, value); }
		}
		public static readonly DependencyProperty ProducedTimeProperty =
			DependencyProperty.Register("ProducedTime", typeof(string), typeof(ReportVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsSelected
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ReportVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (ReportVm)d;
				if ((bool)e.NewValue)
				{
					if (vm.Selected != null)
						vm.Selected(vm);
					//vm.Load() is called via event above
				}
			}));
		#endregion

		#region Title Properties
		/// <summary>
		/// Gets or sets a bindable value that indicates OperatorsText
		/// </summary>
		public string OperatorsText
		{
			get { return (string)GetValue(OperatorsTextProperty); }
			set { SetValue(OperatorsTextProperty, value); }
		}
		public static readonly DependencyProperty OperatorsTextProperty =
			DependencyProperty.Register("OperatorsText", typeof(string), typeof(ReportVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates ActivityName
		/// </summary>
		public string ActivityName
		{
			get { return (string)GetValue(ActivityNameProperty); }
			set { SetValue(ActivityNameProperty, value); }
		}
		public static readonly DependencyProperty ActivityNameProperty =
			DependencyProperty.Register("ActivityName", typeof(string), typeof(ReportVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates TimeRangeText
		/// </summary>
		public string TimeRangeText
		{
			get { return (string)GetValue(TimeRangeTextProperty); }
			set { SetValue(TimeRangeTextProperty, value); }
		}
		public static readonly DependencyProperty TimeRangeTextProperty =
			DependencyProperty.Register("TimeRangeText", typeof(string), typeof(ReportVm), new UIPropertyMetadata(""));
		#endregion

		#region Basic Properties
		/// <summary>
		/// Gets or sets a bindable value that indicates Start
		/// </summary>
		public TimeSpanBox Start
		{
			get { return (TimeSpanBox)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}
		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(TimeSpanBox), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates End
		/// </summary>
		public TimeSpanBox End
		{
			get { return (TimeSpanBox)GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}
		public static readonly DependencyProperty EndProperty =
			DependencyProperty.Register("End", typeof(TimeSpanBox), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Duration
		/// </summary>
		public TimeSpanBox Duration
		{
			get { return (TimeSpanBox)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpanBox), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsTaskRatio
		/// </summary>
		public bool IsTaskRatio
		{
			get { return (bool)GetValue(IsTaskRatioProperty); }
			set { SetValue(IsTaskRatioProperty, value); }
		}
		public static readonly DependencyProperty IsTaskRatioProperty =
			DependencyProperty.Register("IsTaskRatio", typeof(bool), typeof(ReportVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				var vm = (ReportVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{
					vm.Model.DurationSeconds =
						vm.Model.Process.DurationSeconds * vm.Duration.Seconds / vm.Model.Process.Task.DurationSeconds;
				}
				else
				{
					vm.Model.DurationSeconds = vm.Duration.Seconds;
				}
				vm.Model.EndDateTime = vm.Model.StartDateTime.AddSeconds(vm.Model.DurationSeconds);
			}));
		#endregion

		#region More Properties
		/// <summary>
		/// Gets or sets a bindable value that indicates StoppageReports
		/// </summary>
		public Core.ViewModels.PP.Report.StoppageReportCollection StoppageReports
		{
			get { return (Core.ViewModels.PP.Report.StoppageReportCollection)GetValue(StoppageReportsProperty); }
			set { SetValue(StoppageReportsProperty, value); }
		}
		public static readonly DependencyProperty StoppageReportsProperty =
			DependencyProperty.Register("StoppageReports", typeof(Core.ViewModels.PP.Report.StoppageReportCollection), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DefectionReports
		/// </summary>
		public Core.ViewModels.PP.Report.DefectionReportCollection DefectionReports
		{
			get { return (Core.ViewModels.PP.Report.DefectionReportCollection)GetValue(DefectionReportsProperty); }
			set { SetValue(DefectionReportsProperty, value); }
		}
		public static readonly DependencyProperty DefectionReportsProperty =
			DependencyProperty.Register("DefectionReports", typeof(Core.ViewModels.PP.Report.DefectionReportCollection), typeof(ReportVm), new UIPropertyMetadata(null));
		#endregion

		#region Ctor and Init
		DateTime _defaultStart;
		public ReportVm(Model.ProcessReport entity)
		{
			_defaultStart = entity.StartDateTime;
			UOW = new Dal.SoheilEdmContext();
			var repo = new Dal.Repository<Model.ProcessReport>(UOW);
			Model = repo.Single(x => x.Id == entity.Id);
			if (Model == null)
			{
				var processModel = new Dal.Repository<Model.Process>(UOW).Single(x => x.Id == entity.Process.Id);
				Model = new Soheil.Model.ProcessReport
				{
					Code = entity.Code,
					DurationSeconds = entity.DurationSeconds,
					EndDateTime = entity.EndDateTime,
					Process = processModel,
					ProcessReportTargetPoint = entity.ProcessReportTargetPoint,
					ProducedG1 = entity.ProducedG1,
					StartDateTime = entity.StartDateTime,
					ModifiedBy = entity.ModifiedBy,
				};
				foreach (var opr in entity.OperatorProcessReports)
				{
					Model.OperatorProcessReports.Add(new Model.OperatorProcessReport
					{
						ProcessReport = Model,
						ProcessOperator = processModel.ProcessOperators.FirstOrDefault(x => x.Operator.Id == opr.ProcessOperator.Operator.Id),
						OperatorProducedG1 = opr.OperatorProducedG1,
						ModifiedBy = opr.ModifiedBy
					});
				}
			}
			if (!repo.Exists(x => x.Id == Model.Id))
				repo.Add(Model);

			ActivityName = Model.Process.StateStationActivity.Activity.Name;
			TimeRangeText = string.Format("{0} - {1}",
				Model.StartDateTime.TimeOfDay.ToString(@"hh\:mm"),
				Model.EndDateTime.TimeOfDay.ToString(@"hh\:mm"));
			if (Model.Process.ProcessOperators.Any())
				OperatorsText = Model.Process.ProcessOperators
					.Select(x => x.Operator.Name)
					.Aggregate((current, next) => current + ", " + next);

			Start = new TimeSpanBox(Model.StartDateTime);
			End = new TimeSpanBox(Model.EndDateTime);
			Duration = new TimeSpanBox(Model.DurationSeconds);
			Start.Updated += s =>
			{
				if (Start.DateTime < Model.Process.StartDateTime)
					Start.Update(Model.Process.StartDateTime);
				else
				{
					Model.StartDateTime = Start.DateTime;
					Duration.Update(End.Seconds - s);
					UOW.Commit();
				}
			};
			End.Updated += s =>
			{
				if(End.DateTime > Model.Process.EndDateTime)
					End.Update(Model.Process.EndDateTime);
				else
				{
					Duration.Update(s - Start.Seconds);
					if (IsTaskRatio)
					{
						Model.DurationSeconds =
							Model.Process.DurationSeconds * Duration.Seconds / Model.Process.Task.DurationSeconds;
						Model.EndDateTime = Model.StartDateTime.AddSeconds(Model.DurationSeconds);
					}
					else
					{
						Model.DurationSeconds = Duration.Seconds;
						Model.EndDateTime = End.DateTime;
					}
					UOW.Commit();
				}
			};
			Duration.Updated += s =>
			{
				End.Update(Start.Seconds + s);
				if (IsTaskRatio)
				{
					Model.DurationSeconds =
						Model.Process.DurationSeconds * s / Model.Process.Task.DurationSeconds;
				}
				else
				{
					Model.DurationSeconds = s;
				}
				UOW.Commit();
			};
			initializeCommands();
		}

		#endregion

		#region Methods
		public void Load()
		{
			Operators.Clear();
			foreach (var opr in Model.OperatorProcessReports)
			{
				var oprVm = new OperatorVm(opr);
				oprVm.Updated += o =>
				{
					Model.ProducedG1 = Model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);
					var ts = TimeSpan.FromSeconds(Model.ProducedG1 * Model.Process.StateStationActivity.CycleTime);
					ProducedTime = ts.ToString(@"hh\:mm\:ss");
					UOW.Commit();
				};
				Operators.Add(oprVm);
			}
			TargetPointPerOperator = (Model.ProcessReportTargetPoint / Model.Process.StateStationActivity.ManHour).ToString("0.#");

			StoppageReports = new Core.ViewModels.PP.Report.StoppageReportCollection();
			foreach (var item in Model.StoppageReports)
			{
				var vm = new Core.ViewModels.PP.Report.StoppageReportVm(StoppageReports, item, UOW);
				StoppageReports.List.Add(vm);
			}
			//StoppageReports.CountChanged += sum => StoppageCount = sum;
			StoppageReports.AddCommand = new Soheil.Core.Commands.Command(o =>
			{
				var model = new Model.StoppageReport
				{
					ProcessReport = Model,
					LostCount = 0,
					LostTime = 0,
					Cause = null,
					ModifiedBy = Soheil.Core.LoginInfo.Id,
				};
				var vm = new Soheil.Core.ViewModels.PP.Report.StoppageReportVm(StoppageReports, model, UOW);
				StoppageReports.List.Add(vm);
			});

			DefectionReports = new Core.ViewModels.PP.Report.DefectionReportCollection();
			foreach (var item in Model.DefectionReports)
			{
				var vm = new Core.ViewModels.PP.Report.DefectionReportVm(DefectionReports, item, UOW);
				DefectionReports.List.Add(vm);
			}
			//DefectionReports.CountChanged += sum => StoppageCount = sum;
			DefectionReports.AddCommand = new Soheil.Core.Commands.Command(o =>
			{
				var model = new Model.DefectionReport
				{
					ProcessReport = Model,
					LostCount = 0,
					LostTime = 0,
					ProductDefection = null,
					ModifiedBy = Soheil.Core.LoginInfo.Id,
				};
				Model.DefectionReports.Add(model);
				var vm = new Soheil.Core.ViewModels.PP.Report.DefectionReportVm(DefectionReports, model, UOW);
				DefectionReports.List.Add(vm);
			});
		}
		#endregion

		#region Commands
		void initializeCommands()
		{
			AutoStart = new Core.Commands.Command(o =>
			{
				Start.Update(_defaultStart);
			});
			StartAtHour = new Core.Commands.Command(o =>
			{
				Start.Update(DateTime.Now.TimeOfDay.Hours * 3600);
			});
			EndNow = new Core.Commands.Command(o =>
			{
				End.Update(DateTime.Now);
			});
			EndAtHour = new Core.Commands.Command(o =>
			{
				End.Update(DateTime.Now.TimeOfDay.Hours * 3600);
			});
			EndAtEnd = new Core.Commands.Command(o =>
			{
				IsTaskRatio = false;
				End.Update(Model.Process.EndDateTime);
			});
			HalfHour = new Core.Commands.Command(o =>
			{
				Duration.Update(1800);
			});
			OneHour = new Core.Commands.Command(o =>
			{
				Duration.Update(3600);
			});
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates AutoStart
		/// </summary>
		public Core.Commands.Command AutoStart
		{
			get { return (Core.Commands.Command)GetValue(AutoStartProperty); }
			set { SetValue(AutoStartProperty, value); }
		}
		public static readonly DependencyProperty AutoStartProperty = 
			DependencyProperty.Register("AutoStart", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates StartAtHour
		/// </summary>
		public Core.Commands.Command StartAtHour
		{
			get { return (Core.Commands.Command)GetValue(StartAtHourProperty); }
			set { SetValue(StartAtHourProperty, value); }
		}
		public static readonly DependencyProperty StartAtHourProperty =
			DependencyProperty.Register("StartAtHour", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndNow
		/// </summary>
		public Core.Commands.Command EndNow
		{
			get { return (Core.Commands.Command)GetValue(EndNowProperty); }
			set { SetValue(EndNowProperty, value); }
		}
		public static readonly DependencyProperty EndNowProperty =
			DependencyProperty.Register("EndNow", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndAtHour
		/// </summary>
		public Core.Commands.Command EndAtHour
		{
			get { return (Core.Commands.Command)GetValue(EndAtHourProperty); }
			set { SetValue(EndAtHourProperty, value); }
		}
		public static readonly DependencyProperty EndAtHourProperty =
			DependencyProperty.Register("EndAtHour", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndAtEnd
		/// </summary>
		public Core.Commands.Command EndAtEnd
		{
			get { return (Core.Commands.Command)GetValue(EndAtEndProperty); }
			set { SetValue(EndAtEndProperty, value); }
		}
		public static readonly DependencyProperty EndAtEndProperty =
			DependencyProperty.Register("EndAtEnd", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates HalfHour
		/// </summary>
		public Core.Commands.Command HalfHour
		{
			get { return (Core.Commands.Command)GetValue(HalfHourProperty); }
			set { SetValue(HalfHourProperty, value); }
		}
		public static readonly DependencyProperty HalfHourProperty =
			DependencyProperty.Register("HalfHour", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates OneHour
		/// </summary>
		public Core.Commands.Command OneHour
		{
			get { return (Core.Commands.Command)GetValue(OneHourProperty); }
			set { SetValue(OneHourProperty, value); }
		}
		public static readonly DependencyProperty OneHourProperty =
			DependencyProperty.Register("OneHour", typeof(Core.Commands.Command), typeof(ReportVm), new UIPropertyMetadata(null));
		#endregion

	}
}
