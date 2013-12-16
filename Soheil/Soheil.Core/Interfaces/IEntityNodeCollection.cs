using System.Collections;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityNodeCollection : IEntityCollection
    {
        Command ExcludeTreeCommand { get; set; }
        IEntityNode RootNode { get; set; }
        IEntityNode CurrentNode { get; set; }
        void ExcludeTree(object param);
        bool CanExcludeTree();
        IEntityNode FindNode(IEntityNode root, int id);
        void RemoveNode(IList nodeCollection, int id);
    }
}