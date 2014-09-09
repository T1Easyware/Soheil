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
		/// <summary>
		/// Gets or sets the bindable text of this station
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(StationVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets the bindable value for vertical number of rows of this station [1..n]
		/// <para>Changing this value updates VSize to value * 42</para>
		/// </summary>
		public int VCount
		{
			get { return (int)GetValue(VSizeProperty) / 42; }
			set { SetValue(VSizeProperty, value * 42); }
		}
		public static readonly DependencyProperty VSizeProperty =
			DependencyProperty.Register("VSize", typeof(int), typeof(StationVm), new UIPropertyMetadata(42));
		/// <summary>
		/// Gets a bindable collection of Blocks in this station
		/// </summary>
		public ObservableCollection<BlockVm> Blocks { get { return _blocks; } }
		private ObservableCollection<BlockVm> _blocks = new ObservableCollection<BlockVm>();
		/// <summary>
		/// Gets a bindable collection of Non-Productive-Tasks in this station
		/// </summary>
		public ObservableCollection<NPTVm> NPTs { get { return _npts; } }
		private ObservableCollection<NPTVm> _npts = new ObservableCollection<NPTVm>();
	}
}
