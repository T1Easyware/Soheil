using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class MachinesVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<MachineFamilyVM>();
            foreach (var productGroup in MachineFamilyDataService.GetAll())
            {
                groupViewModels.Add(new MachineFamilyVM(productGroup,Access, MachineFamilyDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<MachineVM>();
            foreach (var model in MachineDataService.GetAll())
            {
                viewModels.Add(new MachineVM(model, GroupItems,Access, MachineDataService, MachineFamilyDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
            if (Items.GroupDescriptions != null)
                Items.GroupDescriptions.Add(new PropertyGroupDescription("SelectedGroupVM.Id"));
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineFamilyDataService MachineFamilyDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineDataService MachineDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MachinesVM"/> class.
        /// </summary>
        public MachinesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            MachineFamilyDataService = new MachineFamilyDataService(UnitOfWork);
            MachineDataService = new MachineDataService(UnitOfWork);
            MachineDataService.MachineAdded += OnMachineAdded;
            MachineFamilyDataService.MachineFamilyAdded += OnMachineFamilyAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(AddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (parameter is int)
            {
                MachineVM.CreateNew(MachineDataService,(int)parameter);            
            }
            else if (CurrentContent is MachineFamilyVM)
            {
                MachineVM.CreateNew(MachineDataService, ((MachineFamilyVM)CurrentContent).Id);
            }
            else if (CurrentContent is MachineVM)
            {
                MachineVM.CreateNew(MachineDataService, ((MachineVM)CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            MachineFamilyVM.CreateNew(MachineFamilyDataService);
        }

        public override void View(object content)
        {
            if (content is MachineFamilyVM)
            {
                CurrentContent = content as MachineFamilyVM;
            }
            else if (content is MachineVM)
            {
                CurrentContent = content as MachineVM;
            }
        }

        private void OnMachineAdded(object sender, ModelAddedEventArgs<Machine> e)
        {
            var newMachineVm = new MachineVM(e.NewModel, GroupItems, Access, MachineDataService, MachineFamilyDataService);
            Items.AddNewItem(newMachineVm);
            Items.CommitNew();
            CurrentContent = newMachineVm;
            CurrentContent.IsSelected = true;
        }

        private void OnMachineFamilyAdded(object sender, ModelAddedEventArgs<MachineFamily> e)
        {
            var newMachineFamily = new MachineFamilyVM(e.NewModel, Access, MachineFamilyDataService);
            GroupItems.AddNewItem(newMachineFamily);
            GroupItems.CommitNew();

            foreach (MachineVM item in Items)
            {
                if (!item.Groups.Contains(newMachineFamily))
                {
                    item.Groups.AddNewItem(newMachineFamily);
                    item.Groups.CommitNew();
                }
            }
            Add(newMachineFamily.Id);
            ((MachineVM)CurrentContent).SelectedGroupVM = newMachineFamily;

            CurrentContent = newMachineFamily;
        }

        #endregion

    }
}