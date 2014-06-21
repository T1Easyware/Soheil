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

namespace Soheil.Views.Index
{
	/// <summary>
	/// Interaction logic for MachineOEE.xaml
	/// </summary>
	public partial class MachineOEE : UserControl
	{
		public MachineOEE()
		{
			InitializeComponent();
		}

		private void Rework_MouseEnter(object sender, MouseEventArgs e)
		{
			var fe = sender as UIElement;
			fe.ClipToBounds = false;
			Panel.SetZIndex(fe, 10);
		}

		private void Rework_MouseLeave(object sender, MouseEventArgs e)
		{
			var fe = sender as UIElement;
			fe.ClipToBounds = true;
			Panel.SetZIndex(fe, -10);
		}

	}
}
