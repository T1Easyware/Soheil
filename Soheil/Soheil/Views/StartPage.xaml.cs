using Soheil.Core.Commands;
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

namespace Soheil.Views
{
	/// <summary>
	/// Interaction logic for StartPage.xaml
	/// </summary>
	public partial class StartPage : UserControl
	{
		public StartPage()
		{
			InitializeComponent();
			DataContext = this;
			SetValue(Command1Property, new Command(o => AnyButtonClicked(new Control { Tag = 231 }, null)));
			SetValue(Command2Property, new Command(o => AnyButtonClicked(new Control { Tag = 311 }, null)));
			SetValue(Command3Property, new Command(o => AnyButtonClicked(new Control { Tag = 222 }, null)));
			SetValue(Command4Property, new Command(o => AnyButtonClicked(new Control { Tag = 34 }, null)));
			SetValue(Command5Property, new Command(o => AnyButtonClicked(new Control { Tag = 27 }, null)));
		}
		public static readonly RoutedEvent AnyButtonClickedEvent;
		public event RoutedEventHandler AnyButtonClicked;

		public static readonly DependencyProperty Command1Property = DependencyProperty.Register("Command1", typeof(Command), typeof(StartPage), new PropertyMetadata(null));
		public static readonly DependencyProperty Command2Property = DependencyProperty.Register("Command2", typeof(Command), typeof(StartPage), new PropertyMetadata(null));
		public static readonly DependencyProperty Command3Property = DependencyProperty.Register("Command3", typeof(Command), typeof(StartPage), new PropertyMetadata(null));
		public static readonly DependencyProperty Command4Property = DependencyProperty.Register("Command4", typeof(Command), typeof(StartPage), new PropertyMetadata(null));
		public static readonly DependencyProperty Command5Property = DependencyProperty.Register("Command5", typeof(Command), typeof(StartPage), new PropertyMetadata(null));

	}
}
