using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class FilterBoxVm : DependencyObject
	{
		public FilterBoxVm()
		{
			ClearCommand = new Commands.Command(o => SelectedItem = null);
			DeleteCommand = new Commands.Command(o => { if (_parent != null) _parent.FilterBoxes.Remove(this); });
		}
		public static FilterBoxVm CreateForProductDefections(int selectedId, int productId)
		{
			var vm = new FilterBoxVm();
			
			var productDefectionDS = new DataServices.ProductDefectionDataService();
			var productDefectionModels = productDefectionDS.GetActivesForProduct(productId);
			var count = productDefectionModels.Count();
			for (int i = 0; i < count; i++)
			{
				vm.FilteredList.Add(FilterableItemVm.CreateForProductDefection(vm, productDefectionModels[i]));
			}

			//select
			vm.SelectedItem = vm.FilteredList.FirstOrDefault(x => x.Id == selectedId);

			return vm;
		}
		public static FilterBoxVm CreateForCause(FilterBoxVm nextLevel, IEnumerable<Model.Cause> list = null)
		{
			var vm = new FilterBoxVm();
			vm._nextLevel = nextLevel;
			if (list != null)
			{
				foreach (var item in list)
				{
					vm.FilteredList.Add(FilterableItemVm.CreateForCause(vm, item));
				}
			}
			return vm;
		}
		public static FilterBoxVm CreateForGuiltyOperators(FilterBoxVmCollection parent, int selectedId, FilterableItemVm[] list)
		{
			var vm = new FilterBoxVm();
			vm._parent = parent;
			foreach (var item in list)
			{
				item.Parent = vm;
				vm.FilteredList.Add(item);
			}

			//select
			vm.SelectedItem = vm.FilteredList.FirstOrDefault(x => x.Id == selectedId);

			return vm;
		}

		/// <summary>
		/// <para>in guilty operator used in DeleteCommand (removing this from Parent)</para>
		/// </summary>
		private FilterBoxVmCollection _parent;
		/// <summary>
		/// <para>in causes used in eventHandler (changing nextLevel's items)</para>
		/// </summary>
		private FilterBoxVm _nextLevel;

		//SelectedItem Dependency Property
		public FilterableItemVm SelectedItem
		{
			get { return (FilterableItemVm)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(FilterableItemVm), typeof(FilterBoxVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (FilterBoxVm)d;
				if (vm.FilterBoxSelectedItemChanged != null)
					vm.FilterBoxSelectedItemChanged(vm, (FilterableItemVm)e.NewValue);
			}));
		/// <summary>
		/// For usage in StoppageReport (Cause LevelX)
		/// </summary>
		public event Action<FilterBoxVm, FilterableItemVm> FilterBoxSelectedItemChanged;

		//FilteredList Observable Collection
		private ObservableCollection<FilterableItemVm> _filteredList = new ObservableCollection<FilterableItemVm>();
		public ObservableCollection<FilterableItemVm> FilteredList { get { return _filteredList; } }
		//ClearCommand Dependency Property
		public Commands.Command ClearCommand
		{
			get { return (Commands.Command)GetValue(ClearCommandProperty); }
			set { SetValue(ClearCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearCommandProperty =
			DependencyProperty.Register("ClearCommand", typeof(Commands.Command), typeof(FilterBoxVm), new UIPropertyMetadata(null));
		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(FilterBoxVm), new UIPropertyMetadata(null));
	}
}
