using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Soheil.Controls.Converters
{
	public class PercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return string.Format(System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"), "{0:F2}%", System.Convert.ToDouble(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class OeeHoursConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double val = System.Convert.ToDouble(value);
			int h = (int)val;
			int min = (int)Math.Round((val - h)*60);

			if (parameter == null)
				return string.Format(System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"), "{0}:{1}", h, min);
			else
				return string.Format(System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"), "{0} ساعت و {1} دقیقه", h, min);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
