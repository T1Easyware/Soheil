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
	/// Interaction logic for MachinePage.xaml
	/// </summary>
	public partial class MachinePage : UserControl
	{
		public MachinePage()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates PageVm
		/// </summary>
		//public Core.ViewModels.PM.PmPageBase PageVm
		//{
		//	get { return (Core.ViewModels.PM.PmPageBase)GetValue(PageVmProperty); }
		//	set { SetValue(PageVmProperty, value); }
		//}
		//public static readonly DependencyProperty PageVmProperty =
		//	DependencyProperty.Register("PageVm", typeof(Core.ViewModels.PM.PmPageBase), typeof(MachinePage),
		//	new PropertyMetadata(null, (d, e) =>
		//	{
		//		var vm = (MachinePage)d;
		//		var val = (Core.ViewModels.PM.PmPageBase)e.NewValue;
		//		vm.DataContext = val;
		//		if (val != null)
		//			val.Refresh += vm.RefreshAllColumnWidth;
		//	}));
	}
}
