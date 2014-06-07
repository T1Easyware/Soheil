using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class DefectionReportCollection : DefectionStoppageCollectionBase
	{
		public DefectionReportCollection(ProcessReportVm parent)
		{
			Parent = parent;
			AddCommand = new Commands.Command(o =>
			{
				var model = new Model.DefectionReport
				{
					ProcessReport = parent.Model,
					LostCount = 0,
					LostTime = 0,
					ProductDefection = null,
					ModifiedBy = LoginInfo.Id,
				};
				parent.Model.DefectionReports.Add(model);
				var vm = new DefectionReportVm(this, model);
				List.Add(vm);
			});
		}

		//List Observable Collection
		private ObservableCollection<DefectionReportVm> _list = new ObservableCollection<DefectionReportVm>();
		public ObservableCollection<DefectionReportVm> List { get {return _list;} }

		public override void UpdateParentSumOfCount(int newValue)
		{
			Parent.DefectionCount = newValue;
		}
		public override void Reset()
		{
			List.Clear();
			base.Reset();
		}
	}
}
