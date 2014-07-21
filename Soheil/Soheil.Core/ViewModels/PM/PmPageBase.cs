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
		public event Action Refresh;
		internal void InvokeRefresh() { if (Refresh != null) Refresh(); }

		public PmPageBase()
		{
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
				if (val != null) val.InvokeSelected();
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
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(PmPageBase), new PropertyMetadata(null));
	}
}
