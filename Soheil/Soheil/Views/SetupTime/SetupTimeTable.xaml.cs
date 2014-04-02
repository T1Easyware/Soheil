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

namespace Soheil.Views.SetupTime
{
	/// <summary>
	/// Interaction logic for SetupTimeTable.xaml
	/// </summary>
	public partial class SetupTimeTable : UserControl
	{
		public SetupTimeTable()
		{
			InitializeComponent();
		}

		#region Scroll events
		private void rowHeaderScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (changeoverScrollbar != null)
				changeoverScrollbar.ScrollToVerticalOffset(e.VerticalOffset);
		}
		private void columnHeaderScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (changeoverScrollbar != null)
				changeoverScrollbar.ScrollToHorizontalOffset(e.HorizontalOffset);
		}
		#endregion
	}
}
