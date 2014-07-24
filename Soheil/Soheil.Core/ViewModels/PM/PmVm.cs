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
		public DataServices.PM.ReportDataService ReportDataService { get; set; }
		public DataServices.PM.PartDataService PartDataService { get; set; }

		private bool _isInitialized = false;

		#endregion

		public PmVm(AccessType access)
		{
			Access = access;
			_uow = new Dal.SoheilEdmContext();
			MachineDataService = new DataServices.MachineDataService(_uow);
			MachinePartDataService = new DataServices.PM.MachinePartDataService(_uow);
			PartDataService = new DataServices.PM.PartDataService(_uow);
			MaintenanceDataService = new DataServices.PM.MaintenanceDataService(_uow);
			ReportDataService = new DataServices.PM.ReportDataService(_uow);
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
            FillPages();
            //declare finish
            _isInitialized = true;
        }
        
        
        void CreatePages()
        {
            #region Machines
			MachinesPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
				DeleteCommand = new Commands.Command(o => { }, () => false),
			};
            MachinesPage.SelectedItemChanged += MachinesPage_SelectedItemChanged;
            MachinesPage.Items.CollectionChanged += Machines_CollectionChanged;
            #endregion

			//------------------------------------------------------------------------

			#region MachineParts
			MachinePartsPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};

			MachinePartsPage.DeleteCommand = new Commands.Command(o =>
			{
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
				var mp = MachinePartsPage.SelectedItem as MachinePartItemVm;
				if (mp == null) return;
                int id = mp.Model.Part.Id;
                int idx = MachinePartsPage.Items.IndexOf(mp);

                MachinePartDataService.DeleteModel(mp.Model);
				MachinePartsPage.Items.Remove(mp);
                MachinePartsPage.SelectedItem =
                            idx < MachinePartsPage.Items.Count ?
                            MachinePartsPage.Items[idx] :
                            MachinePartsPage.Items.LastOrDefault();

                //update MachinePartPage layout when -mp
                MachinePartsPage.InvokeRefresh();
                //update LinkCounter when -mp
                var m = PartsPage.Items.FirstOrDefault(x => x.Id == id);
                if (m != null) m.UpdateIsAdded(MachinesPage.SelectedItem);
			}, 
            //can execute
            () => MachinePartsPage.SelectedItem != null
                && (MachinePartsPage.SelectedItem as MachinePartItemVm).Model != null
                && !(MachinePartsPage.SelectedItem as MachinePartItemVm).Model.IsMachine);
            MachinePartsPage.SelectedItemChanged += MachinePartsPage_SelectedItemChanged;
            MachinePartsPage.Items.CollectionChanged += MachineParts_CollectionChanged;
            #endregion

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
                    if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
                    var part = PartsPage.SelectedItem as PartItemVm;
					if (part == null) return;
                    int idx = PartsPage.Items.IndexOf(part);

					PartDataService.DeleteModel(part.Model);
					PartsPage.Items.Remove(part);
                    PartsPage.SelectedItem =
                        idx < PartsPage.Items.Count ?
                        PartsPage.Items[idx] :
                        PartsPage.Items.LastOrDefault();
				}, () => PartsPage.SelectedItem != null),
			};
            PartsPage.Items.CollectionChanged += Parts_CollectionChanged;
            #endregion


			//------------------------------------------------------------------------

			#region MachinePartMaintenances
			MachinePartMaintenancesPage = new PmPageBase
			{
				AddCommand = new Commands.Command(o => { }, () => false),
			};

			MachinePartMaintenancesPage.DeleteCommand = new Commands.Command(o =>
			{
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
                var mp = MachinePartMaintenancesPage.SelectedItem as MPMItemVm;
				if (mp == null) return;
                int idx = MachinePartMaintenancesPage.Items.IndexOf(mp);
                int id = mp.Model.Maintenance.Id;

				MaintenanceDataService.DeleteModel(mp.Model);
				MachinePartMaintenancesPage.Items.Remove(mp);
                MachinePartMaintenancesPage.SelectedItem =
                    idx < MachinePartMaintenancesPage.Items.Count ?
                    MachinePartMaintenancesPage.Items[idx] :
                    MachinePartMaintenancesPage.Items.LastOrDefault();

                //update MachinePartMaintenancesPage layout when -mpm
                MachinePartMaintenancesPage.InvokeRefresh();
                //update LinkCounter when -mpm
                var m = MaintenancesPage.Items.FirstOrDefault(x => x.Id == id);
                if (m != null) m.UpdateIsAdded(MachinePartsPage.SelectedItem);
			}, 
            //can execute
            () => MachinePartMaintenancesPage.SelectedItem != null
                && !(MachinePartMaintenancesPage.SelectedItem as MPMItemVm).Model.MaintenanceReports.Any());
            MachinePartMaintenancesPage.Items.CollectionChanged += MachinePartMaintenances_CollectionChanged;
			#endregion

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
                    if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
                    var mntn = MaintenancesPage.SelectedItem as MaintenanceItemVm;
					if (mntn == null) return;
                    int idx = MaintenancesPage.Items.IndexOf(mntn);

					MaintenanceDataService.DeleteModel(mntn.Model);
					MaintenancesPage.Items.Remove(mntn);
                    MaintenancesPage.SelectedItem =
                        idx < MaintenancesPage.Items.Count ?
                        MaintenancesPage.Items[idx] :
                        MaintenancesPage.Items.LastOrDefault();
				}, () => MaintenancesPage.SelectedItem != null),
			};
            MaintenancesPage.Items.CollectionChanged += Maintenances_CollectionChanged;
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

        void FillPages()
        {
            #region create machines
            MachinesPage.Items.Add(new MachineItemVm(null));
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
                MachinesPage.Items.Add(machineVm);
            }
            #endregion

            #region create parts
            PartsPage.Items.Clear();
            var partModels = PartDataService.GetActives();
            foreach (var partModel in partModels)
            {
                var partVm = new PartItemVm(partModel);
                PartsPage.Items.Add(partVm);
            }
            #endregion

            #region create Maintenances
            MaintenancesPage.Items.Clear();
            var maintenanceModels = MaintenanceDataService.GetActives();
            foreach (var maintenanceModel in maintenanceModels)
            {
                var maintenanceVm = new MaintenanceItemVm(maintenanceModel);
                MaintenancesPage.Items.Add(maintenanceVm);
            }
            #endregion
        }

       
        //Initialize commands for new items in each collection
        //  Machine,MachinePart,MachinePartMaintenace => GotoReportCommand,GotoRepairCommand
        //  MachinePartMaintenace => AddReportCommand
        //  Part,Maintenace => UseCommand
        //Also refresh layout if _isInitialized
        #region CollectionChanged

        void Machines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems!=null)
            {
                foreach (var machineVm in e.NewItems.OfType<MachineItemVm>())
                {

                    machineVm.GotoReportCommand = new Commands.Command(o =>
                    {
						ReportsPage.Items.Clear();
						IEnumerable<Model.MaintenanceReport> models;

						//find all reports regarding the current machineVm
						if (machineVm.Model == null)
						{
							models = ReportDataService.GetActives();
							ReportsPage.HideMachines = false;
							ReportsPage.HideMachineParts = false;
							ReportsPage.HideMachinePartMaintenances = false;
						}
						else
						{
							models = ReportDataService.GetActivesForMachine(machineVm.Model);
							ReportsPage.HideMachines = true;
							ReportsPage.HideMachineParts = false;
							ReportsPage.HideMachinePartMaintenances = false;
						}

						//add them to Items
						foreach (var model in models)
						{
							var vm = new ReportItemVm(model,
								new MPMItemVm(model.MachinePartMaintenance,
									new MachinePartItemVm(model.MachinePartMaintenance.MachinePart,
										new MachineItemVm(model.MachinePartMaintenance.MachinePart.Machine,true),true),true));
							ReportsPage.Items.Add(vm);
						}
						ReportsPage.InvokeRefresh();
					});

                    machineVm.GotoRepairCommand = new Commands.Command(o =>
                    {

                    });
                }
            }
        }
        
        void MachineParts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var machinePartVm in e.NewItems.OfType<MachinePartItemVm>())
                {


                    machinePartVm.GotoReportCommand = new Commands.Command(o =>
                    {
                        ReportsPage.Items.Clear();
						IEnumerable<Model.MaintenanceReport> models;

						Model.Machine machineModel = null;
						if (MachinesPage.SelectedItem != null &&
								(MachinesPage.SelectedItem as MachineItemVm).Model != null)
							machineModel = (MachinesPage.SelectedItem as MachineItemVm).Model;
                        //find all reports regarding the current machinePartVm
						if (machinePartVm.Model == null)
						{
							//if no machine part is selected
							if (machineModel == null)
							{
								//if no machine is selected
								//everything
								models = ReportDataService.GetActives();
								ReportsPage.HideMachines = false;
								ReportsPage.HideMachineParts = false;
								ReportsPage.HideMachinePartMaintenances = false;
							}
							else
							{
								//just the machine
								models = ReportDataService.GetActivesForMachine((MachinesPage.SelectedItem as MachineItemVm).Model);
								ReportsPage.HideMachines = true;
								ReportsPage.HideMachineParts = false;
								ReportsPage.HideMachinePartMaintenances = false;
							}
						}
						else
						{
							//just the machinePart (or machine)
							models = ReportDataService.GetActivesForMachinePart(machinePartVm.Model);
							ReportsPage.HideMachines = (machineModel != null);
							ReportsPage.HideMachineParts = true;
							ReportsPage.HideMachinePartMaintenances = false;
						}

                        //add them to Items
                        foreach (var model in models)
                        {
							var vm = new ReportItemVm(model,
								new MPMItemVm(model.MachinePartMaintenance,
									new MachinePartItemVm(model.MachinePartMaintenance.MachinePart,
										new MachineItemVm(model.MachinePartMaintenance.MachinePart.Machine, true), true), true));
                            ReportsPage.Items.Add(vm);
                        }
						ReportsPage.InvokeRefresh();
					});



                    machinePartVm.GotoRepairCommand = new Commands.Command(o =>
                    {

                    });
                }
            }
            if (_isInitialized) MachinePartsPage.InvokeRefresh();
        }
        
        void MachinePartMaintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var mpmVm in e.NewItems.OfType<MPMItemVm>())
                {
                    mpmVm.GotoReportCommand = new Commands.Command(o =>
                    {
                        ReportsPage.Items.Clear();
                        foreach (var model in mpmVm.Model.MaintenanceReports)
                        {
                            var vm = new ReportItemVm(model, mpmVm);
                            ReportsPage.Items.Add(vm);
                        }

						ReportsPage.HideMachines = true;
						ReportsPage.HideMachineParts = true;
						ReportsPage.HideMachinePartMaintenances = true;
						ReportsPage.InvokeRefresh();
                    });

                    mpmVm.AddReportCommand = new Commands.Command(o =>
                    {
                        var model = new Model.MaintenanceReport
                        {
                            Code = mpmVm.Code,
                            Description = "",
                            MachinePartMaintenance = mpmVm.Model,
                            MaintenanceDate = DateTime.Now.Date,
                            PerformedDate = DateTime.Now.Date,
                        };
                        MaintenanceDataService.AddModel(model);
                        var vm = new ReportItemVm(model, mpmVm);
                        ReportsPage.Items.Add(vm);
						ReportsPage.SelectedItem = vm;

						ReportsPage.HideMachines = true;
						ReportsPage.HideMachineParts = true;
						ReportsPage.HideMachinePartMaintenances = false;
						ReportsPage.InvokeRefresh();
					});
                }
            }
            if (_isInitialized) MachinePartMaintenancesPage.InvokeRefresh();
        }
       
        void Parts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var partVm in e.NewItems.OfType<PartItemVm>())
                {
                    //define UseCommand
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
                            Part = partVm.Model,
                            ModifiedBy = LoginInfo.Id,
                            ModifiedDate = DateTime.Now,
                            RecordStatus = Status.Active,
                        };
                        MachinePartDataService.AddModel(newMachinePartModel);
						var newMachinePartVm = new MachinePartItemVm(newMachinePartModel, machineVm);
                        MachinePartsPage.Items.Add(newMachinePartVm);

                        //update LinkCounter when +machineParts
                        partVm.UpdateIsAdded(machineVm);
                        //update MachinePartsPage layout when +machineParts
                        MachinePartsPage.InvokeRefresh();
                    },
                    //can execute
                    () => (MachinesPage.SelectedItem as MachineItemVm)!=null
                        && (MachinesPage.SelectedItem as MachineItemVm).Model != null);
                }
            }
            //update PartsPage layout when +-part
            if (_isInitialized) PartsPage.InvokeRefresh();
        }

        void Maintenances_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var maintenanceVm in e.NewItems.OfType<MaintenanceItemVm>())
                {
                    //define UseCommand
                    maintenanceVm.UseCommand = new Commands.Command(oo =>
                    {
                        var machinePartVm = MachinePartsPage.SelectedItem as MachinePartItemVm;
                        if (machinePartVm == null) return;

                        var newMachinePartMaintenanceModel = new Model.MachinePartMaintenance
                        {
                            Maintenance = maintenanceVm.Model,
                            MachinePart = machinePartVm.Model,
                            IsOnDemand = false,
                            PeriodDays = 30,
                            LastMaintenanceDate = DateTime.Now.Date,
                            Description = "",
                            Code = maintenanceVm.Code + "." + machinePartVm.Model.Code,
                            ModifiedBy = LoginInfo.Id,
                            ModifiedDate = DateTime.Now,
                            RecordStatus = Status.Active,
                        };
                        MaintenanceDataService.AddModel(newMachinePartMaintenanceModel);
                        var newMachinePartMaintenanceVm = new MPMItemVm(newMachinePartMaintenanceModel, machinePartVm);
                        MachinePartMaintenancesPage.Items.Add(newMachinePartMaintenanceVm);

                        //update LinkCounter when +machinePartMaintenances
                        maintenanceVm.UpdateIsAdded(machinePartVm);
                        //update MachinePartMaintenancesPage layout when +machinePartMaintenances
                        MachinePartMaintenancesPage.InvokeRefresh();
                    },
                    //can execute
                    () => (MachinePartsPage.SelectedItem as MachinePartItemVm) != null
						&& (MachinePartsPage.SelectedItem as MachinePartItemVm).Model != null);
                }
            }
            //update MaintenancesPage layout when +-maintenances
            if (_isInitialized) MaintenancesPage.InvokeRefresh();
        }

        #endregion

		//Each selected item :
		//1. reloads next level
        //2. affect LinkCounters 
		//3. Causes refresh on some pages
        #region SelectedItemChanged
        void MachinesPage_SelectedItemChanged(PmItemBase item_machine)
        {
            MachinePartsPage.Items.Clear();
            MachinePartMaintenancesPage.Items.Clear();
            var machine = item_machine as MachineItemVm;
            if (machine == null) return;

			MachinePartsPage.Items.Add(new MachinePartItemVm(null, machine));
			IEnumerable<Model.MachinePart> models;

			//create machine parts
            if (machine.Model == null)
            {
				MachinePartsPage.HideMachines = false;
				MachinePartMaintenancesPage.HideMachines = false;
				//add all machine parts
				models = MachinePartDataService.GetActives();
            }
            else
            {
				MachinePartsPage.HideMachines = true;
				MachinePartMaintenancesPage.HideMachines = true;
				//add machine's machine parts
				models = machine.Model.MachineParts;
            }

			//add parts
			foreach (var machinePartModel in models)
			{
				var machinePartVm = new MachinePartItemVm(machinePartModel,
					new MachineItemVm(machinePartModel.Machine, true));
				MachinePartsPage.Items.Add(machinePartVm);
			}

            //update parts
            foreach (var part in PartsPage.Items.OfType<PartItemVm>())
            {
                part.UpdateIsAdded(machine);
            }

            MachinePartsPage.InvokeRefresh();
            PartsPage.InvokeRefresh();
        }

        void MachinePartsPage_SelectedItemChanged(PmItemBase item_machinePart)
        {
            //load MPMs
            MachinePartMaintenancesPage.Items.Clear();
            var machinePart = item_machinePart as MachinePartItemVm;
            if (machinePart == null) return;

			IEnumerable<Model.MachinePartMaintenance> models;

            //create machine parts
            if (machinePart.Model == null)
            {
				MachinePartMaintenancesPage.HideMachineParts = false;
				//add all MachinePartMaintenances
                Model.Machine machineModel = null;
                if (MachinesPage.SelectedItem as MachineItemVm != null)
                    machineModel = (MachinesPage.SelectedItem as MachineItemVm).Model;
				models = MaintenanceDataService.GetActivesMachinePartMaintenancesForMachine(machineModel);
            }
            else
            {
				MachinePartMaintenancesPage.HideMachineParts = true;
				//add MachinePart's MachinePartMaintenances
				models = machinePart.Model.MachinePartMaintenances;
            }

			//add mpms
			foreach (var mpmModel in models)
			{
				var mpmVm = new MPMItemVm(mpmModel,
					new MachinePartItemVm(mpmModel.MachinePart,
						new MachineItemVm(mpmModel.MachinePart.Machine, true), true));
				MachinePartMaintenancesPage.Items.Add(mpmVm);
			}

			//update mpms
            foreach (var pm in MaintenancesPage.Items.OfType<MaintenanceItemVm>())
            {
                pm.UpdateIsAdded(machinePart);
            }
            MachinePartMaintenancesPage.InvokeRefresh();
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
