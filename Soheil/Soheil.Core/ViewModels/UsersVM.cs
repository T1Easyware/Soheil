using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UsersVM : GridSplitViewModel
    {
        #region Properties

        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<UserVM>();
            foreach (var model in UserDataService.GetAll())
            {
                viewModels.Add(new UserVM(model, Access, UserDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UserDataService UserDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersVM"/> class.
        /// </summary>
        public UsersVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UserDataService = new UserDataService();
            UserDataService.UserAdded += OnUserAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code"), 
                new ColumnInfo("Name"), 
                new ColumnInfo("Username"), 
                new ColumnInfo("Status") ,
                new ColumnInfo("Mode",true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add ,CanAddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is UserVM)
            {
                UserVM.CreateNew(UserDataService);
            }
        }
        public override void View(object content)
        {
            if (content is UserVM)
            {
                CurrentContent = content as UserVM;
            }
        }
        private void OnUserAdded(object sender, ModelAddedEventArgs<User> e)
        {
            var newUserVm = new UserVM(e.NewModel, Access, UserDataService);
            Items.AddNewItem(newUserVm);
            Items.CommitNew();
            CurrentContent = newUserVm;
            CurrentContent.IsSelected = true;
        }

        #endregion
    }
}