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
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
		}
		
		/// <summary>
		/// Creates an instance of StationVm filled with its active StationMachines
		/// </summary>
		/// <param name="model"></param>
		public StationVm(Model.Station model)
		{
			Model = model;
			foreach (var sm in model.StationMachines.Where(x => x.Status == (int)Common.Status.Active))
			{
				StationMachines.Add(new StationMachineVm(sm, this));
			}
		}
		/// <summary>
		/// A bindable collection of active StationMachines that are associated with this Station
		/// </summary>
		public ObservableCollection<StationMachineVm> StationMachines { get { return _stationMachines; } }
		private ObservableCollection<StationMachineVm> _stationMachines = new ObservableCollection<StationMachineVm>();
	}
}
