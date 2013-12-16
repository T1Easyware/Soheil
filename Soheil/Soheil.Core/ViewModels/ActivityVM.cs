using System;
using System.ComponentModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActivityVM : ItemContentViewModel
    {
        #region Properties

        private Activity _model;

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
        public ActivityDataService ActivityDataService { get; set; }

        /// <summary>
        /// Gets or sets the activity group data service.
        /// </summary>
        /// <value>
        /// The activity group data service.
        /// </value>
        public ActivityGroupDataService GroupDataService { get; set; }

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


        public ActivityOperatorsVM OperatorsVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupVms">The group view models.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public ActivityVM(Activity entity, ListCollectionView groupVms, AccessType access, ActivityDataService dataService, ActivityGroupDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = groupVms;
            foreach (ActivityGroupVM groupVm in Groups)
            {
                if (groupVm.Id == entity.ActivityGroup.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public ActivityVM(Activity entity, AccessType access, ActivityDataService dataService, ActivityGroupDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
        }

        private void InitializeData(ActivityDataService dataService, ActivityGroupDataService groupDataService)
        {
            ActivityDataService = dataService;
            GroupDataService = groupDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            ActivityDataService.AttachModel(_model,SelectedGroupVM.Id);
            _model = ActivityDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
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
                    OperatorsVM = new ActivityOperatorsVM(this, Access);
                    CurrentLink = OperatorsVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Activity CreateNew(ActivityDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new Activity { Code = string.Empty, Name = "جدید",CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active}, groupId);
            return dataService.GetSingle(id);
        }
        #endregion
    }
}