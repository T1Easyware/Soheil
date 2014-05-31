using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for StateStationActivity inside a StateStation
	/// </summary>
	public class StateStationActivityVm : TreeItemVm
	{
		/// <summary>
		/// Gets the model for this StateStationActivity
		/// </summary>
		public Model.StateStationActivity Model { get; private set; }
		/// <summary>
		/// Gets Id for model of this StateStationActivity
		/// </summary>
		public override int Id { get { return Model == null ? -1 : Model.Id; } }
		
		/// <summary>
		/// Creates a new instance of StateStationActivityVm with given model and parent window
		/// </summary>
		/// <param name="parentWindowVm"></param>
		/// <param name="model">Can't be null</param>
		public StateStationActivityVm(FpcWindowVm parentWindowVm, Model.StateStationActivity model)
			: base(parentWindowVm)
		{
			TreeLevel = 2;
			Model = model;
			IsFixed = model.Processes.Any();
			ContentsList.CollectionChanged += ContentsList_CollectionChanged;
		}

		/// <summary>
		/// Gets or sets a bindable value for CycleTime
		/// </summary>
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(60f, (d, e) => 
				{
					((StateStationActivityVm)d).Model.CycleTime = (float)e.NewValue;
					StateVm.AnyPropertyChangedCallback(d, e);
				}));
		/// <summary>
		/// Gets or sets a bindable value for ManHour
		/// </summary>
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(StateStationActivityVm),
			new UIPropertyMetadata(1f, (d, e) => 
				{
					((StateStationActivityVm)d).Model.ManHour = (float)e.NewValue;
					StateVm.AnyPropertyChangedCallback(d, e);
				}));
		/// <summary>
		/// Gets or sets a bindable value for IsMany
		/// </summary>
		public bool IsMany
		{
			get { return (bool)GetValue(IsManyProperty); }
			set { SetValue(IsManyProperty, value); }
		}
		public static readonly DependencyProperty IsManyProperty =
			DependencyProperty.Register("IsMany", typeof(bool), typeof(StateStationActivityVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				((StateStationActivityVm)d).Model.IsMany = (bool)e.NewValue;
				StateVm.AnyPropertyChangedCallback(d, e);
			}));

		/// <summary>
		/// Gets or sets Container of this StateStationActivity (cast to StateStationVm)
		/// </summary>
		public StateStationVm ContainerSS { get { return (StateStationVm)base.Container; } set { base.Container = value; } }
		/// <summary>
		/// Gets or sets Containment of this StateStationActivity (cast to ActivityVm)
		/// </summary>
		public ActivityVm ContainmentActivity { get { return (ActivityVm)base.Containment; } set { base.Containment = value; } }


		/// <summary>
		/// Updates Model recursively according to the changes made to View model and then saves
		/// <para>Does not take effect when in InitializingPhase</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ContentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (ContainerSS.ContainerS.State.InitializingPhase) return;
			//remove old StateStationActivityMachines and their children
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as StateStationActivityMachineVm;
					if (vm != null)
					{
						Parent.fpcDataService.stateDataService.RemoveRecursive(vm.Model);
					}
				}
			}
			//add new StateStationActivityMachines
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as StateStationActivityMachineVm;
					if (vm != null)
					{
						Model.StateStationActivityMachines.Add(vm.Model);
					}
				}
			}
			Parent.fpcDataService.ApplyChanges();
		}

		/// <summary>
		/// If called with newValue = true, Collapses siblings StateStationActivities of this
		/// And also sets focus to the parent StateStation and selects the parent Station
		/// </summary>
		/// <param name="newValue"></param>
		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				//collapse other StateStationActivities in parent StateStation of this
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
				//set focus to parent StateStation
				Parent.FocusedStateStation = Container as StateStationVm;
				//select parent StateStation
				Parent.OnStationSelected(Parent.FocusedStateStation);
			}
		}

		/// <summary>
		/// Removes this StateStationActivity from its parent StateStation
		/// </summary>
		public override void Delete()
		{
			//check for constraints
			var entity = new Dal.Repository<Model.StateStationActivity>(new Dal.SoheilEdmContext()).Single(x => x.Id == Id);
			if (entity != null && entity.Processes.Any())
			{
				ContainerSS.ContainerS.Parent.Message = new Common.SoheilException.DependencyMessageBox("این فعالیت در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
				return;
			}
			//delete
			Container.ContentsList.Remove(this);
		}

		/// <summary>
		/// Adds the specified machine to this StateStationActivity
		/// </summary>
		/// <param name="fpc"></param>
		/// <param name="machine"></param>
		public void AddNewStateStationActivityMachine(FpcWindowVm fpc, MachineVm machine)
		{
			//create model for StateStationActivityMachine
			var ssam = new Soheil.Model.StateStationActivityMachine
			{
				StateStationActivity = this.Model,
				Machine = /*machine.Model???*/ Parent.fpcDataService.machineFamilyDataService.GetMachine__(machine.Id),
				IsFixed = true,
			};

			//create vm for StateStationActivityMachine and add it
			ContentsList.Add(new StateStationActivityMachineVm(fpc, ssam)
			{
				Container = this,
				Containment = machine,
				IsExpanded = true,
			});
		}
	}
}
