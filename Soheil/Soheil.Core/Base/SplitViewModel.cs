using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class SplitViewModel : ViewModelBase, ISplitList
    {
        #region ISplitList Members

		public static readonly DependencyProperty CurrentContentProperty =
			DependencyProperty.Register("CurrentContent", typeof(ISplitItemContent), typeof(SplitViewModel),
			new PropertyMetadata(null, (d, e) =>
			{
				if (((SplitViewModel)d).ResetCommand != null)
					if (((SplitViewModel)d).ResetCommand.CanExecute())
						((SplitViewModel)d).ResetCommand.Execute(e.OldValue);
			}));

        public static readonly DependencyProperty CurrentNodeProperty =
            DependencyProperty.Register("CurrentNode", typeof(ISplitItemContent), typeof(SplitViewModel), null);

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ListCollectionView), typeof(SplitViewModel), null);

        public static readonly DependencyProperty GroupItemsProperty =
            DependencyProperty.Register("GroupItems", typeof(ListCollectionView), typeof(SplitViewModel), null);

        //protected SplitViewModel()
        //{
        //    RefreshCommand = new Command(CreateItems);
        //}

        public AccessType Access { get; set; }

        public virtual Command AddCommand { get; set; }

        public virtual Command AddGroupCommand { get; set; }

		public virtual Command ViewCommand { get; set; }
		
		public virtual Command ResetCommand { get; set; }

        public Command RefreshCommand { get; set; }


        public List<ColumnInfo> ColumnHeaders { get; set; }


        public static readonly DependencyProperty SearchBoxProperty =
            DependencyProperty.Register("SearchBox", typeof (string), typeof (SplitViewModel), new PropertyMetadata(default(string)));

        public string SearchBox
        {
            get { return (string) GetValue(SearchBoxProperty); }
            set { SetValue(SearchBoxProperty, value); }
        }

        public virtual void Add(object param)
        {
            throw new NotImplementedException();
        }

        public virtual void AddGroup(object param)
        {
            throw new NotImplementedException();
        }

        public virtual void View(object param)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Resets the specified item of grid or tree
		/// </summary>
		/// <param name="param">The ItemVM to reset to its database values</param>
		public virtual void Reset(object param)
		{
			//??? should be implemeneted asap
		}

        public virtual bool CanAdd()
        {
            return (Access & AccessType.Insert) == AccessType.Insert;
        }

        public virtual bool CanAddGroup()
        {
            return (Access & AccessType.Insert) == AccessType.Insert;
        }

        public virtual bool CanView()
        {
            return (Access & AccessType.View) == AccessType.View;
        }

		public virtual bool CanReset()
		{
			return true;//??? can be left like this (implement in those classes with special situations where canReset can be false)
		}

        protected SplitViewModel(AccessType access)
        {
            Access = access;
        }

        public virtual ISplitItemContent CreateClone(ISplitItemContent original)
        {
            return null;
        }

        public ISplitContent CurrentContent
        {
            get { return (ISplitItemContent)GetValue(CurrentContentProperty); }
            set { SetValue(CurrentContentProperty, value); SetValue(CurrentContentProperty, value); }
        }

        public ListCollectionView Items
        {
            get { return (ListCollectionView)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public ListCollectionView GroupItems
        {
            get { return (ListCollectionView)GetValue(GroupItemsProperty); }
            set { SetValue(GroupItemsProperty, value); }
        }

        public abstract void CreateItems(object param);

        #endregion
    }

    public abstract class GridSplitViewModel : SplitViewModel, IGridSplitList
    {
        protected GridSplitViewModel(AccessType access) : base(access)
        {
            
        }
    }
    public abstract class TreeSplitViewModel : SplitViewModel, ITreeSplitList
    {
        public static readonly DependencyProperty RootNodeProperty =
            DependencyProperty.Register("RootNode", typeof(IEntityNode), typeof(TreeSplitViewModel), null);

        public IEntityNode RootNode
        {
            get { return (IEntityNode)GetValue(RootNodeProperty); }
            set { SetValue(RootNodeProperty, value); }
        }

        public ObservableCollection<IEntityNode> ChildNodes { get; set; }

        public static void Remove(int id, IEntityNode root)
        {
            foreach (var node in root.ChildNodes)
            {
                if (node.Id == id)
                {
                    root.ChildNodes.Remove(node);
                    return;
                }
                Remove(id,node);
            }
        }

        protected TreeSplitViewModel(AccessType access): base(access)
        {
            ChildNodes = new ObservableCollection<IEntityNode>();
        }
    }
}