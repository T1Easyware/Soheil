
using System.Windows;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface ISplitLink
    {
        Visibility LinkVisibility { get; set; }
    }

    public interface ISplitCollectionLink : ISplitLink, IEntityCollection
    {
        ISplitDetail Details { get; set; }
        Command ViewDetailsCommand { get; set; }
        void ViewDetails(object param);
        bool CanViewDetails();

    }
    public interface ISplitItemCollectionLink : ISplitCollectionLink, IEntityItemCollection
    {
    }
    public interface ISplitNodeCollectionLink : ISplitCollectionLink, IEntityNodeCollection
    {
    }

}