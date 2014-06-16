
using System.Windows;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface ISplitLink
    {
        Visibility LinkVisibility { get; set; }
    }

    public interface ISplitCollectionLink : ISplitLink, IEntityRelationCollection
    {
        ISplitDetail Details { get; set; }
        Command ViewDetailsCommand { get; set; }
        void ViewDetails(object param);
        bool CanViewDetails();

    }
    public interface ISplitItemCollectionLink : ISplitCollectionLink, IEntityItemRelationCollection
    {
    }
    public interface ISplitNodeCollectionLink : ISplitCollectionLink, IEntityNodeRelationCollection
    {
    }

}