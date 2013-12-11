using System;
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
				view.VM.ChangeFpc(val);
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
	}
}
