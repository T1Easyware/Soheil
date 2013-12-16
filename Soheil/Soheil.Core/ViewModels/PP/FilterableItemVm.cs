using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class FilterableItemVm : DependencyObject
	{
		private FilterableItemVm(int id, string text, DependencyObject vm = null)
		{
			Id = id;
			Text = text;
			ViewModel = vm;
			SelectCommand = new Commands.Command(o => Parent.SelectedItem = this);
		}
		//remember to set the parent of FilterableItemVm properly
		public static FilterableItemVm CreateForGuiltyOperator(Model.Operator model)
		{
			var vm = new FilterableItemVm(model.Id, model.Name);
			return vm;
		}
		public static FilterableItemVm CreateForProductDefection(FilterBoxVm parent, Model.ProductDefection model)
		{
			var vm = new FilterableItemVm(model.Id, model.Defection.Name, new DefectionVm(model));
			vm.Parent = parent;
			return vm;
		}
		public static FilterableItemVm CreateForCause(FilterBoxVm parent, Model.Cause model)
		{
			var vm = new FilterableItemVm(model.Id, model.Name, new CauseVm(model));
			vm.Parent = parent;
			return vm;
		}


		public FilterBoxVm Parent { get; set; }
		/// <summary>
		/// ProductDefection Id or Operator Id
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// ViewModel Dependency Property. Set to Null if not useful
		/// </summary>
		public DependencyObject ViewModel
		{
			get { return (DependencyObject)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel", typeof(DependencyObject), typeof(FilterableItemVm), new UIPropertyMetadata(null));
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(FilterableItemVm), new UIPropertyMetadata(null));
		//SelectCommand Dependency Property
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(FilterableItemVm), new UIPropertyMetadata(null));
	}
}
