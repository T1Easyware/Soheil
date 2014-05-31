using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Common;

namespace Soheil.Views.Fpc
{
	public partial class States : ResourceDictionary
	{
		public States()
		{
			InitializeComponent();
		}

		private void State_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Canvas dragTarget = sender as Canvas;
			if (dragTarget == null)
			{
				dragTarget = (sender as FrameworkElement).Tag as Canvas;
				if (dragTarget == null) return;
			}
			var state = dragTarget.DataContext as StateVm;

			state.ParentWindowVm.MouseClicksState(state, 
				e.GetPosition(dragTarget.GetVisualParent("DrawingArea")), 
				e.GetPosition(dragTarget.Children[0]), 
				e.GetPosition(dragTarget));
		}

		private void State_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var state = (sender as FrameworkElement).DataContext as StateVm;
			if (state == null) return;
			state.Width = e.NewSize.Width;
			state.Height = e.NewSize.Height;
		}

		[System.Diagnostics.DebuggerStepThrough]
		private void stateContainer_MouseEnter(object sender, MouseEventArgs e)
		{
			var state = (sender as FrameworkElement).DataContext as StateVm;
			if (state != null)
				state.ParentWindowVm.MouseEntersState(state);
		}
		[System.Diagnostics.DebuggerStepThrough]
		private void stateContainer_MouseLeave(object sender, MouseEventArgs e)
		{
			var state = (sender as FrameworkElement).DataContext as StateVm;
			if (state != null)
				state.ParentWindowVm.MouseLeavesState(state);
		}

		//This eventHandler is for all state except for temp states
		private void midStateLoaded(object sender, RoutedEventArgs e)
		{
			var dc = sender.GetDataContext<StateVm>();
			if(dc != null) dc.InitializingPhase = false;
		}
	}
}
