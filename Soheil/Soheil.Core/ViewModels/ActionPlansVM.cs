using System.Collections.Generic;
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
    public class ActionPlansVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<ActionPlanVM>();
            foreach (var model in ActionPlanDataService.GetAll())
            {
                viewModels.Add(new ActionPlanVM(model, Access, ActionPlanDataService));
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
        public ActionPlanDataService ActionPlanDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPlansVM"/> class.
        /// </summary>
        public ActionPlansVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            ActionPlanDataService = new ActionPlanDataService(UnitOfWork);
            ActionPlanDataService.ActionPlanAdded += OnActionPlanAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2),
                new ColumnInfo("Mode",3,true) 
            };
            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is ActionPlanVM)
            {
                ActionPlanVM.CreateNew(ActionPlanDataService);
            }
        }

        private void OnActionPlanAdded(object sender, ModelAddedEventArgs<ActionPlan> e)
        {
            var newActionPlanVm = new ActionPlanVM(e.NewModel, Access, ActionPlanDataService);
            Items.AddNewItem(newActionPlanVm);
            Items.CommitNew();
            CurrentContent = newActionPlanVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}