using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Soheil.Views
{
	/// <summary>
	/// Interaction logic for SplashScreen.xaml
	/// </summary>
	public partial class SplashScreen : Window
	{
		public SplashScreen()
		{
			InitializeComponent();
			SetValue(VersionProperty, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}
		public static readonly DependencyProperty VersionProperty = DependencyProperty.Register("Version", typeof(string), typeof(SplashScreen), new PropertyMetadata("0.0.0.0"));
		Timer _timer;
		private void root_Loaded(object sender, RoutedEventArgs e)
		{
			_timer = new Timer(1000);
			_timer.Elapsed += (s, ea) =>
			{
				_timer.Stop();
				Dispatcher.Invoke(() =>
				{
					Soheil.Core.ViewModels.MessageCenter.NotificationArea.Singleton = new Core.ViewModels.MessageCenter.NotificationArea();
					Soheil.Core.ViewModels.MessageCenter.NotificationArea.Singleton.Loaded += () =>
					{
						new MainWindow().Show();
						this.Close();
					};
					Soheil.Core.ViewModels.MessageCenter.NotificationArea.Singleton.Load();
				});
			};
			_timer.Start();
		}
	}
}
