using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class NodeContentViewModel : ItemContentViewModel, ISplitNodeContent
    {
        public ObservableCollection<IEntityNode> ChildNodes { get; set; }
        //public abstract int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }

        protected NodeContentViewModel(AccessType access)
            : base(access)
        {
            ChildNodes = new ObservableCollection<IEntityNode>();
        }
        public override string ToString()
        {
            return Title +": "+ Id + "-" + ParentId;
        }

        public override string SearchItem { get; set; }
    }
}
