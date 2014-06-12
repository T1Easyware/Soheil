using System;
using System.Linq;
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
	public class FpcsVm : GridSplitViewModel
	{
        #region Properties
        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<ProductVM>();
            foreach (var product in ProductDataService.GetActives())
            {
                groupViewModels.Add(new ProductVM(product, Access, ProductDataService, ProductGroupDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<FpcVm>();
			foreach (var model in FpcDataService.GetAll().Where(x => x.RecordStatus != Status.Deleted && x.Product.RecordStatus == Status.Active))
			{
				viewModels.Add(new FpcVm(model, GroupItems, Access, FpcDataService));
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
        public FPCDataService FpcDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDataService ProductDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductGroupDataService ProductGroupDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DefectionsVM"/> class.
        /// </summary>
        public FpcsVm(AccessType access)
            : base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
			FpcDataService = new FPCDataService();
			ProductDataService = new ProductDataService();
			ProductGroupDataService = new ProductGroupDataService();
			FpcDataService.FpcAdded += OnFpcAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);
			RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, () => false);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (parameter is int)
            {
                FpcVm.CreateNew(FpcDataService, (int)parameter);
            }
            else if (CurrentContent is ProductVM)
            {
                FpcVm.CreateNew(FpcDataService, ((ProductVM)CurrentContent).Id);
            }
            else if (CurrentContent is FpcVm)
            {
                FpcVm.CreateNew(FpcDataService, ((FpcVm)CurrentContent).SelectedGroupVM.Id);
            }
        }

		private void OnFpcAdded(object sender, ModelAddedEventArgs<FPC> e)
        {
			var newFpcVm = new FpcVm(e.NewModel, GroupItems, Access, FpcDataService);
            Items.AddNewItem(newFpcVm);
            Items.CommitNew();

            CurrentContent = newFpcVm;
            CurrentContent.IsSelected = true;
        }

        public override ISplitItemContent CreateClone(ISplitItemContent original)
        {
            var viewModel = original as FpcVm;
			var clone = FpcDataService.CloneModelById(viewModel.Id);
			return new FpcVm(clone, Access, FpcDataService);
        }
        #endregion

	}
}
