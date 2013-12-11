using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class CostsVM : GridSplitViewModel
    {
        #region Properties

        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<CostCenterVM>();
            foreach (var costCenter in CostCenterDataService.GetAll())
            {
                groupViewModels.Add(new CostCenterVM(costCenter, Access, CostCenterDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<CostVM>();
            foreach (var model in CostDataService.GetAll())
            {
                viewModels.Add(new CostVM(model, GroupItems, Warehouses, GetCostSourceItems((CostSourceType) model.CostCenter.SourceType), Access, CostDataService, CostCenterDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent) Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
            if (Items.GroupDescriptions != null)
                Items.GroupDescriptions.Add(new PropertyGroupDescription("SelectedGroupVM.Id"));
        }

        private ListCollectionView GetCostSourceItems(CostSourceType type)
        {
            switch (type)
            {
                case CostSourceType.Machines:
                    return Machines;
                case CostSourceType.Operators:
                    return Operators;
                case CostSourceType.Stations:
                    return Stations;
                case CostSourceType.Activities:
                    return Activities;
                default:
                    return null;
            }
        }

        public ListCollectionView Machines { get; set; }
        public ListCollectionView Operators { get; set; }
        public ListCollectionView Stations { get; set; }
        public ListCollectionView Activities { get; set; }
        public ListCollectionView Warehouses { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CostCenterDataService CostCenterDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CostDataService CostDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineDataService MachineDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OperatorDataService OperatorDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public StationDataService StationDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivityDataService ActivityDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PartWarehouseDataService WarehouseDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesVM"/> class.
        /// </summary>
        public CostsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            CostCenterDataService = new CostCenterDataService();
            MachineDataService = new MachineDataService();
            StationDataService = new StationDataService();
            OperatorDataService = new OperatorDataService();
            ActivityDataService = new ActivityDataService();
            WarehouseDataService = new PartWarehouseDataService();

            CostDataService = new CostDataService();
            CostDataService.CostAdded += OnCostAdded;
            CostCenterDataService.CostCenterAdded += OnCostCenterAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Description"), 
                new ColumnInfo("CostType",true), 
                new ColumnInfo("SelectedCostSource","txtCostSource",true), 
                new ColumnInfo("CostValue","txtCost",true),
                new ColumnInfo("Mode",true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(AddGroup);
            ViewCommand = new Command(View, CanView);

            var machineViewModels = new ObservableCollection<MachineInfoVM>();
            foreach (var entity in MachineDataService.GetAll())
            {
                machineViewModels.Add(new MachineInfoVM(entity));
            }
            Machines = new ListCollectionView(machineViewModels);
            var operatorViewModels = new ObservableCollection<OperatorInfoVM>();
            foreach (var entity in OperatorDataService.GetAll())
            {
                operatorViewModels.Add(new OperatorInfoVM(entity));
            }
            Operators = new ListCollectionView(operatorViewModels);
            var stationViewModels = new ObservableCollection<StationInfoVM>();
            foreach (var entity in StationDataService.GetAll())
            {
                stationViewModels.Add(new StationInfoVM(entity));
            }
            Stations = new ListCollectionView(stationViewModels);

            var activityViewModels = new ObservableCollection<ActivityInfoVM>();
            foreach (var entity in ActivityDataService.GetAll())
            {
                activityViewModels.Add(new ActivityInfoVM(entity));
            }
            Activities = new ListCollectionView(activityViewModels);

            var warehouseViewModels = new ObservableCollection<PartWarehouseInfoVM>();
            foreach (var entity in WarehouseDataService.GetAll())
            {
                warehouseViewModels.Add(new PartWarehouseInfoVM(entity));
            }
            Warehouses = new ListCollectionView(warehouseViewModels);

            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (parameter is int)
            {
                CostVM.CreateNew(CostDataService,(int)parameter);
            }
            else if (CurrentContent is CostCenterVM)
            {
                CostVM.CreateNew(CostDataService,((CostCenterVM) CurrentContent).Id);
            }
            else if (CurrentContent is CostVM)
            {
                CostVM.CreateNew(CostDataService,((CostVM) CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            CostCenterVM.CreateNew(CostCenterDataService);
        }

        public override void View(object content)
        {
            if (content is CostCenterVM)
            {
                CurrentContent = content as CostCenterVM;
            }
            else if (content is CostVM)
            {
                CurrentContent = content as CostVM;
            }
            //CurrentContent = content as ISplitItemContent;
        }

        private void OnCostAdded(object sender, ModelAddedEventArgs<Cost> e)
        {
            var newContent = new CostVM(e.NewModel, GroupItems, Warehouses, GetCostSourceItems((CostSourceType) e.NewModel.CostCenter.SourceType), Access, CostDataService, CostCenterDataService);
            Items.AddNewItem(newContent);
            Items.CommitNew();

            CurrentContent = newContent;
            CurrentContent.IsSelected = true;
        }

        private void OnCostCenterAdded(object sender, ModelAddedEventArgs<CostCenter> e)
        {
            var newCostCenter = new CostCenterVM(e.NewModel, Access, CostCenterDataService);
            GroupItems.AddNewItem(newCostCenter);
            GroupItems.CommitNew();

            foreach (CostVM item in Items)
            {
                if (!item.Groups.Contains(newCostCenter))
                {
                    item.Groups.AddNewItem(newCostCenter);
                    item.Groups.CommitNew();
                }
            }
            Add(newCostCenter.Id);
            ((CostVM) CurrentContent).SelectedGroupVM = newCostCenter;

            CurrentContent = newCostCenter;
        }

        #endregion

    }
}