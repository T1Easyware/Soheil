using Soheil.Common;

namespace Soheil.Core.Interfaces
{
    public interface ISplitDetail
    {
        RelationDirection PresentationType { get; set; }
        bool IsChecked { get; set; }

    }

    public interface ISplitItemRelationDetail : ISplitDetail, IEntityItemRelation
    {
    }

    public interface ISplitNodeRelationDetail : ISplitDetail, IEntityNodeRelation
    {
    }
}