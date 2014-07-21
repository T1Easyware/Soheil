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
		#region Members, Events and props
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }

		Dal.SoheilEdmContext _uow;
		public DataServices.MachineDataService MachineDataService { get; set; }
		public DataServices.PM.MachinePartDataService MachinePartDataService { get; set; }
		public DataServices.PM.MaintenanceDataService MaintenanceDataService { get; set; }
		public DataServices.PM.PartDataService PartDataService { get; set; }

		PmPageBase page_machines, page_machineParts, page_parts, page_macPartMaintenances, page_macPartMaintenanceReports, page_repairs, page_maintenances, page_maintenanceReports;
		private bool _isInitializing = true;

		#endregion

		public PmVm(AccessType access)
		{
			Access = access;
			_uow = new Dal.SoheilEdmContext();
			MachineDataService = new DataServices.MachineDataService(_uow);
			MachinePartDataService = new DataServices.PM.MachinePartDataService(_uow);
			PartDataService = new DataServices.PM.PartDataService(_uow);
			MaintenanceDataService = new DataServices.PM.MaintenanceDataService(_uow);
			SaveCommand = new Commands.Command(o =>
			{
				foreach (var level in Levels)
					if (level != null)
						foreach (var page in level.Pages)
							if (page != null)
								page.InvokeRefresh();
				_uow.Commit();
			});

			#region Create pages

			//------------------------------------------------------------------------
			//-----------------------------LEVEL 0------------------------------------
			//------------------------------------------------------------------------

			#region Machines
			page_machines = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			}; 
			#endregion

			//------------------------------------------------------------------------
			//-----------------------------LEVEL 1------------------------------------
			//------------------------------------------------------------------------

			#region MachineParts
			page_machineParts = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};
			page_machineParts.DeleteCommand = new Commands.Command(o =>
			{
				var mp = page_machineParts.SelectedItem as MachinePartItemVm;
				if (mp == null) return;
				MachinePartDataService.DeleteModel(mp.Model);
				page_machineParts.Items.Remove(mp);
				page_machineParts.SelectedItem = null;
			}, () => page_machineParts.SelectedItem != null && !(page_machineParts.SelectedItem as MachinePartItemVm).Model.IsMachine);
			page_machineParts.Items.CollectionChanged += MachineParts_CollectionChanged; 
			#endregion

			//------------------------------------------------------------------------

			#region Parts
			page_parts = new PmPageBase
				{
					AddCommand = new Commands.Command(o =>
					{
						var newPartModel = new Model.Part
						{
							Name = "*",
							Code = "*",
							Description = "",
							RecordStatus = Status.Active,
							ModifiedBy = LoginInfo.Id,
							ModifiedDate = DateTime.Now,
						};
						newPartModel.Id = PartDataService.AddModel(newPartModel);
						var newPartVm = new PartItemVm(newPartModel);
						page_parts.Items.Add(newPartVm);
						page_parts.SelectedItem = newPartVm;
					}, () => true),

					DeleteCommand = new Commands.Command(o =>
					{
						var part = page_parts.SelectedItem as PartItemVm;
						if (part == null) return;
						PartDataService.DeleteModel(part.Model);
						page_parts.Items.Remove(part);
						page_parts.SelectedItem = null;
					}, () => page_parts.SelectedItem != null),
				}; 
			#endregion

			//------------------------------------------------------------------------

			#region MachinePartMaintenances
			page_macPartMaintenances = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};
			page_macPartMaintenances.DeleteCommand = new Commands.Command(o =>
			{
				var mp = page_macPartMaintenances.SelectedItem as MPMItemVm;
				if (mp == null) return;
				MaintenanceDataService.DeleteModel(mp.Model);
				page_macPartMaintenances.Items.Remove(mp);
				page_macPartMaintenances.SelectedItem = null;
			}, () => page_macPartMaintenances.SelectedItem != null && !(page_macPartMaintenances.SelectedItem as MPMItemVm).Model.MaintenanceReports.Any());
			page_macPartMaintenances.Items.CollectionChanged += MachinePartMaintenances_CollectionChanged; 
			#endregion
	
			//------------------------------------------------------------------------

			#region MachinePartMaintenanceReports
			page_macPartMaintenanceReports = new PmPageBase
			{
			}; 
			#endregion
			
			//------------------------------------------------------------------------

			#region Repairs
			page_repairs = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			}; 
			#endregion

			//------------------------------------------------------------------------
			//------------------------------LEVEL 2-----------------------------------
			//------------------------------------------------------------------------

			#region Maintenances
			page_maintenances = new PmPageBase
			{
				AddCommand = new Commands.Command(o =>
				{
					var newMaintenanceModel = new Model.Maintenance
					{
						Name = "*",
						Code = "*",
						Description = "",
						RecordStatus = Status.Active,
						ModifiedBy = LoginInfo.Id,
						ModifiedDate = DateTime.Now,
					};
					newMaintenanceModel.Id = MaintenanceDataService.AddModel(newMaintenanceModel);
					var newMaintenanceVm = new MaintenanceItemVm(newMaintenanceModel);
					page_maintenances.Items.Add(newMaintenanceVm);
					page_maintenances.SelectedItem = newMaintenanceVm;
				}, () => true),

				DeleteCommand = new Commands.Command(o =>
				{
					var mntn = page_maintenances.SelectedItem as MaintenanceItemVm;
					if (mntn == null) return;
					MaintenanceDataService.DeleteModel(mntn.Model);
					page_maintenances.Items.Remove(mntn);
					page_maintenances.SelectedItem = null;
				}, () => page_maintenances.SelectedItem != null),
			};
			page_maintenances.Items.CollectionChanged += Maintenances_CollectionChanged; 
			#endregion
			
			//------------------------------------------------------------------------

			#region MaintenanceReports
			page_maintenanceReports = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			}; 
			#endregion
			
			//------------------------------------------------------------------------
			//-----------------------------LEVEL 3------------------------------------
			//------------------------------------------------------------------------

			#region ...Add to viewModel
			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels.Add(new PmLevelBase());
			Levels[0].Pages.Add(page_machines);
			Levels[1].Pages.Add(page_machineParts);
			Levels[2].Pages.Add(page_parts);
			Levels[2].Pages.Add(page_macPartMaintenances);
			Levels[2].Pages.Add(page_macPartMaintenanceReports);
			Levels[2].Pages.Add(page_repairs);
			Levels[3].Pages.Add(page_maintenances);
			Levels[3].Pages.Add(page_maintenanceReports); 
			#endregion
			#endregion


			//create and correct machines
			var machineModels = MachineDataService.GetActives();
			foreach (var machineModel in machineModels)
			{
				//create default part for machine
				if (!machineModel.MachineParts.Any(x => x.IsMachine))
					MachinePartDataService.AddModel(new Model.MachinePart
					{
						IsMachine = true,
						Machine = machineModel,
						Name = machineModel.Name,
						Code = machineModel.Code,
						Description = "auto",
						ModifiedBy = LoginInfo.Id,
						ModifiedDate = DateTime.Now,
					});

				//create view model for machine and add to items
				var machineVm = new MachineItemVm(machineModel);
				machineVm.Selected += machine_Selected;
				page_machines.Items.Add(machineVm);
			}



			//create parts
			page_parts.Items.Clear();
			var partModels = PartDataService.GetActives();
			foreach (var partModel in partModels)
			{
				var partVm = new PartItemVm(partModel);

				//part is selected
				partVm.Selected += item_part =>
				{
				};

				//part is included
				partVm.UseCommand = new Commands.Command(oo =>
				{
					var machineVm = page_machines.SelectedItem as MachineItemVm;
					if (machineVm == null) return;

					var newMachinePartModel = new Model.MachinePart
					{
						Name = machineVm.Name + "." + partVm.Name,
						Code = machineVm.Code + "." + partVm.Code,
						Description = "",
						IsMachine= false,
						Machine = machineVm.Model,
						Part = partModel,
						ModifiedBy = LoginInfo.Id,
						ModifiedDate = DateTime.Now,
						RecordStatus = Status.Active,
					};
					MachinePartDataService.AddModel(newMachinePartModel);
					var newMachinePartVm = new MachinePartItemVm(newMachinePartModel);
					newMachinePartVm.Selected += newMachinePartVm_Selected;
					page_machineParts.Items.Add(newMachinePartVm);
				});

				partVm.Selected += part_Selected;
				page_parts.Items.Add(partVm);
			}

			//create maintenances
			page_maintenances.Items.Clear();
			var maintenanceModels = MaintenanceDataService.GetActives();
			foreach (var maintenanceModel in maintenanceModels)
			{
				var maintenanceVm = new MaintenanceItemVm(maintenanceModel);

				//maintenance is selected
				maintenanceVm.Selected += item_mntn =>
				{
				};

				//maintenance is included
				maintenanceVm.UseCommand = new Commands.Command(oo =>
				{
					var machinePartVm = page_machineParts.SelectedItem as MachinePartItemVm;
					if (machinePartVm == null) return;

					var newMachinePartMaintenanceModel = new Model.MachinePartMaintenance
					{
						Maintenance = maintenanceModel,
						MachinePart = machinePartVm.Model,
						IsOnDemand = false,
						LastMaintenanceDate = DateTime.Now.Date,
						Description = "",
						Code = maintenanceModel.Code + "." + machinePartVm.Model.Code,
						ModifiedBy = LoginInfo.Id,
						ModifiedDate = DateTime.Now,
						RecordStatus = Status.Active,
					};
					MaintenanceDataService.AddModel(newMachinePartMaintenanceModel);
					var newMachinePartMaintenanceVm = new MPMItemVm(newMachinePartMaintenanceModel);
					newMachinePartMaintenanceVm.Selected += newMachinePartMaintenanceVm_Selected;
					page_macPartMaintenances.Items.Add(newMachinePartMaintenanceVm);
				});

				maintenanceVm.Selected += maintenance_Selected;
				page_maintenances.Items.Add(maintenanceVm);
			}

			//declare finish
			_isInitializing = false;
		}





		#region Collection|Selections changed
		void MachineParts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(_isInitializing) return;
			if (e.NewItems != null)
				foreach (var item in e.NewItems.OfType<MachinePartItemVm>())
				{
					item.UpdateLinkCounter(page_machines.SelectedItem);
					page_machineParts.InvokeRefresh();
				}
			if (e.OldItems != null)
				foreach (var item in e.OldItems.OfType<MachinePartItemVm>())
				{
					item.UpdateLinkCounter(page_machines.SelectedItem);
					page_machineParts.InvokeRefresh();
				}
		}
		void Maintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(_isInitializing) return;
			if (e.NewItems != null)
				foreach (var item in e.NewItems.OfType<MaintenanceItemVm>())
				{
					item.UpdateLinkCounter(page_machineParts.SelectedItem);
					page_machineParts.InvokeRefresh();
				}
			if (e.OldItems != null)
				foreach (var item in e.OldItems.OfType<MaintenanceItemVm>())
				{
					item.UpdateLinkCounter(page_machineParts.SelectedItem);
					page_machineParts.InvokeRefresh();
				}
		}
		void MachinePartMaintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(_isInitializing) return;
			if (e.NewItems != null)
				foreach (var item in e.NewItems.OfType<MPMItemVm>())
				{
					item.UpdateLinkCounter(page_machineParts.SelectedItem);
					page_macPartMaintenances.InvokeRefresh();
				}
			if (e.OldItems != null)
				foreach (var item in e.OldItems.OfType<MPMItemVm>())
				{
					item.UpdateLinkCounter(page_machineParts.SelectedItem);
					page_macPartMaintenances.InvokeRefresh();
				}
		}
		

		
		void machine_Selected(PmItemBase item_machine)
		{
			page_machineParts.Items.Clear();
			var machine = item_machine as MachineItemVm;
			if (machine == null) return;

			//create machine parts
			foreach (var machinePartModel in machine.Model.MachineParts)
			{
				var machinePartVm = new MachinePartItemVm(machinePartModel);

				//machine part is selected
				machinePartVm.Selected += item_machinePart =>
				{
					page_machineParts.SelectedItem = machinePartVm;
				};

				//machine part is excluded
				machinePartVm.UseCommand = new Commands.Command(oo =>
				{
					if (page_machineParts.SelectedItem == machinePartVm) page_machineParts.SelectedItem = null;
					// remove machinePart.Model
					//...
				});
				page_machineParts.Items.Add(machinePartVm);
			}

			//update linkcounter of parts
			foreach (var partVm in page_parts.Items.OfType<PartItemVm>())
			{
				partVm.UpdateLinkCounter(page_machines.SelectedItem);
			}
			page_parts.InvokeRefresh();
		}

		void part_Selected(PmItemBase item_part)
		{
			//page_parts.SelectedItem = item_part;
			//var partVm = item_part as PartItemVm;
			//if (partVm == null) return;
		}
		void maintenance_Selected(PmItemBase item_maintenance)
		{
			//page_parts.SelectedItem = item_part;
			//var partVm = item_part as PartItemVm;
			//if (partVm == null) return;
		}
		
		void newMachinePartVm_Selected(PmItemBase item_machinePart)
		{
			//page_machineParts.SelectedItem = item_machinePart;
		}
		void newMachinePartMaintenanceVm_Selected(PmItemBase item_mpm)
		{

		}
		#endregion

		#region Bindable Properties
		/// <summary>
		/// Gets or sets a bindable collection that indicates Levels
		/// </summary>
		public ObservableCollection<PmLevelBase> Levels { get { return _levels; } }
		private ObservableCollection<PmLevelBase> _levels = new ObservableCollection<PmLevelBase>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SaveCommand
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(PmVm), new PropertyMetadata(null));

		#endregion

	}
}
