using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class ItemContentViewModel : EntityObjectBase, ISplitItemContent
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof (bool), typeof (ItemContentViewModel), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsLinkedProperty =
            DependencyProperty.Register("IsLinked", typeof (bool), typeof (ItemContentViewModel), new PropertyMetadata(default(bool)));

        public bool IsLinked
        {
            get { return (bool) GetValue(IsLinkedProperty); }
            set { SetValue(IsLinkedProperty, value); }
        }

        public static readonly DependencyProperty IsDeletingProperty =
            DependencyProperty.Register("IsDeleting", typeof(bool), typeof(ItemContentViewModel), new PropertyMetadata(default(bool)));

        public bool IsDeleting
        {
            get { return (bool)GetValue(IsDeletingProperty); }
            set { SetValue(IsDeletingProperty, value); }
        }

        private Visibility _visibility;
        public Visibility MainContentVisibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged("MainContentVisibility");
            }
        }

        public static readonly DependencyProperty CurrentLinkProperty =
            DependencyProperty.Register("CurrentLink", typeof(ISplitCollectionLink), typeof(ItemContentViewModel), null);

        public ISplitCollectionLink CurrentLink
        {
            get { return (ISplitCollectionLink)GetValue(CurrentLinkProperty); }
            set { SetValue(CurrentLinkProperty, value); }
        }

        private ModificationStatus _mode;
        public ModificationStatus Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
            }
        }

        public Command ViewItemLinkCommand { get; set; }
        public Command ViewNodeLinkCommand { get; set; }
        public Command BackToMainContentCommand { get; set; }

        public virtual void ViewItemLink(object param)
        {
            CurrentLink.LinkVisibility = Visibility.Visible;
            MainContentVisibility = Visibility.Collapsed;
        }

        public virtual void ViewNodeLink(object param)
        {
            CurrentLink.LinkVisibility = Visibility.Visible;
            MainContentVisibility = Visibility.Collapsed;
        }

        public void BackToMainContent(object param)
        {
            CurrentLink.LinkVisibility = Visibility.Collapsed;
            MainContentVisibility = Visibility.Visible;
        }

        public bool CanViewItemLinks()
        {
            return true;
        }

        public bool CanViewNodeLinks()
        {
            return true;
        }

        public bool CanNavigateBack()
        {
            return true;
        }


        protected ItemContentViewModel(AccessType access) : base(access)
        {
            ViewItemLinkCommand = new Command(ViewItemLink, CanViewItemLinks);
            ViewNodeLinkCommand = new Command(ViewNodeLink, CanViewNodeLinks);
            BackToMainContentCommand = new Command(BackToMainContent, CanNavigateBack);
        }

        public abstract int Id { get; set; }

        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("GroupsProperty", typeof(ListCollectionView), typeof(EntityObjectBase), null);

        public ListCollectionView Groups
        {
            get { return (ListCollectionView)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public IEntityItem SelectedGroupVM { get; set; }

        public virtual string SearchItem { get; set; }

    }

}
