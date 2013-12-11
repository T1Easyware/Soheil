using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using Soheil.Core.Interfaces;

namespace Soheil.Controls.Convertors
{
    public class NodeExtractorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            //if (value is INodeContentViewModel)
            //{
            //    var node = ((INodeContentViewModel)value).ChildNodes;
            //    return node;
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}