using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StationVm : NamedVM
	{
		public Model.Station Model { get; private set; }
		
		public StationVm(Model.Station model)
		{
			Model = model;
			Id = model.Id;
			Name = model.Name;
			foreach (var sm in model.StationMachines)
			{
				StationMachines.Add(new StationMachineVm(sm, this));
			}
		}
		//Station-Machines Observable Collection
		private ObservableCollection<StationMachineVm> _stationMachines = new ObservableCollection<StationMachineVm>();
		public ObservableCollection<StationMachineVm> StationMachines { get { return _stationMachines; } }

	}
}
