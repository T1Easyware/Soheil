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
using Soheil.Common;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for ProcessReportBuilder.xaml
	/// </summary>
	public partial class ProcessReportBuilder : UserControl
	{
		public ProcessReportBuilder()
		{
			InitializeComponent();
		}
		private void productDefection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 1)
			{
				sender.GetDataContext<Core.ViewModels.PP.Report.DefectionReportVm>().ProductDefection.SelectedItem = (e.AddedItems[0] as FilterableItemVm);
			}
			else
				sender.GetDataContext<Core.ViewModels.PP.Report.DefectionReportVm>().ProductDefection.SelectedItem = null;
		}

		private void CausesSelectedCode_TextChanged(object sender, TextChangedEventArgs e)
		{
			var vm = sender.GetDataContext<Core.ViewModels.PP.Report.StoppageReportVm>();
			var val = (sender as TextBox).Text;
			if (string.IsNullOrWhiteSpace(val)) vm.StoppageLevels.FilterBoxes[0].SelectedItem = null;
			if (val.Length >= 2)
				vm.StoppageLevels.FilterBoxes[0].SelectedItem =
					vm.StoppageLevels.FilterBoxes[0].FilteredList.FirstOrDefault(x => ((CauseVm)x.ViewModel).Code == val.Substring(0, 2));
			if (val.Length >= 4)
				vm.StoppageLevels.FilterBoxes[1].SelectedItem =
					vm.StoppageLevels.FilterBoxes[1].FilteredList.FirstOrDefault(x => ((CauseVm)x.ViewModel).Code == val.Substring(2, 2));
			if (val.Length == 6)
				vm.StoppageLevels.FilterBoxes[2].SelectedItem =
					vm.StoppageLevels.FilterBoxes[2].FilteredList.FirstOrDefault(x => ((CauseVm)x.ViewModel).Code == val.Substring(4, 2));
		}

	}
}
