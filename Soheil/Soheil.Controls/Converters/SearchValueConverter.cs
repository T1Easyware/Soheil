using System;
using System.Windows.Data;
using Soheil.Core.Interfaces;

namespace Soheil.Controls.Converters
{
    public class SearchValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var source = values[0] as ISplitItemContent;
            string cellText = source == null? string.Empty : source.SearchItem;
            string searchText = values[1] as string ?? string.Empty;

            if (!string.IsNullOrEmpty(cellText))
            {
                return cellText.ToLower().Contains(searchText.ToLower());
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
