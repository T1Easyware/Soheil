using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for configurations of a state (StateStations and their children)
	/// </summary>
	public class StateConfigVm : TreeItemVm
	{
		public override int Id { get { return State.Model == null ? -1 : State.Model.Id; } }

		/// <summary>
		/// Creates a configuration tree item (level0) for this state
		/// </summary>
		/// <param name="state">parent. can't be null</param>
		public StateConfigVm(StateVm state)
			: base(state.ParentWindowVm)
		{
			State = state;
			TreeLevel = 0;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}
		
		/// <summary>
		/// State : Container of this view model
		/// </summary>
		public StateVm State
		{
			get { return (StateVm)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(StateVm), typeof(StateConfigVm), new UIPropertyMetadata(null, (d, e) =>
						d.SetValue(NameProperty, ((StateVm)e.NewValue).Name)));

		/// <summary>
		/// Updates Model recursively according to the changes made to View model and then saves
		/// <para>Does not take effect when in InitializingPhase</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (State.InitializingPhase) return;

			//remove old StateStations and their children
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationVm;
					if (vm != null)
					{
						Parent.fpcDataService.stateDataService.RemoveRecursive(vm.Model);
					}
				}
			}
			//add new state stations
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationVm;
					if (vm != null)
					{
						State.Model.StateStations.Add(vm.Model);
					}
				}
			}
			Parent.fpcDataService.ApplyChanges();
		}

		/// <summary>
		/// Shows or hides the details
		/// </summary>
		/// <param name="newValue"></param>
		protected override void isExpandedChanged(bool newValue)
		{
			State.ShowDetails = newValue;
		}

		/// <summary>
		/// Executes DeleteCommand on state
		/// </summary>
		public override void Delete()
		{
			if (this.State.Model.StateStations.Any(ss=>ss.Blocks.Any()))
			{
				Parent.Message = new Common.SoheilException.DependencyMessageBox("این مرحله (بعضی از ایستگاهها) در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
				return;
			}
			State.DeleteCommand.Execute(null); 
		}

		/// <summary>
		/// Adds the specified station to this State
		/// </summary>
		/// <param name="fpc"></param>
		/// <param name="station"></param>
		public void AddNewStateStation(FpcWindowVm fpc, StationVm station)
		{
			//if no station is already there and no name or code is set, set them to station's
			if (!ContentsList.Any(x => !x.IsDropIndicator) && State.Name == "*" && State.Code == "*")
			{
				State.Name = station.Name;
				State.Code = station.Code;
			}

			//create model for StateStation
			var ss = new Soheil.Model.StateStation
			{
				State = this.State.Model,
				Station = station.Model,
			};

			//create vm for StateStation and add it
			ContentsList.Add(new StateStationVm(fpc, ss)
			{
				Container = this,
				Containment = station,
				IsExpanded = true,
			});
		}
	}
}
