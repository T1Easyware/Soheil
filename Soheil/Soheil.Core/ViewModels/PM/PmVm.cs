using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Interfaces;
using Soheil.Common;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PM
{
	public class PmVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }
		public DataServices.MachineDataService MachineDataService { get; set; }
		public DataServices.MachinePartDataService MachinePartDataService { get; set; }
		public PmVm(AccessType access)
		{
			Access = access;

			#region Create pages
			//0
			var pg_machines = new PmPageBase("ماشین ها");
			//1
			var pg_machineParts = new PmPageBase("قطعات ماشین");
			//2
			var pg_parts = new PmPageBase("کلیه قطعات");
			var pg_macPartMaintenances = new PmPageBase("نگهداری");
			var pg_repairs = new PmPageBase("تعمیرات");
			//3
			var pg_maintenances = new PmPageBase("کلیه نگهداری ها");
			var pg_maintenanceReports = new PmPageBase("گزارش PM");

			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels[0].Pages.Add(pg_machines);
			Levels[1].Pages.Add(pg_machineParts);
			Levels[2].Pages.Add(pg_parts);
			Levels[2].Pages.Add(pg_macPartMaintenances);
			Levels[2].Pages.Add(pg_repairs);
			Levels[3].Pages.Add(pg_maintenances);
			Levels[3].Pages.Add(pg_maintenanceReports);
			#endregion

			//create machines
			MachineDataService = new DataServices.MachineDataService();
			var machineModels = MachineDataService.GetActives();
			foreach (var machineModel in machineModels)
			{
				var machine = new MachineItemVm(machineModel);

				#region machine is selected
				machine.SelectCommand = new Commands.Command(o =>
				{
					//create machine parts
					pg_machineParts.Items.Clear();
					MachinePartDataService = new DataServices.MachinePartDataService();
					var machinePartModels = MachinePartDataService.GetActives();
					foreach (var machinePartModel in machinePartModels)
					{
						var machinePart = new MachinePartItemVm(machinePartModel);

						//machine part is selected
						machinePart.SelectCommand = new Commands.Command(oo =>
						{
							pg_machineParts.SelectedItem = machinePart;
						});

						//machine part is excluded
						machinePart.UseCommand = new Commands.Command(oo =>
						{
							if (pg_machineParts.SelectedItem == machinePart) pg_machineParts.SelectedItem = null;
							// remove machinePart.Model
							//...
						});
						pg_machineParts.Items.Add(machinePart);
					}
				}); 
				#endregion
				pg_machines.Items.Add(machine);
			}



		}

		/// <summary>
		/// Gets or sets a bindable collection that indicates Levels
		/// </summary>
		public ObservableCollection<PmLevelBase> Levels { get { return _levels; } }
		private ObservableCollection<PmLevelBase> _levels = new ObservableCollection<PmLevelBase>();

	}
}
