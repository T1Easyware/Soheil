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
                MachinesPage.InvokeRefresh();
                MachinePartsPage.InvokeRefresh();
                MachinePartMaintenancesPage.InvokeRefresh();
                PartsPage.InvokeRefresh();
                MaintenancesPage.InvokeRefresh();
                RepairsPage.InvokeRefresh();
                ReportsPage.InvokeRefresh();
				_uow.Commit();
			});
            CreatePages();
            //declare finish
            _isInitializing = false;
        }

        void CreatePages()
        {
            #region Machines
			MachinesPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			};
            #endregion

            #region create machines
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
                MachinesPage.Items.Add(machineVm);
            } 
            #endregion

			//------------------------------------------------------------------------

			#region MachineParts
			MachinePartsPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};

			MachinePartsPage.DeleteCommand = new Commands.Command(o =>
			{
				var mp = MachinePartsPage.SelectedItem as MachinePartItemVm;
				if (mp == null) return;
				MachinePartDataService.DeleteModel(mp.Model);
				MachinePartsPage.Items.Remove(mp);
				MachinePartsPage.SelectedItem = null;
			}, () => MachinePartsPage.SelectedItem != null && !(MachinePartsPage.SelectedItem as MachinePartItemVm).Model.IsMachine);
			#endregion
			MachinePartsPage.Items.CollectionChanged += MachineParts_CollectionChanged; 

			//------------------------------------------------------------------------

			#region Parts
			PartsPage = new PmPageBase
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
						PartsPage.Items.Add(newPartVm);
						PartsPage.SelectedItem = newPartVm;
					}, () => true),

					DeleteCommand = new Commands.Command(o =>
					{
						var part = PartsPage.SelectedItem as PartItemVm;
						if (part == null) return;
						PartDataService.DeleteModel(part.Model);
						PartsPage.Items.Remove(part);
						PartsPage.SelectedItem = null;
					}, () => PartsPage.SelectedItem != null),
				};

			#endregion
            PartsPage.Items.CollectionChanged += Parts_CollectionChanged;

            #region create parts
            PartsPage.Items.Clear();
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
                    var machineVm = MachinesPage.SelectedItem as MachineItemVm;
                    if (machineVm == null) return;

                    var newMachinePartModel = new Model.MachinePart
                    {
                        Name = machineVm.Name + "." + partVm.Name,
                        Code = machineVm.Code + "." + partVm.Code,
                        Description = "",
                        IsMachine = false,
                        Machine = machineVm.Model,
                        Part = partModel,
                        ModifiedBy = LoginInfo.Id,
                        ModifiedDate = DateTime.Now,
                        RecordStatus = Status.Active,
                    };
                    MachinePartDataService.AddModel(newMachinePartModel);
                    var newMachinePartVm = new MachinePartItemVm(newMachinePartModel);
                    MachinePartsPage.Items.Add(newMachinePartVm);
                });

                PartsPage.Items.Add(partVm);
            } 
            #endregion

			//------------------------------------------------------------------------

			#region MachinePartMaintenances
			MachinePartMaintenancesPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};

			MachinePartMaintenancesPage.DeleteCommand = new Commands.Command(o =>
			{
				var mp = MachinePartMaintenancesPage.SelectedItem as MPMItemVm;
				if (mp == null) return;
				MaintenanceDataService.DeleteModel(mp.Model);
				MachinePartMaintenancesPage.Items.Remove(mp);
				MachinePartMaintenancesPage.SelectedItem = null;
			}, () => MachinePartMaintenancesPage.SelectedItem != null && !(MachinePartMaintenancesPage.SelectedItem as MPMItemVm).Model.MaintenanceReports.Any());

			#endregion
            MachinePartMaintenancesPage.Items.CollectionChanged += MachinePartMaintenances_CollectionChanged; 
	
			//------------------------------------------------------------------------

			#region Maintenances
			MaintenancesPage = new PmPageBase
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
					MaintenancesPage.Items.Add(newMaintenanceVm);
					MaintenancesPage.SelectedItem = newMaintenanceVm;
				}, () => true),

				DeleteCommand = new Commands.Command(o =>
				{
					var mntn = MaintenancesPage.SelectedItem as MaintenanceItemVm;
					if (mntn == null) return;
					MaintenanceDataService.DeleteModel(mntn.Model);
					MaintenancesPage.Items.Remove(mntn);
					MaintenancesPage.SelectedItem = null;
				}, () => MaintenancesPage.SelectedItem != null),
			};

			#endregion
            MaintenancesPage.Items.CollectionChanged += Maintenances_CollectionChanged; 

            #region create Maintenances
            MaintenancesPage.Items.Clear();
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
                    var machinePartVm = MachinePartsPage.SelectedItem as MachinePartItemVm;
                    if (machinePartVm == null) return;

                    var newMachinePartMaintenanceModel = new Model.MachinePartMaintenance
                    {
                        Maintenance = maintenanceModel,
                        MachinePart = machinePartVm.Model,
                        IsOnDemand = false,
                        PeriodDays = 30,
                        LastMaintenanceDate = DateTime.Now.Date,
                        Description = "",
                        Code = maintenanceModel.Code + "." + machinePartVm.Model.Code,
                        ModifiedBy = LoginInfo.Id,
                        ModifiedDate = DateTime.Now,
                        RecordStatus = Status.Active,
                    };
                    MaintenanceDataService.AddModel(newMachinePartMaintenanceModel);
                    var newMachinePartMaintenanceVm = new MPMItemVm(newMachinePartMaintenanceModel);
                    MachinePartMaintenancesPage.Items.Add(newMachinePartMaintenanceVm);
                });

                MaintenancesPage.Items.Add(maintenanceVm);
            } 
            #endregion

			//------------------------------------------------------------------------

			#region Reports
			ReportsPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			};
			#endregion

            //------------------------------------------------------------------------

            #region Repairs
            RepairsPage = new PmPageBase
            {
                AddCommand = new Commands.Command(o => { }, () => false),
                DeleteCommand = new Commands.Command(o => { }, () => false),
            };
            #endregion
		}









		#region Collection|Selections changed

		void MachineParts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.OfType<MachinePartItemVm>())
				{
					item.Selected += MachinePartVm_Selected;
					item.UpdateIsAdded(MachinesPage.SelectedItem);
				}
				if (!_isInitializing)
				{
					MachinePartsPage.InvokeRefresh();
					PartsPage.InvokeRefresh();
				}
			}
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.OfType<MachinePartItemVm>())
				{
					item.UpdateIsAdded(MachinesPage.SelectedItem);
				}
				if (!_isInitializing)
				{
					MachinePartsPage.InvokeRefresh();
					PartsPage.InvokeRefresh();
				}
			}
		}
		void Parts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.OfType<MaintenanceItemVm>())
				{
					item.Selected += Part_Selected;
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) PartsPage.InvokeRefresh();
			}
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.OfType<MaintenanceItemVm>())
				{
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) PartsPage.InvokeRefresh();
			}
		}
		void Maintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.OfType<MaintenanceItemVm>())
				{
					item.Selected += Maintenance_Selected;
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) MaintenancesPage.InvokeRefresh();
			}
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.OfType<MaintenanceItemVm>())
				{
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) MaintenancesPage.InvokeRefresh();
			}
		}
		void MachinePartMaintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.OfType<MPMItemVm>())
				{
					item.Selected += MachinePartMaintenanceVm_Selected;
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) MachinePartMaintenancesPage.InvokeRefresh();
			}
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.OfType<MPMItemVm>())
				{
					item.UpdateIsAdded(MachinePartsPage.SelectedItem);
				}
				if (!_isInitializing) MachinePartMaintenancesPage.InvokeRefresh();
			}
		}
		

		
		void machine_Selected(PmItemBase item_machine)
		{
			MachinePartsPage.Items.Clear();
			var machine = item_machine as MachineItemVm;
			if (machine == null) return;

			//create machine parts
			foreach (var machinePartModel in machine.Model.MachineParts)
			{
				var machinePartVm = new MachinePartItemVm(machinePartModel);

				//machine part is selected
				machinePartVm.Selected += item_machinePart =>
				{
					MachinePartsPage.SelectedItem = item_machinePart;
				};

				//machine part is excluded
				machinePartVm.UseCommand = new Commands.Command(oo =>
				{
					if (MachinePartsPage.SelectedItem == machinePartVm) MachinePartsPage.SelectedItem = null;
					// remove machinePart.Model
					//...
				});
				MachinePartsPage.Items.Add(machinePartVm);
			}

			PartsPage.InvokeRefresh();
		}

		void MachinePartVm_Selected(PmItemBase item_machinePart)
		{
			//load MPMs
			MachinePartMaintenancesPage.Items.Clear();
			var machinePart = item_machinePart as MachinePartItemVm;
			if (machinePart == null) return;

			//create machine parts
			foreach (var mpmModel in machinePart.Model.MachinePartMaintenances)
			{
				var mpmVm = new MPMItemVm(mpmModel);

				//machine part maintenance is selected
				mpmVm.Selected += item_mpm =>
				{
					MachinePartMaintenancesPage.SelectedItem = item_mpm;
				};

				//machine part maintenance is excluded
				mpmVm.UseCommand = new Commands.Command(oo =>
				{
					if (MachinePartMaintenancesPage.SelectedItem == mpmVm) 
						MachinePartMaintenancesPage.SelectedItem = null;
				});
				MachinePartMaintenancesPage.Items.Add(mpmVm);
			}

			MaintenancesPage.InvokeRefresh();
		}
		#endregion

        #region Pages
        public PmPageBase MachinesPage
        {
            get { return (PmPageBase)GetValue(MachinesPageProperty); }
            set { SetValue(MachinesPageProperty, value); }
        }
        public static readonly DependencyProperty MachinesPageProperty =
            DependencyProperty.Register("MachinesPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase MachinePartsPage
        {
            get { return (PmPageBase)GetValue(MachinePartsPageProperty); }
            set { SetValue(MachinePartsPageProperty, value); }
        }
        public static readonly DependencyProperty MachinePartsPageProperty =
            DependencyProperty.Register("MachinePartsPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase MachinePartMaintenancesPage
        {
            get { return (PmPageBase)GetValue(MachinePartMaintenancesPageProperty); }
            set { SetValue(MachinePartMaintenancesPageProperty, value); }
        }
        public static readonly DependencyProperty MachinePartMaintenancesPageProperty =
            DependencyProperty.Register("MachinePartMaintenancesPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase PartsPage
        {
            get { return (PmPageBase)GetValue(PartsPageProperty); }
            set { SetValue(PartsPageProperty, value); }
        }
        public static readonly DependencyProperty PartsPageProperty =
            DependencyProperty.Register("PartsPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase MaintenancesPage
        {
            get { return (PmPageBase)GetValue(MaintenancesPageProperty); }
            set { SetValue(MaintenancesPageProperty, value); }
        }
        public static readonly DependencyProperty MaintenancesPageProperty =
            DependencyProperty.Register("MaintenancesPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase ReportsPage
        {
            get { return (PmPageBase)GetValue(ReportsPageProperty); }
            set { SetValue(ReportsPageProperty, value); }
        }
        public static readonly DependencyProperty ReportsPageProperty =
            DependencyProperty.Register("ReportsPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        public PmPageBase RepairsPage
        {
            get { return (PmPageBase)GetValue(RepairsPageProperty); }
            set { SetValue(RepairsPageProperty, value); }
        }
        public static readonly DependencyProperty RepairsPageProperty =
            DependencyProperty.Register("RepairsPage", typeof(PmPageBase), typeof(PmVm), new PropertyMetadata(null));
        #endregion

		#region Commands
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
