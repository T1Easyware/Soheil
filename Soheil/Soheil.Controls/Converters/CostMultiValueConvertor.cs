using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Soheil.Controls.Converters
{
    public class CostMultiValueConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 5)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue
                    ||values[2] == DependencyProperty.UnsetValue ||values[3] == DependencyProperty.UnsetValue )
                {
                    return "0";
                }
                var quantity = (int)values[0];
                var originalQuantity = (int)values[1];
                var selectedQuantity = (int)values[2];
                var totalCost = (double)values[3];
                var unitCost = totalCost / originalQuantity;

                if (quantity == selectedQuantity)
                {
                    return System.Convert.ToString(( totalCost - ((originalQuantity - selectedQuantity) * unitCost)));
                }
                return System.Convert.ToString( selectedQuantity * unitCost);   
            }
            return "0";
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}