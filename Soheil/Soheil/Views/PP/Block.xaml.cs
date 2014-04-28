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
	/// Interaction logic for Block.xaml (one Block in PPTable with its Tasks and Buttons)
	/// </summary>
	public partial class Block : UserControl
	{
		public Block()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Set this member from the containing PPTable
		/// <para>This member is used to fetch some info from its parent</para>
		/// </summary>
		public PPTableVm PPTable
		{
			get { return (PPTableVm)GetValue(PPTableProperty); }
			set { SetValue(PPTableProperty, value); }
		}
		public static readonly DependencyProperty PPTableProperty =
			DependencyProperty.Register("PPTable", typeof(PPTableVm), typeof(Block), new UIPropertyMetadata(null));

		/// <summary>
		/// A timer to load task reports with a delay
		/// </summary>
		System.Threading.Timer _detailsTimer;
		/// <summary>
		/// Delay of loading the task reports
		/// </summary>
		const int _detailsTimerDelay = 20;
		private void TaskReports_Loaded(object sender, RoutedEventArgs e)
		{
			var task = sender.GetDataContext<PPTaskVm>();
			if (task != null)
			{
				//kill the current timer
				if (_detailsTimer != null)
					_detailsTimer.Dispose();
				//starts a new timer in order to load the task reports
				_detailsTimer = new System.Threading.Timer(o =>
				{
					//load the task reports
					Dispatcher.Invoke(() => task.ReloadTaskReports());
				}, null, _detailsTimerDelay, System.Threading.Timeout.Infinite);
			}
		}
		private void TaskReports_Unloaded(object sender, RoutedEventArgs e)
		{
			var task = sender.GetDataContext<PPTaskVm>();
			if (task != null)
			{
				task.TaskReports.Clear();
			}
		}
	}
}
