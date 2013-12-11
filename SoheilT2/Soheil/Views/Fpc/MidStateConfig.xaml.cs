using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Common;

namespace Soheil.Views.Fpc
{
	public partial class MidStateConfig : ResourceDictionary
	{
		public MidStateConfig()
		{
			InitializeComponent();
		}
		private TreeItemVm GetVm(object sender)
		{
			var fe = sender as FrameworkElement;
			if (fe == null) return null;
			var vm = fe.DataContext as TreeItemVm;
			return vm;
		}

		private void SelectStateButton_Click(object sender, RoutedEventArgs e)
		{
			var config = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).Content as StateConfigVm;
			if (config != null)
			{
				config.State.ParentWindowVm.CallPPEditorToCreateNewStateFrom(config.State);
			}
		}
	}
}
