using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Soheil.Controls.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if(value is Color)
              return new SolidColorBrush((Color) value);
            return new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
