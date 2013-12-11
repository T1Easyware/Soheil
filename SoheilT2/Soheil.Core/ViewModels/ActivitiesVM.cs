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
    public class ActivitiesVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<ActivityGroupVM>();
            foreach (var activityGroup in ActivityGroupDataService.GetAll())
            {
                groupViewModels.Add(new ActivityGroupVM(activityGroup, Access, ActivityGroupDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<ActivityVM>();
            foreach (var model in ActivityDataService.GetAll())
            {
                viewModels.Add(new ActivityVM(model, GroupItems, Access, ActivityDataService, ActivityGroupDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
            if (Items.GroupDescriptions != null)
                Items.GroupDescriptions.Add(new PropertyGroupDescription("SelectedGroupVM.Id"));
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivityGroupDataService ActivityGroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivityDataService ActivityDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesVM"/> class.
        /// </summary>
        public ActivitiesVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            ActivityGroupDataService = new ActivityGroupDataService();
            ActivityDataService = new ActivityDataService();
            ActivityDataService.ActivityAdded += OnActivityAdded;
            ActivityGroupDataService.ActivityGroupAdded += OnActivityGroupAdded;
            
            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code"), 
                new ColumnInfo("Name"), 
                new ColumnInfo("Status") ,
                new ColumnInfo("Mode",true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(AddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (parameter is int)
            {
                ActivityVM.CreateNew(ActivityDataService,(int)parameter);
            }
            else if (CurrentContent is ActivityGroupVM)
            {
                ActivityVM.CreateNew(ActivityDataService, ((ActivityGroupVM)CurrentContent).Id);
            }
            else if (CurrentContent is ActivityVM)
            {
                ActivityVM.CreateNew(ActivityDataService, ((ActivityVM)CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            ActivityGroupVM.CreateNew(ActivityGroupDataService);
        }

        public override void View(object content)
        {
            if (content is ActivityGroupVM)
            {
                var groupVm = content as ActivityGroupVM;
                CurrentContent = groupVm;
            }
            else if (content is ActivityVM)
            {
                var activityVm = content as ActivityVM;
                CurrentContent = activityVm;
            }
        }

        private void OnActivityAdded(object sender, ModelAddedEventArgs<Activity> e)
        {
            var newContent = new ActivityVM(e.NewModel, GroupItems, Access, ActivityDataService, ActivityGroupDataService);
            Items.AddNewItem(newContent);
            Items.CommitNew();

            CurrentContent = newContent;
            CurrentContent.IsSelected = true;
        }

        private void OnActivityGroupAdded(object sender, ModelAddedEventArgs<ActivityGroup> e)
        {
            var newActivityGroup = new ActivityGroupVM(e.NewModel, Access,ActivityGroupDataService);
            GroupItems.AddNewItem(newActivityGroup);
            GroupItems.CommitNew();

            foreach (ActivityVM item in Items)
            {
                if (!item.Groups.Contains(newActivityGroup))
                {
                    item.Groups.AddNewItem(newActivityGroup);
                    item.Groups.CommitNew();
                }
            }
            Add(newActivityGroup.Id);
            ((ActivityVM) CurrentContent).SelectedGroupVM = newActivityGroup;

            CurrentContent =  newActivityGroup;
        }

        #endregion


    }
}