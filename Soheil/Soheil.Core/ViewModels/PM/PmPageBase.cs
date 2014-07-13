using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PM
{
	public class PmPageBase : DependencyObject
	{
		public event Action Selected;

		public PmPageBase(string title)
		{
			Title = title;
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates IsSelected
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(PmPageBase),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (PmPageBase)d;
				var val = (bool)e.NewValue;
				if (vm.Selected != null)
					vm.Selected();
			}));


		/// <summary>
		/// Gets or sets a bindable value that indicates Title
		/// </summary>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(PmPageBase), new PropertyMetadata(""));


		/// <summary>
		/// Gets or sets a bindable collection that indicates Items
		/// </summary>
		public ObservableCollection<PmItemBase> Items { get { return _items; } }
		private ObservableCollection<PmItemBase> _items = new ObservableCollection<PmItemBase>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedItem
		/// </summary>
		public PmItemBase SelectedItem
		{
			get { return (PmItemBase)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(PmItemBase), typeof(PmPageBase),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (PmPageBase)d;
				var val = (PmItemBase)e.NewValue;
			}));



		/// <summary>
		/// Gets or sets a bindable command that adds a new source item to this page
		/// </summary>
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(PmPageBase), new PropertyMetadata(null));

	}
}
