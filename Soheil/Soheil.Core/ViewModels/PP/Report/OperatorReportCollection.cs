using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class OperatorReportCollection : DependencyObject
	{
		public OperatorReportCollection(ProcessReportVm parent)
		{
			Parent = parent;
		}

		public ProcessReportVm Parent { get; private set; }

		//List Observable Collection
		public ObservableCollection<OperatorReportVm> List { get { return _list; } }
		private ObservableCollection<OperatorReportVm> _list = new ObservableCollection<OperatorReportVm>();

		public void Reset()
		{
			List.Clear();
		}

		internal void Add(OperatorReportVm operatorReportVm)
		{
			operatorReportVm.ProduceG1Changed += () =>
			{
				Parent.SumOfProducedG1 = List.Sum(x => x.ProducedG1);
			};
			List.Add(operatorReportVm);
		}
	}
}
