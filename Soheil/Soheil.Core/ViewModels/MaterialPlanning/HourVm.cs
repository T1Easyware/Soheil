using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class HourVm : DependencyObject
	{
		public DateTime Data { get; set; }

		/// <summary>
		/// Gets or sets a bindable value that indicates Index
		/// </summary>
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(HourVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates Text
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(HourVm), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Color of shift
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(HourVm), new PropertyMetadata(Colors.Transparent));

	}
}
