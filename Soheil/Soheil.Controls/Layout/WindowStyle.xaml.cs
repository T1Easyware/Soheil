using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Soheil.Controls.Layout
{
    public partial class WindowStyle
    {
        public WindowStyle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event. This event handler is used here to facilitate
        /// dragging of the Window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;

            // Check if the control have been double clicked.
            if (e.ClickCount == 2)
            {
                // If double clicked then maximize the window.
                MaximizeWindow(sender, e);
            }
            else
            {
                // If not double clicked then just drag the window around.
                window.DragMove();
            }
        }

        /// <summary>
        /// Fires when the user clicks the Close button on the window's custom title bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;
            window.Close();
        }

        /// <summary>
        /// Fires when the user clicks the minimize button on the window's custom title bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Fires when the user clicks the maximize button on the window's custom title bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;

            // Check the current state of the window. If the window is currently maximized, return the
            // window to it's normal state when the maximize button is clicked, otherwise maximize the window.
            if (window.WindowState == WindowState.Maximized)
            {
				window.WindowState = WindowState.Normal;
            }
            else
            {
                window.Focus();
                window.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Called when the window gets resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;

            // Update window's contraints like max height and width.
            UpdateWindowConstraints(window);

            // Get window sub parts
            var icon = (Image) window.Template.FindName("IconApp", window);

            // Adjust the window icon size
            if (icon != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    icon.Height = 20;
                    icon.Width = 20;
                    icon.Margin = new Thickness(10, 5, 0, 0);
                }
                else
                {
                    icon.Height = 24;
                    icon.Width = 24;
                    icon.Margin = new Thickness(10, 3, 0, 0);
                }
            }
        }

        /// <summary>
        /// Called when a window gets loaded.
        /// We initialize resizers and update constraints.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;
            UpdateWindowConstraints(window);
        }

        /// <summary>
        /// Called when the user drags the title bar when maximized.
        /// </summary>
        private void OnBorderMouseMove(object sender, MouseEventArgs e)
        {
            var window = (Window) ((FrameworkElement) sender).TemplatedParent;

            if (window != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed && window.WindowState == WindowState.Maximized)
                {
                    var maxSize = new Size(window.ActualWidth, window.ActualHeight);
                    Size resSize = window.RestoreBounds.Size;

                    double curX = e.GetPosition(window).X;
                    double curY = e.GetPosition(window).Y;

                    double newX = curX/maxSize.Width*resSize.Width;
                    double newY = curY;

                    //window.WindowState = WindowState.Normal;???

                    window.Left = curX - newX;
                    window.Top = curY - newY;
                    window.DragMove();
                }
            }
        }

        /// <summary>
        /// Updates the window constraints based on its state.
        /// For instance, the max width and height of the window is set to prevent overlapping over the taskbar.
        /// </summary>
        /// <param name="window">Window to set properties</param>
        private void UpdateWindowConstraints(Window window)
        {
            if (window != null)
            {
                // Make sure we don't bump the max width and height of the desktop when maximized
                var borderWidth = (GridLength) window.FindResource("BorderWidth");
                window.MaxHeight = SystemParameters.WorkArea.Height + borderWidth.Value*2;
                window.MaxWidth = SystemParameters.WorkArea.Width + borderWidth.Value*2;
            }
        }
    }
}