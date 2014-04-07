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
				config.State.ParentWindowVm.FireSelectState(config.State);
			}
		}

		private void textBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Space)
			{
				var sc = sender.GetDataContext<StateConfigVm>();
				if (sc != null) sc.State.SaveCommand.Execute(null);
				else
				{
					var ssa = sender.GetDataContext<StateStationActivityVm>();
					if (ssa != null) ssa.ContainerSS.ContainerS.State.SaveCommand.Execute(null);
				}
			}
		}
	}
}
