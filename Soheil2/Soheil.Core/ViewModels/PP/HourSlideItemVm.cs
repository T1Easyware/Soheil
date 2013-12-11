using System;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
    public class HourSlideItemVm : DependencyObject
    {
		public HourSlideItemVm(DateTime dt)
		{
			Data = dt;
			ColumnIndex = (dt.GetPersianDayOfYear() - 1) * 24 + dt.Hour;
			Text = string.Format("{0:D2}:00",dt.Hour);
			DateText = string.Format("{0:D2}/{1:D2}", (int)dt.GetPersianMonth(), dt.GetPersianDayOfMonth());
		}
		//Data Dependency Property
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(HourSlideItemVm), new UIPropertyMetadata(default(DateTime)));
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(HourSlideItemVm), new UIPropertyMetadata(null));
		//ColumnIndex Dependency Property (in hours, from the start of the year)
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(HourSlideItemVm), new UIPropertyMetadata(0));
		//DateText Dependency Property
		public string DateText
		{
			get { return (string)GetValue(DateTextProperty); }
			set { SetValue(DateTextProperty, value); }
		}
		public static readonly DependencyProperty DateTextProperty =
			DependencyProperty.Register("DateText", typeof(string), typeof(HourSlideItemVm), new UIPropertyMetadata(null));
    }
}
