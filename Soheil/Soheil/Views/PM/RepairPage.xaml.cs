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

namespace Soheil.Views.PM
{
	/// <summary>
	/// Interaction logic for RepairPage.xaml
	/// </summary>
	public partial class RepairPage : UserControl
	{
		public RepairPage()
		{
			InitializeComponent();
			DataContextChanged += RepairPage_DataContextChanged;
		}
		void RepairPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			/*		
				var val = e.NewValue as Core.ViewModels.PM.PmPageBase;
			if (val != null)
					val.Refresh += RefreshAllColumnWidth;*/
		}
		public Core.ViewModels.PM.PmPageBase PageVm
		{
			get { return DataContext as Core.ViewModels.PM.PmPageBase; }
		}
	}
}
