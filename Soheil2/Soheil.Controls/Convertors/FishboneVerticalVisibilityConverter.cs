using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.ViewModels;

namespace Soheil.Controls.Convertors
{
    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class FishboneVerticalVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRoot = ((FishboneNodeVM)value).NodeType == FishboneNodeType.Root;

            return isRoot ?
                       Visibility.Visible :
                       Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
}