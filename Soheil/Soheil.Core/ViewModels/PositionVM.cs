using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PositionVM : ItemContentViewModel
    {
        #region Properties
        private Position _model;

        public override int Id
        {
            get { return _model.Id; }
            set { throw new NotImplementedException(); }
        }

        public override string SearchItem { get { return Name; } set { } }

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

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PositionDataService PositionDataService { get; set; }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }

        public PositionUsersVM UsersVM { get; set; }
        public PositionAccessRulesVM AccessRulesVM { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public PositionVM(AccessType access, PositionDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public PositionVM(Position entity, AccessType access, PositionDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;

        }

        private void InitializeData(PositionDataService dataService)
        {
            PositionDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            PositionDataService.AttachModel(_model);
            _model = PositionDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
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
                    UsersVM = new PositionUsersVM(this, Access);
                    CurrentLink = UsersVM;
                    break;
                case 1:
                    AccessRulesVM = new PositionAccessRulesVM(this, Access);
                    CurrentLink = AccessRulesVM;
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Position CreateNew(PositionDataService dataService)
        {
            int id = dataService.AddModel(new Position { Name = "جدید", ModifiedDate = DateTime.Now, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}