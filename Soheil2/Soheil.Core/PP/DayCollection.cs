using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.ViewModels.PP;

namespace Soheil.Core.PP
{
    public class DayCollection : ObservableCollection<DaySlideItemVm>
    {
		public DayCollection()
		{

		}
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
