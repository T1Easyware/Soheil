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
using Soheil.Core.ViewModels.PP.Report;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for ProcessReport.xaml
	/// </summary>
	public partial class ProcessReport : UserControl
	{
		public ProcessReport()
		{
			InitializeComponent();
		}

		private static System.Windows.Controls.Primitives.Popup _openedPopup;
		private static void openPopup(System.Windows.Controls.Primitives.Popup newpopup)
		{
			if (_openedPopup != null)
				_openedPopup.IsOpen = false;
			if (newpopup != null)
				newpopup.IsOpen = true;
			_openedPopup = newpopup;
		}

		//PPTable Dependency Property
		public Soheil.Core.ViewModels.PP.PPTableVm PPTable
		{
			get { return (Soheil.Core.ViewModels.PP.PPTableVm)GetValue(PPTableProperty); }
			set { SetValue(PPTableProperty, value); }
		}
		public static readonly DependencyProperty PPTableProperty =
			DependencyProperty.Register("PPTable", typeof(Soheil.Core.ViewModels.PP.PPTableVm), typeof(ProcessReport), new UIPropertyMetadata(null));

		//Process Dependency Property
		public ProcessVm Process
		{
			get { return (ProcessVm)GetValue(ProcessProperty); }
			set { SetValue(ProcessProperty, value); }
		}
		public static readonly DependencyProperty ProcessProperty =
			DependencyProperty.Register("Process", typeof(ProcessVm), typeof(ProcessReport), new UIPropertyMetadata(null));



		private double _onThumbStartX;
		private double getDeltaOnLine()
		{
			var line = Tag as FrameworkElement;
			if (line != null)
			{
				return Mouse.GetPosition(line).X;
			}
			return double.NaN;
		}



		private void startDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			sender.GetDataContext<ProcessReportVm>().IsUserDrag = true;
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void startDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine();
			var procReport = sender.GetDataContext<ProcessReportVm>();
			if (procReport != null && !double.IsNaN(onLineX))
				procReport.StartDateTime = Process.StartDateTime.Add(
					TimeSpan.FromHours((onLineX - _onThumbStartX) / PPTable.HourZoom));
		}

		private void startDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var procReport = sender.GetDataContext<ProcessReportVm>();
			if (procReport != null)
			{
				procReport.IsUserDrag = false;
				procReport.Save();
			}

			startPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			startPopup.PlacementTarget = sender as UIElement;
			openPopup(startPopup);
		}

		private void endDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			sender.GetDataContext<ProcessReportVm>().IsUserDrag = true;
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void endDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine();
			var procReport = sender.GetDataContext<ProcessReportVm>();
			if (procReport != null && !double.IsNaN(onLineX))
				procReport.EndDateTime = Process.StartDateTime.Add(
					TimeSpan.FromHours((onLineX + _onThumbStartX) / PPTable.HourZoom));
		}

		private void endDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var procReport = sender.GetDataContext<ProcessReportVm>();
			if (procReport != null)
			{
				procReport.IsUserDrag = false;
				procReport.Save();
			}

			endPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			endPopup.PlacementTarget = sender as UIElement;
			openPopup(endPopup);
		}

		private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
		{
			openPopup(null);
		}
	}
}
