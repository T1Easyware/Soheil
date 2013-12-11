using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.ViewModels;

namespace Soheil.Controls.Convertors
{
    [ValueConversion(typeof(ItemsPresenter), typeof(Orientation))]
    public class ItemsPanelOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemsPresenter = value as ItemsPresenter;
            if (itemsPresenter == null)
                return Binding.DoNothing;

            var item = itemsPresenter.TemplatedParent as TreeViewItem;
            if (item == null)
                return Binding.DoNothing;

            bool isRoot = ((FishboneNodeVM)item.Header).NodeType != FishboneNodeType.None;

            return isRoot ?
                Orientation.Horizontal :
                Orientation.Vertical;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
}