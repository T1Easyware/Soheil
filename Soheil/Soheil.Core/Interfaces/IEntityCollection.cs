using System.Windows.Data;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityCollection : IEntity
    {
        Command ExcludeCommand { get; set; }

        Command IncludeCommand { get; set; }

        ListCollectionView AllItems { get; set; }


        void Exclude(object param);

        bool CanExclude();

        bool CanInclude();

        void Include(object param);

        void RefreshItems();
    }
}
