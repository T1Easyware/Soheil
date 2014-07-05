using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
    public class WorkProfilePlanVm : ItemContentViewModel
    {
        #region Properties

        private WorkProfilePlan _model;

        public override int Id
        {
            get { return _model.Id; }
            set { }
        }
        public override string SearchItem { get { return Name + (SelectedProfile == null ? "" : SelectedProfile.Name); } set { } }

        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        public DateTime StartDate
        {
            get { return _model.StartDate; }
            set { _model.StartDate = value; OnPropertyChanged("StartDate"); }
        }
        
        public DateTime EndDate
        {
            get { return _model.EndDate; }
            set { _model.EndDate = value; OnPropertyChanged("EndDate"); }
        }

        public WorkProfileMergingStrategy MergingStrategy
        {
            get { return _model.MergingStrategy; }
            set { _model.MergingStrategy = value; OnPropertyChanged("MergingStrategy"); }
        }
        
        #region Create/Modify Properties
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
        #endregion

        #region WorkProfile
        private WorkProfileInfo _selectedProfile;
        public WorkProfileInfo SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                WorkProfilePlanDataService.SetWorkProfile(_model, value.Id);
                OnPropertyChanged("SelectedProfile");
            }
        }

        //Profiles Observable Collection
		public ObservableCollection<WorkProfileInfo> Profiles { get { return _profiles; } }
		private ObservableCollection<WorkProfileInfo> _profiles = new ObservableCollection<WorkProfileInfo>();

        #endregion

        public WorkProfilePlanDataService WorkProfilePlanDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkProfilePlanVm"/> class initialized with default values.
        /// </summary>
        public WorkProfilePlanVm(AccessType access, WorkProfilePlanDataService dataService)
            : base(access)
        {
            _model = dataService.CreateDefault();
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkProfilePlanVm"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public WorkProfilePlanVm(WorkProfilePlan entity, AccessType access, WorkProfilePlanDataService dataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService);
        }

        private void InitializeData(WorkProfilePlanDataService dataService)
        {
            WorkProfilePlanDataService = dataService;
            SaveCommand = new Command(Save, CanSave);

            //add all profiles
            foreach (var profile in new WorkProfileDataService().GetAll())
            {
                Profiles.Add(new WorkProfileInfo(profile));
            }
            if (_model != null)
                if (_model.WorkProfile != null)
                    SelectedProfile = Profiles.FirstOrDefault(x => x.Id == _model.WorkProfile.Id);
        }

        public override void Save(object param)
        {
            WorkProfilePlanDataService.UpdateModel(_model);
            OnPropertyChanged("ModifiedBy");
            OnPropertyChanged("ModifiedDate");
            Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return (Access >= AccessType.Update) && AllDataValid() && base.CanSave();
        }
        #endregion

        #region Static Methods
        public static WorkProfilePlan CreateNew(WorkProfilePlanDataService dataService)
        {
            int id = dataService.AddModel(dataService.CreateDefault());
            return dataService.GetSingle(id);
        }
        #endregion

    }
}
