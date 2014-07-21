using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	public class PMBarVm : DependencyObject
	{
		/// <summary>
		/// Gets or sets a bindable value that indicates RemainingPercent
		/// </summary>
		public void SafeUpdateTimings(double percent)
		{
			Dispatcher.Invoke(new Action<double>(perc =>
			{
				SetValue(IsPastDeadlineProperty, perc < 0);
				SetValue(RemainingPercentProperty, Math.Abs(perc));
			}), percent);
		}
		public static readonly DependencyProperty RemainingPercentProperty =
			DependencyProperty.Register("RemainingPercent", typeof(double), typeof(PMBarVm), new PropertyMetadata(0d));
		public static readonly DependencyProperty IsPastDeadlineProperty =
			DependencyProperty.Register("IsPastDeadline", typeof(bool), typeof(PMBarVm), new PropertyMetadata(false));
	}
}
