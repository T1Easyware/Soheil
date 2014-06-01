using System.Collections.Generic;
using System.Threading;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Reports
{
	public class OperatorBarSlideProvider : IItemsProvider<BarSlideItemVm>
    {

        private readonly int _count;
        public OperatorBarSlideProvider(int count)
		{
            _count = count;
		}

	    public int FetchCount()
	    {
            Thread.Sleep(1000);
            return _count;
	    }

	    public IList<BarSlideItemVm> FetchRange(int startCost, int count)
        {
			var list = new List<BarSlideItemVm>();
            return list;
        }
    }
}
