using System.ComponentModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UnitSetVM : ItemContentViewModel
    {
        #region Properties

        private UnitSet _model;

        public override int Id
        {
            get { return _model.Id; }
            set { }

        }

        public override string SearchItem { get { return Code + Description; } set { } }

        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
        // ReSharper restore PropertyNotResolved
        public string Description
        {
            get { return _model.Description; }
            set { _model.Description = value; OnPropertyChanged("Description"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UnitSetDataService UnitSetDataService { get; set; }

        /// <summary>
        /// Gets or sets the UnitSet group data service.
        /// </summary>
        /// <value>
        /// The UnitSet group data service.
        /// </value>
        public UnitGroupDataService GroupDataService { get; set; }

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
            get { return (Status)_model.Status; }
            set { _model.Status = (byte)value; }
        }

        [ReadOnly(true)]
        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }

        }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupVms">The group view models.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public UnitSetVM(UnitSet entity, ListCollectionView groupVms, AccessType access, UnitSetDataService dataService, UnitGroupDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = groupVms;
            foreach (UnitGroupVM groupVm in Groups)
            {
                if (groupVm.Id == entity.UnitGroup.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public UnitSetVM(UnitSet entity, AccessType access, UnitSetDataService dataService, UnitGroupDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
        }

        private void InitializeData(UnitSetDataService dataService, UnitGroupDataService groupDataService)
        {
            UnitSetDataService = dataService;
            GroupDataService = groupDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            UnitSetDataService.AttachModel(_model, SelectedGroupVM.Id);
            _model = UnitSetDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy"); OnPropertyChanged("ModifiedDate"); Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; UnitSetDataService.AttachModel(_model, SelectedGroupVM.Id);
        }

        public override void ViewItemLink(object param)
        {
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static UnitSet CreateNew(UnitSetDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new UnitSet { Code = string.Empty, Description = "جدید", Status = (byte)Status.Active }, groupId);
            return dataService.GetSingle(id);
        }
        #endregion
    }
}