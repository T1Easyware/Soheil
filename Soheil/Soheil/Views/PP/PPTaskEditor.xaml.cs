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
		private void blockListItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var block = (sender as FrameworkElement).DataContext as PPEditorBlock;
			if (VM.SelectedBlock == block)
			{
				VM.ShowFpc = false;
			}
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
			var vm = sender.GetDataContext<PPEditorTask>();
			vm.SetToToday();
		}

		private void SelectTomorrowButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorTask>();
			vm.SetToTomorrow();
		}

		private void SelectNextHourButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = sender.GetDataContext<PPEditorTask>();
			vm.SetToNextHour();
		}
		#endregion
	}
}
