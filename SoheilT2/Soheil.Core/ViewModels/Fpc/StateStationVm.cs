using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateStationVm : TreeItemVm
	{
		public StateStationVm(FpcWindowVm parentWindowVm, Model.StateStation model)
			: base(parentWindowVm)
		{
			TreeLevel = 1;
			Model = model;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}

		public Model.StateStation Model { get; private set; }

		public StateConfigVm ContainerS { get { return (StateConfigVm)base.Container; } set { base.Container = value; } }
		public StationVm ContainmentStation { get { return (StationVm)base.Containment; } set { base.Containment = value; } }
		
		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (ContainerS.State.InitializingPhase) return;
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationActivityVm;
					if (vm != null)
						Model.StateStationActivities.Remove(vm.Model);
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationActivityVm;
					if (vm != null)
						Model.StateStationActivities.Add(vm.Model);
				}
			}
		}

		public override void Change()
		{
			ContainerS.State.IsChanged = true;
		}

		public void AddNewStateStationActivity(FpcWindowVm fpc, ActivityVm activity)
		{
			ContentsList.Add(new StateStationActivityVm(fpc, new Soheil.Model.StateStationActivity
			{
				StateStation = this.Model,
				Activity = activity.Model,
				ManHour = 1,
				CycleTime = 60,
			})
			{
				Container = this,
				Containment = activity,
				IsExpanded = true,
			});
		}
	}
}
