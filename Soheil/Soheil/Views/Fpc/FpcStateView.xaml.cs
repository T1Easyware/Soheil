using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Views.Fpc
{
	/// <summary>
	/// Interaction logic for FpcStateView.xaml
	/// </summary>
	public partial class FpcStateView : UserControl
	{
		public FpcStateView()
		{
			InitializeComponent();
		}

		//VM Dependency Property
		public FpcWindowVm VM
		{
			get { return DataContext as FpcWindowVm; }
			set { DataContext = value; }
		}

		Point _backDragStartPt;
		private void DrawingArea_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (DrawingArea.IsMouseDirectlyOver)
			{
				_backDragStartPt = e.GetPosition(this);
				_backDragStartPt.Offset(-DrawingArea.Margin.Left, -DrawingArea.Margin.Top);
				DrawingArea.CaptureMouse();
			}
		}
		private void DrawingArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			VM.Zoom += (e.Delta > 0) ? 0.1 : -0.1;
			e.Handled = true;
		}

		private void Area_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DrawingArea.ReleaseMouseCapture();

			if (VM == null) return;
			//perform release mechanism
			if (VM.DragTarget != null) VM.ReleaseDragAt(e.GetPosition(DrawingArea));
		}

		[System.Diagnostics.DebuggerStepThrough]
		private void Area_MouseMove(object sender, MouseEventArgs e)
		{
			if (DrawingArea.IsMouseCaptured)
			{
				var pt = e.GetPosition(this);
				var x = pt.X - _backDragStartPt.X;
				if (x > 0) x = 0;
				var y = pt.Y - _backDragStartPt.Y;
				if (y > 0) y = 0;
				DrawingArea.Margin = new Thickness(x, y, 0, 0);
				return;
			}

			if (VM != null)
				//perform drag mechanism
				if (VM.DragTarget != null)
					VM.DragTarget.Location = new Vector(
							e.GetPosition(DrawingArea).X - VM.RelativeDragPoint.X,
							e.GetPosition(DrawingArea).Y - VM.RelativeDragPoint.Y);
		}

		private void ShadowToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			Application.Current.Resources["shadowOpacity"] = 0.3d;
		}

		private void ShadowToggleButton_Unchecked(object sender, RoutedEventArgs e)
		{
			Application.Current.Resources["shadowOpacity"] = 0d;
		}

	}
}
