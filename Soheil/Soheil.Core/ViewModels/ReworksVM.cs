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
    public class ReworksVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<ReworkVM>();
            foreach (var model in ReworkDataService.GetAll())
            {
                viewModels.Add(new ReworkVM(model, Access, ReworkDataService));
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
        public ReworkDataService ReworkDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ReworksVM"/> class.
        /// </summary>
        public ReworksVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            ReworkDataService = new ReworkDataService();
            ReworkDataService.ReworkAdded += OnReworkAdded;

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
            if (CurrentContent == null || CurrentContent is ReworkVM)
            {
                ReworkVM.CreateNew(ReworkDataService);
            }
        }

        private void OnReworkAdded(object sender, ModelAddedEventArgs<Rework> e)
        {
            var newReworkVm = new ReworkVM(e.NewModel, Access, ReworkDataService);
            Items.AddNewItem(newReworkVm);
            Items.CommitNew();
            CurrentContent = newReworkVm;
            CurrentContent.IsSelected = true;
        }

        #endregion
   }
}