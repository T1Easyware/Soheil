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

		#region Ctor and Init
		public ReportVm(Model.ProcessReport model, Dal.SoheilEdmContext uow)
		{
			UOW = uow;
			Model = model;

			ActivityName = model.Process.StateStationActivity.Activity.Name;
			TimeRangeText = string.Format("{0} - {1}", 
				model.StartDateTime.TimeOfDay.ToString(@"hh\:mm"),
				model.EndDateTime.TimeOfDay.ToString(@"hh\:mm"));
			OperatorsText = model.Process.ProcessOperators
				.Select(x => x.Operator.Name)
				.Aggregate((current, next) => current + ", " + next);
		}
		#endregion

		#region Methods
		public void Load()
		{
			Operators.Clear();
			foreach (var opr in Model.OperatorProcessReports)
			{
				var oprVm = new OperatorVm(opr);
				oprVm.Updated += o => UOW.Commit();
				Operators.Add(oprVm);
			}
			TargetPointPerOperator = (Model.ProcessReportTargetPoint / Model.OperatorProcessReports.Count).ToString("0.#");
		}
		#endregion

		#region Commands
		#endregion
	}
}
