using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateStationActivityVm : TreeItemVm
	{
		public StateStationActivityVm(FpcWindowVm parentWindowVm)
			: base(parentWindowVm)
		{
			TreeLevel = 2;
		}
		//CycleTime Dependency Property
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(60f, (d, e) => StateVm.AnyPropertyChangedCallback(((StateStationActivityVm)d).ContainerSS.ContainerS.State, e)));
		//ManHour Dependency Property
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(1f, (d, e) => StateVm.AnyPropertyChangedCallback(((StateStationActivityVm)d).ContainerSS.ContainerS.State, e)));

		public StateStationVm ContainerSS { get { return (StateStationVm)base.Container; } set { base.Container = value; } }
		public ActivityVm ContainmentActivity { get { return (ActivityVm)base.Containment; } set { base.Containment = value; } }

		public override void Change()
		{
			ContainerSS.ContainerS.State.IsChanged = true;
		}
	}
}
