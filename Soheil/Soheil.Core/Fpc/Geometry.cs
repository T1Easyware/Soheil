using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Soheil.Core.Fpc
{
	public enum Side { Top, Left, Right, Bottom }

	public class StateGeometry
	{
		public StateGeometry(object margin, object w, object h)
		{
			if (margin == DependencyProperty.UnsetValue) margin = new Thickness(0);
			if(w == DependencyProperty.UnsetValue) w = h = 0d;
			Location = new Vector(((Thickness)margin).Left, ((Thickness)margin).Top);
			Size = new Vector((double)w, (double)h);
			if (double.IsNaN(Size.X)) Size.X = 40;
			if (double.IsNaN(Size.Y)) Size.Y = 38;
		}
		public double CenterX { get { return Location.X + Size.X / 2; } }
		public double CenterY { get { return Location.Y + Size.Y / 2; } }
		/// <summary>
		/// Gets the coordinates of the connection point of state when connected to the given state
		/// </summary>
		/// <param name="state">other state</param>
		/// <returns></returns>
		public Vector CenterOf(StateGeometry state)
		{
			double dx = state.CenterX - CenterX;
			double dy = state.CenterY - CenterY;
			if (dx >= 0 && dy <= dx && dy >= -dx) return new Vector(Location.X + Size.X, CenterY);//Right
			if (dx <= 0 && dy <= -dx && dy >= dx) return new Vector(Location.X, CenterY);//Left
			if (dy >= 0 && dx <= dy && dx >= -dy) return new Vector(CenterX, Location.Y + Size.Y);//Bottom
			return new Vector(CenterX, Location.Y);//Top
		}
		public Vector Location;
		public Vector Size;
	}

	public class ConnectorLocationConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var start = new StateGeometry(values[0], values[1], values[2]);
			var end = new StateGeometry(values[3], values[4], values[5]);
			var vec = start.CenterOf(end);
			return new Thickness(vec.X, vec.Y, 0, 0);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class ConnectorAngleConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var start = new StateGeometry(values[0], values[1], values[2]);
			var end = new StateGeometry(values[3], values[4], values[5]);
			var startvec = start.CenterOf(end);
			var endvec = end.CenterOf(start);
			//dy/dx = tan(t) => t = arcTan(dy/dx)
			double t = Math.Atan2(
						Math.Abs(startvec.Y - endvec.Y),
						Math.Abs(startvec.X - endvec.X)) * 180 / Math.PI;
			if (endvec.X <= startvec.X && endvec.Y >= startvec.Y) return 180 - t;
			if (endvec.X >= startvec.X && endvec.Y <= startvec.Y) return -t;
			if (endvec.X <= startvec.X && endvec.Y <= startvec.Y) return 180 + t;
			return t;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
	public class ConnectorLengthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var start = new StateGeometry(values[0], values[1], values[2]);
			var end = new StateGeometry(values[3], values[4], values[5]);
			//get side for states
			var startvec = start.CenterOf(end);
			var endvec = end.CenterOf(start);
			return Math.Sqrt(Math.Pow(startvec.X - endvec.X, 2) + Math.Pow(startvec.Y - endvec.Y, 2));
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
