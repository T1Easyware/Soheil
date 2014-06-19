using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class OperatorEditorVm : OperatorVm
	{
		/// <summary>
		/// Occurs when this operator is selected/deselected
		/// <para>params: operator, IsSelected, value that indicates whether to update SelectedOperatorCount</para>
		/// </summary>
		public event Action<OperatorEditorVm, bool, bool> SelectedOperatorChanged;
		
		#region Ctor
		/// <summary>
		/// Use this constructor to create an operator outside a process
		/// </summary>
		/// <param name="model"></param>
		public OperatorEditorVm(Model.Operator model)
			: base(model)
		{
			initializeCommands();
		}

		bool _changeOperatorCount = false;
		void initializeCommands()
		{
			SelectCommand = new Commands.Command(o =>
			{
				_changeOperatorCount = true;
				IsSelected = !IsSelected;
				_changeOperatorCount = false;
			});
		}

		#endregion

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this operator is in this process
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(OperatorEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (OperatorEditorVm)d;
				if (vm.SelectedOperatorChanged != null)
					vm.SelectedOperatorChanged(vm, (bool)e.NewValue, vm._changeOperatorCount);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this operator is (also) in other processes of this task
		/// </summary>
		public bool IsInTask
		{
			get { return (bool)GetValue(IsInTaskProperty); }
			set { SetValue(IsInTaskProperty, value); }
		}
		public static readonly DependencyProperty IsInTaskProperty =
			DependencyProperty.Register("IsInTask", typeof(bool), typeof(OperatorEditorVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this operator is in this time range (other stations)
		/// </summary>
		public bool IsInTimeRange
		{
			get { return (bool)GetValue(IsInTimeRangeProperty); }
			set { SetValue(IsInTimeRangeProperty, value); }
		}
		public static readonly DependencyProperty IsInTimeRangeProperty =
			DependencyProperty.Register("IsInTimeRange", typeof(bool), typeof(OperatorEditorVm), new UIPropertyMetadata(false));

		//SelectCommand Dependency Property
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(OperatorEditorVm), new UIPropertyMetadata(null));


	}
}
