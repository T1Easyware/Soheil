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
	/// Interaction logic for PartPage.xaml or maintenancePage
	/// </summary>
	public partial class PartPage : UserControl
	{
		public PartPage()
		{
			InitializeComponent();
			DataContextChanged += PartPage_DataContextChanged;
		}
		void PartPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var val = e.NewValue as Core.ViewModels.PM.PmPageBase;
			if (val != null)
				val.Refresh += RefreshAllColumnWidth;
		}
		public Core.ViewModels.PM.PmPageBase PageVm
		{
			get { return DataContext as Core.ViewModels.PM.PmPageBase; }
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowAddButton
		/// </summary>
		public bool ShowAddButton
		{
			get { return (bool)GetValue(ShowAddButtonProperty); }
			set { SetValue(ShowAddButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowAddButtonProperty =
			DependencyProperty.Register("ShowAddButton", typeof(bool), typeof(PartPage), new PropertyMetadata(false));

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
