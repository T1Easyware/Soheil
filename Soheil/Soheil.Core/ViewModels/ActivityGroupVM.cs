using System;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActivityGroupVM : ItemContentViewModel
    {
        #region Properties

        private ActivityGroup _model;

        private ObservableCollection<Activity> _activitys;

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


// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
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

        public ObservableCollection<Activity> Activitys
        {
            get { return _activitys; }
            set
            {
                _activitys = value;
                OnPropertyChanged("Activitys");
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivityGroupDataService ActivityGroupDataService { get; set; }



        #endregion

        #region Method

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public ActivityGroupVM(ActivityGroup entity, AccessType access, ActivityGroupDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;

            Activitys = new ObservableCollection<Activity>();
            foreach (Activity activity in entity.Activities)
            {
                Activitys.Add(activity);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityGroupVM"/> class initialized with default values.
        /// </summary>
        public ActivityGroupVM(AccessType access, ActivityGroupDataService dataService):base(access)
        {
            InitializeData(dataService);
            Activitys = new ObservableCollection<Activity>();
        }

        private void InitializeData(ActivityGroupDataService dataService)
        {
            ActivityGroupDataService = dataService;
            SaveCommand = new Command(Save,CanSave);
        }

        public override void Save(object param)
        {
            ActivityGroupDataService.AttachModel(_model);
            _model = ActivityGroupDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
        public static ActivityGroup CreateNew(ActivityGroupDataService dataService)
        {
            int id = dataService.AddModel(new ActivityGroup { Name = "جدید", Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}