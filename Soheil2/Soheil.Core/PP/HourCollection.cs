using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.ViewModels.PP;

namespace Soheil.Core.PP
{
	public class HourCollection : ObservableCollection<HourSlideItemVm>
	{
		public HourCollection()
		{
		}
		public void FetchRange(DateTime rangeStart, DateTime rangeEnd)
		{
			rangeStart = new DateTime(rangeStart.Year, rangeStart.Month, rangeStart.Day, rangeStart.Hour, 0, 0);
			rangeEnd = new DateTime(rangeEnd.Year, rangeEnd.Month, rangeEnd.Day, rangeEnd.Hour, 0, 0);
			//add inside-the-box hours
			var tmp = rangeStart;
			while (tmp <= rangeEnd)
			{
				if (!this.Any(x => x.Data == tmp))
					Add(new HourSlideItemVm(tmp));
				tmp = tmp.AddHours(1);
			}
			//remove outside-the-box hours
			foreach (var hour in this.Where(x => x.Data < rangeStart && x.Data > rangeEnd))
			{
				this.Remove(hour);
			}
		}
	}
}
