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
using Soheil.Core.ViewModels.PP.Editor;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for TaskEditor.xaml
	/// </summary>
	public partial class TaskEditor : UserControl
	{
		public TaskEditor()
		{
			InitializeComponent();
		}

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
		#endregion
	}
}
