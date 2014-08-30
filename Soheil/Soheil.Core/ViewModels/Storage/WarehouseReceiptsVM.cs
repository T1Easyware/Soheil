using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.DataServices.Storage;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class WarehouseReceiptsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var rawMaterials = new ObservableCollection<RawMaterialInfoVM>();
            foreach (var model in RawMaterialDataService.GetActives())
            {
                rawMaterials.Add(new RawMaterialInfoVM(model));
            }
            var viewModels = new ObservableCollection<WarehouseReceiptVM>();
            foreach (var model in WarehouseReceiptDataService.GetAll())
            {
                viewModels.Add(new WarehouseReceiptVM(model, Access, WarehouseReceiptDataService, WarehouseReceiptType.Storage /* determine later*/, rawMaterials));
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
        public WarehouseReceiptDataService WarehouseReceiptDataService { get; set; }
        public RawMaterialDataService RawMaterialDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseReceiptsVM"/> class.
        /// </summary>
        public WarehouseReceiptsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            WarehouseReceiptDataService = new WarehouseReceiptDataService(UnitOfWork);
            WarehouseReceiptDataService.WarehouseReceiptAdded += OnWarehouseReceiptAdded;
            RawMaterialDataService = new RawMaterialDataService();

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Description",1), 
                new ColumnInfo("Type",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is WarehouseReceiptVM)
            {
                WarehouseReceiptVM.CreateNew(WarehouseReceiptDataService);
            }
        }

        public override void View(object content)
        {
            //else if (content is MachineVM)
            //{
            //    CurrentContent = content as MachineVM;
            //}
        }

        private void OnWarehouseReceiptAdded(object sender, ModelAddedEventArgs<WarehouseReceipt> e)
        {
            var newWarehouseReceiptVm = new WarehouseReceiptVM(e.NewModel, Access, WarehouseReceiptDataService, WarehouseReceiptType.Storage /* determine later*/, new ObservableCollection<RawMaterialInfoVM>());
            Items.AddNewItem(newWarehouseReceiptVm);
            Items.CommitNew();
            CurrentContent = newWarehouseReceiptVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}