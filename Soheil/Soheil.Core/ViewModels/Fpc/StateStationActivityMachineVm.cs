using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for StateStationActivityMachine inside a StateStationActivityVm
	/// </summary>
	public class StateStationActivityMachineVm : TreeItemVm
	{
		/// <summary>
		/// Gets the model for this StateStationActivityMachine
		/// </summary>
		public Model.StateStationActivityMachine Model { get; private set; }
		/// <summary>
		/// Gets Id for model of this StateStationActivityMachine
		/// </summary>
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		/// <summary>
		/// Creates a new instance of StateStationActivityMachineVm with given model and parent window
		/// </summary>
		/// <param name="parentWindowVm"></param>
		/// <param name="model">Can't be null</param>
		public StateStationActivityMachineVm(FpcWindowVm parentWindowVm, Model.StateStationActivityMachine model)
			: base(parentWindowVm)
		{
			TreeLevel = 3;
			Model = model;
			IsFixed = model.SelectedMachines.Any();
		}


		/// <summary>
		/// Gets or sets a bindable value that indicates whether this is a default machine in tasks
		/// </summary>
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(StateStationActivityMachineVm),
			new UIPropertyMetadata(true, (d, e) => 
				{
					((StateStationActivityMachineVm)d).Model.IsFixed = (bool)e.NewValue;
					StateVm.AnyPropertyChangedCallback(((StateStationActivityMachineVm)d).ContainerSSA.ContainerSS.ContainerS.State, e);
				}));

		/// <summary>
		/// Gets or sets Container of this StateStationActivityMachine (cast to StateStationActivityVm)
		/// </summary>
		public StateStationActivityVm ContainerSSA { get { return (StateStationActivityVm)base.Container; } set { base.Container = value; } }
		/// <summary>
		/// Gets or sets Containment of this StateStationActivityMachine (cast to MachineVm)
		/// </summary>
		public MachineVm ContainmentMachine { get { return (MachineVm)base.Containment; } set { base.Containment = value; } }

		/// <summary>
		/// If called with newValue = true, Collapses siblings StateStationActivityMachines of this
		/// And also sets focus to the parent StateStation and selects the parent Station
		/// </summary>
		/// <param name="newValue"></param>
		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				//collapse other StateStationActivityMachines in parent StateStationActivity of this
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
				//set focus to parent StateStation
				Parent.FocusedStateStation
					= (Container as StateStationActivityVm).Container as StateStationVm;
				//select parent StateStation
				Parent.OnStationSelected(Parent.FocusedStateStation);
			} 
		}

		/// <summary>
		/// Removes this StateStationActivityMachine from its parent StateStationActivity
		/// </summary>
		public override void Delete()
		{
			//check for constraints
			var entity = new Dal.Repository<Model.StateStationActivityMachine>(new Dal.SoheilEdmContext()).Single(x => x.Id == Id);
			if (entity != null && entity.SelectedMachines.Any())
			{
				ContainerSSA.ContainerSS.ContainerS.Parent.Message = new Common.SoheilException.DependencyMessageBox("این ماشین در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
				return;
			}
			//delete
			Container.ContentsList.Remove(this);
		}
	}
}
