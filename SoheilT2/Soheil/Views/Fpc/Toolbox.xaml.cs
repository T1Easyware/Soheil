using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Views.Fpc
{
	public partial class Toolbox
	{
		Vector _dragStart;
		FpcWindow _parentWindow;//don't use this
		FpcWindow FindAndSetParentWindow(object sender)//use this instead
		{
			if (_parentWindow != null) return _parentWindow;
			_parentWindow = (sender as FrameworkElement).GetVisualParent("ParentWindow") as FpcWindow;
			return _parentWindow;
		}

		private void ToolboxItem_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			var window = FindAndSetParentWindow(sender);
			var fe = sender as FrameworkElement;
			if (fe == null) return;
			var data = fe.DataContext as NamedVM;
			if (data == null) return;
			var dragPoint = Mouse.GetPosition(fe);
			var dA = window.DrawingArea;
			var drawingAreaPoint = Mouse.GetPosition(dA);

			_dragStart = new Vector(drawingAreaPoint.X - dragPoint.X, drawingAreaPoint.Y - dragPoint.Y);
			window.VM.SelectedToolboxItem = new ToolboxItemVm(window.VM)
			{
				ContentData = data,
				Id = data.Id,
				Text = data.Name,
				Location = _dragStart
			};
		}
		private void ToolboxItem_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var window = FindAndSetParentWindow(sender);
			window.VM.SelectedToolboxItem.Location = new Vector(
				_dragStart.X + e.HorizontalChange, _dragStart.Y + e.VerticalChange);
			var mouse = Mouse.GetPosition(window.DrawingArea);
			window.VM.UpdateDropIndicator(mouse);
		}


		#region Station

		private void Station_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var window = FindAndSetParentWindow(sender);
			if (window.VM.SelectedToolboxItem.CanDrop == true)
			{
				var mouse = Mouse.GetPosition(window.DrawingArea);
				var config = window.VM.SelectedToolboxItem.GetUnderlyingStateConfig(mouse);
				if (config != null)
					config.ContentsList.Add(new StateStationVm(window.VM)
					{
						Container = config,
						Containment = window.VM.SelectedToolboxItem.ContentData,
						IsExpanded = true,
					});
				window.VM.FocusedState.IsChanged = true;
			}
			window.VM.StopDragToolboxItem();
		}
		#endregion

		#region Activity

		private void Activity_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var window = FindAndSetParentWindow(sender);
			if (window.VM.SelectedToolboxItem.CanDrop == true)
			{
				var mouse = Mouse.GetPosition(window.DrawingArea);
				var station = window.VM.SelectedToolboxItem.GetUnderlyingStateStation(mouse);
				if (station != null)
					station.ContentsList.Add(new StateStationActivityVm(window.VM)
					{
						Container = station,
						Containment = window.VM.SelectedToolboxItem.ContentData,
						IsExpanded = true,
					});
				window.VM.FocusedState.IsChanged = true;
			}
			window.VM.StopDragToolboxItem();
		}
		#endregion

		#region Machine

		private void Machine_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var window = FindAndSetParentWindow(sender);
			if (window.VM.SelectedToolboxItem.CanDrop == true)
			{
				var mouse = Mouse.GetPosition(window.DrawingArea);
				var activity = window.VM.SelectedToolboxItem.GetUnderlyingStateStationActivity(mouse);
				if (activity != null)
					activity.ContentsList.Add(new StateStationActivityMachineVm(window.VM)
					{
						Container = activity,
						Containment = window.VM.SelectedToolboxItem.ContentData,
						IsDefault = true,
					});
				window.VM.FocusedState.IsChanged = true;
			}
			window.VM.StopDragToolboxItem();
		}
		#endregion
	}
}
