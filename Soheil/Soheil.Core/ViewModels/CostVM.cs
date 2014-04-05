using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class CostVM : ItemContentViewModel
    {
        #region Properties
        private Cost _model;

        public override int Id
        {
            get { return _model.Id;}
            set { }
        }

        public override string SearchItem {get {return Description + CostValue;} set{} }

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
        public PartWarehouseDataService WarehouseDataService { get; set; }

        /// <summary>
        /// Gets or sets the cost group data service.
        /// </summary>
        /// <value>
        /// The cost group data service.
        /// </value>
        public CostCenterDataService GroupDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Description
        {
            get { return _model.Description; }
            set { _model.Description = value; OnPropertyChanged("Description"); }
        }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtQuantityRequired")]
// ReSharper restore PropertyNotResolved
        public int SelectedQuantity
        {
            get { return _model.Quantity?? 0; }
            set { _model.Quantity = value; OnPropertyChanged("SelectedQuantity"); }
        }
        public DateTime Date
        {
            get { return _model.Date; }
            set { _model.Date = value;  OnPropertyChanged("Date"); }
        }

        public double CostValue
        {
            get { return _model.CostValue ?? 0; }
            set
            {
                _model.CostValue = value;
                OnPropertyChanged("CostValue"); 
                OnPropertyChanged("IsCostValueSet");
                OnPropertyChanged("AllowChangeCostType");
                OnPropertyChanged("IsEnable");
                OnPropertyChanged("ComputationalCostVisibility");
                OnPropertyChanged("FixedCostVisibility"); 
            }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte) value; OnPropertyChanged("Status"); }
        }

        public CostType CostType
        {
            get { return (CostType)_model.CostType; }
            set { _model.CostType = (byte)value; OnPropertyChanged("CostType"); }
        }


        public static readonly DependencyProperty SelectedCostSourceProperty =
            DependencyProperty.Register("SelectedCostSource", typeof(IInfoViewModel), typeof(CostVM), null);
        public static readonly DependencyProperty SelectedWarehouseVMProperty =
            DependencyProperty.Register("SelectedWarehouseVM", typeof(PartWarehouseInfoVM), typeof(CostVM), null);

        public IInfoViewModel SelectedCostSource
        {
            get { return (IInfoViewModel)GetValue(SelectedCostSourceProperty); }
            set { SetValue(SelectedCostSourceProperty, value); }
        }

        public PartWarehouseInfoVM SelectedWarehouseVM
        {
            get { return (PartWarehouseInfoVM)GetValue(SelectedWarehouseVMProperty); }
            set { SetValue(SelectedWarehouseVMProperty, value); }
        }

        public static readonly DependencyProperty CostSourcesProperty =
            DependencyProperty.Register("CostSources", typeof(ListCollectionView), typeof(CostVM), null);
        public static readonly DependencyProperty WarehousesProperty =
            DependencyProperty.Register("Warehouses", typeof(ListCollectionView), typeof(CostVM), null);

        public ListCollectionView CostSources
        {
            get { return (ListCollectionView)GetValue(CostSourcesProperty); }
            set { SetValue(CostSourcesProperty, value); }
        }

        public ListCollectionView Warehouses
        {
            get { return (ListCollectionView)GetValue(WarehousesProperty); }
            set { SetValue(WarehousesProperty, value); }
        }

        public CostSourceType SourceType
        {
            get
            {
                return (CostSourceType)_model.CostCenter.SourceType;
            }
        }

        public bool IsCostValueSet
        {
            get
            {
                return Math.Abs(CostValue - 0) > 0.00001;
            }
        }

        public bool AllowChangeCostType
        {
            get
            {
                return !IsCostValueSet;
            }
        }

        public bool IsEnable
        {
            get
            {
                return !IsCostValueSet || CostType == CostType.Cash;
            }
        }
        public Visibility ComputationalCostVisibility
        {
            get
            {
                if (IsCostValueSet) return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        public Visibility FixedCostVisibility
        {
            get
            {
                if (IsCostValueSet) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CostVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupItems">The group view models.</param>
        /// <param name="warehouseItems">the warehouse view models</param>
        /// <param name="costSourceItems">machines, operators or stations</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public CostVM(Cost entity, ListCollectionView groupItems, ListCollectionView warehouseItems, ListCollectionView costSourceItems, AccessType access, CostDataService dataService, CostCenterDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = groupItems;
            CostSources = costSourceItems;
            Warehouses = warehouseItems;

            foreach (CostCenterVM groupVm in groupItems)
            {
                if (groupVm.Id == entity.CostCenter.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
            if (entity.PartWarehouse != null)
            {
                foreach (PartWarehouseInfoVM warehouse in Warehouses)
                {
                    if (warehouse.Id == entity.PartWarehouse.Id)
                    {
                        SelectedWarehouseVM = warehouse;
                        break;
                    }
                }
            }

            switch (SourceType)
            {
                case CostSourceType.Machines:
                    if(entity.Machine == null) break;
                    foreach (IInfoViewModel infoVM in CostSources)
                    {
                        if (infoVM.Id == entity.Machine.Id)
                        {
                            SelectedCostSource = infoVM;
                            break;
                        }
                    }
                    break;
                case CostSourceType.Operators:
                    if (entity.Operator == null) break;
                    foreach (IInfoViewModel infoVM in CostSources)
                    {
                        if (infoVM.Id == entity.Operator.Id)
                        {
                            SelectedCostSource = infoVM;
                            break;
                        }
                    }
                    break;
                case CostSourceType.Stations:
                    if (entity.Station == null) break;
                    foreach (IInfoViewModel infoVM in CostSources)
                    {
                        if (infoVM.Id == entity.Station.Id)
                        {
                            SelectedCostSource = infoVM;
                            break;
                        }
                    }
                    break;
                case CostSourceType.Activities:
                    if (entity.Activity == null) break;
                    foreach (IInfoViewModel infoVM in CostSources)
                    {
                        if (infoVM.Id == entity.Activity.Id)
                        {
                            SelectedCostSource = infoVM;
                            break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CostVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public CostVM(Cost entity, AccessType access, CostDataService dataService, CostCenterDataService groupDataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService, groupDataService);
            Groups = new ListCollectionView(new ObservableCollection<CostCenterVM>());
        }

        private void InitializeData(CostDataService dataService, CostCenterDataService groupDataService)
        {
            CostDataService = dataService;
            GroupDataService = groupDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            if (CostType == CostType.Stock && !IsCostValueSet)
                SetCostParams();
            CostDataService.UpdateModel(_model, 
                SelectedGroupVM.Id, SelectedWarehouseVM == null? null : SelectedWarehouseVM.Model, 
                SelectedCostSource == null? -1 : SelectedCostSource.Id);
            _model = CostDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            if (AllDataValid())
                return (IsCostValueSet && CostType == CostType.Cash)
                    || (SelectedWarehouseVM != null 
                    && SelectedQuantity > 0
                    && Status == Status.Active
                    && SelectedWarehouseVM.Quantity >= SelectedQuantity);
            return false;
        }

        private void SetCostParams()
        {
            bool readonlyStatus = IsCostValueSet;
            var unitCost = SelectedWarehouseVM.TotalCost / SelectedWarehouseVM.OriginalQuantity;

            if (SelectedWarehouseVM.Quantity == SelectedQuantity)
            {
                CostValue = SelectedWarehouseVM.TotalCost - ((SelectedWarehouseVM.OriginalQuantity - SelectedQuantity) * unitCost);
            }
            CostValue = SelectedQuantity * unitCost;

            if (!readonlyStatus)
            {
                switch (Status)
                {
                    case Status.Active:
                        SelectedWarehouseVM.Quantity = SelectedWarehouseVM.Quantity - SelectedQuantity;
                        break;
                    default:
                        SelectedWarehouseVM.Quantity = SelectedWarehouseVM.Quantity + SelectedQuantity;
                        break;
                }
            }
            else
            {
                if (Status != Status.Active) SelectedWarehouseVM.Quantity = SelectedWarehouseVM.Quantity + SelectedQuantity;
            }
        }

        #endregion

        #region Static Methods
        public static Cost CreateNew(CostDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new Cost { Description = "جدید", CostValue = 0, CostType = 1, Date = DateTime.Now }, groupId);
            return dataService.GetSingle(id);
        }

        #endregion
    }
}