using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;
using System.Windows;

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
        public UnitGroupDataService UnitGroupDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public double Inventory
        {
            get { return _model.Inventory; }
            set { _model.Inventory = value; OnPropertyChanged("Inventory"); }
        }

        public int SafetyStock
        {
            get { return _model.SafetyStock; }
            set { _model.SafetyStock = value; OnPropertyChanged("SafetyStock"); OnPropertyChanged("InventoryStatusColor"); }
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

        public SolidColorBrush InventoryStatusColor
        {
            get
            {
                return Inventory >= SafetyStock ? new SolidColorBrush(Colors.DarkSeaGreen) : new SolidColorBrush(Colors.IndianRed);
            }
        }
        public ObservableCollection<UnitGroupInfoVM> UnitGroupsVm { get; set; }

        /// <summary>
        /// Gets or sets a bindable value that indicates SelectedUnitGroup
        /// </summary>
        public UnitGroupInfoVM SelectedUnitGroup
        {
            get { return (UnitGroupInfoVM)GetValue(SelectedUnitGroupProperty); }
            set { SetValue(SelectedUnitGroupProperty, value); }
        }
        public static readonly DependencyProperty SelectedUnitGroupProperty =
            DependencyProperty.Register("SelectedUnitGroup", typeof(UnitGroupInfoVM), typeof(RawMaterialVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (RawMaterialVM)d;
                var val = (UnitGroupInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.UnitGroup = val.Model; 
            }));


        /// <summary>
        /// Gets or sets a bindable value that indicates BaseUnit
        /// </summary>
        public UnitSetInfoVM BaseUnit
        {
            get { return (UnitSetInfoVM)GetValue(BaseUnitProperty); }
            set { SetValue(BaseUnitProperty, value); }
        }
        public static readonly DependencyProperty BaseUnitProperty =
            DependencyProperty.Register("BaseUnit", typeof(UnitSetInfoVM), typeof(RawMaterialVM),
            new PropertyMetadata(null, (d, e) =>
            {
                var vm = (RawMaterialVM)d;
                var val = (UnitSetInfoVM)e.NewValue;
                if (val == null) return;
                vm._model.BaseUnit = val.Model;
            }));
        
		/// <summary>
		/// Gets or sets a bindable value that indicates UnitGroupVm
		/// </summary>
		public UnitGroupVM UnitGroupVm
		{
			get { return (UnitGroupVM)GetValue(UnitGroupVmProperty); }
			set { SetValue(UnitGroupVmProperty, value); }
		}
		public static readonly DependencyProperty UnitGroupVmProperty =
			DependencyProperty.Register("UnitGroupVm", typeof(UnitGroupVM), typeof(RawMaterialVM), new PropertyMetadata(null));

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
        public RawMaterialVM(RawMaterial entity, AccessType access, RawMaterialDataService dataService, UnitGroupDataService unitDataService)
            : base(access)
        {
            _model = entity;
            UnitGroupDataService = unitDataService;
            UnitGroupsVm = new ObservableCollection<UnitGroupInfoVM>();
            foreach (var unitGroup in UnitGroupDataService.GetActives())
            {
                UnitGroupsVm.Add(new UnitGroupInfoVM(unitGroup));
            }
            SelectedUnitGroup = UnitGroupsVm.FirstOrDefault(item => _model.UnitGroup != null && item.Id == _model.UnitGroup.Id);
            if (SelectedUnitGroup != null)
                BaseUnit = SelectedUnitGroup.UnitSets.FirstOrDefault(item => item.Id == _model.BaseUnit.Id);
            InitializeData(dataService);
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

        #endregion

        #region Static Methods
        public static RawMaterial CreateNew(RawMaterialDataService dataService)
        {
            int id = dataService.AddModel(new RawMaterial { Name = "جدید", Code = string.Empty, 
                Inventory = 0, SafetyStock = 0,
                CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}