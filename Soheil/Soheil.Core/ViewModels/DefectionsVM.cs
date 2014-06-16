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
    public class DefectionsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<DefectionVM>();
            foreach (var model in DefectionDataService.GetAll())
            {
                viewModels.Add(new DefectionVM(model, Access,DefectionDataService));
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
        public DefectionDataService DefectionDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectionsVM"/> class.
        /// </summary>
        public DefectionsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            DefectionDataService = new DefectionDataService(UnitOfWork);
            DefectionDataService.DefectionAdded += OnDefectionAdded;

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
            if (CurrentContent == null || CurrentContent is DefectionVM)
            {
                DefectionVM.CreateNew(DefectionDataService);
            }
        }

        private void OnDefectionAdded(object sender, ModelAddedEventArgs<Defection> e)
        {
            var newDefectionVm = new DefectionVM(e.NewModel, Access, DefectionDataService);
            Items.AddNewItem(newDefectionVm);
            Items.CommitNew();
            CurrentContent = newDefectionVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}