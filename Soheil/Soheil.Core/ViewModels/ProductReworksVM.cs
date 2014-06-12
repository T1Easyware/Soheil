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
    public class ProductReworksVM : ItemLinkViewModel
    {

        public ProductReworksVM(ProductVM product, AccessType access)
            : base(access)
        {
            CurrentProduct = product;
            ProductDataService = new ProductDataService();
            ProductDataService.ReworkAdded += OnReworkAdded;
            ProductDataService.ReworkRemoved += OnReworkRemoved;
            ReworkDataService = new ReworkDataService();
            ProductReworkDataService = new ProductReworkDataService();

            var selectedVms = new ObservableCollection<ProductReworkVM>();
            foreach (var productRework in ProductDataService.GetReworks(product.Id))
            {
                selectedVms.Add(new ProductReworkVM(productRework, Access,ProductReworkDataService, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<ReworkVM>();
            foreach (var rework in ReworkDataService.GetActives(SoheilEntityType.Products, CurrentProduct.Id))
            {
                allVms.Add(new ReworkVM(rework, Access, ReworkDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public ProductVM CurrentProduct { get; set; }

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
        public ReworkDataService ReworkDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductReworkDataService ProductReworkDataService { get; set; }

        private void OnReworkRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ProductReworkVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = ReworkDataService.GetSingle(item.ReworkId);
                    var returnedVm = new ReworkVM(model, Access, ReworkDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnReworkAdded(object sender, ModelAddedEventArgs<ProductRework> e)
        {
            var productReworkVM = new ProductReworkVM(e.NewModel, Access, ProductReworkDataService, RelationDirection.Straight);
            SelectedItems.AddNewItem(productReworkVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == productReworkVM.ReworkId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(ReworkDataService.GetActives());
        }

        public override void Include(object param)
        {
            ProductDataService.AddRework(CurrentProduct.Id, ((IEntityItem) param).Id, string.Empty, string.Empty, 0);
        }

        public override void Exclude(object param)
        {
            ProductDataService.RemoveRework(CurrentProduct.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    ProductDataService.AddRework(CurrentProduct.Id, ((IEntityItem)item).Id,string.Empty,string.Empty,0);
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
                    ProductDataService.RemoveRework(CurrentProduct.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}