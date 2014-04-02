using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// An observable collection of <see cref="DaySlideItemVm"/>s used to populate timeline
	/// </summary>
    public class DayCollection : ObservableCollection<DaySlideItemVm>
    {
		/// <summary>
		/// Loads one month worth of <see cref="DaySlideItemVm"/>s
		/// </summary>
		/// <param name="startDate">DateTime to start from</param>
		public void Reload(DateTime startDate)
		{
			Clear();
			int daysInMonth = startDate.GetPersianMonthDays();
			for (int i = 0; i < daysInMonth; i++)
			{
				DaySlideItemVm item = new DaySlideItemVm(CommonExtensions.PersianCalendar.AddDays(startDate, i));
				Add(item);
			}
		}
    }
}
