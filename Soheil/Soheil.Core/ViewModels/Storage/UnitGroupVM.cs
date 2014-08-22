using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UnitGroupVM : ItemContentViewModel
    {
        #region Properties

        private UnitGroup _model;

        private ObservableCollection<UnitSet> _unitSets;

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

        public ObservableCollection<UnitSet> UnitSets
        {
            get { return _unitSets; }
            set
            {
                _unitSets = value;
                OnPropertyChanged("UnitSets");
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UnitGroupDataService UnitGroupDataService { get; set; }



        #endregion

        #region Method

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public UnitGroupVM(UnitGroup entity, AccessType access, UnitGroupDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;

            UnitSets = new ObservableCollection<UnitSet>();
            foreach (UnitSet unitSet in entity.UnitSets)
            {
                UnitSets.Add(unitSet);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitGroupVM"/> class initialized with default values.
        /// </summary>
        public UnitGroupVM(AccessType access, UnitGroupDataService dataService):base(access)
        {
            InitializeData(dataService);
            UnitSets = new ObservableCollection<UnitSet>();
        }

        private void InitializeData(UnitGroupDataService dataService)
        {
            UnitGroupDataService = dataService;
            SaveCommand = new Command(Save,CanSave);
        }

        public override void Save(object param)
        {
            UnitGroupDataService.AttachModel(_model);
            _model = UnitGroupDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }



        #endregion

        #region Static Methods
        public static UnitGroup CreateNew(UnitGroupDataService dataService)
        {
            int id = dataService.AddModel(new UnitGroup { Name = "جدید"});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}