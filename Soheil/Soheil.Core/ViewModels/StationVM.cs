using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class StationVM : ItemContentViewModel
    {
        #region Properties

        private Station _model;
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
        public StationDataService StationDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

		public int Index
		{
			get { return _model.Index; }
			set { _model.Index = value; OnPropertyChanged("Index"); }
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

        public StationMachinesVM MachinesVM { get; set; }
  #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public StationVM(AccessType access, StationDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public StationVM(Station entity, AccessType access, StationDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        private void InitializeData(StationDataService dataService)
        {
            StationDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            StationDataService.AttachModel(_model);
            _model = StationDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    MachinesVM = new StationMachinesVM(this, Access);
                    CurrentLink = MachinesVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Station CreateNew(StationDataService dataService)
        {
            int id = dataService.AddModel(new Station { Name = "جدید", Code = string.Empty, Index = dataService.GetNextIndex(), CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}