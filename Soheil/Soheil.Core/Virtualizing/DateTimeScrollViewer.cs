using System.Windows;
using System.Windows.Controls;
using Soheil.Common;

namespace Soheil.Core.Virtualizing
{
	/// <summary>
	///     <MyNamespace:DateTimeScrollViewer/>
	/// </summary>
	public class DateTimeScrollViewer : ScrollViewer
	{
		static DateTimeScrollViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeScrollViewer), new FrameworkPropertyMetadata(typeof(DateTimeScrollViewer)));
		}

		//DateLevel Dependency Property
		public DateTimeIntervals DateLevel
		{
            get { return (DateTimeIntervals)GetValue(DateLevelProperty); }
			set { SetValue(DateLevelProperty, value); }
		}
		public static readonly DependencyProperty DateLevelProperty =
            DependencyProperty.Register("DateLevel", typeof(DateTimeIntervals), typeof(DateTimeScrollViewer), new UIPropertyMetadata(DateTimeIntervals.Hourly));

		//LittleWindowWidth Dependency Property
		public double LittleWindowWidth
		{
			get { return (double)GetValue(LittleWindowWidthProperty); }
			set { SetValue(LittleWindowWidthProperty, value); }
		}
		public static readonly DependencyProperty LittleWindowWidthProperty =
			DependencyProperty.Register("LittleWindowWidth", typeof(double), typeof(DateTimeScrollViewer), new UIPropertyMetadata(0d));
	}
}
