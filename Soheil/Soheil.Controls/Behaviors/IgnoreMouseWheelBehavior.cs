using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows.Input;

namespace Soheil.Controls.Behaviors
{
	/// <summary>
	/// Captures and eats MouseWheel events so that a nested ListBox does not
	/// prevent an outer scrollable control from scrolling.
	/// </summary>
	public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
	{

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
			base.OnDetaching();
		}


		public static UIElement GetScrollTarget(DependencyObject obj)
		{
			return (UIElement)obj.GetValue(ScrollTargetProperty);
		}

		public static void SetScrollTarget(DependencyObject obj, UIElement value)
		{
			obj.SetValue(ScrollTargetProperty, value);
		}

		// Using a DependencyProperty as the backing store for ScrollTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ScrollTargetProperty =
			DependencyProperty.RegisterAttached("ScrollTarget", typeof(UIElement), typeof(IgnoreMouseWheelBehavior), new PropertyMetadata(null));



		void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{

			e.Handled = true;

			var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
			//e2.
			e2.RoutedEvent = UIElement.MouseWheelEvent;

			AssociatedObject.RaiseEvent(e2);

		}

	}
}
