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
		public void Update(double value)
		{
			if (double.IsNaN(value))
			{
				SetValue(IsPastDeadlineProperty, false);
				SetValue(RemainingPercentProperty, 0d);
			}
			else
			{
				SetValue(IsPastDeadlineProperty, value < 0);
				var x = Math.Abs(value);
				if (x > 10) x = 10;
				if (x == 0) x = 10;
				SetValue(RemainingPercentProperty, x * 10);
			}
		}
		public static readonly DependencyProperty RemainingPercentProperty =
			DependencyProperty.Register("RemainingPercent", typeof(double), typeof(PMBarVm), new PropertyMetadata(0d));
		public static readonly DependencyProperty IsPastDeadlineProperty =
			DependencyProperty.Register("IsPastDeadline", typeof(bool), typeof(PMBarVm), new PropertyMetadata(false));
	}
}
