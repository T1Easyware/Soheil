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
		/// <summary>
		/// Gets the state Id
		/// </summary>
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
			IsFixed = state.Model.StateStations.Any(ss => ss.Blocks.Any());
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
					if (item is StateStationVm)
					{
						Parent.fpcDataService.stateDataService.RemoveRecursive((item as StateStationVm).Model);
					}
					else if (item is BomVm)
					{
						Parent.fpcDataService.bomDataService.DeleteModel((item as BomVm).Model);
					}
				}
			}
			//add new state stations
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					if (item is StateStationVm)
					{
						State.Model.StateStations.Add((item as StateStationVm).Model);
					}
					else if (item is BomVm)
					{
						State.Model.BOMs.Add((item as BomVm).Model);
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
			//in DeleteCommand it checks for realtime entity constraints (used Blocks)
			State.DeleteCommand.Execute(null); 
		}

		/// <summary>
		/// Adds the specified station to this State
		/// </summary>
		/// <param name="fpc"></param>
		/// <param name="station"></param>
		public void AddNewStateStation(FpcWindowVm fpc, StationVm station)
		{
			bool isDefault = false;
			if (!ContentsList.Any(x => !x.IsDropIndicator))
			{
				isDefault = true;
				//if no station is already there and no name or code is set, set them to station's
				if (State.Name == "*" && State.Code == "*")
				{
					State.Name = station.Name;
					State.Code = station.Code;
				}
			}

			//create model for StateStation
			var ss = new Soheil.Model.StateStation
			{
				State = this.State.Model,
				Station = station.Model,
				IsDefault = isDefault
			};

			//create vm for StateStation and add it
			int idx = ContentsList.OfType<StateStationVm>().Count();
			ContentsList.Insert(idx, new StateStationVm(fpc, ss)
			{
				Container = this,
				Containment = station,
				IsExpanded = true,
			});
		}

		/// <summary>
		/// Adds the specified rawMaterial to this StateConfig
		/// </summary>
		/// <param name="VM"></param>
		/// <param name="rawMaterial"></param>
		public void AddNewBOM(FpcWindowVm fpc, RawMaterialVm rawMaterial)
		{
			//create model for BOM
			var bom = new Soheil.Model.BOM
			{
				Code = rawMaterial.Code + "." + State.Code,
				Name = rawMaterial.Name,
				IsDefault = true,
				RawMaterial = /*rawMaterialVm.Model???*/ Parent.fpcDataService.rawMaterialDataService.GetRawMaterial__(rawMaterial.Id),
			};
			bom.UnitSet = Parent.fpcDataService.rawMaterialDataService.GetUnitSets(bom.RawMaterial).FirstOrDefault();

			//create vm for BOM and add it
			ContentsList.Add(new BomVm(fpc, bom)
			{
				Container = this,
				Containment = rawMaterial,
				IsExpanded = true,
			});
		}
	}
}
