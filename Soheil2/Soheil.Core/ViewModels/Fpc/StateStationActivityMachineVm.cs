using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateStationActivityMachineVm : TreeItemVm
	{
		public StateStationActivityMachineVm(FpcWindowVm parentWindowVm)
			: base(parentWindowVm)
		{
			TreeLevel = 3;
		}
		//IsDefault Dependency Property
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(StateStationActivityMachineVm),
			new UIPropertyMetadata(true, (d, e) => StateVm.AnyPropertyChangedCallback(((StateStationActivityMachineVm)d).ContainerSSA.ContainerSS.ContainerS.State, e)));

		public StateStationActivityVm ContainerSSA { get { return (StateStationActivityVm)base.Container; } set { base.Container = value; } }
		public MachineVm ContainmentMachine { get { return (MachineVm)base.Containment; } set { base.Containment = value; } }

		public override void Change()
		{
			ContainerSSA.ContainerSS.ContainerS.State.IsChanged = true;
		}
	}
}
