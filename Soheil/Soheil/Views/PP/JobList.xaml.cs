using Soheil.Core.ViewModels.PP;
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

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for JobList.xaml
	/// </summary>
	public partial class JobList : UserControl
	{
		public JobList()
		{
			InitializeComponent();
		}

		private void PPTableJobListDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			try
			{
				if (e.EditAction == DataGridEditAction.Commit && e.Column is DataGridTextColumn)
				{
					var item = e.Row.Item as JobListItemVm;
					var val = (e.EditingElement as TextBox).Text;
					item.UpdateDescription(val);
				}
			}
			catch { }
		}
	}
}
