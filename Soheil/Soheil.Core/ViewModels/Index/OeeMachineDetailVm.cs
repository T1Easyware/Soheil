using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Index;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeMachineDetailVm : DependencyObject
	{
		public event Action Selected;
		public OeeMachineDetailVm()
		{
			SelectCommand = new Commands.Command(o =>
			{
				IsSelected = true;
				if (Selected != null) Selected();
			});
		}
		OeeRecord _data;
		public void Load(OeeRecord data)
		{
			_data = data;
			refresh(data);
		}
		private void refresh(OeeRecord data)
		{
			TimeText = data.TimeRange;
			OEE = data.OEE;
			AvailabilityRate = data.AvailabilityRate;
			EfficiencyRate = data.EfficiencyRate;
			SchedulingRate = data.SchedulingRate;
			QualityRate = data.QualityRate;

			ScheduledTime = new IndexTime(data.MainScheduledTime, data.TotalHours);
			ReworkTime = new IndexTime(data.ReworkScheduledTime, data.TotalHours);
			UnscheduledTime = new IndexTime(data.TotalHours - data.ScheduledTime, data.TotalHours);
			AvailableTime = new IndexTime(data.AvailableTime, data.TotalHours);
			StoppageTime = new IndexTime(data.StoppageTime, data.TotalHours);
			WorkingTime = new IndexTime(data.WorkingTime, data.TotalHours);
			UnreportedTime = new IndexTime(data.UnreportedTime, data.TotalHours);
			IdleTime = new IndexTime(data.IdleTime, data.TotalHours);
			UnsStpTime = new IndexTime(data.UnscheduledTime + data.StoppageTime, data.TotalHours);
			ProductionTime = new IndexTime(data.ProductionTime, data.TotalHours);
			DefectionTime = new IndexTime(data.DefectionTime, data.TotalHours);
			LostTime = new IndexTime(data.TotalHours - data.WorkingTime, data.TotalHours);

			//commands
			DefectionTime.Selected += item =>
			{
				
			};
			StoppageTime.Selected += item =>
			{
				data.LoadStoppageDetails();
				this.StoppageTime.SubItems.Clear();
				foreach (var l1 in data.StoppageDetails)
				{
					var vm1 = new IndexTime(l1.Hours, l1.ParentHours, l1.Text);
					vm1.Selected += i => StoppageTime.CurrentItem = vm1;
					foreach (var l2 in l1.SubRecords)
					{
						var vm2 = new IndexTime(l2.Hours, l2.ParentHours, l2.Text);
						vm2.Selected += i =>
						{
							if (StoppageTime.CurrentItem != null)
								StoppageTime.CurrentItem.CurrentItem = vm2;
						};
						foreach (var l3 in l2.SubRecords)
						{
							var vm3 = new IndexTime(l3.Hours, l3.ParentHours, l3.Text);
							//vm3.Selected += i => vm2.CurrentItem = vm3;
							vm2.SubItems.Add(vm3);
						}
						vm1.SubItems.Add(vm2);
					}
					this.StoppageTime.SubItems.Add(vm1);
				}
			};
		}
		internal void ShowUnreported(bool val)
		{
			if (_data == null) return;
			if (val) refresh(_data);
			else
			{
				var newData = OeeRecord.CreateUnreportedFrom(_data);
				refresh(newData);
			}
		}

		/// <summary>
		/// Gets or sets a bindable string that indicates the Time of this report
		/// </summary>
		public string TimeText
		{
			get { return (string)GetValue(TimeTextProperty); }
			set { SetValue(TimeTextProperty, value); }
		}
		public static readonly DependencyProperty TimeTextProperty =
			DependencyProperty.Register("TimeText", typeof(string), typeof(OeeMachineDetailVm), new PropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this object is selected among other items of timeline
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(OeeMachineDetailVm), new PropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable command that handles the selection of a timeline item in OEE
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(OeeMachineDetailVm), new PropertyMetadata(null));



		#region Rates
		/// <summary>
		/// Gets or sets a bindable value that indicates OEE Rate
		/// </summary>
		public double OEE
		{
			get { return (double)GetValue(OEEProperty); }
			set { SetValue(OEEProperty, value); }
		}
		public static readonly DependencyProperty OEEProperty =
			DependencyProperty.Register("OEE", typeof(double), typeof(OeeMachineDetailVm), new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets a bindable value that indicates SchedulingRate
		/// </summary>
		public double SchedulingRate
		{
			get { return (double)GetValue(SchedulingRateProperty); }
			set { SetValue(SchedulingRateProperty, value); }
		}
		public static readonly DependencyProperty SchedulingRateProperty =
			DependencyProperty.Register("SchedulingRate", typeof(double), typeof(OeeMachineDetailVm), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates AvailabilityRate
		/// </summary>
		public double AvailabilityRate
		{
			get { return (double)GetValue(AvailabilityRateProperty); }
			set { SetValue(AvailabilityRateProperty, value); }
		}
		public static readonly DependencyProperty AvailabilityRateProperty =
			DependencyProperty.Register("AvailabilityRate", typeof(double), typeof(OeeMachineDetailVm), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates EfficiencyRate
		/// </summary>
		public double EfficiencyRate
		{
			get { return (double)GetValue(EfficiencyRateProperty); }
			set { SetValue(EfficiencyRateProperty, value); }
		}
		public static readonly DependencyProperty EfficiencyRateProperty =
			DependencyProperty.Register("EfficiencyRate", typeof(double), typeof(OeeMachineDetailVm), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates QualityRate
		/// </summary>
		public double QualityRate
		{
			get { return (double)GetValue(QualityRateProperty); }
			set { SetValue(QualityRateProperty, value); }
		}
		public static readonly DependencyProperty QualityRateProperty =
			DependencyProperty.Register("QualityRate", typeof(double), typeof(OeeMachineDetailVm), new PropertyMetadata(0d));

		#endregion

		#region Times
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates ScheduledTime
		/// </summary>
		public IndexTime ScheduledTime
		{
			get { return (IndexTime)GetValue(ScheduledTimeProperty); }
			set { SetValue(ScheduledTimeProperty, value); }
		}
		public static readonly DependencyProperty ScheduledTimeProperty =
			DependencyProperty.Register("ScheduledTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates ReworkTime
		/// </summary>
		public IndexTime ReworkTime
		{
			get { return (IndexTime)GetValue(ReworkTimeProperty); }
			set { SetValue(ReworkTimeProperty, value); }
		}
		public static readonly DependencyProperty ReworkTimeProperty =
			DependencyProperty.Register("ReworkTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates UnscheduledTime
		/// </summary>
		public IndexTime UnscheduledTime
		{
			get { return (IndexTime)GetValue(UnscheduledTimeProperty); }
			set { SetValue(UnscheduledTimeProperty, value); }
		}
		public static readonly DependencyProperty UnscheduledTimeProperty =
			DependencyProperty.Register("UnscheduledTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates AvailableTime
		/// </summary>
		public IndexTime AvailableTime
		{
			get { return (IndexTime)GetValue(AvailableTimeProperty); }
			set { SetValue(AvailableTimeProperty, value); }
		}
		public static readonly DependencyProperty AvailableTimeProperty =
			DependencyProperty.Register("AvailableTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates StoppageTime
		/// </summary>
		public IndexTime StoppageTime
		{
			get { return (IndexTime)GetValue(StoppageTimeProperty); }
			set { SetValue(StoppageTimeProperty, value); }
		}
		public static readonly DependencyProperty StoppageTimeProperty =
			DependencyProperty.Register("StoppageTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates WorkingTime
		/// </summary>
		public IndexTime WorkingTime
		{
			get { return (IndexTime)GetValue(WorkingTimeProperty); }
			set { SetValue(WorkingTimeProperty, value); }
		}
		public static readonly DependencyProperty WorkingTimeProperty =
			DependencyProperty.Register("WorkingTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates UnreportedTime
		/// </summary>
		public IndexTime UnreportedTime
		{
			get { return (IndexTime)GetValue(UnreportedTimeProperty); }
			set { SetValue(UnreportedTimeProperty, value); }
		}
		public static readonly DependencyProperty UnreportedTimeProperty =
			DependencyProperty.Register("UnreportedTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates IdleTime
		/// </summary>
		public IndexTime IdleTime
		{
			get { return (IndexTime)GetValue(IdleTimeProperty); }
			set { SetValue(IdleTimeProperty, value); }
		}
		public static readonly DependencyProperty IdleTimeProperty =
			DependencyProperty.Register("IdleTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates UnsStpTime
		/// </summary>
		public IndexTime UnsStpTime
		{
			get { return (IndexTime)GetValue(UnsStpTimeProperty); }
			set { SetValue(UnsStpTimeProperty, value); }
		}
		public static readonly DependencyProperty UnsStpTimeProperty =
			DependencyProperty.Register("UnsStpTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates ProductionTime
		/// </summary>
		public IndexTime ProductionTime
		{
			get { return (IndexTime)GetValue(ProductionTimeProperty); }
			set { SetValue(ProductionTimeProperty, value); }
		}
		public static readonly DependencyProperty ProductionTimeProperty =
			DependencyProperty.Register("ProductionTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates DefectionTime
		/// </summary>
		public IndexTime DefectionTime
		{
			get { return (IndexTime)GetValue(DefectionTimeProperty); }
			set { SetValue(DefectionTimeProperty, value); }
		}
		public static readonly DependencyProperty DefectionTimeProperty =
			DependencyProperty.Register("DefectionTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable IndexTime that indicates LostTime
		/// </summary>
		public IndexTime LostTime
		{
			get { return (IndexTime)GetValue(LostTimeProperty); }
			set { SetValue(LostTimeProperty, value); }
		}
		public static readonly DependencyProperty LostTimeProperty =
			DependencyProperty.Register("LostTime", typeof(IndexTime), typeof(OeeMachineDetailVm), new PropertyMetadata(null));
		#endregion

	}
}
