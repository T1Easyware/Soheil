using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for a filterable item that can be used in a <see cref="FilterBoxVm"/>
	/// </summary>
	public class FilterableItemVm : DependencyObject
	{
		/// <summary>
		/// Gets or sets the parent of this FilterableItemVm
		/// </summary>
		public FilterBoxVm Parent { get; set; }
		/// <summary>
		/// Gets the CauseId, ProductDefectionId or OperatorId
		/// </summary>
		public int Id { get; protected set; }

		/// <summary>
		/// Item's Operator Model (in guilty operators collection)
		/// </summary>
		public dynamic Model { get; set; }

		/// <summary>
		/// Creates an instance of FilterableItemVm
		/// </summary>
		/// <param name="id">Id of the model represented by this vm</param>
		/// <param name="text">Text to show for this Vm in GUI</param>
		/// <param name="vm">Use this parameter only if this FilterableItemVm is a wrapper around the given vm (parameter)</param>
		private FilterableItemVm(int id, string text, DependencyObject vm = null)
		{
			Id = id;
			Text = text;
			ViewModel = vm;
			SelectCommand = new Commands.Command(o => Parent.SelectedItem = this);
		}
		/// <summary>
		/// Creates an instance of FilterableItemVm for guilty operator provided by the given model
		/// <para>This constructor does not set the Parent so remember to set it manually</para>
		/// </summary>
		/// <param name="model">model of operator to use in this vm</param>
		/// <returns></returns>
		public static FilterableItemVm CreateForGuiltyOperator(Model.Operator model)
		{
			var vm = new FilterableItemVm(model.Id, model.Name);
			vm.Model = model;
			return vm;
		}
		/// <summary>
		/// Creates an instance of FilterableItemVm for product defection provided by the given model
		/// <para>This constructor needs a <see cref="FilterBoxVm"/> as parent</para>
		/// </summary>
		/// <param name="model">model of product defection to use in this vm</param>
		/// <returns></returns>
		public static FilterableItemVm CreateForProductDefection(FilterBoxVm parent, Model.ProductDefection model)
		{
			var vm = new FilterableItemVm(model.Id, model.Defection.Name, new DefectionVm(model));
			vm.Parent = parent;
			return vm;
		}
		/// <summary>
		/// Creates an instance of FilterableItemVm for cause provided by the given model
		/// <para>This constructor needs a <see cref="FilterBoxVm"/> as parent</para>
		/// </summary>
		/// <param name="model">model of cause to use in this vm</param>
		/// <returns></returns>
		public static FilterableItemVm CreateForCause(FilterBoxVm parent, Model.Cause model)
		{
			var vm = new FilterableItemVm(model.Id, model.Name, new CauseVm(model));
			vm.Parent = parent;
			return vm;
		}

		/// <summary>
		/// Gets a bindable value for another viewModel which this FilterableItemVm wraps around
		/// <para>Null if no other ViewModel is wrapped by this FilterableItemVm</para>
		/// </summary>
		public DependencyObject ViewModel
		{
			get { return (DependencyObject)GetValue(ViewModelProperty); }
			protected set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel", typeof(DependencyObject), typeof(FilterableItemVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable value for Text of this ViewModel to show in GUI
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			protected set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FilterableItemVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command that handles the selection of this vm
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			protected set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(FilterableItemVm), new UIPropertyMetadata(null));
	}
}
