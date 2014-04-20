using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for StateStation inside a StateConfigVm
	/// </summary>
	public class StateStationVm : TreeItemVm
	{
		/// <summary>
		/// Gets the model for this StateStation
		/// </summary>
		public Model.StateStation Model { get; private set; }
		/// <summary>
		/// Gets Id for model of this StateStation
		/// </summary>
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		/// <summary>
		/// Creates a new instance of StateStationVm with given model and parent window
		/// </summary>
		/// <param name="parentWindowVm"></param>
		/// <param name="model">Can't be null</param>
		public StateStationVm(FpcWindowVm parentWindowVm, Model.StateStation model)
			: base(parentWindowVm)
		{
			TreeLevel = 1;
			Model = model;
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}

		/// <summary>
		/// Gets or sets Container of this StateStation (cast to StateConfigVm)
		/// </summary>
		public StateConfigVm ContainerS { get { return (StateConfigVm)base.Container; } set { base.Container = value; } }
		/// <summary>
		/// Gets or sets Containment of this StateStation (cast to StationVm)
		/// </summary>
		public StationVm ContainmentStation { get { return (StationVm)base.Containment; } set { base.Containment = value; } }

		/// <summary>
		/// Updates Model recursively according to the changes made to View model and then saves
		/// <para>Does not take effect when in InitializingPhase</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (ContainerS.State.InitializingPhase) return;
			//remove old StateStationActivities and their children
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationActivityVm;
					if (vm != null)
					{
						Parent.fpcDataService.stateDataService.RemoveRecursive(vm.Model);
					}
				}
			}
			//add new StateStationActivities
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationActivityVm;
					if (vm != null)
					{
						Model.StateStationActivities.Add(vm.Model);
					}
				}
			}
			Parent.fpcDataService.ApplyChanges();
		}

		/// <summary>
		/// If called with newValue = true, Collapses siblings StateStations of this
		/// And also sets focus to this StateStation and selects this Station
		/// </summary>
		/// <param name="newValue"></param>
		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				//set focus to this StateStation
				Parent.FocusedStateStation = this;
				//select this StateStation
				Parent.OnStationSelected(Parent.FocusedStateStation);
				//collapse other StateStations in parent State of this
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
			}
		}


		/// <summary>
		/// Removes this StateStation from State
		/// </summary>
		public override void Delete()
		{
			if (this.Model.Blocks.Any())
			{
				ContainerS.Parent.Message = new Common.SoheilException.DependencyMessageBox("این مرحله (همین ایستگاه) در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
				return;
			}
			Container.ContentsList.Remove(this);
		}

		/// <summary>
		/// Adds the specified activity to this StateStation
		/// </summary>
		/// <param name="fpc"></param>
		/// <param name="activity"></param>
		public void AddNewStateStationActivity(FpcWindowVm fpc, ActivityVm activity)
		{
			//create model for StateStationActivity
			var ssa = new Soheil.Model.StateStationActivity
			{
				StateStation = this.Model,
				Activity = activity.Model,
				ManHour = 1,
				CycleTime = 60,
			};

			//create vm for StateStationActivity and add it
			ContentsList.Add(new StateStationActivityVm(fpc, ssa)
			{
				Container = this,
				Containment = activity,
				IsExpanded = true,
			});
		}
	}
}
