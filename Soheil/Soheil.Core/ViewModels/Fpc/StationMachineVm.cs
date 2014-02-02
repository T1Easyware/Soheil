using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Core.DataServices;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StationMachineVm : ViewModelBase
	{
		public StationMachineVm(Model.StationMachine model, StationVm station)
		{
			Model = model;
			Station = station;
			Machine = new MachineVm(model.Machine, new MachineFamilyVm(model.Machine.MachineFamily));
		}

		public Model.StationMachine Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }

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
