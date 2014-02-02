using System.Windows.Data;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityItemCollection : IEntityCollection
    {
        ListCollectionView SelectedItems { get; set; }
    }
}