using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Soheil.Controls.Converters
{
	public class ShiftStartSecondsToMargin : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(parameter!=null)
				return new Thickness(((int)value - Soheil.Common.SoheilConstants.EDITOR_START_SECONDS) / 60, 28, 0, 0);
			return new Thickness(((int)value - Soheil.Common.SoheilConstants.EDITOR_START_SECONDS) / 60, 5, 0, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class ShiftEndSecondsToMargin : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new Thickness(((int)value - Soheil.Common.SoheilConstants.EDITOR_START_SECONDS) / 60 - 20, 5, 0, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class ShiftDurationToWidth : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((int)values[1] - (int)values[0]) / 60;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class ShiftBreakDurationToWidth : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((int)values[1] - (int)values[0]) / 120d;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ShiftBreakDurationToEndMargin : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new Thickness(
				(
					(int)values[0] + 
					((int)values[1] - (int)values[0]) / 2 - 
					Soheil.Common.SoheilConstants.EDITOR_START_SECONDS
				) / 60, 15, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class WorkShiftSecondsToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Soheil.Common.SoheilFunctions.GetWorkShiftTime((int)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ColorToRgbBrush : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var argb = (Color)value;
			return new SolidColorBrush(Color.FromRgb(argb.R, argb.G, argb.B));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
