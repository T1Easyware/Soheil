using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Controls.Converters.PP
{
	#region Colors and Visual

	public class IsEqualToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == values[1]) return ((Brush[])parameter)[0];
			else return ((Brush[])parameter)[1];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class BooleanToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? ((Brush[])parameter)[0] : ((Brush[])parameter)[1];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class TaskProgressColorConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double val = (double)values[0];
			double max = (double)values[1];
			if (val == 0) return null;
			if (val >= max) return new SolidColorBrush(Colors.LimeGreen);
			if ((int)parameter == 0)//Task
				return new SolidColorBrush(Colors.Gold);
			else//Report
				return new SolidColorBrush(Color.FromRgb(255, 162, 0));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ColorFixer : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new SolidColorBrush((Color)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class HighContrastForecolor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return Colors.White;
			return ((Color)value).IsDark() ? Colors.White : Colors.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class OpaqueIfPositiveInt : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value > 0) ? 1 : 0.5;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class HasErrorToShadowColor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new SolidColorBrush((bool)value ? Colors.Red : Colors.Black);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region Visibility
	public class BooleanToInvisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class IsEqualToInvisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (parameter == value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class IsEqualToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (parameter == value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	public class IsEqualToInvisibilityConverter2 : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return (values[0] == values[1]) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class IsEqualToVisibilityConverter2 : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return (values[0] == values[1]) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	public class IsNotNullToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class IsNullToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value == null) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class LogicalAndToVisibility : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Any(x => x == DependencyProperty.UnsetValue)) return Visibility.Collapsed;
			return values.All(x => (bool)x) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region PP coordinations
	public class PortionToStarConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new GridLength(System.Convert.ToDouble(value), GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/*public class ProducedG1VisualWidth : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			int ProcessTargetPoint = (int)values[0];
			int ProducedG1 = (int)values[1];
			int StoppageCount = (int)values[2];
			int DefectionCount = (int)values[3];
			int sumOfReports = ProducedG1 + DefectionCount + StoppageCount;
			if (sumOfReports > ProcessTargetPoint)
			{
				Excess = sumOfReports - ProcessTargetPoint;
				return ProcessTargetPoint - DefectionCount - StoppageCount;
			}
			return ProducedG1;
		}
																					<!--ColumnDefinition>
																					<ColumnDefinition.Width>
																						<MultiBinding Converter="{StaticResource ProducedG1VisualWidth}">
																							<Binding Path="ProcessTargetPoint"/>
																							<Binding Path="ProducedG1"/>
																							<Binding Path="StoppageCount"/>
																							<Binding Path="DefectionCount"/>
																						</MultiBinding>
																					</ColumnDefinition.Width>
																				</ColumnDefinition-->
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}*/

	public class TaskReportMargin : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) return new Thickness(0);
			var reportStart = (DateTime)values[0];
			var taskStart = (DateTime)values[1];
			var oneHourWidth = (double)values[2];
			return new Thickness(reportStart.Subtract(taskStart).TotalHours * oneHourWidth, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class TaskReportWidth : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) return 0d;
			var reportDuration = (int)values[0];
			var oneHourWidth = (double)values[1];
			return reportDuration * oneHourWidth / 3600;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}



	public class PPTaskBorderWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) return 0d;
			var duration = (TimeSpan)values[0];
			var oneHourWidth = (double)values[1];
            return duration.TotalHours * oneHourWidth;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class ProcessBorderMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue) return new Thickness();
			var startDt = (DateTime)values[0];
			var oneHourWidth = (double)values[1];
			var rowIndex = (int)values[2];
			return new Thickness(startDt.Subtract(startDt.GetNorooz()).TotalHours * oneHourWidth, rowIndex * 42 + 1, 0, 1);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class PPTaskBorderHeightConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue) return 10;
			var oneStationHeight = (double)values[0];
			return oneStationHeight - 8;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
    public class testConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

	public class PPTaskBorderMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) return new Thickness(0, 2, 0, 2);
			var startDt = (DateTime)values[0];
			var oneHourWidth = (double)values[1];
            return new Thickness(startDt.Subtract(startDt.GetNorooz()).TotalHours * oneHourWidth, 2, 0, 2);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class StartEndToWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue) return 0d;
			var oneHourWidth = (double)values[2];
			//if (values[0].GetType() == typeof(DateTime))
				return ((DateTime)values[1]).Subtract((DateTime)values[0]).TotalSeconds * oneHourWidth / 3600;
			/*else if (values[0].GetType() == typeof(int))
				return ((int)values[1] - (int)values[0]) * oneHourWidth / 3600;
			else
				return ((double)values[1] - (double)values[0]) * oneHourWidth / 3600;*/
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class StartToMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) return 0d;
			var oneHourWidth = (double)values[1];
			return new Thickness(
				(
					(((DateTime)values[0]).GetPersianDayOfYear() * 24)
					+ 
					((DateTime)values[0]).TimeOfDay.TotalHours 
					- 
					24
				) * oneHourWidth, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BalloonVerticalMargin : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue) return new Thickness(0, 2, 0, 2);
			int rowIndex = System.Convert.ToInt32(values[0]);
			var startDt = (DateTime)values[1];
			var oneHourWidth = (double)values[2];
			return new Thickness(startDt.Subtract(startDt.GetNorooz()).TotalHours * oneHourWidth, rowIndex * 42, 0, 2);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class HideIfSmallerThan20 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return Visibility.Collapsed;
			return (double)value < 20 ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class HideIfSmallerThan40 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return Visibility.Collapsed;
			return (double)value < 40 ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}



	#endregion

	#region Operators
	public class OperatorRoleTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (PersianOperatorRole)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class OperatorRoleIsValue : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(parameter is string))
				return DependencyProperty.UnsetValue;

			int role = System.Convert.ToInt32(parameter);
			return ((OperatorRole)role == (OperatorRole)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(parameter is string))
				return DependencyProperty.UnsetValue;

			return (OperatorRole)System.Convert.ToInt32(parameter);
		}
	}
	public class OperatorFilter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
				return Visibility.Collapsed;
			string query = (string)values[0];
			string code = (string)values[1];
			string name = (string)values[2];
			return (string.IsNullOrWhiteSpace(query) || name.Contains(query) || code.StartsWith(query)) ?
				Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}



	/// <summary>
	/// First binding is Code (match if starts with query)
	/// Second binding is Name (match if contains query)
	/// Third binding is query (match if empty or whitespace)
	/// </summary>
	public class FilterMaker : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var code = ((string)values[0]).ToUpper();
			var name = ((string)values[1]).ToUpper();
			var filter = ((string)values[2]).ToUpper();
			return string.IsNullOrWhiteSpace(filter) || (code.StartsWith(filter)) || ((name != null) && (name.Contains(filter)))
				? Visibility.Visible : Visibility.Hidden;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region PPTable Sliders
	//a.b/c/24
	public class ZoomMathDayLittleWindowWidth : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
				return 0d;
			var a = System.Convert.ToDouble(values[0]);
			var b = System.Convert.ToDouble(values[1]);
			var c = System.Convert.ToDouble(values[2]);
			if (c == 0) return 0d;
			return a * b / (c * 24);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	//a*b/24
	public class ZoomMathDayLittleWindowMargin : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
				return 0d;
			var a = System.Convert.ToDouble(values[0]);
			var b = System.Convert.ToDouble(values[1]);
			return new Thickness(a * b / 24, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
	public class HoursPanelMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[2] == DependencyProperty.UnsetValue) return new Thickness(0);
			var hoursPassed = (double)values[0];
			var hourZoom = (double)values[1];
			var daysFromStartOfYear = (int)values[2];
			return new Thickness(-(hoursPassed + daysFromStartOfYear * 24) * hourZoom, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class MainPanelMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[2] == DependencyProperty.UnsetValue) return new Thickness(0);
			var hoursPassed = (double)values[0];
			var hourZoom = (double)values[1];
			var daysFromStartOfYear = (int)values[2];
			var verticalOffset = (double)values[3];
			return new Thickness(-(hoursPassed + daysFromStartOfYear * 24) * hourZoom, verticalOffset, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region General
	public class IsEqual : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (parameter == value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return parameter;
			return null;
		}
	}
	public class IsEqual2 : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values[0] == values[1];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class Inverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}

	public class GridColumnToMargin : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue ||
				values[1] == DependencyProperty.UnsetValue) return new Thickness(0);
			return new Thickness((int)values[0] * (double)values[1], 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IsNotNull : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class IsNull : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value == null);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class LogicalAnd : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values.All(x => (bool)x);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion
	
	#region Math

	//a.b/c
	public class ACrossBDividebByC : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
				return 0d;
			var a = System.Convert.ToDouble(values[0]);
			var b = System.Convert.ToDouble(values[1]);
			var c = System.Convert.ToDouble(values[2]);
			if (c == 0) c = 1;
			var val = a * b / c;
			return val;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class FloatMultiplier2 : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue ||
				values[1] == DependencyProperty.UnsetValue ||
				values[0] == null || values[1] == null) return 0f;
			return (System.Convert.ToSingle(values[0]) * System.Convert.ToSingle(values[1])).ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	//has a minus
	public class DoubleMultiplierToThickness : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue ||
				values[1] == DependencyProperty.UnsetValue ||
				values[0] == null || values[1] == null) return 0d;
			return new Thickness(-System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[1]), 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class IntX7Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value * 7d;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SumOfThree : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue && values[2] != DependencyProperty.UnsetValue && values[3] != DependencyProperty.UnsetValue)
			{
				return (double)values[0] + (double)values[1] + (double)values[2] + (double)values[3];
			}
			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class FloatMultiplier : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return 0f;
			float val1;
			float val2;
			if ((float.TryParse(value.ToString(), out val1))
				&& (float.TryParse(parameter.ToString(), out val2)))
				return (val1 * val2);
			else return 0f;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class DoubleMultiplier2 : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] == DependencyProperty.UnsetValue ||
				values[1] == DependencyProperty.UnsetValue ||
				values[0] == null || values[1] == null) return 0d;
			return System.Convert.ToDouble(values[0]) * System.Convert.ToDouble(values[1]);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LogarithmicSlider : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Sqrt(System.Convert.ToDouble(value) / 20d);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return 20 * Math.Pow(System.Convert.ToDouble(value), 2);
		}
	}
	#endregion

	#region DateTime...
	public class TimeSpanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var ts = (TimeSpan)value;
			return ts.ToString("hh\\ \\:\\ mm\\ \\:\\ ss", culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var tmp = ((string)value).Split(':');
				switch (tmp.Length)
				{
					case 1:
						return new TimeSpan(
						0,
						0,
						System.Convert.ToInt32(tmp[1]));
					case 2:
						return new TimeSpan(
						0,
						System.Convert.ToInt32(tmp[0]),
						System.Convert.ToInt32(tmp[1]));
					case 3:
						return new TimeSpan(
							System.Convert.ToInt32(tmp[0]),
							System.Convert.ToInt32(tmp[1]),
							System.Convert.ToInt32(tmp[2]));
					default:
						return null;
				}
			}
			catch { return null; }
		}
	}
	public class FullTimeSpanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var ts = (TimeSpan)value;

			return string.Format(culture, "{0:D2} : {1:D2} : {2:D2}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var tmp = ((string)value).Split(':');
				switch (tmp.Length)
				{
					case 1:
						return new TimeSpan(
						0,
						0,
						System.Convert.ToInt32(tmp[1]));
					case 2:
						return new TimeSpan(
						0,
						System.Convert.ToInt32(tmp[0]),
						System.Convert.ToInt32(tmp[1]));
					case 3:
						return new TimeSpan(
							System.Convert.ToInt32(tmp[0]),
							System.Convert.ToInt32(tmp[1]),
							System.Convert.ToInt32(tmp[2]));
					default:
						return null;
				}
			}
			catch { return null; }
		}
	}
	public class SecondsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var ts = new TimeSpan(0, 0, System.Convert.ToInt32(value));
			return string.Format(culture, "{0:D2} : {1:D2} : {2:D2}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var tmp = ((string)value).Split(':');
				switch (tmp.Length)
				{
					case 1:
						return System.Convert.ToInt32(tmp[0].Trim());
					case 2:
						return System.Convert.ToInt32(tmp[0].Trim()) * 60 + System.Convert.ToInt32(tmp[1].Trim());
					case 3:
						return System.Convert.ToInt32(tmp[0].Trim()) * 3600 + System.Convert.ToInt32(tmp[1].Trim()) * 60 + System.Convert.ToInt32(tmp[2].Trim());
					default:
						return null;
				}
			}
			catch { return null; }
		}
	}
	public class CycleTimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var ts = new TimeSpan(0, 0, (int)((float)value));
			if (ts.Hours == 0) return ts.ToString("mm\\ \\:\\ ss", culture);
			return ts.ToString("hh\\ \\:\\ mm\\ \\:\\ ss", culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var tmp = ((string)value).Split(':');
				switch (tmp.Length)
				{
					case 1:
						return System.Convert.ToInt32(tmp[0].Trim());
					case 2:
						return System.Convert.ToInt32(tmp[0].Trim()) * 60 + System.Convert.ToInt32(tmp[1].Trim());
					case 3:
						return System.Convert.ToInt32(tmp[0].Trim()) * 3600 + System.Convert.ToInt32(tmp[1].Trim()) * 60 + System.Convert.ToInt32(tmp[2].Trim());
					default:
						return null;
				}
			}
			catch { return null; }
		}
	}
	public class DateToLocalStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var dt = (DateTime)value;
			return dt.ToPersianDateString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return DateTime.Now;
			if (value == null) return DateTime.Now;
			return ((string)value).ToPersianDate();
		}
	}

	public class DateToTimeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue) return "";
			var dt = (DateTime)value;
			return dt.ToShortTimeString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}


	[ValueConversion(typeof(DateTime), typeof(Arash.PersianDate))]
	public class DateToPersianDateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return null;
			DateTime date = (DateTime)value;
			return new Arash.PersianDate(date);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) return null;
			Arash.PersianDate pDate = (Arash.PersianDate)value;
			return pDate.ToDateTime();
		}
	}

	public class DateTimeToCompactConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;
			if (value == DependencyProperty.UnsetValue) return null;
			var dt = (DateTime)value;
			return dt.ToPersianCompactDateTimeString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	#endregion

}
