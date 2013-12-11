using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;

namespace Soheil.Controls.Convertors
{
    public class InsertAccessTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var access = value is AccessType ? (AccessType) value : AccessType.None;
            bool hasAccess = (access & AccessType.Insert) == AccessType.Insert;
            return hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class UpdateAccessTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var access = value is AccessType ? (AccessType)value : AccessType.None;
            bool hasAccess = (access & AccessType.Update) == AccessType.Update;
            return hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class PrintAccessTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var access = value is AccessType ? (AccessType)value : AccessType.None;
            bool hasAccess = (access & AccessType.Print) == AccessType.Print;
            return hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class ViewAccessTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var access = value is AccessType ? (AccessType)value : AccessType.None;
            bool hasAccess = (access & AccessType.View) == AccessType.View;
            return hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class FullAccessTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var access = value is AccessType ? (AccessType)value : AccessType.None;
            bool hasAccess = (access & AccessType.Insert) == AccessType.Insert
                && (access & AccessType.Update) == AccessType.Update
                && (access & AccessType.Print) == AccessType.Print
                && (access & AccessType.View) == AccessType.View;
            return hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}