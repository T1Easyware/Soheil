using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// An observable collection of <see cref="MonthSlideItemVm"/>s used to populate timeline
	/// </summary>
	public class MonthCollection : ObservableCollection<MonthSlideItemVm>
    {
		/// <summary>
		/// Occurs when selected month of this collection is changed
		/// </summary>
		public event Action<MonthSlideItemVm> SelectedMonthChanged;

		/// <summary>
		/// Creates an instance of MonthCollection starting at the startDate
		/// <para>and configures its items</para>
		/// </summary>
		/// <param name="startDate">Pass the start of year</param>
		public MonthCollection(DateTime startDate)
		{
			int daysFromStartOfYear = 0;
			for (int i = 0; i < 12; i++)
			{
				//create a new month
				MonthSlideItemVm item = new MonthSlideItemVm(
					CommonExtensions.PersianCalendar.AddMonths(startDate, i), 
					ref daysFromStartOfYear);
				
				//add the event handler for Selected event of each item
				item.Selected += month =>
				{
					//when a month is selected, deselect other monthes
					foreach (var anotherMonth in this.Where(x => x != month))
					{
						anotherMonth.IsSelected = false;
					}

					//fire SelectedMonthChanged event
					if (SelectedMonthChanged != null)
						SelectedMonthChanged(month);
				};

				//configures month to be self-conscience about when it can't be deselected
				item.CanDeselected = month =>
				{
					return this.Where(x => x != month).Any(x => x.IsSelected);
				};

				//add the new month item to the collection
				Add(item);
			}
		}

    }
}
