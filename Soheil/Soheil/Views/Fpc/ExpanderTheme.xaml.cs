using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Views.Fpc
{
	public partial class ExpanderTheme : ResourceDictionary
	{
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = (sender as Button).Tag as TreeItemVm;
			vm.Delete();
		}
	}
}
