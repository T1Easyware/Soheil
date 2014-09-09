using System;
using Soheil.Common;
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

namespace Soheil.Views.MaterialPlanning
{
	/// <summary>
	/// Interaction logic for MaterialPlanning.xaml
	/// </summary>
	public partial class MaterialPlanning : UserControl
	{
		public MaterialPlanning()
		{
			InitializeComponent();
		}

		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			hoursScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
		}
		private static System.Windows.Controls.Primitives.Popup _openedPopup;
		private static void openPopup(System.Windows.Controls.Primitives.Popup newpopup, object dc = null)
		{
			if (_openedPopup != null)
				_openedPopup.IsOpen = false;
			if (newpopup != null)
			{
				newpopup.IsOpen = true;
				newpopup.DataContext = dc;
			}
			_openedPopup = newpopup;
		}
		private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
		{
			openPopup(null);
		}
		private void OpenTransactionButton_Click(object sender, RoutedEventArgs e)
		{
			transactionPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			var target = sender as FrameworkElement;
			transactionPopup.PlacementTarget = target;
			openPopup(transactionPopup, target.DataContext);
		}
	}
}
