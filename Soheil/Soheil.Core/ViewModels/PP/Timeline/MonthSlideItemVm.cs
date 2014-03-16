using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// ViewModel for one month in PPTable's timeline
	/// </summary>
	public class MonthSlideItemVm: DependencyObject
    {
		/// <summary>
		/// Occurs when this vm is selected
		/// </summary>
		public event Action<MonthSlideItemVm> Selected;
		/// <summary>
		/// Returns a value that indicates a month item in the parent month collection is selected
		/// </summary>
		public Func<MonthSlideItemVm, bool> CanDeselected;

		/// <summary>
		/// Creates an instance of MonthSlideItemVm starting at the given DateTime
		/// </summary>
		/// <remarks>This constructor adds the number of days in month to its ref parameter</remarks>
		/// <param name="dt">Pass the start of the month</param>
		/// <param name="daysFromStartOfYear">Pass 0 for first month, so the value can be accumulated over each month</param>
		public MonthSlideItemVm(DateTime dt, ref int daysFromStartOfYear)
		{
			Data = dt;
			NumOfDays = dt.GetPersianMonthDays();
			DaysFromStartOfYear = daysFromStartOfYear;
			daysFromStartOfYear += NumOfDays;
			ColumnIndex = (int)dt.GetPersianMonth() - 1;
			Text = string.Format("{0}/{1}", dt.GetPersianYear(), dt.GetPersianMonth());
		}

		/// <summary>
		/// Gets a bindable value that represents the DateTime of this Vm
		/// </summary>
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			protected set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(MonthSlideItemVm), new UIPropertyMetadata(default(DateTime)));
	
		/// <summary>
		/// Gets a bindable text to show in GUI as this month
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			protected set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(MonthSlideItemVm), new UIPropertyMetadata(null));
	
		/// <summary>
		/// Gets a bindable number that shows the number of days in this month
		/// </summary>
		public int NumOfDays
		{
			get { return (int)GetValue(NumOfDaysProperty); }
			protected set { SetValue(NumOfDaysProperty, value); }
		}
		public static readonly DependencyProperty NumOfDaysProperty =
			DependencyProperty.Register("NumOfDays", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(31));
	
		/// <summary>
		/// Gets a bindable number (zero-biased) that shows the number of month since the start of year
		/// </summary>
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			protected set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(0));

		/// <summary>
		/// Gets a bindable number (zero-biased) that shows the number of days since the start of year
		/// </summary>
		public int DaysFromStartOfYear
		{
			get { return (int)GetValue(DaysFromStartOfYearProperty); }
			protected set { SetValue(DaysFromStartOfYearProperty, value); }
		}
		public static readonly DependencyProperty DaysFromStartOfYearProperty =
			DependencyProperty.Register("DaysFromStartOfYear", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(0));

		/// <summary>
		/// Gets or sets a bindable value that indicates if this month is selected in the timeline
		/// </summary>
		/// <remarks>
		/// <para>Changing this value causes the previous selected month to be deselected</para>
		/// <para>If no other month is selected, this month can't be deselected</para>
		/// <para>Also changing this value causes the reload of the day collection in the parent month collection</para>
		/// </remarks>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(MonthSlideItemVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
				{
					var vm = (MonthSlideItemVm)d;
					//this event will deselect previousely selected month
					if (vm.Selected != null) vm.Selected(vm);
				}
			}, (d, v) =>
			{
				if ((bool)v) return true;
				var vm = (MonthSlideItemVm)d;
				if (vm.CanDeselected != null)
					return !vm.CanDeselected(vm);
				return false;
			}));		
    }
}
