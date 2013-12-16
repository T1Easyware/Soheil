using System;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
    public class DaySlideItemVm : DependencyObject
    {
		public DaySlideItemVm(DateTime dt)
		{
			Data = dt;
			ColumnIndex = dt.GetPersianDayOfMonth()-1;
			Text = dt.GetPersianDayOfMonth().ToString();
			DayOfWeek = dt.GetPersianDayOfWeek().ToString()[0].ToString();
		}
		//Data Dependency Property
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(DaySlideItemVm), new UIPropertyMetadata(default(DateTime)));
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(DaySlideItemVm), new UIPropertyMetadata(null));
		//DayOfWeek Dependency Property
		public string DayOfWeek
		{
			get { return (string)GetValue(DayOfWeekProperty); }
			set { SetValue(DayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty DayOfWeekProperty =
			DependencyProperty.Register("DayOfWeek", typeof(string), typeof(DaySlideItemVm), new UIPropertyMetadata(null));
		//ColumnIndex Dependency Property (in days, from the start of the year)
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(DaySlideItemVm), new UIPropertyMetadata(0));
    }
}
