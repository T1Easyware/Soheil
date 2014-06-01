using System;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.Reports
{
	public class BarSlideItemVm: DependencyObject
    {
		public BarSlideItemVm(DateTime dt)
		{
			Data = dt;
			Header = string.Format("{0}/{1}", dt.GetPersianYear(), dt.GetPersianMonth());
		}
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(BarSlideItemVm), new UIPropertyMetadata(default(DateTime)));

		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(BarSlideItemVm), new UIPropertyMetadata(null));
    }
}
