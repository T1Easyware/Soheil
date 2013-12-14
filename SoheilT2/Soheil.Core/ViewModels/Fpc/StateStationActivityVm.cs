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
		public StateStationActivityVm(FpcWindowVm parentWindowVm, Model.StateStationActivity model)
			: base(parentWindowVm)
		{
			TreeLevel = 2;
			Model = model;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}

		public Model.StateStationActivity Model { get; private set; }

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

		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (ContainerSS.ContainerS.State.InitializingPhase) return;
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationActivityMachineVm;
					if (vm != null) 
						Model.StateStationActivityMachines.Remove(vm.Model);
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationActivityMachineVm;
					if (vm != null)
						Model.StateStationActivityMachines.Add(vm.Model);
				}
			}
		}

		public override void Change()
		{
			ContainerSS.ContainerS.State.IsChanged = true;
		}


		public void AddNewStateStationActivityMachine(FpcWindowVm fpc, MachineVm machine)
		{
			ContentsList.Add(new StateStationActivityMachineVm(fpc, new Soheil.Model.StateStationActivityMachine
			{
				StateStationActivity = this.Model,
				Machine = machine.Model,
				IsFixed = true,
			})
			{
				Container = this,
				Containment = machine,
				IsExpanded = true,
			});
		}
	}
}
