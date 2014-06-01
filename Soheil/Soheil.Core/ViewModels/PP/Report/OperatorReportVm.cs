using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class OperatorReportVm : OperatorVm
	{
		public event Action ProduceG1Changed;
		public Model.OperatorProcessReport Model { get; private set; }

		#region Ctor
		/// <summary>
		/// Use this constructor to use an existing OperatorProcessReport
		/// </summary>
		/// <param name="model">role and all skill related relations are used</param>
		public OperatorReportVm(Model.OperatorProcessReport model)
			:base(model.ProcessOperator)
		{
			Model = model;
			ProducedG1 = model.OperatorProducedG1;
		}

		#endregion

		/// <summary>
		/// Gets or sets a bindable value for operator produced G1
		/// </summary>
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(OperatorReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = d as OperatorReportVm;
				vm.Model.OperatorProducedG1 = (int)e.NewValue;
				if (vm.ProduceG1Changed != null) vm.ProduceG1Changed();
			}));
	}
}
