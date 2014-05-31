using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    /// <summary>
    /// Represents user view model
    /// </summary>
    public class UserVM : ItemContentViewModel
    {
        #region Properties

        private User _model;
        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem { get { return Code + Name + Username; } set { } }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Name
        {
            get { return _model.Title; }
            set { _model.Title = value; OnPropertyChanged("Title"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UserDataService UserDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public int Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }
// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Username
        {
            get { return _model.Username; }
            set { _model.Username = value; OnPropertyChanged("Username"); }
        }
// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtPasswordRequired")]
// ReSharper restore PropertyNotResolved
        public string Password
        {
            get { return _model.Password; }
            set { _model.Password = value; OnPropertyChanged("Password"); }
        }

        private string _confirmPassword;
// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtPasswordRequired")]
// ReSharper restore PropertyNotResolved
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; OnPropertyChanged("Status"); }
        }

        public bool BypassPositionAccess
        {
            get { return _model.BypassPositionAccess ?? false; }
            set { _model.BypassPositionAccess = value; OnPropertyChanged("BypassPositionAccess"); }
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

        public int CreatedBy
        {
            get { return _model.CreatedBy; }
            set { _model.CreatedBy = value; OnPropertyChanged("CreatedBy"); }
        }
        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }

        public UserPositionsVM PositionsVM { get; set; }
        public UserAccessRulesVM AccessRulesVM { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public UserVM(AccessType access, UserDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public UserVM(User entity, AccessType access, UserDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        private void InitializeData(UserDataService dataService)
        {
            UserDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            if (UserDataService.IsCodeUnique(_model.Code))
            {
                return;
            }
            UserDataService.AttachModel(_model);
            _model = UserDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; UserDataService.AttachModel(_model);
        }
        public override bool CanSave()
        {
            return AllDataValid() && Password == ConfirmPassword;
        }
        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    PositionsVM = new UserPositionsVM(this, Access);
                    CurrentLink = PositionsVM;
                    break;
                case 1:
                    AccessRulesVM = new UserAccessRulesVM(this, Access);
                    CurrentLink = AccessRulesVM;
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static User CreateNew(UserDataService dataService)
        {
            int code = dataService.GenerateCode();
            int id = dataService.AddModel(new User { Code = code, Title = "جدید" ,CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Username = string.Empty, Password = string.Empty, Status = (byte)Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}