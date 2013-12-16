using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Soheil.Core.ViewModels.PP;
using Soheil.Core.ViewModels.PP.Editor;
using Soheil.Common;
using System.Windows.Input;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for PPEditor.xaml
	/// </summary>
	public partial class PPTaskEditor : UserControl
	{
		public PPTaskEditor()
		{
			InitializeComponent();
		}

		#region Vm and Basic
		public PPTaskEditorVm VM
		{
			get { return (PPTaskEditorVm)DataContext; }
			set { DataContext = value; }
		}
		#endregion

		#region Product And State
		private void Product_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue is ProductVm)
			{
				VM.SelectedProduct = (ProductVm)e.NewValue;
				VM.ShowFpc = true;
			}
			else if (e.NewValue is ProductGroupVm)
			{
				var tv = sender as TreeView;
				var tvi = tv.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItem;
				tvi.IsExpanded = true;
			}
		}
		private void Product_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var product = (sender as FrameworkElement).DataContext as ProductVm;
			if (product != null)
			{
				VM.SelectedProduct = product;
				VM.ShowFpc = true;
			}
		}
		private void PPStateListItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var task = (sender as FrameworkElement).DataContext as PPEditorState;
			if (VM.SelectedState == task)
			{
				VM.ShowFpc = false;
			}
		}
		private void deletePPStateButton_Click(object sender, RoutedEventArgs e)
		{
			VM.FpcViewer_RemovePPState((sender as FrameworkElement).DataContext as PPEditorState);
		} 
		#endregion


		#region operator click and today etc
		private void SelectOperator(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorOperator>();
			vm.IsSelected = true;
		}
		private void DeselectOperator(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorOperator>();
			vm.IsSelected = false;
		}


		private void SelectTodayButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorStation>();
			vm.SetToToday();
		}

		private void SelectTomorrowButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorStation>();
			vm.SetToTomorrow();
		}

		private void SelectNextHourButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorStation>();
			vm.SetToNextHour();
		}
		#endregion


		#region Button Events
		private void btnClearAll_Click(object sender, RoutedEventArgs e)
		{
			VM.Reset();
		}
		private void btnSaveAll_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				VM.SaveAllAsTasks();
				VM.Reset();
				VM.IsVisible = false;
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
		}
		private void btnClearStation_Click(object sender, RoutedEventArgs e)
		{
			VM.SelectedState.ResetCurrentStation();
		}
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (VM.SelectedState == null) return;
			try
			{
				VM.SaveSelectedStateStationAsTask();
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
		}
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			VM.IsVisible = false;
		} 
		#endregion
	}
}
