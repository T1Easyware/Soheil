using System.Collections.ObjectModel;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class NodeRelationDetailViewModel : EntityObjectBase, ISplitNodeRelationDetail
    {
        public static readonly DependencyProperty PresentationTypeProperty =
            DependencyProperty.Register("PresentationType", typeof(RelationDirection), typeof(NodeRelationDetailViewModel), null);

        protected NodeRelationDetailViewModel(AccessType access) : base(access)
        {
            ChildNodes = new ObservableCollection<IEntityNode>();
        }

        public RelationDirection PresentationType
        {
            get { return (RelationDirection)GetValue(PresentationTypeProperty); }
            set { SetValue(PresentationTypeProperty, value); }
        }

        public bool IsChecked { get; set; }

        public abstract int Id { get; set; }

        public ObservableCollection<IEntityNode> ChildNodes { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }

    }
}