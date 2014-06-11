using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface ISplitContent
    {
        bool IsSelected { get; set; }
        bool IsLinked { get; set; }
        bool IsDeleting { get; set;}

        bool IsChecked { get; set; }

        Visibility MainContentVisibility { get; set; }
        ISplitCollectionLink CurrentLink { get; set; }
        ModificationStatus Mode { get; set; }

        Command ViewItemLinkCommand { get; set; }
        Command ViewNodeLinkCommand { get; set; }
        Command BackToMainContentCommand { get; set; }


        void ViewItemLink(object param);
        void ViewNodeLink(object param);
        void BackToMainContent(object param);

        bool CanViewItemLinks();
        bool CanViewNodeLinks();
        bool CanNavigateBack();
    }

    public interface ISplitItemContent : ISplitContent, IEntityItem
    {
        ListCollectionView Groups { get; set; }
        IEntityItem SelectedGroupVM { get; set; }
        string SearchItem { get; set; }
    }

    public interface ISplitNodeContent : ISplitItemContent, IEntityNode
    {
        bool IsExpanded { get; set; }
    }

}