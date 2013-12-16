using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StateStationActivityVm : DependencyObject
	{
		public StateStationActivityVm(Model.StateStationActivity model)
		{
			Id = model.Id;
			CycleTime = model.CycleTime;
			ManHour = model.ManHour;
			Activity = new ActivityVm(model.Activity);
		}
		public int Id { get; set; }
		//CycleTime Dependency Property
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(StateStationActivityVm), new UIPropertyMetadata(60f));
		//ManHour Dependency Property
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(StateStationActivityVm), new UIPropertyMetadata(1f));
		//Activity Dependency Property
		public ActivityVm Activity
		{
			get { return (ActivityVm)GetValue(ActivityProperty); }
			set { SetValue(ActivityProperty, value); }
		}
		public static readonly DependencyProperty ActivityProperty =
			DependencyProperty.Register("Activity", typeof(ActivityVm), typeof(StateStationActivityVm), new UIPropertyMetadata(null));
	}
}
