using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Soheil.Core.ViewModels.PP;
using Soheil.Core.ViewModels.PP.Editor;
using Soheil.Common;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for PPJobEditor.xaml
	/// </summary>
	public partial class PPJobEditor : UserControl
	{
		#region Vm and Basic
		public PPJobEditor()
		{
			InitializeComponent();
		}
		public PPJobEditorVm VM
		{
			get { return (PPJobEditorVm)DataContext; }
			set { DataContext = value; }
		}
		#endregion


		private void pgTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is ProductGroupVm)
			{
				var tv = sender as TreeView;
				var tvi = tv.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItem;
				tvi.IsExpanded = true;
			}
		}
		private void pgTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			/*var product = ((TreeView)sender).SelectedItem as ProductVm;
			if (product != null)
				VM.JobList.Add(new PPEditorJob(product));*/
		}
	}
}
