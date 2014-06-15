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
    public class RootsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<RootVM>();
            foreach (var model in RootDataService.GetAll())
            {
                viewModels.Add(new RootVM(model, Access, RootDataService));
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
        public RootDataService RootDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RootsVM"/> class.
        /// </summary>
        public RootsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            RootDataService = new RootDataService(UnitOfWork);
            RootDataService.RootAdded += OnRootAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is RootVM)
            {
                RootVM.CreateNew(RootDataService);
            }
        }

        private void OnRootAdded(object sender, ModelAddedEventArgs<Root> e)
        {
            var newRootVm = new RootVM(e.NewModel, Access, RootDataService);
            Items.AddNewItem(newRootVm);
            Items.CommitNew();
            CurrentContent = newRootVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}