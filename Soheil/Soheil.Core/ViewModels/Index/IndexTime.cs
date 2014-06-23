using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.Index
{
	public class IndexTime : DependencyObject
	{
		/// <summary>
		/// Creates a ViewModel for an index parameter in which the duration is the given number of hours
		/// </summary>
		/// <param name="hours">Number of hours applied for the index parameter</param>
		/// <param name="total">Total number of hours for the index</param>
		public IndexTime(double hours, double total)
		{
			Hours = hours;
			Perc = 100 * hours / total;
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates total Hours of current object
		/// </summary>
		public double Hours
		{
			get { return (double)GetValue(HoursProperty); }
			set { SetValue(HoursProperty, value); }
		}
		public static readonly DependencyProperty HoursProperty =
			DependencyProperty.Register("Hours", typeof(double), typeof(IndexTime), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates Percentage of current object
		/// </summary>
		public double Perc
		{
			get { return (double)GetValue(PercProperty); }
			set { SetValue(PercProperty, value); }
		}
		public static readonly DependencyProperty PercProperty =
			DependencyProperty.Register("Perc", typeof(double), typeof(IndexTime), new PropertyMetadata(0d));

	}
}
