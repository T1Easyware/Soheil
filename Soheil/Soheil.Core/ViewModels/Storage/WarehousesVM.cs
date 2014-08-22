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
    public class WarehousesVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<WarehouseVM>();
            foreach (var model in WarehouseDataService.GetAll())
            {
                viewModels.Add(new WarehouseVM(model, Access, WarehouseDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public WarehouseDataService WarehouseDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehousesVM"/> class.
        /// </summary>
        public WarehousesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            WarehouseDataService = new WarehouseDataService(UnitOfWork);
            WarehouseDataService.WarehouseAdded += OnWarehouseAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is WarehouseVM)
            {
                WarehouseVM.CreateNew(WarehouseDataService);
            }
        }

        public override void View(object content)
        {
            //else if (content is MachineVM)
            //{
            //    CurrentContent = content as MachineVM;
            //}
        }

        private void OnWarehouseAdded(object sender, ModelAddedEventArgs<Warehouse> e)
        {
            var newWarehouseVm = new WarehouseVM(e.NewModel, Access, WarehouseDataService);
            Items.AddNewItem(newWarehouseVm);
            Items.CommitNew();
            CurrentContent = newWarehouseVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}