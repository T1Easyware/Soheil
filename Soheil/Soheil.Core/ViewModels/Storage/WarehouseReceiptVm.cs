using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.DataServices.Storage;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class WarehouseReceiptVM : ItemContentViewModel
    {
        #region Properties

        private WarehouseReceipt _model;
        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem { get { return Code + Description; } set { } }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtDescriptionRequired")]
// ReSharper restore PropertyNotResolved
        public string Description
        {
            get { return _model.Description; }
            set { _model.Description = value; OnPropertyChanged("Description"); }
        }

        public bool Transported
        {
            get { return _model.Transported; }
            set { _model.Transported = value; OnPropertyChanged("Transported"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>

        public WarehouseTransactionDataService TransactionDataService { get; set; }

        public WarehouseReceiptDataService WarehouseReceiptDataService { get; set; }

        public UnitSetDataService UnitDataService { get; set; }

        public IEnumerable<WarehouseInfoVM> Warehouses { get; set; }

        public IEnumerable<RawMaterialInfoVM> RawMaterials { get; set; }

        public IEnumerable<ProductInfoVM> Products { get; set; }

        public IEnumerable<UnitSetInfoVM> UnitSets { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; }
        }

        public WarehouseReceiptType Type
        {
            get { return (WarehouseReceiptType)_model.Type; }
            set { _model.Type = (byte)value; }
        }

        public WarehouseTransactionType TransactionType { get; set; }

        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value; OnPropertyChanged("CreatedDate"); }
        }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }

        public ObservableCollection<WarehouseTransactionVM> Transactions { get; set; }

        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register(
            "SelectedSource", typeof (WarehouseInfoVM), typeof (WarehouseReceiptVM), new PropertyMetadata(default(WarehouseInfoVM)));

        public WarehouseInfoVM SelectedSource
        {
            get { return (WarehouseInfoVM) GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedDestinationProperty = DependencyProperty.Register(
            "SelectedDestination", typeof (WarehouseInfoVM), typeof (WarehouseReceiptVM), new PropertyMetadata(default(WarehouseInfoVM)));

        public WarehouseInfoVM SelectedDestination
        {
            get { return (WarehouseInfoVM) GetValue(SelectedDestinationProperty); }
            set { SetValue(SelectedDestinationProperty, value); }
        }

        public static readonly DependencyProperty CurrentTransactionProperty = DependencyProperty.Register(
            "CurrentTransaction", typeof (WarehouseTransactionVM), typeof (WarehouseReceiptVM), new PropertyMetadata(default(WarehouseTransactionVM)));

        public WarehouseTransactionVM CurrentTransaction
        {
            get { return (WarehouseTransactionVM) GetValue(CurrentTransactionProperty); }
            set { SetValue(CurrentTransactionProperty, value); }
        }
  #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public WarehouseReceiptVM(AccessType access, WarehouseReceiptDataService dataService):base(access)
        {
            //InitializeData(dataService, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public WarehouseReceiptVM(WarehouseReceipt entity, AccessType access, WarehouseReceiptDataService dataService, WarehouseTransactionDataService transactionDataService, IEnumerable<WarehouseInfoVM> warehouses, IEnumerable<RawMaterialInfoVM> materials, IEnumerable<UnitSetInfoVM> units, WarehouseReceiptType type)
            : base(access)
        {
            _model = entity;
            TransactionDataService = transactionDataService;
            Type = type;
            TransactionType = WarehouseTransactionType.RawMaterial;
            TransactionDataService.TransactionRemoved += TransactionRemoved;
            Warehouses = warehouses;
            RawMaterials = materials;
            UnitSets = units;

            Transactions = new ObservableCollection<WarehouseTransactionVM>();
            foreach (var transaction in TransactionDataService.GetActives(Id))
            {
                Transactions.Add(new WarehouseTransactionVM(transaction, Access, TransactionDataService, _model, warehouses, materials, units));
            }

            SaveCommand = new Command(Save, CanSave);
        }

        public WarehouseReceiptVM(WarehouseReceipt entity, AccessType access, WarehouseReceiptDataService dataService, WarehouseTransactionDataService transactionDataService, IEnumerable<WarehouseInfoVM> warehouses, IEnumerable<ProductInfoVM> products, WarehouseReceiptType type)
            : base(access)
        {
            _model = entity;
            TransactionDataService = transactionDataService;
            Type = type;
            TransactionType = WarehouseTransactionType.Product;
            TransactionDataService.TransactionRemoved += TransactionRemoved;
            Warehouses = warehouses;
            Products = products;

            Transactions = new ObservableCollection<WarehouseTransactionVM>();
            foreach (var transaction in TransactionDataService.GetActives(Id))
            {
                Transactions.Add(new WarehouseTransactionVM(transaction, Access, TransactionDataService, _model, warehouses, products));
            }

            SaveCommand = new Command(Save, CanSave);
        }
        public override void Save(object param)
        {
            WarehouseReceiptDataService.AttachModel(_model);
            _model = WarehouseReceiptDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        void TransactionRemoved(object sender, ModelRemovedEventArgs e)
        {
            CurrentTransaction = Transactions.FirstOrDefault(item => item.Id != e.Id);
            Transactions.RemoveWhere(item=> item.Id == e.Id);
        }
        public void AddBlankTransaction()
        {
            var model = WarehouseTransactionVM.CreateNew(TransactionDataService, _model);
            WarehouseTransactionVM blankVm = null;
            switch (TransactionType)
            {
                case WarehouseTransactionType.None:
                    break;
                case WarehouseTransactionType.RawMaterial:
                    blankVm = new WarehouseTransactionVM(model, Access, TransactionDataService, _model, Warehouses, RawMaterials, UnitSets);
                    break;
                case WarehouseTransactionType.Product:
                    blankVm = new WarehouseTransactionVM(model, Access, TransactionDataService, _model, Warehouses, Products);
                    break;
                case WarehouseTransactionType.Good:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("TransactionType");
            }
            Transactions.Add(blankVm);
            CurrentTransaction = Transactions[0];
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; WarehouseReceiptDataService.AttachModel(_model);
        }
        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
        public override void ViewItemLink(object param)
        {
            base.ViewItemLink(param);
        }

        #endregion

        #region Static Methods
        public static WarehouseReceipt CreateNew(WarehouseReceiptDataService dataService, WarehouseReceiptType type)
        {
            int id = dataService.AddModel(new WarehouseReceipt { Description = "جدید", Code = string.Empty, Type = (byte) type, RecordDateTime = DateTime.Now ,CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
            
        }
        #endregion
    }
}