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
    public class FishboneLabelRotateAngelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return 0;
            var rootType = ((FishboneNodeVM)value).RootType;
            var nodeType = ((FishboneNodeVM)value).NodeType;
            if (rootType == nodeType)
            {
                return 0;
            }
            switch (rootType)
            {
                case FishboneNodeType.None:
                case FishboneNodeType.Root:
                case FishboneNodeType.Man:
                case FishboneNodeType.Material:
                case FishboneNodeType.Maintenance:
                    return 0;
                case FishboneNodeType.Method:
                case FishboneNodeType.Machines:
                    return 180;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }
}