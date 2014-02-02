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
using Soheil.Core.ViewModels.SetupTime;

namespace Soheil.Views.SetupTime
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
				"AllColumns", typeof(IEnumerable<ProductGroup>), typeof(GridHelper),
				new PropertyMetadata(null, AllColumnsChanged));

		// Get
		public static IEnumerable<ProductGroup> GetAllColumns(DependencyObject obj)
		{
			return (IEnumerable<ProductGroup>)obj.GetValue(AllColumnsProperty);
		}

		// Set
		public static void SetAllColumns(DependencyObject obj, IEnumerable<ProductGroup> value)
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

			var pgList = GetAllColumns(grid);
			foreach (var productgroup in pgList)
			{
				var cd = new ColumnDefinition();
				cd.Width = new GridLength(ProductGroup.HeaderColumnWidth, GridUnitType.Pixel);
				grid.ColumnDefinitions.Add(cd);
				foreach (var product in productgroup.Products)
				{
					foreach (var rework in product.Reworks)
					{
						cd = new ColumnDefinition();
						cd.DataContext = rework;

						var style = new Style(typeof(ColumnDefinition));
						style.Setters.Add(new Setter(
							ColumnDefinition.WidthProperty,
							new GridLength(ProductGroup.ColumnWidth, GridUnitType.Pixel)));

						#region open animation
						var animation = new GridLengthAnimation();
						animation.From = new GridLength(0, GridUnitType.Pixel);
						animation.To = new GridLength(ProductGroup.ColumnWidth, GridUnitType.Pixel);
						animation.Duration = TimeSpan.FromSeconds(0.2);
						Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));

						var storyboard = new Storyboard();
						storyboard.Children.Add(animation);
						#endregion

						#region close animation
						var animation2 = new GridLengthAnimation();
						animation2.From = new GridLength(ProductGroup.ColumnWidth, GridUnitType.Pixel);
						animation2.To = new GridLength(0, GridUnitType.Pixel);
						animation2.Duration = TimeSpan.FromSeconds(0.2);
						Storyboard.SetTargetProperty(animation2, new PropertyPath("Width"));

						var storyboard2 = new Storyboard();
						storyboard2.Children.Add(animation2);
						#endregion

						if (rework.IsRework)
						{
							var multitrigger = new MultiDataTrigger();
							multitrigger.Conditions.Add(new Condition(new Binding("Product.IsExpanded"), true));
							multitrigger.Conditions.Add(new Condition(new Binding("Product.ProductGroup.IsExpanded"), true));
							multitrigger.EnterActions.Add(new BeginStoryboard { Storyboard = storyboard });
							multitrigger.ExitActions.Add(new BeginStoryboard { Storyboard = storyboard2 });
							style.Triggers.Add(multitrigger);
						}
						else
						{
							var trigger = new DataTrigger();
							trigger.Binding = new Binding("Product.ProductGroup.IsExpanded");
							trigger.Value = true;
							trigger.EnterActions.Add(new BeginStoryboard { Storyboard = storyboard });
							trigger.ExitActions.Add(new BeginStoryboard { Storyboard = storyboard2 });
							style.Triggers.Add(trigger);
						}

						cd.Style = style;
						grid.ColumnDefinitions.Add(cd);
					}

				}
			}
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}
		#endregion

		#region AllRows Property

		/// <summary>
		/// Creates some Rows and binds their Height (Fixed) to the specified numbers
		/// Set for all Rows
		/// </summary>
		public static readonly DependencyProperty AllRowsProperty =
			DependencyProperty.RegisterAttached(
				"AllRows", typeof(IEnumerable<ProductGroup>), typeof(GridHelper),
				new PropertyMetadata(null, AllRowsChanged));

		// Get
		public static IEnumerable<ProductGroup> GetAllRows(DependencyObject obj)
		{
			return (IEnumerable<ProductGroup>)obj.GetValue(AllRowsProperty);
		}

		// Set
		public static void SetAllRows(DependencyObject obj, IEnumerable<ProductGroup> value)
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

			var pgList = GetAllRows(grid);
			foreach (var productgroup in pgList)
			{
				var rd = new RowDefinition();
				rd.Height = new GridLength(ProductGroup.HeaderRowHeight, GridUnitType.Pixel);
				grid.RowDefinitions.Add(rd);
				foreach (var product in productgroup.Products)
				{
					foreach (var rework in product.Reworks)
					{
						rd = new RowDefinition();
						rd.DataContext = rework;

						var style = new Style(typeof(RowDefinition));
						style.Setters.Add(new Setter(
							RowDefinition.HeightProperty,
							new GridLength(ProductGroup.RowHeight, GridUnitType.Pixel)));

						#region open
						var animation = new GridLengthAnimation();
						animation.From = new GridLength(0, GridUnitType.Pixel);
						animation.To = new GridLength(ProductGroup.RowHeight, GridUnitType.Pixel);
						animation.Duration = TimeSpan.FromSeconds(0.2);
						Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));

						var storyboard = new Storyboard();
						storyboard.Children.Add(animation);
						#endregion

						#region close
						var animation2 = new GridLengthAnimation();
						animation2.From = new GridLength(ProductGroup.RowHeight, GridUnitType.Pixel);
						animation2.To = new GridLength(0, GridUnitType.Pixel);
						animation2.Duration = TimeSpan.FromSeconds(0.2);
						Storyboard.SetTargetProperty(animation2, new PropertyPath("Height"));

						var storyboard2 = new Storyboard();
						storyboard2.Children.Add(animation2);
						
						#endregion

						if (rework.IsRework)
						{
							var multitrigger = new MultiDataTrigger();
							multitrigger.Conditions.Add(new Condition(new Binding("Product.IsExpanded"), true));
							multitrigger.Conditions.Add(new Condition(new Binding("Product.ProductGroup.IsExpanded"), true));
							multitrigger.EnterActions.Add(new BeginStoryboard { Storyboard = storyboard });
							multitrigger.ExitActions.Add(new BeginStoryboard { Storyboard = storyboard2 });
							style.Triggers.Add(multitrigger);
						}
						else
						{
							var trigger = new DataTrigger();
							trigger.Binding = new Binding("Product.ProductGroup.IsExpanded");
							trigger.Value = true;
							trigger.EnterActions.Add(new BeginStoryboard { Storyboard = storyboard });
							trigger.ExitActions.Add(new BeginStoryboard { Storyboard = storyboard2 });
							style.Triggers.Add(trigger);
						}

						rd.Style = style;
						grid.RowDefinitions.Add(rd);
					}
				} 
			}
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		}
		#endregion

	}
}
