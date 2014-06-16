using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class AccessRulesVM : TreeSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            RootNode = new AccessRuleVM(Access, AccessRuleDataService) { Title = string.Empty, Id = -1, ParentId = -2 };

            var viewModels = new ObservableCollection<AccessRuleVM>();
            foreach (var model in AccessRuleDataService.GetAll())
            {
                viewModels.Add(new AccessRuleVM(AccessRuleDataService, model, Access));
            }

            foreach (AccessRuleVM item in viewModels)
            {
                if (item.ParentId == RootNode.Id)
                {
                    RootNode.ChildNodes.Add(item);
                    break;
                }
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
        public AccessRuleDataService AccessRuleDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRulesVM"/> class.
        /// </summary>
        public AccessRulesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            AccessRuleDataService = new AccessRuleDataService(UnitOfWork);
            AccessRuleDataService.AccessRuleAdded += OnAccessRuleAdded;

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            ViewCommand = new Command(View, CanView);

            //var viewModels = AccessRuleDataService.GetAll();
            CreateItems(null);

            //CurrentNode = (ISplitNodeContent) CurrentContent;
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is AccessRuleVM)
            {
                AccessRuleVM.CreateNew(AccessRuleDataService);
            }
        }
        public override bool CanAdd()
        {
            return false;
        }

        public override void View(object content)
        {
            if (content is AccessRuleVM)
            {
                CurrentContent = content as AccessRuleVM;
            }
        }
        private void OnAccessRuleAdded(object sender, ModelAddedEventArgs<AccessRule> e)
        {
            var newAccessRuleVm = new AccessRuleVM(AccessRuleDataService, e.NewModel, Access);
            Items.AddNewItem(newAccessRuleVm);
            Items.CommitNew();

            CurrentContent = newAccessRuleVm;
            CurrentContent.IsSelected = true;
        }
        #endregion
    }
}