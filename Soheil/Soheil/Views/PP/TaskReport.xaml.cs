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
using Soheil.Core.ViewModels.PP;
using Soheil.Core.ViewModels.PP.Report;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for TaskReport.xaml
	/// </summary>
	public partial class TaskReport : UserControl
	{
		public TaskReport()
		{
			InitializeComponent();
		}

		TaskVm _task;
		public FrameworkElement TaskUI
		{
			get { return (FrameworkElement)GetValue(TaskUIProperty); }
			set { SetValue(TaskUIProperty, value); }
		}
		public static readonly DependencyProperty TaskUIProperty =
			DependencyProperty.Register("TaskUI", typeof(FrameworkElement), typeof(TaskReport),
			new UIPropertyMetadata(null, (d, e) => ((TaskReport)d)._task = e.NewValue.GetDataContext<TaskVm>()));


		/// <summary>
		/// Set this member from the containing PPTable
		/// <para>This member is used to fetch some info from its parent</para>
		/// </summary>
		public PPTableVm PPTable
		{
			get { return (PPTableVm)GetValue(PPTableProperty); }
			set { SetValue(PPTableProperty, value); }
		}
		public static readonly DependencyProperty PPTableProperty =
			DependencyProperty.Register("PPTable", typeof(PPTableVm), typeof(TaskReport), new UIPropertyMetadata(null));


		private double _onThumbStartX;
		private double getDeltaOnLine(object sender)
		{
			if (TaskUI != null)
			{
				return Mouse.GetPosition(TaskUI).X;
			}
			return double.NaN;
		}



		private void startDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			sender.GetDataContext<TaskReportVm>().IsUserDrag = true;
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void startDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var taskReport = sender.GetDataContext<Soheil.Core.ViewModels.PP.Report.TaskReportVm>();
			if (taskReport != null && !double.IsNaN(onLineX))
				taskReport.StartDateTime = _task.StartDateTime.Add(
					TimeSpan.FromHours((onLineX - _onThumbStartX) / PPTable.HourZoom));
		}

		private void startDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			sender.GetDataContext<TaskReportVm>().IsUserDrag = false;
			startPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			startPopup.PlacementTarget = sender as UIElement;
			startPopup.IsOpen = true;
		}

		private void endDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			sender.GetDataContext<TaskReportVm>().IsUserDrag = true;
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void endDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var taskReport = sender.GetDataContext<Soheil.Core.ViewModels.PP.Report.TaskReportVm>();
			if (taskReport != null && !double.IsNaN(onLineX))
				taskReport.EndDateTime = _task.StartDateTime.Add(
					TimeSpan.FromHours((onLineX - _onThumbStartX) / PPTable.HourZoom));
		}

		private void endDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			sender.GetDataContext<TaskReportVm>().IsUserDrag = false;
			endPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			endPopup.PlacementTarget = sender as UIElement;
			endPopup.IsOpen = true;
		}
	}
}
