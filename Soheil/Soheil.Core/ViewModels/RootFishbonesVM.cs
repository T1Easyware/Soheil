using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.InfoViewModels;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class RootFishbonesVM : NodeLinkViewModel
    {
        public RootFishbonesVM(RootVM root, ProductDefectionVM productDefection, AccessType access) : base(access)
        {
            CurrentRoot = root;
            RootDataService = new RootDataService();
            RootDataService.FishboneAdded += OnFishboneNodeAdded;
            RootDataService.FishboneRemoved += OnFishboneNodeRemoved;
            ProductDefectionDataService = new ProductDefectionDataService();
            ProductDataService = new ProductDataService();
            DefectionDataService = new DefectionDataService();
            SwitchItemsCommand = new Command(SetProductDefections);
            ViewProductDefectionsCommand = new Command(ViewProductDefections);
            InitializeFishboneCommand = new Command(InitializeFishbone);
            FishboneNodeDataService = new FishboneNodeDataService();

            RootNode = new FishboneNodeVM(Access, FishboneNodeDataService) { Title = string.Empty, Id = -1, ParentId = -2 };

            var selectedVms = new ObservableCollection<FishboneNodeVM>();
            foreach (var rootFishboneNode in RootDataService.GetFishboneNodes(root.Id))
            {
                selectedVms.Add(new FishboneNodeVM(rootFishboneNode, Access, FishboneNodeDataService));
            }

            SetProductDefections();

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeTreeCommand = new Command(ExcludeTree, CanExcludeTree);


            foreach (FishboneNodeVM item in selectedVms)
            {
                if (item.ParentId == RootNode.Id)
                {
                    RootNode.ChildNodes.Add(item);
                    break;
                }
            }

            CurrentNode = RootNode;

            if (productDefection != null)
            {
                foreach (IInfoViewModel item in DefectionList)
                {
                    if (item.Id == productDefection.DefectionId)
                    {
                        CurrentDefection = item;
                        ViewProductDefections(null);
                        break;
                    }
                }
				if(AllItems!=null)
                foreach (ProductDefectionVM item in AllItems)
                {
                    if (item.Id == productDefection.Id)
                    {
                        CurrentProductDefection = item;
                        IsEnabled = false;
                        break;
                    }
                }
            }

        }

        public RootVM CurrentRoot { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public RootDataService RootDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public FishboneNodeDataService FishboneNodeDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDefectionDataService ProductDefectionDataService { get; set; }

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

        public static readonly DependencyProperty CurrentProductDefectionPoperty =
            DependencyProperty.Register("CurrentProductDefection", typeof(ProductDefectionVM), typeof(RootFishbonesVM), null);

        public ProductDefectionVM CurrentProductDefection
        {
            get { return (ProductDefectionVM)GetValue(CurrentProductDefectionPoperty); }
            set { SetValue(CurrentProductDefectionPoperty, value); SetValue(CurrentProductDefectionPoperty, value); }
        }

        public static readonly DependencyProperty CurrentDefectionPoperty =
            DependencyProperty.Register("CurrentDefection", typeof(IInfoViewModel), typeof(RootFishbonesVM), null);

        public IInfoViewModel CurrentDefection
        {
            get { return (IInfoViewModel)GetValue(CurrentDefectionPoperty); }
            set { SetValue(CurrentDefectionPoperty, value); SetValue(CurrentDefectionPoperty, value); }
        }

        public static readonly DependencyProperty DefectionListProperty =
            DependencyProperty.Register("DefectionList", typeof(ListCollectionView), typeof(RootFishbonesVM), null);

        public ListCollectionView DefectionList
        {
            get { return (ListCollectionView)GetValue(DefectionListProperty); }
            set { SetValue(DefectionListProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(RootFishbonesVM), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }


        private bool IsRoot
        {
            get { return CurrentNode == null || ((FishboneNodeVM)CurrentNode).NodeType != FishboneNodeType.None; }
        }
        public Visibility HorizontalNodeVisibility
        {
            get { return IsRoot ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility VerticalNodeVisibility
        {
            get { return IsRoot ? Visibility.Collapsed : Visibility.Visible; }
        }


        private string _descriptionToAdd;
        public string DescriptionToAdd
        {
            get { return _descriptionToAdd; }
            set { _descriptionToAdd = value; OnPropertyChanged("DescriptionToAdd"); }
        }

        private void OnFishboneNodeRemoved(object sender, ModelRemovedEventArgs e)
        {
            var removedNode = FindNode(RootNode, e.Id);
            int parentId = removedNode.ParentId;
            RemoveNode(RootNode.ChildNodes, removedNode.Id);
            CurrentNode = FindNode(RootNode, parentId);
        }

        private void OnFishboneNodeAdded(object sender, ModelAddedEventArgs<FishboneNode> e)
        {
            var fishboneRootVm = new FishboneNodeVM(e.NewModel, Access, FishboneNodeDataService);
            CurrentNode.ChildNodes.Add(fishboneRootVm);

            if (CurrentNode == RootNode)
            {
                CurrentNode = fishboneRootVm;
            }
        }

        public override void RefreshItems()
        {
            SetProductDefections();
        }

        public override void Exclude(object param)
        {
            throw new NotImplementedException();
        }

        public override void Include(object param)
        {
            CurrentNode = FindNode(RootNode, (int)param);
            if (CurrentNode.ParentId == RootNode.Id) return;
            var rootType = FindRootType((FishboneNodeVM) CurrentNode);
            RootDataService.AddFishboneNode(CurrentRoot.Id, rootType, CurrentNode.Id, DescriptionToAdd, FishboneNodeType.None);
        }

        public override void ExcludeTree(object fishboneRootVm)
        {
            CurrentNode = (IEntityNode)fishboneRootVm;
            var relationIdList = new List<Tuple<int, int>>();
            FindRelationIdList(CurrentNode, relationIdList);
            foreach (Tuple<int, int> tuple in relationIdList)
            {
                RootDataService.RemoveFishboneNode(tuple.Item1, tuple.Item2);
            }
            RootDataService.RemoveFishboneNode(CurrentRoot.Id, CurrentNode.Id);
        }

        public Command SwitchItemsCommand { get; set; }
        public void SetProductDefections(object param = null)
        {
            var defectionList = new ObservableCollection<DefectionInfoVM>();
            foreach (Defection defection in DefectionDataService.GetActives())
            {
                defectionList.Add(new DefectionInfoVM(defection));
            }
            DefectionList = new ListCollectionView(defectionList);
            CurrentDefection = DefectionList.CurrentItem as IInfoViewModel;
            if (CurrentDefection != null)
            {
                var productList = new ObservableCollection<ProductDefectionVM>();
                foreach (ProductDefection product in DefectionDataService.GetProducts(CurrentDefection.Id))
                {
                    productList.Add(new ProductDefectionVM(product, Access, ProductDefectionDataService, RelationDirection.Straight));
                }
                AllItems = new ListCollectionView(productList);
            }
        }

        public Command ViewProductDefectionsCommand { get; set; }
        public void ViewProductDefections(object param)
        {
            if (CurrentDefection == null) return;

            var productList = new ObservableCollection<ProductDefectionVM>();
            foreach (ProductDefection product in DefectionDataService.GetProducts(CurrentDefection.Id))
            {
                productList.Add(new ProductDefectionVM(product, Access, ProductDefectionDataService, RelationDirection.Straight));
            }
            AllItems = new ListCollectionView(productList);

        }

        public Command InitializeFishboneCommand { get; set; }
        public void InitializeFishbone(object param)
        {
            if(CurrentProductDefection == null) return;
            RootDataService.InitializeFishboneRoots(CurrentRoot.Id, CurrentProductDefection.Id);
            IsEnabled = false;
        }

        private void FindRelationIdList(IEntityNode parentNode, List<Tuple<int, int>> relationIdList)
        {
            foreach (IEntityNode node in parentNode.ChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                {
                    FindRelationIdList(node, relationIdList);
                }
                else
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentRoot.Id,node.Id));
                }
            }
        }
        private FishboneNodeType FindRootType(FishboneNodeVM node)
        {
            if (node.NodeType!= FishboneNodeType.None)
            {
                return node.NodeType;
            }
            while (node.Id != RootNode.Id)
            {
                node = (FishboneNodeVM) FindNode(RootNode, node.ParentId);
                if (node.NodeType != FishboneNodeType.None)
                {
                    return node.NodeType;
                }
            }
            return FishboneNodeType.None;
        }
    }
}