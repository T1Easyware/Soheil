using Soheil.Core.ViewModels.Fpc;
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

namespace Soheil.Views.Fpc
{
	/// <summary>
	/// Interaction logic for Toolbox.xaml
	/// </summary>
	public partial class Toolbox : UserControl
	{
		public Toolbox()
		{
			InitializeComponent();
		}

		Vector _dragStart;

		//DrawingArea of containing window
		public Canvas DrawingArea
		{
			get { return (Canvas)GetValue(DrawingAreaProperty); }
			set { SetValue(DrawingAreaProperty, value); }
		}
		public static readonly DependencyProperty DrawingAreaProperty =
			DependencyProperty.Register("DrawingArea", typeof(Canvas), typeof(Toolbox), new UIPropertyMetadata(null));
		//Gets ViewModel value (DataContext)
		public FpcWindowVm VM
		{
			get { return (FpcWindowVm)DataContext; }
		}

		private void ToolboxItem_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			var fe = sender as FrameworkElement;
			if (fe == null) return;
			var data = fe.DataContext as IToolboxData;
			if (data == null) return;
			var dragPoint = Mouse.GetPosition(fe);
			var drawingAreaPoint = Mouse.GetPosition(DrawingArea);

			_dragStart = new Vector(drawingAreaPoint.X - dragPoint.X, drawingAreaPoint.Y - dragPoint.Y);
			VM.SelectedToolboxItem = new ToolboxItemVm(VM)
			{
				ContentData = data,
				Text = data.Name,
				Location = _dragStart
			};
		}
		private void ToolboxItem_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			VM.SelectedToolboxItem.Location = new Vector(
				_dragStart.X + e.HorizontalChange / VM.Zoom, _dragStart.Y + e.VerticalChange / VM.Zoom);
			VM.UpdateDropIndicator(Mouse.GetPosition(DrawingArea));
		}



		#region Station

		private void Station_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (VM.SelectedToolboxItem.CanDrop == true)
			{
				//find mouse position
				var mouse = Mouse.GetPosition(DrawingArea);

				//find the target state config
				var config =
					VM.SelectedToolboxItem.GetUnderlyingStateConfig(mouse);

				//add the machine to that state config
				if (config != null)
					config.AddNewStateStation(
						VM,
						VM.SelectedToolboxItem.ContentData as StationVm);
			}
			VM.StopDragToolboxItem();
		}
		#endregion

		#region Activity

		private void Activity_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (VM.SelectedToolboxItem.CanDrop == true)
			{
				//find mouse position
				var mouse = Mouse.GetPosition(DrawingArea);

				//find the target ss
				var stateStation =
					VM.SelectedToolboxItem.GetUnderlyingStateStation(mouse);

				//add the machine to that ss
				if (stateStation != null)
					stateStation.AddNewStateStationActivity(
						VM,
						VM.SelectedToolboxItem.ContentData as ActivityVm);
			}
			VM.StopDragToolboxItem();
		}
		#endregion

		#region Machine

		private void Machine_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (VM.SelectedToolboxItem.CanDrop == true)
			{
				//find mouse position
				var mouse = Mouse.GetPosition(DrawingArea);

				//find the target ssa
				var stateStationActivity =
					VM.SelectedToolboxItem.GetUnderlyingStateStationActivity(mouse);

				//add the machine to that ssa
				if (stateStationActivity != null)
					stateStationActivity.AddNewStateStationActivityMachine(
						VM,
						VM.SelectedToolboxItem.ContentData as MachineVm);
			}
			VM.StopDragToolboxItem();
		}
		#endregion

	}
}
