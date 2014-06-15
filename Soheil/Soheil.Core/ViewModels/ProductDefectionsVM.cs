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
    public class ProductDefectionsVM : ItemLinkViewModel
    {
        public ProductDefectionsVM(ProductVM product, AccessType access):base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentProduct = product;
            ProductDataService = new ProductDataService(UnitOfWork);
            ProductDataService.DefectionAdded += OnDefectionAdded;
            ProductDataService.DefectionRemoved += OnDefectionRemoved;
            DefectionDataService = new DefectionDataService(UnitOfWork);
            ProductDefectionDataService = new ProductDefectionDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<ProductDefectionVM>();
            foreach (var productDefection in ProductDataService.GetDefections(product.Id))
            {
                selectedVms.Add(new ProductDefectionVM(productDefection, Access, ProductDefectionDataService, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<DefectionVM>();
            foreach (var defection in DefectionDataService.GetActives(SoheilEntityType.Products, CurrentProduct.Id))
            {
                allVms.Add(new DefectionVM(defection, Access, DefectionDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
            IncludeRangeCommand = new Command(IncludeRange, CanIncludeRange);
            ExcludeRangeCommand = new Command(ExcludeRange, CanExcludeRange);
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
        public DefectionDataService DefectionDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDefectionDataService ProductDefectionDataService { get; set; }



        private void OnDefectionRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ProductDefectionVM  item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = DefectionDataService.GetSingle(item.DefectionId);
                    var returnedVm = new DefectionVM(model, Access, DefectionDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnDefectionAdded(object sender, ModelAddedEventArgs<ProductDefection> e)
        {
            var productDefectionVM = new ProductDefectionVM(e.NewModel, Access, ProductDefectionDataService, RelationDirection.Straight);
            SelectedItems.AddNewItem(productDefectionVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == productDefectionVM.DefectionId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(DefectionDataService.GetActives());
        }

        public override void Include(object param)
        {
            ProductDataService.AddDefection(CurrentProduct.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            ProductDataService.RemoveDefection(CurrentProduct.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    ProductDataService.AddDefection(CurrentProduct.Id, ((IEntityItem)item).Id);
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
                    ProductDataService.RemoveDefection(CurrentProduct.Id, ((IEntityItem)item).Id);
                }
            }
        }

    }
}