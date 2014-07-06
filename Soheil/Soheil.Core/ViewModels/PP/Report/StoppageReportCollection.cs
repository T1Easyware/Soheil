using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class StoppageReportCollection : DefectionStoppageCollectionBase
	{
		public StoppageReportCollection(ProcessReportVm parent)
		{
			Parent = parent;
			AddCommand = new Commands.Command(o =>
			{
				var model = new Model.StoppageReport
				{
					ProcessReport = parent.Model,
					LostCount = 0,
					LostTime = 0,
					Cause = null,
					ModifiedBy = LoginInfo.Id,
				};
				foreach (var po in Parent.Model.Process.ProcessOperators)
				{
					model.OperatorStoppageReports.Add(new Model.OperatorStoppageReport
					{
						Operator = po.Operator,
						StoppageReport = model,
						Code = po.Operator.Code,
						ModifiedBy = LoginInfo.Id,
					});
				}
				parent.Model.StoppageReports.Add(model);
				var vm = new StoppageReportVm(this, model);
				List.Add(vm);
			});
		}

		
		public ObservableCollection<StoppageReportVm> List { get { return _list; } }
		private ObservableCollection<StoppageReportVm> _list = new ObservableCollection<StoppageReportVm>();
		
		public override void UpdateParentSumOfCount(int newValue)
		{
			Parent.StoppageCount = newValue;
		}
		public override void Reset()
		{
			List.Clear();
			base.Reset();
		}
	}
}
