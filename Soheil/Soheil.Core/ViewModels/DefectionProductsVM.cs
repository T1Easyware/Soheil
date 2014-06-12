using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class DefectionProductsVM : ItemLinkViewModel
    {

        public DefectionProductsVM(DefectionVM defection, AccessType access)
            : base(access)
        {
            CurrentDefection = defection;
            DefectionDataService = new DefectionDataService();
            DefectionDataService.ProductAdded += OnProductAdded;
            DefectionDataService.ProductRemoved += OnProductRemoved;
            ProductDataService = new ProductDataService();
            ProductGroupDataService = new ProductGroupDataService();
            ProductDefectionDataService = new ProductDefectionDataService();

            var selectedVms = new ObservableCollection<ProductDefectionVM>();
            foreach (var productDefection in DefectionDataService.GetProducts(defection.Id))
            {
                selectedVms.Add(new ProductDefectionVM(productDefection, Access, ProductDefectionDataService, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<ProductVM>();
            foreach (var product in ProductDataService.GetActives(SoheilEntityType.Defections, CurrentDefection.Id))
            {
                allVms.Add(new ProductVM(product, Access, ProductDataService, ProductGroupDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public DefectionVM CurrentDefection { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public DefectionDataService DefectionDataService { get; set; }

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

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDefectionDataService ProductDefectionDataService { get; set; }

        private void OnProductRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ProductDefectionVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = ProductDataService.GetSingle(item.ProductId);
                    var returnedVm = new ProductVM(model, Access, ProductDataService, ProductGroupDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnProductAdded(object sender, ModelAddedEventArgs<ProductDefection> e)
        {
            var productDefectionVM = new ProductDefectionVM(e.NewModel, Access, ProductDefectionDataService, RelationDirection.Reverse);
            SelectedItems.AddNewItem(productDefectionVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == productDefectionVM.ProductId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(ProductDataService.GetActives());
        }

        public override void Include(object param)
        {
            DefectionDataService.AddProduct(CurrentDefection.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            DefectionDataService.RemoveProduct(CurrentDefection.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    DefectionDataService.AddProduct(CurrentDefection.Id, ((IEntityItem)item).Id);
                }
            }
        }

        public override void ExcludeRange(object param)
        {
            var tempList = new List<ISplitDetail>();
            tempList.AddRange(SelectedItems.Cast<ISplitDetail>());
            foreach (ISplitDetail item in tempList)
            {
                if (item.IsChecked)
                {
                    DefectionDataService.RemoveProduct(CurrentDefection.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}