using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PartWarehousesVM : GridSplitViewModel
    {
        #region Properties

        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<PartWarehouseGroupVM>();
            foreach (var partGroup in PartWarehouseGroupDataService.GetAll())
            {
                groupViewModels.Add(new PartWarehouseGroupVM(partGroup, Access, PartWarehouseGroupDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<PartWarehouseVM>();
            foreach (var model in PartWarehouseDataService.GetAll())
            {
                viewModels.Add(new PartWarehouseVM(model, GroupItems, Access, PartWarehouseDataService, PartWarehouseGroupDataService, CostDataService));
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

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PartWarehouseGroupDataService PartWarehouseGroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PartWarehouseDataService PartWarehouseDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CostDataService CostDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesVM"/> class.
        /// </summary>
        public PartWarehousesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            PartWarehouseGroupDataService = new PartWarehouseGroupDataService();
            PartWarehouseDataService = new PartWarehouseDataService();
            PartWarehouseDataService.PartWarehouseAdded += OnPartWarehouseAdded;
            PartWarehouseGroupDataService.PartWarehouseGroupAdded += OnPartWarehouseGroupAdded;


            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code"), 
                new ColumnInfo("Name"), 
                new ColumnInfo("Quantity","txtQuantity",true), 
                new ColumnInfo("TotalCost","txtCost",true),
                new ColumnInfo("Status") ,
                new ColumnInfo("Mode",true) 
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
                PartWarehouseVM.CreateNew(PartWarehouseDataService,(int)parameter);
            }
            else if (CurrentContent is PartWarehouseGroupVM)
            {
                PartWarehouseVM.CreateNew(PartWarehouseDataService,((PartWarehouseGroupVM) CurrentContent).Id);
            }
            else if (CurrentContent is PartWarehouseVM)
            {
                PartWarehouseVM.CreateNew(PartWarehouseDataService,((PartWarehouseVM) CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            PartWarehouseGroupVM.CreateNew(PartWarehouseGroupDataService);
        }

        public override void View(object content)
        {
            if (content is PartWarehouseGroupVM)
            {
                CurrentContent = content as PartWarehouseGroupVM;
            }
            else if (content is PartWarehouseVM)
            {
                CurrentContent = content as PartWarehouseVM;
            }
            //CurrentContent = content as ISplitItemContent;
        }

        private void OnPartWarehouseAdded(object sender, ModelAddedEventArgs<PartWarehouse> e)
        {
            var newContent = new PartWarehouseVM(e.NewModel, GroupItems, Access,PartWarehouseDataService,PartWarehouseGroupDataService,CostDataService);
            Items.AddNewItem(newContent);
            Items.CommitNew();

            CurrentContent = newContent;
            CurrentContent.IsSelected = true;
        }

        private void OnPartWarehouseGroupAdded(object sender, ModelAddedEventArgs<PartWarehouseGroup> e)
        {
            var newPartWarehouseGroup = new PartWarehouseGroupVM(e.NewModel, Access, PartWarehouseGroupDataService);
            GroupItems.AddNewItem(newPartWarehouseGroup);
            GroupItems.CommitNew();

            foreach (PartWarehouseVM item in Items)
            {
                if (!item.Groups.Contains(newPartWarehouseGroup))
                {
                    item.Groups.AddNewItem(newPartWarehouseGroup);
                    item.Groups.CommitNew();
                }
            }
            Add(newPartWarehouseGroup.Id);
            ((PartWarehouseVM) CurrentContent).SelectedGroupVM = newPartWarehouseGroup;

            CurrentContent = newPartWarehouseGroup;
        }

        #endregion

    }
}