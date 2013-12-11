using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PartWarehouseVM : ItemContentViewModel
    {
        #region Properties
        private PartWarehouse _model;

        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem {get {return Code + Name + TotalCost;} set{} }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PartWarehouseDataService PartWarehouseDataService { get; set; }

        /// <summary>
        /// Gets or sets the part group data service.
        /// </summary>
        /// <value>
        /// The part group data service.
        /// </value>
        public PartWarehouseGroupDataService GroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CostDataService CostDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public int Quantity
        {
            get { return _model.Quantity ?? 0; }
            set { _model.Quantity = value; OnPropertyChanged("Quantity"); }
        }

        public int OriginalQuantity
        {
            get { return _model.OriginalQuantity ?? 0; }
            set { _model.OriginalQuantity = value; OnPropertyChanged("OriginalQuantity"); }
        }

        public double TotalCost
        {
            get { return _model.TotalCost?? 0; }
            set { _model.TotalCost = value; OnPropertyChanged("TotalCost"); }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte) value; }
        }

        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value;  OnPropertyChanged("CreatedDate"); }
        }

        [ReadOnly(true)]
        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        [ReadOnly(true)]
        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }

        public bool IsReadOnly
        {
            get
            {
                return PartWarehouseDataService.HasCost(Id);
            }
        }

        public StockStatus StockStatus
        {
            get
            {
                bool isReadOnly = IsReadOnly;
                if (!isReadOnly)
                {
                    return StockStatus.Full;
                }
                if (Quantity > 0)
                {
                    return StockStatus.Used;
                }
                return StockStatus.Empty;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PartWarehouseVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupItems">The group view models.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        /// <param name="costDataService"></param>
        public PartWarehouseVM(PartWarehouse entity, ListCollectionView groupItems, AccessType access, PartWarehouseDataService dataService, PartWarehouseGroupDataService groupDataService, CostDataService costDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService, costDataService);
            _model = entity;
            Groups = groupItems;
            foreach (PartWarehouseGroupVM groupVm in groupItems)
            {
                if (groupVm.Id == entity.PartWarehouseGroup.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartWarehouseVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        /// <param name="costDataService"></param>
        public PartWarehouseVM(PartWarehouse entity, AccessType access, PartWarehouseDataService dataService, PartWarehouseGroupDataService groupDataService, CostDataService costDataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService, groupDataService, costDataService);
            Groups = new ListCollectionView(new ObservableCollection<PartWarehouseGroupVM>());
        }

        private void InitializeData(PartWarehouseDataService dataService, PartWarehouseGroupDataService groupDataService, CostDataService costDataService)
        {
            PartWarehouseDataService = dataService;
            GroupDataService = groupDataService;
            CostDataService = costDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            if (!IsReadOnly)
                OriginalQuantity = Quantity;
            PartWarehouseDataService.AttachModel(_model,SelectedGroupVM.Id);
            _model = PartWarehouseDataService.GetSingle(_model.Id);
            OnPropertyChanged("ModifiedBy");
            OnPropertyChanged("ModifiedDate");
            OnPropertyChanged("IsReadOnly");
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
        public static PartWarehouse CreateNew(PartWarehouseDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new PartWarehouse { Name = "جدید", Code = string.Empty, 
                Quantity = 0, OriginalQuantity = 0, TotalCost= 0, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now}, groupId);
            return dataService.GetSingle(id);
        }

        #endregion
    }
}