using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.ViewModels;

namespace Soheil.Controls.Converters
{
    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class FishboneHorizontalUpVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRoot = ((FishboneNodeVM)value).NodeType == FishboneNodeType.Root;
            bool isUpsideDown = ((FishboneNodeVM)value).RootType == FishboneNodeType.Method
                || ((FishboneNodeVM)value).RootType == FishboneNodeType.Machines;

            return isRoot || isUpsideDown ?
                Visibility.Collapsed :
                Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class FishboneHorizontalDownVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRoot = ((FishboneNodeVM)value).NodeType == FishboneNodeType.Root;
            bool isUpsideDown = ((FishboneNodeVM)value).RootType == FishboneNodeType.Method
                || ((FishboneNodeVM)value).RootType == FishboneNodeType.Machines;

            return isRoot || !isUpsideDown ?
                Visibility.Collapsed :
                Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }

    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class FishboneUpGapVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUpNode = ((FishboneNodeVM)value).NodeType == FishboneNodeType.Method
                || ((FishboneNodeVM)value).NodeType == FishboneNodeType.Machines;

            return isUpNode ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class FishboneDownGapVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDownNode = ((FishboneNodeVM)value).NodeType == FishboneNodeType.Man
                || ((FishboneNodeVM)value).NodeType == FishboneNodeType.Material
                || ((FishboneNodeVM)value).NodeType == FishboneNodeType.Maintenance;

            return isDownNode ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
}