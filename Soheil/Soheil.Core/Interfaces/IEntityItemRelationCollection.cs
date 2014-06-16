using System.Windows.Data;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityItemRelationCollection : IEntityRelationCollection
    {
        ListCollectionView SelectedItems { get; set; }
    }
}