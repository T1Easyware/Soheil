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
using Soheil.Core.ViewModels.PP.Editor;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for TaskEditor.xaml
	/// </summary>
	public partial class PlanEditor : UserControl
	{
		public PlanEditor()
		{
			InitializeComponent();
		}

		#region Vm and Basic
		public PlanEditorVm VM
		{
			get { return (PlanEditorVm)DataContext; }
			set { DataContext = value; }
		}
		#endregion

		#region Product And State
		private void Product_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
			{
				VM.FpcViewer = null;
			}
			else if (e.NewValue is ProductVm)
			{
				VM.SelectedProduct = null;
				VM.SelectedProduct = (ProductVm)e.NewValue;
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
			VM.SelectedProduct = null;
			VM.SelectedProduct = product;
		}
		private void blockListItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var block = (sender as FrameworkElement).DataContext as BlockEditorVm;
			VM.SelectedBlock = null;
			VM.SelectedBlock = block;
		}
		#endregion
	}
}
