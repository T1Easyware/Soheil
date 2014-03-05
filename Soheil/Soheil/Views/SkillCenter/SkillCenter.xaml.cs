using Soheil.Core.ViewModels.SkillCenter;
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

namespace Soheil.Views.SkillCenter
{
	/// <summary>
	/// Interaction logic for SkillCenter.xaml
	/// </summary>
	public partial class SkillCenter : UserControl
	{
		public SkillCenter()
		{
			InitializeComponent();
		}
		public SkillCenterVm VM
		{
			get { return DataContext as SkillCenterVm; }
			set { DataContext = value; }
		}


		bool _suppressScroll = false;
		private void VerticalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			_suppressScroll = true;
			scrollBar.ScrollToVerticalOffset(e.VerticalOffset);
			_suppressScroll = false;
		}

		private void HorizontalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			_suppressScroll = true;
			scrollBar.ScrollToHorizontalOffset(e.HorizontalOffset);
			_suppressScroll = false;
		}

		private void scrollBar_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if(!_suppressScroll)
			{
				scrollBarV.ScrollToVerticalOffset(e.VerticalOffset);
				scrollBarH.ScrollToHorizontalOffset(e.HorizontalOffset);
			}
		} 
	}
}
