using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class ReworkProductsVM : ItemLinkViewModel
    {
        public ReworkProductsVM(ReworkVM rework, AccessType access)
            : base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentRework = rework;
            ReworkDataService = new ReworkDataService(UnitOfWork);
            ReworkDataService.ProductAdded += OnProductAdded;
            ReworkDataService.ProductRemoved += OnProductRemoved;
            ProductDataService = new ProductDataService(UnitOfWork);
            ProductReworkDataService = new ProductReworkDataService(UnitOfWork);
            ProductGroupDataService = new ProductGroupDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<ProductReworkVM>();
            foreach (var reworkProduct in ReworkDataService.GetProducts(rework.Id))
            {
                selectedVms.Add(new ProductReworkVM(reworkProduct, Access,ProductReworkDataService, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<ProductVM>();
            foreach (var product in ProductDataService.GetActives(SoheilEntityType.Reworks, CurrentRework.Id))
            {
                allVms.Add(new ProductVM(product, Access, ProductDataService, ProductGroupDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public ReworkVM CurrentRework { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ReworkDataService ReworkDataService { get; set; }

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
        public ProductReworkDataService ProductReworkDataService { get; set; }

        private void OnProductRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ProductReworkVM item in SelectedItems)
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

        private void OnProductAdded(object sender, ModelAddedEventArgs<ProductRework> e)
        {
            var productReworkVM = new ProductReworkVM(e.NewModel, Access, ProductReworkDataService, RelationDirection.Reverse);
            SelectedItems.AddNewItem(productReworkVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == productReworkVM.ProductId)
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
            ReworkDataService.AddProduct(CurrentRework.Id, ((IEntityItem)param).Id, string.Empty, string.Empty, 0);
        }

        public override void Exclude(object param)
        {
            ReworkDataService.RemoveProduct(CurrentRework.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    ReworkDataService.AddProduct(CurrentRework.Id, ((IEntityItem)item).Id,string.Empty,string.Empty,0);
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
                    ReworkDataService.RemoveProduct(CurrentRework.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}