using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.ViewModels.OrganizationCalendar;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// An observable collection of <see cref="DaySlideItemVm"/>s used to populate timeline
	/// </summary>
    public class DayCollection : ObservableCollection<DaySlideItemVm>
    {
		/// <summary>
		/// Loads one month of <see cref="DaySlideItemVm"/>s from given date
		/// </summary>
		/// <param name="startDate">DateTime to start from</param>
		public void Reload(DateTime startDate)
		{
			Clear();
			int daysInMonth = startDate.GetPersianMonthDays();
			int dayOfYear = startDate.GetPersianDayOfYear() - 1;//to make it zero-biased
			for (int i = 0; i < daysInMonth; i++)
			{
				DaySlideItemVm item = new DaySlideItemVm(
					CommonExtensions.PersianCalendar.AddDays(startDate, i));
				Add(item);
			}
		}
    }
}
