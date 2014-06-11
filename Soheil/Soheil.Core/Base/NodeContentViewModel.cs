using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class NodeContentViewModel : ItemContentViewModel, ISplitNodeContent
    {
        public ObservableCollection<IEntityNode> ChildNodes { get; set; }
        //public abstract int Id { get; set; }
        public int ParentId { get; set; }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (NodeContentViewModel), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        protected NodeContentViewModel(AccessType access)
            : base(access)
        {
            ChildNodes = new ObservableCollection<IEntityNode>();
        }
        public override string ToString()
        {
            return Title +": "+ Id + "-" + ParentId;
        }

        public override string SearchItem { get; set; }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof (bool), typeof (NodeContentViewModel), new PropertyMetadata(default(bool)));

        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
    }
}
