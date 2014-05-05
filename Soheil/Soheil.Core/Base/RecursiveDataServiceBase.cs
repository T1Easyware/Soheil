using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class RecursiveDataServiceBase : DataServiceBase
    {
        public IEntityNode Root { get; set; }
        public abstract ObservableCollection<IEntityNode> GetChildren(int id);
        protected ObservableCollection<IEntityNode> GetChildrenNodes(IEnumerable<IEntityNode> allViewModels, IEntityNode parentNode)
        {
            var nodes = allViewModels.Where(x => parentNode == null ? x.ParentId == 0 : x.ParentId == parentNode.Id);
            foreach (var node in nodes)
            {
                if (parentNode == null)
                {
                    Root.ChildNodes.Add(node);
                }
                else
                {
                    parentNode.ChildNodes.Add(node);
                }
                GetChildrenNodes(allViewModels, node);
            }
            return parentNode == null? Root.ChildNodes : parentNode.ChildNodes;
        }

    }
}