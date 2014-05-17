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
		public event Action SelectedOperatorsChanged;
		
		#region Ctor
		/// <summary>
		/// Use this constructor to create an operator outside a process
		/// </summary>
		/// <param name="model"></param>
		public OperatorEditorVm(Model.Operator model, Model.StateStationActivity ssa)
			: base(model, ssa)
		{
		}

		/// <summary>
		/// Use this constructor to create an operator inside a process
		/// </summary>
		/// <param name="model"></param>
		public OperatorEditorVm(Model.ProcessOperator model)
			: base(model)
		{
			IsSelected = true;
		}
		public OperatorEditorVm(Model.Operator model)
			: base(model)
		{

		}

		#endregion


		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(OperatorEditorVm),
			new UIPropertyMetadata(false, (d, e) => { if (((OperatorEditorVm)d).SelectedOperatorsChanged != null) ((OperatorEditorVm)d).SelectedOperatorsChanged(); }));
	}
}
