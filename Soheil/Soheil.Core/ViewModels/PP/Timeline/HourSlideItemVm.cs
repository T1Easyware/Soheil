using System;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Timeline
{
	/// <summary>
	/// ViewModel for one hour in PPTable's timeline
	/// </summary>
    public class HourSlideItemVm : DependencyObject
    {
		/// <summary>
		/// Creates an instance of HourSlideItemVm starting at the given DateTime
		/// </summary>
		/// <param name="dt">Pass the start of the hour</param>
		public HourSlideItemVm(DateTime dt)
		{
			Data = dt;
			ColumnIndex = (dt.GetPersianDayOfYear() - 1) * 24 + dt.Hour;
			Text = string.Format("{0:D2}:00",dt.Hour);
			DateText = string.Format("{0:D2}/{1:D2}", (int)dt.GetPersianMonth(), dt.GetPersianDayOfMonth());
		}
		
		/// <summary>
		/// Gets a bindable value that represents the DateTime of this Vm
		/// </summary>
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(HourSlideItemVm), new UIPropertyMetadata(default(DateTime)));
		
		/// <summary>
		/// Gets a bindable text to show in GUI as this hour
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(HourSlideItemVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable number (zero-biased) that shows the number of hours since the start of year
		/// </summary>
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(HourSlideItemVm), new UIPropertyMetadata(0));
		
		/// <summary>
		/// Gets a bindable text to show in GUI as this hour
		/// </summary>
		public string DateText
		{
			get { return (string)GetValue(DateTextProperty); }
			set { SetValue(DateTextProperty, value); }
		}
		public static readonly DependencyProperty DateTextProperty =
			DependencyProperty.Register("DateText", typeof(string), typeof(HourSlideItemVm), new UIPropertyMetadata(null));
    }
}
