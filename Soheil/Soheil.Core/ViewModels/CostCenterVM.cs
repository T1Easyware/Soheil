
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class CostCenterVM : ItemContentViewModel
    {
        #region Properties

        private CostCenter _model;

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

        public CostSourceType CostSource
        {
            get { return (CostSourceType) _model.SourceType; }
            set { _model.SourceType = (byte) value; OnPropertyChanged("CostSource"); }
        }

        public Status Status
        {
            get { return (Status)_model.Status; }
            set { _model.Status = (byte)value; OnPropertyChanged("Status"); }
        }


        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CostCenterDataService CostCenterDataService { get; set; }


        #endregion

        #region Method

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public CostCenterVM(CostCenter entity, AccessType access, CostCenterDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CostCenterVM"/> class initialized with default values.
        /// </summary>
        public CostCenterVM(AccessType access, CostCenterDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        private void InitializeData(CostCenterDataService dataService)
        {
            CostCenterDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            CostCenterDataService.AttachModel(_model);
            _model = CostCenterDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }



        #endregion

        #region Static Methods
        public static CostCenter CreateNew(CostCenterDataService dataService)
        {
            int id = dataService.AddModel(new CostCenter { Name = "گروه جدید"});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}