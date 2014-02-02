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
		public Model.StateStation Model { get; private set; }
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		public StateStationVm(FpcWindowVm parentWindowVm, Model.StateStation model)
			: base(parentWindowVm)
		{
			TreeLevel = 1;
			Model = model;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}


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
					var vm = item as StateStationActivityVm;
					if (vm != null)
					{
						Model.StateStationActivities.Add(vm.Model);
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
				Parent.FocusedStateStation = this;
				Parent.OnStationSelected(Parent.FocusedStateStation);
			}
		}

		public override void Change()
		{
			ContainerS.State.IsChanged = true;
		}

		public override void Delete()
		{
			Container.ContentsList.Remove(this);
		}

		public void AddNewStateStationActivity(FpcWindowVm fpc, ActivityVm activity)
		{
			var ssa = new Soheil.Model.StateStationActivity
			{
				StateStation = this.Model,
				Activity = activity.Model,
				ManHour = 1,
				CycleTime = 60,
			};
			ContentsList.Add(new StateStationActivityVm(fpc, ssa)
			{
				Container = this,
				Containment = activity,
				IsExpanded = true,
			});
		}
	}
}
