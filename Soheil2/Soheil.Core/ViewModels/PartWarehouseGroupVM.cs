using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PartWarehouseGroupVM : ItemContentViewModel
    {
        #region Properties

        private PartWarehouseGroup _model;

        public override int Id
        {
            get { return _model.Id; } set{}
        }

        public override string SearchItem {get {return Name;} set{} }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }


        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PartWarehouseGroupDataService PartWarehouseGroupDataService { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// Initializes a new instance of the <see cref="PartWarehouseVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public PartWarehouseGroupVM(PartWarehouseGroup entity, AccessType access, PartWarehouseGroupDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartWarehouseGroupVM"/> class initialized with default values.
        /// </summary>
        public PartWarehouseGroupVM(AccessType access, PartWarehouseGroupDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        private void InitializeData(PartWarehouseGroupDataService dataService)
        {
            PartWarehouseGroupDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            PartWarehouseGroupDataService.AttachModel(_model);
            _model = PartWarehouseGroupDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
        public static PartWarehouseGroup CreateNew(PartWarehouseGroupDataService dataService)
        {
            int id = dataService.AddModel(new PartWarehouseGroup { Name = "گروه جدید" });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}