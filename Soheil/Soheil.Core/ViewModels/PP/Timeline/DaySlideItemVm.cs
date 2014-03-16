using System;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// ViewModel for one day in PPTable's timeline
	/// </summary>
    public class DaySlideItemVm : DependencyObject
    {
		/// <summary>
		/// Creates an instance of DaySlideItemVm at the start of given DateTime
		/// </summary>
		/// <param name="dt">DateTime which is used to initialize this Vm</param>
		public DaySlideItemVm(DateTime dt)
		{
			Data = dt;
			ColumnIndex = dt.GetPersianDayOfMonth()-1;
			Text = dt.GetPersianDayOfMonth().ToString();
			DayOfWeek = dt.GetPersianDayOfWeek().ToString()[0].ToString();
		}
		/// <summary>
		/// Gets a bindable value to describe the starting DateTime of this Vm
		/// </summary>
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			protected set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(DaySlideItemVm), new UIPropertyMetadata(default(DateTime)));
		/// <summary>
		/// Gets a bindable Text to show in GUI as the DayOfMonth
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			protected set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(DaySlideItemVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable Text to show in GUI as the DayOfWeek
		/// </summary>
		public string DayOfWeek
		{
			get { return (string)GetValue(DayOfWeekProperty); }
			protected set { SetValue(DayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty DayOfWeekProperty =
			DependencyProperty.Register("DayOfWeek", typeof(string), typeof(DaySlideItemVm), new UIPropertyMetadata(null));
		//ColumnIndex Dependency Property (in days, from the start of the year)
		/// <summary>
		/// Gets a bindable number (zero-biased) that represents the number of days passes since the start of the year
		/// </summary>
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			protected set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(DaySlideItemVm), new UIPropertyMetadata(0));
    }
}
