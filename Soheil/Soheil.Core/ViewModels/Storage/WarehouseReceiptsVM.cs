using System;
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
            var viewModels = new ObservableCollection<WarehouseReceiptVM>();
            switch (ReceiptType)
            {
                case WarehouseReceiptType.None:
                    break;
                case WarehouseReceiptType.Storage:
                    //switch trans type
                    foreach (var model in WarehouseReceiptDataService.GetAll(ReceiptType))
                    {
                        viewModels.Add(new WarehouseReceiptVM(model, Access, WarehouseReceiptDataService, TransactionDataService,
                            Warehouses, RawMaterials, UnitSets, ReceiptType));
                    }
                    break;
                case WarehouseReceiptType.Transfer:
                    //switch trans type
                    break;
                case WarehouseReceiptType.Discharge:
                    //switch trans type
                    foreach (var model in WarehouseReceiptDataService.GetAll(ReceiptType))
                    {
                        viewModels.Add(new WarehouseReceiptVM(model, Access, WarehouseReceiptDataService, TransactionDataService,
                            Warehouses, Products, ReceiptType));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
        public RawMaterialDataService RawMaterialDataService { get; set; }
        public WarehouseReceiptDataService WarehouseReceiptDataService { get; set; }
        public WarehouseTransactionDataService TransactionDataService { get; set; }
        public WarehouseDataService WarehouseDataService { get; set; }
        public ProductDataService ProductDataService { get; set; }
        public UnitSetDataService UnitDataService { get; set; }

        public ObservableCollection<WarehouseInfoVM> Warehouses { get; set; }

        public ObservableCollection<RawMaterialInfoVM> RawMaterials { get; set; }

        public ObservableCollection<UnitSetInfoVM> UnitSets { get; set; }

        public ObservableCollection<ProductInfoVM> Products { get; set; }

        public WarehouseReceiptType ReceiptType { get; set; }
        public WarehouseTransactionType TransactionType { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseReceiptsVM"/> class.
        /// </summary>
        public WarehouseReceiptsVM(AccessType access, WarehouseReceiptType type, WarehouseTransactionType transType):base(access)
        {
            ReceiptType = type;
            TransactionType = transType;
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            WarehouseReceiptDataService = new WarehouseReceiptDataService(UnitOfWork);
            WarehouseReceiptDataService.WarehouseReceiptAdded += OnWarehouseReceiptAdded;

            TransactionDataService = new WarehouseTransactionDataService(UnitOfWork);
            WarehouseDataService = new WarehouseDataService(UnitOfWork);
            RawMaterialDataService = new RawMaterialDataService(UnitOfWork);
            UnitDataService = new UnitSetDataService(UnitOfWork);
            ProductDataService = new ProductDataService(UnitOfWork);

            ColumnHeaders = new List<ColumnInfo>
            {
                new ColumnInfo("Code", 0),
                new ColumnInfo("Description", 1),
                new ColumnInfo("Type", 2),
                new ColumnInfo("Mode", 3, true)
            };

            AddCommand = new Command(Add, CanAdd);
            RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);

            Warehouses = new ObservableCollection<WarehouseInfoVM>();
            foreach (var model in WarehouseDataService.GetActives())
            {
                Warehouses.Add(new WarehouseInfoVM(model));
            }
            switch (ReceiptType)
            {
                case WarehouseReceiptType.Storage:
                    RawMaterials = new ObservableCollection<RawMaterialInfoVM>();
                    foreach (var model in RawMaterialDataService.GetActives())
                    {
                        RawMaterials.Add(new RawMaterialInfoVM(model));
                    }
                    UnitSets = new ObservableCollection<UnitSetInfoVM>();
                    foreach (var model in UnitDataService.GetActives())
                    {
                        UnitSets.Add(new UnitSetInfoVM(model));
                    }
                    break;
                case WarehouseReceiptType.Transfer:
                    break;
                case WarehouseReceiptType.Discharge:
                    Products = new ObservableCollection<ProductInfoVM>();
                    foreach (var model in ProductDataService.GetActives())
                    {
                        Products.Add(new ProductInfoVM(model));
                    }
                    break;
            }

            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is WarehouseReceiptVM)
            {
                WarehouseReceiptVM.CreateNew(WarehouseReceiptDataService, ReceiptType);
                if (CurrentContent != null)
                {
                    ((WarehouseReceiptVM) CurrentContent).AddBlankTransaction();
                }
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
            WarehouseReceiptVM newWarehouseReceiptVm = null;
            switch ((WarehouseReceiptType)e.NewModel.Type)
            {
                case WarehouseReceiptType.None:
                    break;
                case WarehouseReceiptType.Storage:
                    newWarehouseReceiptVm = new WarehouseReceiptVM(e.NewModel, Access, WarehouseReceiptDataService, TransactionDataService, Warehouses, RawMaterials, UnitSets, ReceiptType);
                    break;
                case WarehouseReceiptType.Transfer:
                    break;
                case WarehouseReceiptType.Discharge:
                    newWarehouseReceiptVm = new WarehouseReceiptVM(e.NewModel, Access, WarehouseReceiptDataService, TransactionDataService, Warehouses, Products, ReceiptType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Items.AddNewItem(newWarehouseReceiptVm);
            Items.CommitNew();
            CurrentContent = newWarehouseReceiptVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}