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

namespace Soheil.Controls.CustomControls
{
	/// <summary>
	/// Interaction logic for NumericBox.xaml
	/// </summary>
	public partial class NumericBox : UserControl
	{
		public NumericBox()
		{
			Minimum = 0;
			Maximum = int.MaxValue;
			InitializeComponent();
		}
		public void ChangeValueBy(int delta)
		{
			Value += (JumpCount * delta);
			textBox.SelectAll();
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates Value
		/// </summary>
		public int Value
		{
			get { return (int)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(int), typeof(NumericBox),
			new PropertyMetadata(0, (d, e) => { },
				(d, v) =>
				{
					var vm = (NumericBox)d;
					var val = (int)v;
					if (val < vm.Minimum) return vm.Minimum;
					if (val > vm.Maximum) return vm.Maximum;
					return v;
				}));

		/// <summary>
		/// Gets or sets a bindable value that indicates JumpCount
		/// </summary>
		public int JumpCount
		{
			get { return (int)GetValue(JumpCountProperty); }
			set { SetValue(JumpCountProperty, value); }
		}
		public static readonly DependencyProperty JumpCountProperty =
			DependencyProperty.Register("JumpCount", typeof(int), typeof(NumericBox), new PropertyMetadata(1));

		public int Minimum { get; set; }
		public int Maximum { get; set; }

		private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			//if (e.Delta != 0)
			//	ChangeValueBy(e.Delta < 1 ? -1 : 1);
		}
		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			textBox.SelectAll();
		}
		private void Decrease(object sender, RoutedEventArgs e)
		{
			ChangeValueBy(-1);
		}
		private void Increase(object sender, RoutedEventArgs e)
		{
			ChangeValueBy(1);
		}
	}
}
