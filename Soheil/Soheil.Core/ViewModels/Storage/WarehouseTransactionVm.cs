using System;
using System.Collections.Generic;
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
                vm.OnPropertyChanged("Fee");
            }));

        public double Price
        {
            get
            {
                double value = 0;
                if ( _model.Product != null && _model.Product.ProductPrices.Any())
                {
                    var pricesQuery = _model.Product.ProductPrices.Where(p => p.StartDateTime <= DateTime.Now);
                    var prices = pricesQuery.ToList();
                    if (prices.Any())
                    {
                        DateTime max = DateTime.MinValue;
                        foreach (var price in prices)
                        {
                            if (price.StartDateTime > max)
                            {
                                max = price.StartDateTime;
                                value = price.Value;
                            }
                        }
                    }
                }
                return value;
            }
        }

        public double ProductInventory
        {
            get
            {
                if (_model.ProductRework != null)
                {
                    return _model.ProductRework.Inventory;
                }
                return 0;
            }
        }

        public double MaterialInventory
        {
            get
            {
                if (_model.RawMaterial != null)
                {
                    // value depends on selected unit
                    return _model.RawMaterial.Inventory;
                }
                return 0;
            }
        }

        public double Fee
        {
            get { return SalePrice == 0 ? Price*Quantity : SalePrice*Quantity; }
        }

        public double SalePrice
        {
            get { return _model.Price; }
            set { _model.Price = value; OnPropertyChanged("SalePrice"); }
        }

        public bool Transported
        {
            get { return _model.Transported; }
            set { _model.Transported = value; OnPropertyChanged("Transported"); }
        }
        public IEnumerable<UnitSetInfoVM> UnitSets { get; set; }

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
                    foreach (var unitSet in vm.UnitSets)
                    {
                        if (unitSet.Id == val.Model.BaseUnit.Id)
                        {
                            vm.SelectedUnit = unitSet;
                            break;
                        }
                    }
                }
                vm.OnPropertyChanged("MaterialInventory");
            }));

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
                vm.OnPropertyChanged("MaterialInventory");
            }));

        /// <summary>
        /// Gets or sets a bindable value that indicates SelectedProduct
        /// </summary>
        public ProductReworkInfoVM SelectedProduct
        {
            get { return (ProductReworkInfoVM)GetValue(SelectedProductProperty); }
            set { SetValue(SelectedProductProperty, value); }
        }
        public static readonly DependencyProperty SelectedProductProperty =
            DependencyProperty.Register("SelectedProduct", typeof(ProductReworkInfoVM), typeof(WarehouseTransactionVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (WarehouseTransactionVM)d;
                var val = (ProductReworkInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.ProductRework = val.Model;
                vm._model.Product = val.Model.Product;
                vm.OnPropertyChanged("ProductInventory");
                vm.OnPropertyChanged("Price");
                vm.OnPropertyChanged("Fee");
            }));
        

        public bool IsReadOnly
        {
            get
            {
                switch (Type)
                {
                    case WarehouseTransactionType.None:
                        return true;
                    case WarehouseTransactionType.RawMaterial:
                        return SelectedDestination != null &&
                               SelectedRawMaterial != null &&
                               SelectedUnit != null &&
                               Quantity != 0;
                    case WarehouseTransactionType.Product:
                        return SelectedSource != null &&
                               SelectedProduct != null &&
                               Quantity != 0;
                    case WarehouseTransactionType.Good:
                        return true;
                    default:
                        return true;
                }
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
        public WarehouseTransactionVM(WarehouseTransaction entity, AccessType access, WarehouseTransactionDataService dataService, WarehouseReceipt groupModel, IEnumerable<WarehouseInfoVM> warehouses, IEnumerable<RawMaterialInfoVM> materials, IEnumerable<UnitSetInfoVM> units)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            _model.WarehouseReceipt = groupModel;
            Type = WarehouseTransactionType.RawMaterial;
            Quantity = _model.Quantity;
            UnitSets = units;

            SelectedDestination = warehouses.FirstOrDefault(dest => _model.DestWarehouse != null && dest.Id == _model.DestWarehouse.Id);
            SelectedSource = warehouses.FirstOrDefault(src => _model.SrcWarehouse != null && src.Id == _model.SrcWarehouse.Id);
            SelectedRawMaterial = materials.FirstOrDefault(mat => _model.RawMaterial != null && mat.Id == _model.RawMaterial.Id);
            SelectedUnit = UnitSets.FirstOrDefault(unit => _model.UnitSet != null && unit.Id == _model.UnitSet.Id);
        }
        public WarehouseTransactionVM(WarehouseTransaction entity, AccessType access, WarehouseTransactionDataService dataService, WarehouseReceipt groupModel, IEnumerable<WarehouseInfoVM> warehouses, IEnumerable<ProductReworkInfoVM> products)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            _model.WarehouseReceipt = groupModel;
            Type = WarehouseTransactionType.Product;
            Quantity = _model.Quantity;

            SelectedDestination = warehouses.FirstOrDefault(dest => _model.DestWarehouse != null && dest.Id == _model.DestWarehouse.Id);
            SelectedSource = warehouses.FirstOrDefault(src => _model.SrcWarehouse != null && src.Id == _model.SrcWarehouse.Id);
            SelectedProduct = products.FirstOrDefault(prd => _model.Product != null && prd.Id == _model.Product.Id);
        }

        private void InitializeData(WarehouseTransactionDataService dataService)
        {
            WarehouseTransactionDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            SalePrice = Price;
            WarehouseTransactionDataService.AttachModel(_model);
            _model = WarehouseTransactionDataService.GetSingle(_model.Id); 
            OnPropertyChanged("ModifiedBy");
            OnPropertyChanged("ModifiedDate");
            OnPropertyChanged("IsReadOnly");
            OnPropertyChanged("SalePrice");
            OnPropertyChanged("ProductInventory");
            OnPropertyChanged("MaterialInventory");
            Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            WarehouseTransactionDataService.DeleteModel(_model);
            OnPropertyChanged("ProductInventory");
            OnPropertyChanged("MaterialInventory");
        }
        public override bool CanSave()
        {
            return AllDataValid() && IsReadOnly && CheckInventory() && base.CanSave();
        }
        public override void ViewItemLink(object param)
        {
            base.ViewItemLink(param);
        }

        private bool CheckInventory()
        {
            switch (Type)
            {
                case WarehouseTransactionType.None:
                    return false;
                case WarehouseTransactionType.RawMaterial:
                    return true;
                case WarehouseTransactionType.Product:
                    if (ProductInventory > Quantity) return true;
                    return false;
                case WarehouseTransactionType.Good:
                    return false;
                default:
                    return false;
            }
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