using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StationVm : ViewModelBase, IToolboxData
	{
		public Model.Station Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		
		public StationVm(Model.Station model)
		{
			Model = model;
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
