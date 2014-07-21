using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PM
{
	public class PmLevelBase : DependencyObject
	{
		public PmLevelBase()
		{
		}

		/// <summary>
		/// Gets or sets a bindable collection that indicates Pages (Tabs within the level)
		/// </summary>
		public ObservableCollection<PmPageBase> Pages { get { return _pages; } }
		private ObservableCollection<PmPageBase> _pages = new ObservableCollection<PmPageBase>();

		/// <summary>
		/// Gets or sets a bindable value that indicates Title
		/// </summary>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(PmLevelBase), new PropertyMetadata(""));

	}
}
