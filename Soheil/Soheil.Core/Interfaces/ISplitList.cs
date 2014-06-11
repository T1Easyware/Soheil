using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface ISplitList
    {
        Command AddCommand { get; set; }
        Command AddGroupCommand { get; set; }
        Command ViewCommand { get; set; }
        Command RefreshCommand { get; set; }
        ListCollectionView Items { get; set; }
        ListCollectionView GroupItems { get; set; }
        ISplitContent CurrentContent { get; set; }
        List<ColumnInfo> ColumnHeaders { get; set; }
        string SearchBox { get; set; }

        void CreateItems(object param);
        ISplitItemContent CreateClone(ISplitItemContent original);
        void Add(object param);
        void AddGroup(object param);
        void View(object param);
        bool CanAdd();
        bool CanAddGroup();
        bool CanView();
    }

    public interface IGridSplitList : ISplitList
    {
    }

    public interface ITreeSplitList : ISplitList
    {
        IEntityNode RootNode { get; set; }
        ObservableCollection<IEntityNode> ChildNodes { get; set; }

    }


}