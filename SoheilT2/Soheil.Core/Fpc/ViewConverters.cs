using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Common;

namespace Soheil.Core.Fpc
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BooleanToInvisibilityConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				return (bool)value ? Visibility.Collapsed : Visibility.Visible;
			else
				return (int)value == System.Convert.ToInt32(parameter) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !((bool)value);
		}
	}
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BooleanToVisibilityConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			else
				return (int)value == System.Convert.ToInt32(parameter) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((bool)value);
		}
	}
	[ValueConversion(typeof(object), typeof(Visibility))]
	public class NullToInvisibilityConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
	[ValueConversion(typeof(TreeItemVm), typeof(double))]
	public class TreeItemToExpanderButtonWidthConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((TreeItemVm)value).TreeLevel == System.Convert.ToInt32(parameter) ? 0d : 20d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
	[ValueConversion(typeof(bool), typeof(double))]
	public class ExpanderHeaderOpacityConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? 0.85d : 0.35d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
	public class FocusedStateBorderBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var thisState = values[0] as StateVm;
			var selState = values[1] as StateVm;
			return thisState == selState ?
				new SolidColorBrush(Color.FromRgb(255, 170, 0)) { Opacity = 1 } :
				new SolidColorBrush(Color.FromRgb(0, 170, 255)) { Opacity = 0.7 };
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class SelectedStateBorderBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var thisState = values[0] as StateVm;
			var selState = values[1] as StateVm;
			return thisState == selState ?
				new SolidColorBrush(Color.FromRgb(255, 170, 0)) { Opacity = 0.67 } :
				new SolidColorBrush(Color.FromRgb(224, 240, 255)) { Opacity = 0.67 };
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
