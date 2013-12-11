using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Soheil.Controls.Convertors
{
    public class ReverseVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            if ((Visibility)value == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}