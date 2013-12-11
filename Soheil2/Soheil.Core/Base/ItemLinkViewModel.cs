using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class ItemLinkViewModel : ViewModelBase, ISplitItemCollectionLink
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ListCollectionView), typeof(ItemLinkViewModel), null);

        public static readonly DependencyProperty AllItemsProperty =
            DependencyProperty.Register("AllItems", typeof(ListCollectionView), typeof(ItemLinkViewModel), null);

        public static readonly DependencyProperty DetailsProperty =
            DependencyProperty.Register("Details", typeof(ISplitDetail), typeof(ItemLinkViewModel), null);

        public ListCollectionView AllItems
        {
            get { return (ListCollectionView)GetValue(AllItemsProperty); }
            set { SetValue(AllItemsProperty, value); }
        }
        public ListCollectionView SelectedItems
        {
            get { return (ListCollectionView)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
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

        public Command IncludeCommand { get; set; }

        public virtual bool CanInclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public abstract void Include(object param);

        public abstract void RefreshItems();

        public Command ExcludeCommand { get; set; }
        public abstract void Exclude(object param);

        public virtual bool CanExclude()
        {
            return (Access & AccessType.Update) == AccessType.Update;
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

        protected ItemLinkViewModel(AccessType access)
        {
            LinkVisibility = Visibility.Collapsed;
            ViewDetailsCommand = new Command(ViewDetails, CanViewDetails);
            Access = access;
        }
    }
}