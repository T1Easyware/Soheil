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
			AddCommand = new Commands.Command(o => List.Add(new StoppageReportVm(this, new Model.StoppageReport
			{
				ProcessReport = parent.Model,
				Cause = null,
				ModifiedBy = LoginInfo.Id,
			})));
		}

		//List Observable Collection
		private ObservableCollection<StoppageReportVm> _list = new ObservableCollection<StoppageReportVm>();
		public ObservableCollection<StoppageReportVm> List { get { return _list; } }
		
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
