using System.Windows;
using Soheil.Common;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class ItemRelationDetailViewModel : EntityObjectBase, ISplitItemRelationDetail
    {
        public static readonly DependencyProperty PresentationTypeProperty =
            DependencyProperty.Register("PresentationType", typeof(RelationDirection), typeof(ItemRelationDetailViewModel), null);

        protected ItemRelationDetailViewModel(AccessType access, RelationDirection presentationType)
            : base(access)
        {
            PresentationType = presentationType;
        }

        public RelationDirection PresentationType
        {
            get { return (RelationDirection)GetValue(PresentationTypeProperty); }
            set { SetValue(PresentationTypeProperty, value); }
        }

        public abstract int Id { get; set; }
    }
}
