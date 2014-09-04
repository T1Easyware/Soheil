using Soheil.Common.Localization;
using Soheil.Tablet.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using Soheil.Core.ViewModels.PP;

namespace Soheil.Tablet
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var culture = CultureInfo.GetCultureInfo("fa-IR");
			Dispatcher.Thread.CurrentCulture = culture;
			Dispatcher.Thread.CurrentUICulture = culture;
			LocalizationManager.UpdateValues();

			InitializeComponent();
			DataContext = new PageVm();
		}

		private TextBox _lastTextbox;
		private void TextBox_TouchDown(object sender, TouchEventArgs e)
		{
			_lastTextbox = sender as TextBox;
			_lastTextbox.SelectAll();
		}
		private void TextBox_Focused(object sender, EventArgs e)
		{
			_lastTextbox = sender as TextBox;
			_lastTextbox.SelectAll();
		}
		void numpadAction(Button button)
		{
			if (_lastTextbox != null)
			{
				var n = button.Content.ToString();
				if (n == "C")
					_lastTextbox.Clear();
				else if (n == "←")
				{
					var txt = _lastTextbox.Text;
					if (txt.Length <= 1) _lastTextbox.Clear();
					else _lastTextbox.Text = txt.Substring(0, txt.Length - 1);
				}
				else
				{
					_lastTextbox.Text += n;
				}
			}
		}
		private void NumpadTouchDown(object sender, TouchEventArgs e)
		{
			//numpadAction(sender as Button);
		}
		private void NumpadClick(object sender, EventArgs e)
		{
			numpadAction(sender as Button);
		}

		private void DatePickerTouchDown(object sender, TouchEventArgs e)
		{
			var border = sender as Border;
			var dp = border.Child as dynamic;
			dp.IsDropDownOpen = true;
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

		private void QuickCauseButtonDown(object sender, RoutedEventArgs e)
		{
			var target = (sender as FrameworkElement).Tag as TextBox;
			var code = (sender as Button).Content.ToString();
			target.Text = code;
		}
	}
	public class Inverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}
