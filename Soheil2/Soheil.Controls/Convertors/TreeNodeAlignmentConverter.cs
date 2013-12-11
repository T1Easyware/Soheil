using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.ViewModels;

namespace Soheil.Controls.Convertors
{
    [ValueConversion(typeof(ItemsPresenter), typeof(HorizontalAlignment))]
    public class TreeNodeAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRoot = ((FishboneNodeVM)value).NodeType != FishboneNodeType.None;

            return isRoot ?
                HorizontalAlignment.Stretch :
                HorizontalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }

    [ValueConversion(typeof(ItemsPresenter), typeof(VerticalAlignment))]
    public class TreeNodeVerAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRoot = ((FishboneNodeVM)value).NodeType != FishboneNodeType.None;

            return isRoot ?
                VerticalAlignment.Stretch :
                VerticalAlignment.Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
}