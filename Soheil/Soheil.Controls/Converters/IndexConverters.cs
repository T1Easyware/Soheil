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
			return string.Format(System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"), "{0:F2} ساعت", System.Convert.ToDouble(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
