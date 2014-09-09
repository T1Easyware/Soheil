using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Soheil.Controls.CustomControls
{
	public class GridHelper
	{
		#region RowCount Property

		/// <summary>
		/// Adds the specified number of Rows to RowDefinitions. 
		/// Default Height is Auto
		/// </summary>
		public static readonly DependencyProperty RowCountProperty =
			DependencyProperty.RegisterAttached(
				"RowCount", typeof(int), typeof(GridHelper),
				new PropertyMetadata(-1, RowCountChanged));

		// Get
		public static int GetRowCount(DependencyObject obj)
		{
			return (int)obj.GetValue(RowCountProperty);
		}

		// Set
		public static void SetRowCount(DependencyObject obj, int value)
		{
			obj.SetValue(RowCountProperty, value);
		}

		// Change Event - Adds the Rows
		public static void RowCountChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || (int)e.NewValue < 0)
				return;

			Grid grid = (Grid)obj;
			grid.RowDefinitions.Clear();

			for (int i = 0; i < (int)e.NewValue; i++)
				grid.RowDefinitions.Add(
					new RowDefinition() { Height = GridLength.Auto });

			SetStarRows(grid);
		}

		#endregion

		#region ColumnCount Property

		/// <summary>
		/// Adds the specified number of Columns to ColumnDefinitions. 
		/// Default Width is Auto
		/// </summary>
		public static readonly DependencyProperty ColumnCountProperty =
			DependencyProperty.RegisterAttached(
				"ColumnCount", typeof(int), typeof(GridHelper),
				new PropertyMetadata(-1, ColumnCountChanged));

		// Get
		public static int GetColumnCount(DependencyObject obj)
		{
			return (int)obj.GetValue(ColumnCountProperty);
		}

		// Set
		public static void SetColumnCount(DependencyObject obj, int value)
		{
			obj.SetValue(ColumnCountProperty, value);
		}

		// Change Event - Add the Columns
		public static void ColumnCountChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || (int)e.NewValue < 0)
				return;

			Grid grid = (Grid)obj;
			grid.ColumnDefinitions.Clear();

			for (int i = 0; i < (int)e.NewValue; i++)
				grid.ColumnDefinitions.Add(
					new ColumnDefinition() { Width = GridLength.Auto });

			SetStarColumns(grid);
		}

		#endregion


		#region StarRows Property

		/// <summary>
		/// Makes the specified Row's Height equal to Star. 
		/// Can set on multiple Rows
		/// </summary>
		public static readonly DependencyProperty StarRowsProperty =
			DependencyProperty.RegisterAttached(
				"StarRows", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, StarRowsChanged));

		// Get
		public static string GetStarRows(DependencyObject obj)
		{
			return (string)obj.GetValue(StarRowsProperty);
		}

		// Set
		public static void SetStarRows(DependencyObject obj, string value)
		{
			obj.SetValue(StarRowsProperty, value);
		}

		// Change Event - Makes specified Row's Height equal to Star
		public static void StarRowsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetStarRows((Grid)obj);
		}

		private static void SetStarRows(Grid grid)
		{
			string[] starRows = GetStarRows(grid).Split(',');
			for (int i = 0; i < grid.RowDefinitions.Count; i++)
				if (starRows.Contains(i.ToString()))
					grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
		}
		#endregion

		#region StarColumns Property

		/// <summary>
		/// Makes the specified Column's Width equal to Star. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty StarColumnsProperty =
			DependencyProperty.RegisterAttached(
				"StarColumns", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, StarColumnsChanged));

		// Get
		public static string GetStarColumns(DependencyObject obj)
		{
			return (string)obj.GetValue(StarColumnsProperty);
		}

		// Set
		public static void SetStarColumns(DependencyObject obj, string value)
		{
			obj.SetValue(StarColumnsProperty, value);
		}

		// Change Event - Makes specified Column's Width equal to Star
		public static void StarColumnsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetStarColumns((Grid)obj);
		}

		private static void SetStarColumns(Grid grid)
		{
			var tmp = GetStarColumns(grid);
			if (tmp == "*")
			{
				for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
					grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
			}
			else
			{
				string[] starColumns = tmp.Split(',');
				for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
					if (starColumns.Contains(i.ToString()))
						grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
			}
		}
		#endregion


		#region AutoRows Property

		/// <summary>
		/// Makes the specified Row's Height equal to Auto. 
		/// Can set on multiple Rows
		/// </summary>
		public static readonly DependencyProperty AutoRowsProperty =
			DependencyProperty.RegisterAttached(
				"AutoRows", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, AutoRowsChanged));

		// Get
		public static string GetAutoRows(DependencyObject obj)
		{
			return (string)obj.GetValue(AutoRowsProperty);
		}

		// Set
		public static void SetAutoRows(DependencyObject obj, string value)
		{
			obj.SetValue(AutoRowsProperty, value);
		}

		// Change Event - Makes specified Row's Height equal to Auto
		public static void AutoRowsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetAutoRows((Grid)obj);
		}

		private static void SetAutoRows(Grid grid)
		{
			string[] AutoRows = GetAutoRows(grid).Split(',');
			for (int i = 0; i < grid.RowDefinitions.Count; i++)
				if (AutoRows.Contains(i.ToString()))
					grid.RowDefinitions[i].Height = GridLength.Auto;
		}
		#endregion

		#region AutoColumns Property

		/// <summary>
		/// Makes the specified Column's Width equal to Auto. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty AutoColumnsProperty =
			DependencyProperty.RegisterAttached(
				"AutoColumns", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, AutoColumnsChanged));

		// Get
		public static string GetAutoColumns(DependencyObject obj)
		{
			return (string)obj.GetValue(AutoColumnsProperty);
		}

		// Set
		public static void SetAutoColumns(DependencyObject obj, string value)
		{
			obj.SetValue(AutoColumnsProperty, value);
		}

		// Change Event - Makes specified Column's Width equal to Auto
		public static void AutoColumnsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetAutoColumns((Grid)obj);
		}

		private static void SetAutoColumns(Grid grid)
		{
			string[] AutoColumns = GetAutoColumns(grid).Split(',');
			for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
				if (AutoColumns.Contains(i.ToString()))
					grid.ColumnDefinitions[i].Width = GridLength.Auto;
		}
		#endregion


		#region AllRowsHeight Property

		/// <summary>
		/// Makes the specified Row's Height equal to Fixed. 
		/// Can set on multiple Rows
		/// </summary>
		public static readonly DependencyProperty AllRowsHeightProperty =
			DependencyProperty.RegisterAttached(
				"AllRowsHeight", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, AllRowsHeightChanged));

		// Get
		public static string GetAllRowsHeight(DependencyObject obj)
		{
			return (string)obj.GetValue(AllRowsHeightProperty);
		}

		// Set
		public static void SetAllRowsHeight(DependencyObject obj, string value)
		{
			obj.SetValue(AllRowsHeightProperty, value);
		}

		// Change Event - Makes specified Row's Height equal to Fixed
		public static void AllRowsHeightChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetAllRowsHeight((Grid)obj);
		}

		private static void SetAllRowsHeight(Grid grid)
		{
			double AllRowsHeight = Convert.ToDouble(GetAllRowsHeight(grid));
			int count = GetRowCount(grid);
			for (int i = 0; i < count; i++)
			{
				grid.RowDefinitions[i].Height = new GridLength(AllRowsHeight, GridUnitType.Pixel);
			}
		}
		#endregion

		#region AllColumnsWidth Property

		/// <summary>
		/// Makes the specified Column's Width equal to Fixed. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty AllColumnsWidthProperty =
			DependencyProperty.RegisterAttached(
				"AllColumnsWidth", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, AllColumnsWidthChanged));

		// Get
		public static string GetAllColumnsWidth(DependencyObject obj)
		{
			return (string)obj.GetValue(AllColumnsWidthProperty);
		}

		// Set
		public static void SetAllColumnsWidth(DependencyObject obj, string value)
		{
			obj.SetValue(AllColumnsWidthProperty, value);
		}

		// Change Event - Makes specified Column's Width equal to Fixed
		public static void AllColumnsWidthChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetAllColumnsWidth((Grid)obj);
		}

		private static void SetAllColumnsWidth(Grid grid)
		{
			double AllColumnsWidth = Convert.ToDouble(GetAllColumnsWidth(grid));
			int count = GetColumnCount(grid);
			for (int i = 0; i < count; i++)
			{
				grid.ColumnDefinitions[i].Width = new GridLength(AllColumnsWidth, GridUnitType.Pixel);
			}
		}
		#endregion


		#region FixedRows Property

		/// <summary>
		/// Makes the specified Row's Height equal to Fixed. 
		/// Can set on multiple Rows
		/// </summary>
		public static readonly DependencyProperty FixedRowsProperty =
			DependencyProperty.RegisterAttached(
				"FixedRows", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, FixedRowsChanged));

		// Get
		public static string GetFixedRows(DependencyObject obj)
		{
			return (string)obj.GetValue(FixedRowsProperty);
		}

		// Set
		public static void SetFixedRows(DependencyObject obj, string value)
		{
			obj.SetValue(FixedRowsProperty, value);
		}

		// Change Event - Makes specified Row's Height equal to Fixed
		public static void FixedRowsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetFixedRows((Grid)obj);
		}

		private static void SetFixedRows(Grid grid)
		{
			string[] FixedRows = GetFixedRows(grid).Split(',');
			for (int i = 0; i < FixedRows.Length / 2; i++)
			{
				int idx = Convert.ToInt32(FixedRows[i * 2]);
				double val = Convert.ToDouble(FixedRows[i * 2 + 1]);
				grid.RowDefinitions[idx].Height = new GridLength(val, GridUnitType.Pixel);
			}
		}
		#endregion

		#region FixedColumns Property

		/// <summary>
		/// Makes the specified Column's Width equal to Fixed. 
		/// Can set on multiple Columns
		/// </summary>
		public static readonly DependencyProperty FixedColumnsProperty =
			DependencyProperty.RegisterAttached(
				"FixedColumns", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, FixedColumnsChanged));

		// Get
		public static string GetFixedColumns(DependencyObject obj)
		{
			return (string)obj.GetValue(FixedColumnsProperty);
		}

		// Set
		public static void SetFixedColumns(DependencyObject obj, string value)
		{
			obj.SetValue(FixedColumnsProperty, value);
		}

		// Change Event - Makes specified Column's Width equal to Fixed
		public static void FixedColumnsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetFixedColumns((Grid)obj);
		}

		private static void SetFixedColumns(Grid grid)
		{
			string[] FixedColumns = GetFixedColumns(grid).Split(',');
			bool isAll = (FixedColumns[0] == "*");
			for (int i = 0; i < FixedColumns.Length / 2; i++)
			{
				int idx = i;
				if (!isAll) idx = Convert.ToInt32(FixedColumns[i * 2]);
				grid.ColumnDefinitions[idx].Width =
					new GridLength(Convert.ToDouble(FixedColumns[i * 2 + 1]), GridUnitType.Pixel);
			}
		}
		#endregion


		#region RelativeStarColumnWidths Property

		/// <summary>
		/// Makes all Column's Width equal to N[i]*. 
		/// Have to be set on all Columns
		/// </summary>
		public static readonly DependencyProperty RelativeStarColumnWidthsProperty =
			DependencyProperty.RegisterAttached(
				"RelativeStarColumnWidths", typeof(string), typeof(GridHelper),
				new PropertyMetadata(string.Empty, RelativeStarColumnWidthsChanged));

		// Get
		public static string GetRelativeStarColumnWidths(DependencyObject obj)
		{
			return (string)obj.GetValue(RelativeStarColumnWidthsProperty);
		}

		// Set
		public static void SetRelativeStarColumnWidths(DependencyObject obj, string value)
		{
			obj.SetValue(RelativeStarColumnWidthsProperty, value);
		}

		// Change Event - Makes all Column's Width equal to N[i]*
		public static void RelativeStarColumnWidthsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
				return;

			SetRelativeStarColumnWidths((Grid)obj);
		}

		private static void SetRelativeStarColumnWidths(Grid grid)
		{
			string[] widths = GetRelativeStarColumnWidths(grid).Split(',');
			for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
				if (i < widths.Length)
					if (!string.IsNullOrEmpty(widths[i]))
						grid.ColumnDefinitions[i].Width = new GridLength(
							Convert.ToDouble(widths[i]), GridUnitType.Star);
		}
		#endregion
	}
}