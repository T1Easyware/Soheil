using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Controls.Converters
{
    public class DeleteColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return new SolidColorBrush(Colors.Crimson);
            }
            return new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            int a;
            throw new NotImplementedException();
        }

        #endregion
    }
}