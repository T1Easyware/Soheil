using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class RawMaterialVM : ItemContentViewModel
    {
        #region Properties

        private RawMaterial _model;
        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem {get {return Code + Name;} set{} }

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
        public RawMaterialDataService RawMaterialDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public double AvailableInventory
        {
            get { return _model.AvailableInventory; }
            set { _model.AvailableInventory = value; OnPropertyChanged("AvailableInventory"); }
        }

        public double ActualInventory
        {
            get { return _model.ActualInventory; }
            set { _model.ActualInventory = value; OnPropertyChanged("ActualInventory"); }
        }

        public int SafetyStock
        {
            get { return _model.SafetyStock; }
            set { _model.SafetyStock = value; OnPropertyChanged("SafetyStock"); }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; }
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

        public RawMaterialUnitGroupsVM UnitGroupsVm { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public RawMaterialVM(AccessType access, RawMaterialDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public RawMaterialVM(RawMaterial entity, AccessType access, RawMaterialDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        private void InitializeData(RawMaterialDataService dataService)
        {
            RawMaterialDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            RawMaterialDataService.AttachModel(_model);
            _model = RawMaterialDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; RawMaterialDataService.AttachModel(_model);
        }
        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        public override void ViewItemLink(object param)
        {
            UnitGroupsVm = new RawMaterialUnitGroupsVM(this, Access);
            CurrentLink = UnitGroupsVm;
            base.ViewItemLink(param);
        }

        #endregion

        #region Static Methods
        public static RawMaterial CreateNew(RawMaterialDataService dataService)
        {
            int id = dataService.AddModel(new RawMaterial { Name = "جدید", Code = string.Empty, 
                AvailableInventory = 0, ActualInventory = 0, SafetyStock = 0,
                CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}