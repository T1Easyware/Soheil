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

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for BlockEditor.xaml
	/// </summary>
	public partial class BlockEditor : UserControl
	{
		public BlockEditor()
		{
			InitializeComponent();
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var vm = DataContext as Soheil.Core.ViewModels.PP.Editor.BlockEditorVm;
			if (vm != null)
				vm.DontUpdateBlockTargetPoint = true;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace((sender as TextBox).Text))
			{
				var vm = DataContext as Soheil.Core.ViewModels.PP.Editor.BlockEditorVm;
				if (vm != null)
					vm.DontUpdateBlockTargetPoint = false;
			}
		}
	}
}
