﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Views.Fpc
{
	/// <summary>
	/// Interaction logic for FpcWindow.xaml
	/// </summary>
	public partial class FpcWindow : UserControl
	{
		public FpcWindow()
		{
			InitializeComponent();
			VM = new FpcWindowVm();
		}

		#region VM
		//VM Dependency Property
		public FpcWindowVm VM
		{
			get { return DataContext as FpcWindowVm; }
			set { DataContext = value; }
		} 
		//Fpc Dependency Property
		/// <summary>
		/// When Fpc is changed, ChangeFpc of VM is called
		/// </summary>
		public Core.ViewModels.FpcVm Fpc
		{
			get { return (Core.ViewModels.FpcVm)GetValue(FpcProperty); }
			set { SetValue(FpcProperty, value); }
		}
		public static readonly DependencyProperty FpcProperty =
			DependencyProperty.Register("Fpc", typeof(Core.ViewModels.FpcVm), typeof(FpcWindow)
			, new UIPropertyMetadata(null, (d, e) =>
			{
				var view = d as FpcWindow;
				var val = e.NewValue as Core.ViewModels.FpcVm;
				if (val == null) return;
				view.VM.ChangeFpcByFpcId(val.Id, val.Access);
			}));
		#endregion

		#region Other Events
		//This event contains important VM calls
		[System.Diagnostics.DebuggerStepThrough]
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
				if (VM.DragTarget != null && !VM.IsLocationsLocked)
					VM.DragTarget.Location = new Vector(
							e.GetPosition(DrawingArea).X - VM.RelativeDragPoint.X * VM.Zoom,
							e.GetPosition(DrawingArea).Y - VM.RelativeDragPoint.Y * VM.Zoom);
		}

		private void ShadowToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			Application.Current.Resources["shadowOpacity"] = 0.3d;
		}

		private void ShadowToggleButton_Unchecked(object sender, RoutedEventArgs e)
		{
			Application.Current.Resources["shadowOpacity"] = 0d;
		}

		private void RootWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (VM.Message.IsEnabled)
				if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Space)
				{
					VM.Message.DefaultButton.Clicked.Execute(null);
					e.Handled = true;
				}
		} 
		#endregion

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
	}
}
