using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Soheil.Core.Interfaces
{
    public interface IEntityNode : IEntityItem
    {
        ObservableCollection<IEntityNode> ChildNodes { get; set; }
        int ParentId { get; set; }
        string Title { get; set; }

    }
}
