using System;
using System.Collections.ObjectModel;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices.Storage;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class WarehouseTransactionVM : ItemContentViewModel
    {
        #region Properties

        private WarehouseTransaction _model;
        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem {get {return Code;} set{} }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public WarehouseTransactionDataService WarehouseTransactionDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public WarehouseTransactionType Type
        {
            get { return (WarehouseTransactionType)_model.Type; }
            set { _model.Type = (byte)value; }
        }
        public WarehouseTransactionFlow Flow
        {
            get { return (WarehouseTransactionFlow)_model.Flow; }
            set { _model.Flow = (byte)value; }
        }

        public double Quantity
        {
            get { return _model.Quantity; }
            set { _model.Quantity = value; OnPropertyChanged("Quantity"); }
        }

        public ObservableCollection<RawMaterialInfoVM> RawMaterials { get; set; }

        public RawMaterialInfoVM SelectedRawMaterial { get; set; }

        public static readonly DependencyProperty SelectedUnitProperty = DependencyProperty.Register(
            "SelectedUnit", typeof(UnitSetInfoVM), typeof(WarehouseTransactionVM), new PropertyMetadata(default(UnitSetInfoVM)));

        public UnitSetInfoVM SelectedUnit
        {
            get { return (UnitSetInfoVM)GetValue(SelectedUnitProperty); }
            set { SetValue(SelectedUnitProperty, value); }
        }

        public DateTime RecordDateTime
        {
            get { return _model.RecordDateTime; }
            set { _model.RecordDateTime = value; OnPropertyChanged("RecordDateTime"); }
        }

        public DateTime TransactionDateTime
        {
            get { return _model.TransactionDateTime; }
            set { _model.TransactionDateTime = value; OnPropertyChanged("TransactionDateTime"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }

  #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public WarehouseTransactionVM(AccessType access, WarehouseTransactionDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public WarehouseTransactionVM(WarehouseTransaction entity, AccessType access, WarehouseTransactionDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public WarehouseTransactionVM(WarehouseTransaction entity, AccessType access, WarehouseTransactionDataService dataService, ObservableCollection<RawMaterialInfoVM> rawMaterials )
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            RawMaterials = rawMaterials;
        }

        private void InitializeData(WarehouseTransactionDataService dataService)
        {
            WarehouseTransactionDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            WarehouseTransactionDataService.AttachModel(_model);
            _model = WarehouseTransactionDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            //_model.Status = (byte)Status.Deleted; WarehouseTransactionDataService.AttachModel(_model);
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
        public static WarehouseTransaction CreateNew(WarehouseTransactionDataService dataService)
        {
            int id = dataService.AddModel(new WarehouseTransaction { Code = string.Empty, Quantity = 0, TransactionDateTime = DateTime.Now, RecordDateTime = DateTime.Now});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}