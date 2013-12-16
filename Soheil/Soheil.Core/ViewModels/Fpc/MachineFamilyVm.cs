using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc
{
	public class MachineFamilyVm : NamedVM
	{
		public MachineFamilyVm()
		{

		}
		public MachineFamilyVm(Model.MachineFamily model)
		{
			Id = model.Id;
			Name = model.Name;
			/*foreach (var machine in model.Machines)
			{
				Machines.Add(new MachineVm(machine, this));
			}*/
		}
		//Machines Observable Collection
		private ObservableCollection<MachineVm> _machines = new ObservableCollection<MachineVm>();
		public ObservableCollection<MachineVm> Machines { get { return _machines; } }
	}
}
