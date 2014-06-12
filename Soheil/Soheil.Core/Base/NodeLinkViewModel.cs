using System.Collections;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using System.Collections.Generic;

namespace Soheil.Core.Base
{
    public abstract class NodeLinkViewModel : ViewModelBase, ISplitNodeCollectionLink
    {
        public static readonly DependencyProperty AllItemsProperty =
            DependencyProperty.Register("AllItems", typeof(ListCollectionView), typeof(NodeLinkViewModel), null);

        public static readonly DependencyProperty DetailsProperty =
            DependencyProperty.Register("Details", typeof(ISplitDetail), typeof(NodeLinkViewModel), null);

        public static readonly DependencyProperty RootNodeProperty =
            DependencyProperty.Register("RootNode", typeof(IEntityNode), typeof(NodeLinkViewModel), null);

        public static readonly DependencyProperty CurrentNodeProperty =
            DependencyProperty.Register("CurrentNode", typeof(IEntityNode), typeof(NodeLinkViewModel), null);

        public Command ExcludeCommand { get; set; }
        public Command IncludeCommand { get; set; }
        public Command ExcludeRangeCommand { get; set; }
        public Command IncludeRangeCommand { get; set; }

        public ListCollectionView AllItems
        {
            get { return (ListCollectionView)GetValue(AllItemsProperty); }
            set { SetValue(AllItemsProperty, value); }
        }

        public abstract void Exclude(object param);


        public bool CanExclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public virtual bool CanInclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public abstract void Include(object param);
        public void ExcludeRange(object param)
        {
            throw new System.NotImplementedException();
        }

        public bool CanExcludeRange()
        {
            throw new System.NotImplementedException();
        }

        public bool CanIncludeRange()
        {
            throw new System.NotImplementedException();
        }

        public void IncludeRange(object param)
        {
            throw new System.NotImplementedException();
        }

        public abstract void RefreshItems();

        public IEntityNode RootNode
        {
            get { return (IEntityNode)GetValue(RootNodeProperty); }
            set { SetValue(RootNodeProperty, value); }
        }
        public IEntityNode CurrentNode
        {
            get { return (IEntityNode)GetValue(CurrentNodeProperty); }
            set { SetValue(CurrentNodeProperty, value); }
        }

        public Command ExcludeTreeCommand { get; set; }
        public abstract void ExcludeTree(object param);

        public virtual bool CanExcludeTree()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public Command CheckAllForExcludeCommand { get; set; }

        public void CheckAllForExclude(object param)
        {

        }

        public virtual bool CanCheckAllForExclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public Command CheckAllForIncludeCommand { get; set; }

        public virtual bool CanCheckAllForInclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public void CheckAllForInclude(object param)
        {
            foreach (ISplitContent item in AllItems)
            {
                item.IsChecked = true;
            }
        }
        public IEntityNode FindNode(IEntityNode root, int id)
        {
            var queue = new Queue<IEntityNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Id == id)
                    return node;
                foreach (var child in node.ChildNodes)
                    queue.Enqueue(child);
            }
            return null;
        }

        public void RemoveNode(IList nodeCollection, int id)
        {
            foreach (IEntityNode node in nodeCollection)
            {
                if (node.Id == id)
                {
                    nodeCollection.Remove(node);
                    return;
                }
                if (node.ChildNodes.Count > 0)
                {
                    RemoveNode(node.ChildNodes, id);
                }
            }
        }

        private Visibility _visibility;
        public Visibility LinkVisibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged("LinkVisibility");
            }
        }
        public AccessType Access { get; set; }

        protected NodeLinkViewModel(AccessType access)
        {
            LinkVisibility = Visibility.Collapsed;
            Access = access;
            ViewDetailsCommand = new Command(ViewDetails, CanViewDetails);
        }

        public ISplitDetail Details
        {
            get { return (ISplitDetail)GetValue(DetailsProperty); }
            set { SetValue(DetailsProperty, value); }
        }

        public Command ViewDetailsCommand { get; set; }

        public void ViewDetails(object param)
        {
            Details = (ISplitDetail) param;
        }

        public bool CanViewDetails()
        {
            return (Access & AccessType.View) == AccessType.View;
        }

        public override string ToString()
        {
            return "Root: "+ RootNode.Title +"-"+ RootNode.ChildNodes.Count +"Current: "+ CurrentNode.Title +"-"+ CurrentNode.ChildNodes.Count;
        }
    }
}