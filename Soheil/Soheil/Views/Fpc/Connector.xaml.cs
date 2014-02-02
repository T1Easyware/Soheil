using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Views.Fpc
{
	public partial class Connector : ResourceDictionary
	{
		public Connector()
		{
			InitializeComponent();
		}
		private void DeleteConnectorButton_Click(object sender, RoutedEventArgs e)
		{
			var conn = (sender as FrameworkElement).DataContext as ConnectorVm;
			if (conn == null) return;
			conn.Start.ParentWindowVm.Connectors.Remove(conn);
		}
	}
}
