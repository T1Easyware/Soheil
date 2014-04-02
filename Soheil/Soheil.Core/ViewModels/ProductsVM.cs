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
    public class ProductsVM : GridSplitViewModel
    {
        #region Properties

        public override void CreateItems(object param)
        {
            var groupViewModels = new ObservableCollection<ProductGroupVM>();
            foreach (var productGroup in ProductGroupDataService.GetAll())
            {
                groupViewModels.Add(new ProductGroupVM(productGroup, Access, ProductGroupDataService));
            }
            GroupItems = new ListCollectionView(groupViewModels);

            var viewModels = new ObservableCollection<ProductVM>();
            foreach (var model in ProductDataService.GetAll())
            {
                viewModels.Add(new ProductVM(model, GroupItems, Access,ProductDataService, ProductGroupDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent) Items.CurrentItem;
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
        public ProductGroupDataService ProductGroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDataService ProductDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesVM"/> class.
        /// </summary>
        public ProductsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            ProductGroupDataService = new ProductGroupDataService();
            ProductDataService = new ProductDataService();
            ProductDataService.ProductAdded += OnProductAdded;
            ProductGroupDataService.ProductGroupAdded += OnProductGroupAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Color",3), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",4,true) 
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
                ProductVM.CreateNew(ProductDataService,(int)parameter);
            }
            else if (CurrentContent is ProductGroupVM)
            {
                ProductVM.CreateNew(ProductDataService,((ProductGroupVM) CurrentContent).Id);
            }
            else if (CurrentContent is ProductVM)
            {
                ProductVM.CreateNew(ProductDataService,((ProductVM) CurrentContent).SelectedGroupVM.Id);
            }
        }

        public override void AddGroup(object param)
        {
            ProductGroupVM.CreateNew(ProductGroupDataService);
        }

        public override void View(object content)
        {
            if (content is ProductGroupVM)
            {
                CurrentContent = content as ProductGroupVM;
            }
            else if (content is ProductVM)
            {
                CurrentContent = content as ProductVM;
            }
        }

        private void OnProductAdded(object sender, ModelAddedEventArgs<Product> e)
        {
            var newContent = new ProductVM(e.NewModel, GroupItems, Access, ProductDataService, ProductGroupDataService);
            Items.AddNewItem(newContent);
            Items.CommitNew();

            CurrentContent = newContent;
            CurrentContent.IsSelected = true;
        }

        private void OnProductGroupAdded(object sender, ModelAddedEventArgs<ProductGroup> e)
        {
            var newProductGroup = new ProductGroupVM(e.NewModel, Access, ProductGroupDataService);
            GroupItems.AddNewItem(newProductGroup);
            GroupItems.CommitNew();

            foreach (ProductVM item in Items)
            {
                if (!item.Groups.Contains(newProductGroup))
                {
                    item.Groups.AddNewItem(newProductGroup);
                    item.Groups.CommitNew();
                }
            }
            Add(newProductGroup.Id);
            ((ProductVM) CurrentContent).SelectedGroupVM = newProductGroup;

            CurrentContent = newProductGroup;
        }

        #endregion

    }
}