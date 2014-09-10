﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtQuantityRequired")]
        // ReSharper restore PropertyNotResolved
        public double Quantity
        {
            get { return (double)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(double), typeof(WarehouseTransactionVM),
            new PropertyMetadata(default(double), (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (double)e.NewValue;
                if (val < 0) return;
                vm._model.Quantity = val;
            }));
          

        public ObservableCollection<WarehouseInfoVM> Warehouses { get; set; }

        public ObservableCollection<RawMaterialInfoVM> RawMaterials { get; set; }

        /// <summary>
        /// Gets or sets a bindable value that indicates SelectedSource
        /// </summary>
        public WarehouseInfoVM SelectedSource
        {
            get { return (WarehouseInfoVM)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }
        public static readonly DependencyProperty SelectedSourceProperty =
            DependencyProperty.Register("SelectedSource", typeof(WarehouseInfoVM), typeof(WarehouseTransactionVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (WarehouseInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.SrcWarehouse = val.Model;
            }));

        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
        // ReSharper restore PropertyNotResolved
        public WarehouseInfoVM SelectedDestination
        {
            get { return (WarehouseInfoVM)GetValue(SelectedDestinationProperty); }
            set { SetValue(SelectedDestinationProperty, value); }
        }
        public static readonly DependencyProperty SelectedDestinationProperty =
            DependencyProperty.Register("SelectedDestination", typeof(WarehouseInfoVM), typeof(WarehouseTransactionVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (WarehouseInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.DestWarehouse = val.Model;
            }));


        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
        // ReSharper restore PropertyNotResolved
        public RawMaterialInfoVM SelectedRawMaterial
        {
            get { return (RawMaterialInfoVM)GetValue(SelectedRawMaterialProperty); }
            set { SetValue(SelectedRawMaterialProperty, value); }
        }
        public static readonly DependencyProperty SelectedRawMaterialProperty =
            DependencyProperty.Register("SelectedRawMaterial", typeof(RawMaterialInfoVM), typeof(WarehouseTransactionVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (RawMaterialInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.RawMaterial = val.Model;
                if (val.Model.BaseUnit != null)
                {
                    foreach (var unitSet in val.UnitSets)
                    {
                        if (unitSet.Id == val.Model.BaseUnit.Id)
                        {
                            vm.SelectedUnit = unitSet;
                            break;
                        }
                    }
                }
            }));

        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
        // ReSharper restore PropertyNotResolved
        public UnitSetInfoVM SelectedUnit
        {
            get { return (UnitSetInfoVM)GetValue(SelectedUnitProperty); }
            set { SetValue(SelectedUnitProperty, value); }
        }
        public static readonly DependencyProperty SelectedUnitProperty =
            DependencyProperty.Register("SelectedUnit", typeof(UnitSetInfoVM), typeof(WarehouseTransactionVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (UnitSetInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.UnitSet = val.Model;
            }));

        public bool IsEditable
        {
            get
            {
                return SelectedDestination == null ||
                       SelectedRawMaterial == null ||
                       SelectedUnit == null ||
                       Quantity == 0;
            }
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
        /// <param name="dataService"></param>
        public WarehouseTransactionVM(WarehouseTransaction entity, AccessType access, WarehouseTransactionDataService dataService, WarehouseReceipt groupModel, ObservableCollection<WarehouseInfoVM> warehouses, ObservableCollection<RawMaterialInfoVM> materials, WarehouseTransactionType type)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            _model.WarehouseReceipt = groupModel;
            Type = type;
            Quantity = _model.Quantity;
            Warehouses = warehouses;
            RawMaterials = materials;
            SelectedDestination = Warehouses.FirstOrDefault(dest => _model.DestWarehouse != null && dest.Id == _model.DestWarehouse.Id);
            SelectedSource = Warehouses.FirstOrDefault(src => _model.SrcWarehouse != null && src.Id == _model.SrcWarehouse.Id);
            SelectedRawMaterial = RawMaterials.FirstOrDefault(mat => _model.RawMaterial != null && mat.Id == _model.RawMaterial.Id);
            if(SelectedRawMaterial != null)
                SelectedUnit = SelectedRawMaterial.UnitSets.FirstOrDefault(unit => _model.UnitSet != null && unit.Id == _model.UnitSet.Id);
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
        public static WarehouseTransaction CreateNew(WarehouseTransactionDataService dataService, WarehouseReceipt groupModel)
        {
            int id = dataService.AddModel(new WarehouseTransaction { WarehouseReceipt = groupModel,  Code = string.Empty, Quantity = 0, TransactionDateTime = DateTime.Now, RecordDateTime = DateTime.Now}, false);
            return dataService.GetSingle(id);
        }
        #endregion
    }
}