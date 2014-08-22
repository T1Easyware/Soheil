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
    public class UnitSetsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<UnitGroupVM>();
            foreach (var unitGroup in UnitGroupDataService.GetAll())
            {
                groupViewModels.Add(new UnitGroupVM(unitGroup, Access, UnitGroupDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<UnitSetVM>();
            foreach (var model in UnitSetDataService.GetAll())
            {
                viewModels.Add(new UnitSetVM(model, GroupItems, Access, UnitSetDataService, UnitGroupDataService));
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
        public UnitGroupDataService UnitGroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UnitSetDataService UnitSetDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetsVM"/> class.
        /// </summary>
        public UnitSetsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            UnitGroupDataService = new UnitGroupDataService(UnitOfWork);
            UnitSetDataService = new UnitSetDataService(UnitOfWork);
            UnitSetDataService.UnitSetAdded += OnUnitSetAdded;
            UnitGroupDataService.UnitGroupAdded += OnUnitGroupAdded;
            
            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Description",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
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
                UnitSetVM.CreateNew(UnitSetDataService,(int)parameter);
            }
            else if (CurrentContent is UnitGroupVM)
            {
                UnitSetVM.CreateNew(UnitSetDataService, ((UnitGroupVM)CurrentContent).Id);
            }
            else if (CurrentContent is UnitSetVM)
            {
                UnitSetVM.CreateNew(UnitSetDataService, ((UnitSetVM)CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            UnitGroupVM.CreateNew(UnitGroupDataService);
        }

        public override void View(object content)
        {
            if (content is UnitGroupVM)
            {
                var groupVm = content as UnitGroupVM;
                CurrentContent = groupVm;
            }
            else if (content is UnitSetVM)
            {
                var unitSetVm = content as UnitSetVM;
                CurrentContent = unitSetVm;
            }
        }

        private void OnUnitSetAdded(object sender, ModelAddedEventArgs<UnitSet> e)
        {
            var newContent = new UnitSetVM(e.NewModel, GroupItems, Access, UnitSetDataService, UnitGroupDataService);
            Items.AddNewItem(newContent);
            Items.CommitNew();

            CurrentContent = newContent;
            CurrentContent.IsSelected = true;
        }

        private void OnUnitGroupAdded(object sender, ModelAddedEventArgs<UnitGroup> e)
        {
            var newUnitGroup = new UnitGroupVM(e.NewModel, Access,UnitGroupDataService);
            GroupItems.AddNewItem(newUnitGroup);
            GroupItems.CommitNew();

            foreach (UnitSetVM item in Items)
            {
                if (!item.Groups.Contains(newUnitGroup))
                {
                    item.Groups.AddNewItem(newUnitGroup);
                    item.Groups.CommitNew();
                }
            }
            Add(newUnitGroup.Id);
            ((UnitSetVM) CurrentContent).SelectedGroupVM = newUnitGroup;

            CurrentContent =  newUnitGroup;
        }

        #endregion


    }
}