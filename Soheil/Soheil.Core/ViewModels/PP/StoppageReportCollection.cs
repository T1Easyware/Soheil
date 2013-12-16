using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StoppageReportCollection : DefectionStoppageCollectionBase
	{
		public StoppageReportCollection(ProcessReportCellVm parent)
		{
			Parent = parent;
			AddCommand = new Commands.Command(o => List.Add(new StoppageReportVm(this, null)));
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
