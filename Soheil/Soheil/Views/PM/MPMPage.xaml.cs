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
	/// Interaction logic for MPMPage.xaml
	/// </summary>
	public partial class MPMPage : UserControl
	{
		public MPMPage()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates PageVm
		/// </summary>
		public Core.ViewModels.PM.PmPageBase PageVm
		{
			get { return (Core.ViewModels.PM.PmPageBase)GetValue(PageVmProperty); }
			set { SetValue(PageVmProperty, value); }
		}
		public static readonly DependencyProperty PageVmProperty =
			DependencyProperty.Register("PageVm", typeof(Core.ViewModels.PM.PmPageBase), typeof(MPMPage),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (MPMPage)d;
				var val = (Core.ViewModels.PM.PmPageBase)e.NewValue;
				if (val != null)
				{
					vm.DataContext = val;
					val.Refresh += vm.RefreshAllColumnWidth;
				}
			}));
		public void RefreshAllColumnWidth()
		{
			var columns = (listview.View as GridView).Columns;
			if (columns != null)
				foreach (var c in columns)
				{
					// Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
					// i.e. it is the same code that is executed when the gripper is double clicked
					// if (adjustAllColumns || App.StaticGabeLib.FieldDefsGrid[colNum].DispGrid)
					if (double.IsNaN(c.Width))
					{
						c.Width = c.ActualWidth;
					}
					c.Width = double.NaN;
				}
		}
	}
}
