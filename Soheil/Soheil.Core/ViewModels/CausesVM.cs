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
    public class CausesVM : TreeSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            RootNode = new CauseVM(Access, CauseDataService) { Title = string.Empty, Id = -1, ParentId = -2 };

            var viewModels = new ObservableCollection<CauseVM>();
            foreach (var model in CauseDataService.GetAll())
            {
                viewModels.Add(new CauseVM(model, Access, CauseDataService));
            }

            foreach (CauseVM item in viewModels)
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
                CurrentContent = (ISplitNodeContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public CauseDataService CauseDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CausesVM"/> class.
        /// </summary>
        public CausesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            CauseDataService = new CauseDataService();
            CauseDataService.CauseAdded += OnCauseAdded;

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            ViewCommand = new Command(View, CanView);

            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent is CauseVM)
            {
                var currentCause = (CauseVM)CurrentContent;
                if (currentCause.Level == CauseLevel.Level3)
                {
                    return;
                }
                CauseVM.CreateNew(CauseDataService, ((ISplitNodeContent)CurrentContent).Id, currentCause.Level + 1);
            }
        }
        public override void View(object content)
        {
            if (content is CauseVM)
            {
                CurrentContent = content as CauseVM;
            }
        }
        private void OnCauseAdded(object sender, ModelAddedEventArgs<Cause> e)
        {
            var newCauseVm = new CauseVM(e.NewModel, Access, CauseDataService);
            Items.AddNewItem(newCauseVm);
            Items.CommitNew();

            ((ISplitNodeContent)CurrentContent).ChildNodes.Add(newCauseVm);
            CurrentContent = newCauseVm;
            CurrentContent.IsSelected = true;

        }
        #endregion
    }
}