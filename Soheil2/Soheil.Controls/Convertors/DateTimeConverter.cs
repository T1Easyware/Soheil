using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Common.Localization;

namespace Soheil.Controls.Convertors
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (CultureInfo.CurrentCulture.IetfLanguageTag == "en-US")
            {
                return (DateTime)value;

            }
            return ((DateTime)value).ToPersianDateTimeString();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            var strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}