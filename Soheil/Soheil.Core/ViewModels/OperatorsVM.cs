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
    public class OperatorsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<OperatorVM>();
            foreach (var model in OperatorDataService.GetAll())
            {
                viewModels.Add(new OperatorVM(model, Access, OperatorDataService));
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
        public OperatorDataService OperatorDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorsVM"/> class.
        /// </summary>
        public OperatorsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            OperatorDataService = new OperatorDataService();
            OperatorDataService.OperatorAdded += OnOperatorAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is OperatorVM)
            {
                OperatorVM.CreateNew(OperatorDataService);
            }
        }
        public override void View(object content)
        {
            if (content is OperatorVM)
            {
                var operatorVm = content as OperatorVM;
                CurrentContent = operatorVm;
            }
        }
        private void OnOperatorAdded(object sender, ModelAddedEventArgs<Operator> e)
        {
            var newOperatorVm = new OperatorVM(e.NewModel, Access, OperatorDataService);
            Items.AddNewItem(newOperatorVm);
            Items.CommitNew();
            CurrentContent = newOperatorVm;
            CurrentContent.IsSelected = true;
        }

        #endregion


    }
}