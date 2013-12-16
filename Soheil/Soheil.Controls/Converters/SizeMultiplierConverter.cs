using System;
using System.Globalization;
using System.Windows.Data;

namespace Soheil.Controls.Converters
{
    public class SizeMultiplierConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return (double) value*(double) parameter;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}