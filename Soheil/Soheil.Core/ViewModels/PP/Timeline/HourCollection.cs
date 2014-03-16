using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// An observable collection of <see cref="HourSlideItemVm"/>s used to populate timeline
	/// </summary>
	public class HourCollection : ObservableCollection<HourSlideItemVm>
	{
		/// <summary>
		/// Loads a range of hours
		/// </summary>
		/// <param name="rangeStart">inclusive Starting hour (minute and seconds are set to zero)</param>
		/// <param name="rangeEnd">inclusive Ending hour (minute and seconds are set to zero)</param>
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
