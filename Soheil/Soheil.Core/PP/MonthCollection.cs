using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.ViewModels.PP;

namespace Soheil.Core.PP
{
	public class MonthCollection : ObservableCollection<MonthSlideItemVm>
    {
		public MonthCollection(DateTime startDate, DayCollection daysOfMonth, PPTableVm parent)
		{
			int daysFromStartOfYear = 0;
			for (int i = 0; i < 12; i++)
			{
				MonthSlideItemVm item = new MonthSlideItemVm(
					CommonExtensions.PersianCalendar.AddMonths(startDate, i), 
					daysOfMonth, 
					ref daysFromStartOfYear, 
					parent);
				Add(item);
			}
		}
    }
}
