using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soheil.Tablet.VM
{
	public class OperatorVm : DependencyObject
	{
		#region Properties and Events
		public event Action<OperatorVm> Updated;
		public Model.OperatorProcessReport Model { get; set; }
		private bool _isInitializing = true;
		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(OperatorVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates ProducedG1
		/// </summary>
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(OperatorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (OperatorVm)d;
				if (vm._isInitializing) return;

				var val = (int)e.NewValue;
				vm.Model.OperatorProducedG1 = val;
				if (vm.Updated != null)
					vm.Updated(vm);
			}));
		#endregion

		#region Ctor and Init
		public OperatorVm(Model.OperatorProcessReport model)
		{
			Model = model;
			Name = model.ProcessOperator.Operator.Name;
			ProducedG1 = model.OperatorProducedG1;
			_isInitializing = false;
		}
		#endregion

		#region Methods
		#endregion

		#region Commands
		#endregion
	}
}
