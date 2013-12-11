using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Core.DataServices;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StationMachineVm : ViewModel
	{
		public StationMachineVm()
		{

		}
		public StationMachineVm(Model.StationMachine model, StationVm station)
		{
			Station = station;
			var machineDs = new MachineDataService();
			var machineFamilyDs = new MachineFamilyDataService();
			var machineModel = machineDs.GetSingleWithFamily(model.Machine.Id);
			var machineFamilyModel = machineFamilyDs.GetSingle(machineModel.MachineFamily.Id);
			Machine = new MachineVm(machineModel, new MachineFamilyVm(machineFamilyModel));
		}
		//Station Dependency Property
		public StationVm Station
		{
			get { return (StationVm)GetValue(StationProperty); }
			set { SetValue(StationProperty, value); }
		}
		public static readonly DependencyProperty StationProperty =
			DependencyProperty.Register("Station", typeof(StationVm), typeof(StationMachineVm), new UIPropertyMetadata(null));
		//Machine Dependency Property
		public MachineVm Machine
		{
			get { return (MachineVm)GetValue(MachineProperty); }
			set { SetValue(MachineProperty, value); }
		}
		public static readonly DependencyProperty MachineProperty =
			DependencyProperty.Register("Machine", typeof(MachineVm), typeof(StationMachineVm), new UIPropertyMetadata(null));
	}
}
