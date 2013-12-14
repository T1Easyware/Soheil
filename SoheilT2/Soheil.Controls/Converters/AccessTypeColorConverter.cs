using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Controls.Converters
{
    public class AccessTypeColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var type = value is AccessType ? (AccessType) value : AccessType.None;
            switch (type)
            {
                case AccessType.None:
                    return Binding.DoNothing;
                case AccessType.View:
                    return new SolidColorBrush( Colors.Gainsboro);
                case AccessType.Print:
                    return new SolidColorBrush(Colors.MediumOrchid);
                case AccessType.Update:
                    return new SolidColorBrush( Colors.Gold);
                case AccessType.Insert:
                    return new SolidColorBrush( Colors.DodgerBlue);
                case AccessType.Full:
                    return new SolidColorBrush(Colors.LimeGreen);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}