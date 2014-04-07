using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Soheil.Controls.CustomControls;
using Soheil.Core.ViewModels.PP;

namespace Soheil.Views.PP
{
	public class GridHelper
	{
		#region AllColumns Property

		/// <summary>
		/// Creates some columns and binds their Width (Fixed) to the specified numbers
		/// Set for all Columns
		/// </summary>
		public static readonly DependencyProperty AllColumnsProperty =
			DependencyProperty.RegisterAttached(
				"AllColumns", typeof(IEnumerable<Core.ViewModels.PP.Report.TaskReportVm>), typeof(GridHelper),
				new PropertyMetadata(null, AllColumnsChanged));

		// Get
		public static IEnumerable<Core.ViewModels.PP.Report.TaskReportVm> GetAllColumns(DependencyObject obj)
		{
			return (IEnumerable<Core.ViewModels.PP.Report.TaskReportVm>)obj.GetValue(AllColumnsProperty);
		}

		// Set
		public static void SetAllColumns(DependencyObject obj, IEnumerable<Core.ViewModels.PP.Report.TaskReportVm> value)
		{
			obj.SetValue(AllColumnsProperty, value);
		}

		// Change Event - creates and sets all Columns
		public static void AllColumnsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || e.NewValue == null)
				return;

			SetAllColumns((Grid)obj);
		}

		private static void SetAllColumns(Grid grid)
		{
			grid.ColumnDefinitions.Clear();

			var trList = GetAllColumns(grid);
			foreach (var taskReport in trList)
			{
				var cd = new ColumnDefinition();
				cd.Width = new GridLength(taskReport.DurationSeconds, GridUnitType.Star);
				grid.ColumnDefinitions.Add(cd);
			}
		}
		#endregion

		#region AllRows Property

		/// <summary>
		/// Creates some columns and binds their Width (Fixed) to the specified numbers
		/// Set for all Rows
		/// </summary>
		public static readonly DependencyProperty AllRowsProperty =
			DependencyProperty.RegisterAttached(
				"AllRows", typeof(IEnumerable<RowDefinition>), typeof(GridHelper),
				new PropertyMetadata(null, AllRowsChanged));

		// Get
		public static IEnumerable<RowDefinition> GetAllRows(DependencyObject obj)
		{
			return (IEnumerable<RowDefinition>)obj.GetValue(AllRowsProperty);
		}

		// Set
		public static void SetAllRows(DependencyObject obj, IEnumerable<PPStationVm> value)
		{
			obj.SetValue(AllRowsProperty, value);
		}

		// Change Event - creates and sets all Rows
		public static void AllRowsChanged(
			DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (!(obj is Grid) || e.NewValue == null)
				return;

			SetAllRows((Grid)obj);
		}

		private static void SetAllRows(Grid grid)
		{
			grid.RowDefinitions.Clear();

			var rdList = GetAllRows(grid);
			foreach (var rd in rdList)
			{
				grid.RowDefinitions.Add(rd);
			}
		}
		#endregion
	}
}
