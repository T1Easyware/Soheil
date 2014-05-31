using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StationVm : DependencyObject
	{
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(StationVm), new UIPropertyMetadata(""));
		//Tasks Observable Collection
		private ObservableCollection<BlockVm> _blocks = new ObservableCollection<BlockVm>();
		public ObservableCollection<BlockVm> Blocks { get { return _blocks; } }
		//NPTs Observable Collection
		private ObservableCollection<NPTVm> _npts = new ObservableCollection<NPTVm>();
		public ObservableCollection<NPTVm> NPTs { get { return _npts; } }
	}
}
