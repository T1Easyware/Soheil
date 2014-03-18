using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for a collection of <see cref="FilterableItemVm"/> with same type of model
	/// <para>Best to use this vm as ItemsSource of a ComboBox</para>
	/// </summary>
	public class FilterBoxVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of FilterBoxVm
		/// </summary>
		public FilterBoxVm()
		{
			ClearCommand = new Commands.Command(o => SelectedItem = null);
			DeleteCommand = new Commands.Command(o => { if (_parent != null) _parent.FilterBoxes.Remove(this); });
		}
		/// <summary>
		/// Creates an instance of FilterBoxVm to be used as a collection of product defections
		/// </summary>
		/// <param name="selectedId">Id of defection that is selected by default</param>
		/// <param name="productId">Id of product that this FilterBoxVm is defined for</param>
		/// <returns></returns>
		public static FilterBoxVm CreateForProductDefections(int selectedId, int productId)
		{
			var vm = new FilterBoxVm();

			//find and add product defections to this vm
			var productDefectionDS = new DataServices.ProductDefectionDataService();
			var productDefectionModels = productDefectionDS.GetActivesForProduct(productId);
			var count = productDefectionModels.Count();
			for (int i = 0; i < count; i++)
			{
				vm.FilteredList.Add(FilterableItemVm.CreateForProductDefection(vm, productDefectionModels[i]));
			}

			//select the default product defection
			vm.SelectedItem = vm.FilteredList.FirstOrDefault(x => x.Id == selectedId);

			return vm;
		}
		/// <summary>
		/// Creates an instance of FilterBoxVm to be used as a collection of causes
		/// </summary>
		/// <param name="nextLevel">reference to a FilterBoxVm that represents next cause level</param>
		/// <param name="list">list of first level cause models (use only for first level)</param>
		/// <returns></returns>
		public static FilterBoxVm CreateForCause(FilterBoxVm nextLevel, IEnumerable<Model.Cause> list = null)
		{
			var vm = new FilterBoxVm();
			vm._nextLevel = nextLevel;

			//fill the first level cause items
			if (list != null)
			{
				foreach (var item in list)
				{
					vm.FilteredList.Add(FilterableItemVm.CreateForCause(vm, item));
				}
			}
			return vm;
		}
		/// <summary>
		/// Creates an instance of FilterBoxVm to be used as a collection of operators
		/// </summary>
		/// <param name="parent">collection of FilterBoxVm that this vm is a part of</param>
		/// <param name="selectedId">Id of defection that is selected by default</param>
		/// <param name="list">array of FilterableItemVm instances as children of this vm</param>
		/// <returns></returns>
		public static FilterBoxVm CreateForGuiltyOperators(FilterBoxVmCollection parent, int selectedId, FilterableItemVm[] list)
		{
			var vm = new FilterBoxVm();
			vm._parent = parent;
			foreach (var item in list)
			{
				item.Parent = vm;
				vm.FilteredList.Add(item);
			}

			//select the default operator
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
		/// <summary>
		/// For usage in StoppageReport (Cause LevelX)
		/// </summary>
		public event Action<FilterBoxVm, FilterableItemVm> FilterBoxSelectedItemChanged;


		/// <summary>
		/// Gets or sets the bindable selected instance of <see cref="FilterableItemVm"/>
		/// </summary>
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
		/// Gets a bindable collection of items in this vm
		/// </summary>
		public ObservableCollection<FilterableItemVm> FilteredList { get { return _filteredList; } }
		private ObservableCollection<FilterableItemVm> _filteredList = new ObservableCollection<FilterableItemVm>();
		
		/// <summary>
		/// Gets a bindable command to deselect the selected item
		/// </summary>
		public Commands.Command ClearCommand
		{
			get { return (Commands.Command)GetValue(ClearCommandProperty); }
			set { SetValue(ClearCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearCommandProperty =
			DependencyProperty.Register("ClearCommand", typeof(Commands.Command), typeof(FilterBoxVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to delete this instance of <see cref="FilterBoxVm"/> from its parent
		/// <para>Used in guilty operators where custom number of FilterBoxVms can be in its parent</para>
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(FilterBoxVm), new UIPropertyMetadata(null));
	}
}
