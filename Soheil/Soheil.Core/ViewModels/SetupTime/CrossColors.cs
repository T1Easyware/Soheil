using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class CrossColors : DependencyObject
	{
		public CrossColors(SolidColorBrush rowColor, SolidColorBrush columnColor)
		{
			if (rowColor != null) HorColor = rowColor;
			if (columnColor != null) VertColor = columnColor;
		}
		//HorColor Dependency Property
		public SolidColorBrush HorColor
		{
			get { return (SolidColorBrush)GetValue(HorColorProperty); }
			set { SetValue(HorColorProperty, value); }
		}
		public static readonly DependencyProperty HorColorProperty =
			DependencyProperty.Register("HorColor", typeof(SolidColorBrush), typeof(CrossColors), 
			new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));
		//VertColor Dependency Property
		public SolidColorBrush VertColor
		{
			get { return (SolidColorBrush)GetValue(VertColorProperty); }
			set { SetValue(VertColorProperty, value); }
		}
		public static readonly DependencyProperty VertColorProperty =
			DependencyProperty.Register("VertColor", typeof(SolidColorBrush), typeof(CrossColors),
			new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));
	}
}
