using System.Windows.Data;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityCollection : IEntity
    {
        Command ExcludeCommand { get; set; }
        Command ExcludeRangeCommand { get; set; }
        Command CheckAllForExcludeCommand { get; set; }

        Command IncludeCommand { get; set; }
        Command IncludeRangeCommand { get; set; }
        Command CheckAllForIncludeCommand { get; set; }

        ListCollectionView AllItems { get; set; }


        void Exclude(object param);
        void ExcludeRange(object param);
        void CheckAllForExclude(object param);

        void Include(object param);
        void IncludeRange(object param);
        void CheckAllForInclude(object param);

        bool CanExclude();
        bool CanExcludeRange();
        bool CanCheckAllForExclude();

        bool CanInclude();
        bool CanIncludeRange();
        bool CanCheckAllForInclude();

        void RefreshItems();
    }
}
