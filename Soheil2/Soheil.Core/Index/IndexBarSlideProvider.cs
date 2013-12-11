using System.Collections.Generic;
using System.Threading;
using Soheil.Common;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Index
{
	public class IndexBarSlideProvider : IItemsProvider<IndexBarSlideItemVm>
    {

        private readonly int _count;
        public IndexBarSlideProvider(int count)
		{
            _count = count;
		}

	    public int FetchCount()
	    {
            Thread.Sleep(1000);
            return _count;
	    }

	    public IList<IndexBarSlideItemVm> FetchRange(int startIndex, int count)
        {
			var list = new List<IndexBarSlideItemVm>();
            for( int i=startIndex; i<startIndex+count; i++ )
            {
                //var item = new IndexBarSlideItemVm(CommonExtensions.PersianCalendar.AddMonths(IndicesVm.StartingPoint, i));
                //list.Add(item);
            }
            return list;
        }
    }
}
