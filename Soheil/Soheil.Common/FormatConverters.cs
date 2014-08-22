using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace Soheil.Common
{
	public static class Format
	{
		/// <summary>
		/// Safely cuts a number so that at most 2 digits exist after the dot
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SafeCut2(string value)
		{
			char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			if (!value.Contains(sep)) return value;
			string temp = "";
			int counter = 0;
			for (int i = 0; i < value.Length; i++)
			{
				temp += value[i];
				if (counter > 0 || value[i] == sep) counter++;
				if (counter == 3) break;
			}
			return temp;
		}
		public static string SafeCut2(float value)
		{
			return SafeCut2(value.ToString());
		}
		public static string SafeCut2(int value)
		{
			return value.ToString();
		}
		/// <summary>
		/// Safely cuts a number so that at most 1 digit exist after the dot
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SafeCut1(string value)
		{
			char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			if (!value.Contains(sep)) return value;
			string temp = "";
			int counter = 0;
			for (int i = 0; i < value.Length; i++)
			{
				temp += value[i];
				if (counter > 0 || value[i] == sep) counter++;
				if (counter == 2) break;
			}
			return temp;
		}
		public static string SafeRound1(float value)
		{
			return (Math.Round(value * 10) / 10).ToString();
		}
		public static string SafeRound2(float value)
		{
			return (Math.Round(value * 100) / 100).ToString();
		}
		public static string SafeCut1(float value)
		{
			return SafeCut1(value.ToString());
		}
		public static string SafeCut1(int value)
		{
			return value.ToString();
		}
		/// <summary>
		/// Safely cuts a number so that no digits exist after the dot
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SafeCut0(string value)
		{
			char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			if (!value.Contains(sep)) return value;
			string temp = "";
			for (int i = 0; i < value.Length; i++)
			{
				temp += value[i];
				if (value[i] == sep) break;
			}
			return temp;
		}
		public static string SafeCut0(float value)
		{
			return ((int)value).ToString();
		}
		public static string SafeCut0(int value)
		{
			return value.ToString();
		}
        public static string ConvertToHMS(int seconds)
        {
            var time = new TimeSpan(0, 0, seconds);
            return string.Format("{0:00}:{1:00}:{2:00}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }
        public static string ConvertToHM(int seconds)
        {
            var time = new TimeSpan(0, 0, seconds);
            return string.Format("{0:00}:{1:00}", (int)time.TotalHours, time.Seconds > 30 ? time.Minutes + 1 : time.Minutes);
        }
        public static string ConvertToHours(int seconds)
        {
            var time = new TimeSpan(0, 0, seconds);
            return string.Format("{0:0}", (int)time.TotalHours);
        }
	}
	public abstract class BaseConverter : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
	#region Cut
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut0StringConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = value as string;
			return (string.IsNullOrEmpty(val)) ? "0" : Format.SafeCut0(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut0FloatConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			float val = (float)value;
			return Format.SafeCut0(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut0IntConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int val = (int)value;
			return Format.SafeCut0(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut1StringConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = value as string;
			return (string.IsNullOrEmpty(val)) ? "0" : Format.SafeCut1(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut1FloatConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			float val = (float)value;
			return Format.SafeCut1(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut1IntConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int val = (int)value;
			return Format.SafeCut1(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut2StringConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = value as string;
			return (string.IsNullOrEmpty(val)) ? "0" : Format.SafeCut2(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut2FloatConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			float val = (float)value;
			return Format.SafeCut2(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeCut2IntConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int val = (int)value;
			return Format.SafeCut2(val);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	#endregion
	[ValueConversion(typeof(string), typeof(string))]
	public class SafeRound1FloatConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Format.SafeRound1(System.Convert.ToSingle(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class MinuteSecondConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			long val = (long)(((float)value) * (long)10000000);
			TimeSpan ts = new TimeSpan(val);
			return string.Format("{0:D2}:{1:D2}", ts.Hours * 60 + ts.Minutes, ts.Seconds);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string ts = (string)value;
			if (string.IsNullOrEmpty(ts)) return 0f;
			var part1 = ts.Split(':');
			float ret = 0;
			float temp;
			ret += (float.TryParse(part1[0], out temp)) ? temp * 60 : 0;
			if (part1.Length == 2)
			{
				ret += (float.TryParse(part1[1], out temp)) ? temp : 0;
			}
			return ret;
		}
	}
	[ValueConversion(typeof(string), typeof(string))]
	public class InvertBoolConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !((bool)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !((bool)value);
		}
	}
}
