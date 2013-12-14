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
		public StateConfigVm(StateVm state, FpcWindowVm parentWindowVm)
			: base(parentWindowVm)
		{
			State = state;
			TreeLevel = 0;
			IsExpanded = true;
			Id = state.Id;
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
						State.Model.StateStations.Remove(vm.Model);
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationVm;
					if (vm != null)
						State.Model.StateStations.Add(vm.Model);
				}
			}
		}

		public override void Change()
		{
			State.IsChanged = true;
		}

		public void AddNewStateStation(FpcWindowVm fpc, StationVm station)
		{
			ContentsList.Add(new StateStationVm(fpc, new Soheil.Model.StateStation
			{
				State = this.State.Model,
				Station = station.Model,
			})
			{
				Container = this,
				Containment = station,
				IsExpanded = true,
			});
		}
	}
}
