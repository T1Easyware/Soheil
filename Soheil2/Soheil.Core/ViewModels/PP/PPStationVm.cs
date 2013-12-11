using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class PPStationVm : DependencyObject
	{
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(PPStationVm), new UIPropertyMetadata(""));
		//Tasks Observable Collection
		private ObservableCollection<PPTaskVm> _tasks = new ObservableCollection<PPTaskVm>();
		public ObservableCollection<PPTaskVm> Tasks { get { return _tasks; } }
		//NPTs Observable Collection
		private ObservableCollection<NPTVm> _npts = new ObservableCollection<NPTVm>();
		public ObservableCollection<NPTVm> NPTs { get { return _npts; } }
	}
}
