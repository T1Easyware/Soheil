using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateStationActivityMachineVm : TreeItemVm
	{
		public Model.StateStationActivityMachine Model { get; private set; }
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		public StateStationActivityMachineVm(FpcWindowVm parentWindowVm, Model.StateStationActivityMachine model)
			: base(parentWindowVm)
		{
			TreeLevel = 3;
			Model = model;
		}


		//IsDefault Dependency Property
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(StateStationActivityMachineVm),
			new UIPropertyMetadata(true, (d, e) => 
				{
					StateVm.AnyPropertyChangedCallback(((StateStationActivityMachineVm)d).ContainerSSA.ContainerSS.ContainerS.State, e);
					((StateStationActivityMachineVm)d).Model.IsFixed = (bool)e.NewValue;
				}));

		public StateStationActivityVm ContainerSSA { get { return (StateStationActivityVm)base.Container; } set { base.Container = value; } }
		public MachineVm ContainmentMachine { get { return (MachineVm)base.Containment; } set { base.Containment = value; } }
		

		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
				Parent.FocusedStateStation
					= (Container as StateStationActivityVm).Container as StateStationVm;
				Parent.OnStationSelected(Parent.FocusedStateStation);
			} 
		}

		public override void Change()
		{
			ContainerSSA.ContainerSS.ContainerS.State.IsChanged = true;
		}

		public override void Delete()
		{
			Container.ContentsList.Remove(this);
		}
	}
}
