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
	/// Interaction logic for PmPage.xaml
	/// </summary>
	public partial class MachinePartPage : UserControl
	{
		public MachinePartPage()
		{
			InitializeComponent();
			DataContextChanged += MachinePartPage_DataContextChanged;
		}

		void MachinePartPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = e.NewValue as Core.ViewModels.PM.PmPageBase;
			if (val != null)
				val.Refresh += RefreshAllColumnWidth;
		}
		public Core.ViewModels.PM.PmPageBase PageVm
		{
			get { return DataContext as Core.ViewModels.PM.PmPageBase; }
		}
		

		public void RefreshAllColumnWidth()
		{
			if (PageVm == null) return;
			var columns = (listview.View as GridView).Columns;
			if (columns != null)
			{
				for (int i = 0; i < columns.Count; i++)
				{
					var c = columns[i];
					if (PageVm.HideMachines && i == 0) { c.Width = 0; continue; }
					else if (c.Header == null) continue;
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
}
