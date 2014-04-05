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
		public Model.StateStationActivity Model { get; private set; }
		public override int Id { get { return Model == null ? -1 : Model.Id; } }
		
		public StateStationActivityVm(FpcWindowVm parentWindowVm, Model.StateStationActivity model)
			: base(parentWindowVm)
		{
			TreeLevel = 2;
			Model = model;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}

		//CycleTime Dependency Property
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(60f, (d, e) => 
				{
					StateVm.AnyPropertyChangedCallback(((StateStationActivityVm)d).ContainerSS.ContainerS.State, e);
					((StateStationActivityVm)d).Model.CycleTime = (float)e.NewValue;
				}));
		//ManHour Dependency Property
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(1f, (d, e) => 
				{
					StateVm.AnyPropertyChangedCallback(((StateStationActivityVm)d).ContainerSS.ContainerS.State, e);
					((StateStationActivityVm)d).Model.ManHour = (float)e.NewValue;
				}));

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
					{
						Parent.fpcDataService.stateDataService.RemoveRecursive(vm.Model);
						Change();
					}
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationActivityMachineVm;
					if (vm != null)
					{
						Model.StateStationActivityMachines.Add(vm.Model);
						Change();
					}
				}
			}
		}

		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
				Parent.FocusedStateStation = Container as StateStationVm;
				Parent.OnStationSelected(Parent.FocusedStateStation);
			}
		}

		public override void Change()
		{
			ContainerSS.ContainerS.State.IsChanged = true;
		}

		public override void Delete()
		{
			Container.ContentsList.Remove(this);
		}

		public void AddNewStateStationActivityMachine(FpcWindowVm fpc, MachineVm machine)
		{
			var ssam = new Soheil.Model.StateStationActivityMachine
			{
				StateStationActivity = this.Model,
				Machine = /*machine.Model???*/ Parent.fpcDataService.machineFamilyDataService.GetMachine__(machine.Id),
				IsFixed = true,
			};
			ContentsList.Add(new StateStationActivityMachineVm(fpc, ssam)
			{
				Container = this,
				Containment = machine,
				IsExpanded = true,
			});
		}
	}
}
