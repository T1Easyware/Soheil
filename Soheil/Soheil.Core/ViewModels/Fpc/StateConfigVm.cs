using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateConfigVm : TreeItemVm
	{
		public override int Id { get { return State.Model == null ? -1 : State.Model.Id; } }

		public StateConfigVm(StateVm state)
			: base(state.ParentWindowVm)
		{
			State = state;
			TreeLevel = 0;
			IsExpanded = true;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}
		//State Dependency Property
		public StateVm State
		{
			get { return (StateVm)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(StateVm), typeof(StateConfigVm), new UIPropertyMetadata(null, (d, e) =>
						d.SetValue(NameProperty, ((StateVm)e.NewValue).Name)));

		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (State.InitializingPhase) return;
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationVm;
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
					var vm = item as StateStationVm;
					if (vm != null)
					{
						State.Model.StateStations.Add(vm.Model);
						Change();
					}
				}
			}
		}

		protected override void isExpandedChanged(bool newValue)
		{
			State.ShowDetails = newValue;
		}

		public override void Change()
		{
			State.IsChanged = true;
		}

		public override void Delete()
		{
			State.DeleteCommand.Execute(null); 
		}

		public void AddNewStateStation(FpcWindowVm fpc, StationVm station)
		{
			var ss = new Soheil.Model.StateStation
			{
				State = this.State.Model,
				Station = station.Model,
			};
			ContentsList.Add(new StateStationVm(fpc, ss)
			{
				Container = this,
				Containment = station,
				IsExpanded = true,
			});
		}
	}
}
