using System.Collections.Generic;
using System.Threading;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Reports
{
	public class CostBarSlideProvider : IItemsProvider<CostBarSlideItemVm>
    {

        private readonly int _count;
        public CostBarSlideProvider(int count)
		{
            _count = count;
		}

	    public int FetchCount()
	    {
            Thread.Sleep(1000);
            return _count;
	    }

	    public IList<CostBarSlideItemVm> FetchRange(int startCost, int count)
        {
			var list = new List<CostBarSlideItemVm>();
            for( int i=startCost; i<startCost+count; i++ )
            {
                //var item = new CostBarSlideItemVm(CommonExtensions.PersianCalendar.AddMonths(IndicesVm.StartingPoint, i));
                //list.Add(item);
            }
            return list;
        }
    }
}
