using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.DataServices.Storage;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Dal;
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

        public ObservableCollection<WarehouseInfoVM> Sources { get; set; }

        public ObservableCollection<WarehouseInfoVM> Destinations { get; set; }

        public ObservableCollection<WarehouseTransactionVM> Transactions { get; set; }

        public ObservableCollection<RawMaterialInfoVM> RawMaterials { get; set; }

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
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public WarehouseReceiptVM(WarehouseReceipt entity, AccessType access, WarehouseReceiptDataService dataService, WarehouseDataService warehouseDataService, RawMaterialDataService rawMaterialDataService, WarehouseTransactionDataService transactionDataService, WarehouseReceiptType type )
            : base(access)
        {
            _model = entity;
            Type = type;
            WarehouseDataService = warehouseDataService;
            RawMaterialDataService = rawMaterialDataService;
            TransactionDataService = transactionDataService;
            InitializeData(dataService);
        }

        private void InitializeData(WarehouseReceiptDataService dataService)
        {
            WarehouseReceiptDataService = dataService;

            RawMaterials = new ObservableCollection<RawMaterialInfoVM>();
            foreach (var model in RawMaterialDataService.GetActives())
            {
                RawMaterials.Add(new RawMaterialInfoVM(model));
            }

            switch (Type)
            {
                case WarehouseReceiptType.Storage:
                    Destinations = new ObservableCollection<WarehouseInfoVM>();
                    foreach (var model in WarehouseDataService.GetActives())
                    {
                        Destinations.Add(new WarehouseInfoVM(model));
                    }
                    break;
                case WarehouseReceiptType.Transfer:
                    break;
                case WarehouseReceiptType.Discharge:
                    break;
            }

            Transactions = new ObservableCollection<WarehouseTransactionVM>();
            foreach (var transaction in TransactionDataService.GetActives(Id))
            {
                Transactions.Add(new WarehouseTransactionVM(transaction, Access, TransactionDataService, _model, Destinations, RawMaterials, WarehouseTransactionType.RawMaterial));
            }

            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            WarehouseReceiptDataService.AttachModel(_model);
            _model = WarehouseReceiptDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public void AddBlankTransaction()
        {
            var model = WarehouseTransactionVM.CreateNew(TransactionDataService, _model);
            var blankVm = new WarehouseTransactionVM(model, Access, TransactionDataService, _model, Destinations, RawMaterials, WarehouseTransactionType.RawMaterial);
            Transactions.Add(blankVm);
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
        public static WarehouseReceipt CreateNew(WarehouseReceiptDataService dataService)
        {
            int id = dataService.AddModel(new WarehouseReceipt { Description = "جدید", Code = string.Empty, Type = (byte) WarehouseReceiptType.None, RecordDateTime = DateTime.Now ,CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
            
        }
        #endregion
    }
}