using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using Soheil.Core.Base;
using Soheil.Core.Interfaces;

namespace Soheil.Controls.Convertors
{
    public class ItemExtendedConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return ((IList<object>) value).Any(item => ((ISplitContent) item).IsSelected);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}