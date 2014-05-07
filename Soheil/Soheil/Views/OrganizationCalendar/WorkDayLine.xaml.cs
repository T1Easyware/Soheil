using Soheil.Common;
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

namespace Soheil.Views.OrganizationCalendar
{
	/// <summary>
	/// Interaction logic for WorkDayLine.xaml
	/// </summary>
	public partial class WorkDayLine : UserControl
	{
		public WorkDayLine()
		{
			InitializeComponent();
		}

		#region WorkProfile Events
		private void workShiftLineMouseMove(object sender, MouseEventArgs e)
		{
			var line = (FrameworkElement)sender;
			var x = e.GetPosition(line).X;
			var timetip = line.Tag as FrameworkElement;
			showTimetip(timetip, x);
		}
		private void showTimetip(FrameworkElement timetip, double x)
		{
			if (timetip != null)
			{
				timetip.Margin = new Thickness(x, 0, 0, 0);
				int seconds = SoheilFunctions.RoundFiveMinutes((int)x * 60 + SoheilConstants.EDITOR_START_SECONDS);
				timetip.Tag = SoheilFunctions.GetWorkShiftTime(seconds);
				var fadeout = timetip.Resources["fadeout"] as System.Windows.Media.Animation.Storyboard;
				fadeout.Stop();
				timetip.Opacity = 1;
				fadeout.Begin();
			}
		}
		private double _onThumbStartX;
		private double getDeltaOnLine(object sender)
		{
			var thumb = sender as FrameworkElement;
			if (thumb != null)
			{
				var line = thumb.Tag as FrameworkElement;
				if (line != null)
				{
					return Mouse.GetPosition(line).X;
				}
			}
			return double.NaN;
		}
		private FrameworkElement updateTimetip(object sender, System.Windows.Visibility visiblility)
		{
			var thumb = sender as FrameworkElement;
			if (thumb == null) return null;
			var timebelow = thumb.GetNextVisual();
			if (timebelow != null)
				timebelow.Visibility = visiblility;
			return timebelow;
		}
		private void showTimetipForLine(object sender, int seconds)
		{
			showTimetip(
				(sender as FrameworkElement).Tag as FrameworkElement,
				(SoheilFunctions.RoundFiveMinutes(seconds) - SoheilConstants.EDITOR_START_SECONDS) / 60);
		}
		private void shiftStartDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Visible);
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void shiftStartDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var shift = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm>();
			if (shift != null && !double.IsNaN(onLineX))
				shift.StartSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX - _onThumbStartX) * 60);
		}
		private void breakStartDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var wbreak = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm>();
			if (wbreak != null && !double.IsNaN(onLineX))
				wbreak.StartSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX - _onThumbStartX) * 60);
		}

		private void shiftStartDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Collapsed);
		}

		private void shiftEndDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Visible);
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = thumb.ActualWidth - Mouse.GetPosition(thumb).X;
		}

		private void shiftEndDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var shift = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm>();
			if (shift != null && !double.IsNaN(onLineX))
				shift.EndSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX + _onThumbStartX) * 60);
		}
		private void breakEndDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var wbreak = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm>();
			if (wbreak != null && !double.IsNaN(onLineX))
				wbreak.EndSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX + _onThumbStartX) * 60);
		}

		private void shiftEndDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Collapsed);
		}

		Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm _currentDrawingBreak;
		private void shiftLineDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			var thumb = sender as FrameworkElement;
			var day = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkDayVm>();
			if (thumb == null || day == null) return;

			int seconds = (int)(e.HorizontalOffset * 60) + SoheilConstants.EDITOR_START_SECONDS;
			_currentDrawingBreak = null;
			Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm currentDrawingShift = null;
			foreach (var shift in day.Shifts)
			{
				if (seconds >= shift.StartSeconds && seconds <= shift.EndSeconds)
					currentDrawingShift = shift;
			}

			if (currentDrawingShift == null) MessageBox.Show("ساعت استراحت بایستی داخل شیفت افزوده شود");
			else
			{
				_currentDrawingBreak = currentDrawingShift.AddTemporaryBreak(seconds);
				_onThumbStartX = Mouse.GetPosition(thumb).X;

				//showTimetipForLine(sender, seconds);
			}
		}
		private void shiftLineDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var thumb = sender as FrameworkElement;
			if (thumb == null || _currentDrawingBreak == null) return;

			int oldSeconds = (int)(_onThumbStartX * 60) + SoheilConstants.EDITOR_START_SECONDS;
			int newSeconds = (int)(Mouse.GetPosition(thumb).X * 60) + SoheilConstants.EDITOR_START_SECONDS;
			if (newSeconds > oldSeconds)
			{
				_currentDrawingBreak.StartSeconds = oldSeconds;
				_currentDrawingBreak.EndSeconds = newSeconds;
			}
			else
			{
				_currentDrawingBreak.StartSeconds = newSeconds;
				_currentDrawingBreak.EndSeconds = oldSeconds;
			}
			showTimetipForLine(sender, newSeconds);
		}
		private void shiftLineDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
		}

		#endregion
	}
}
