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
		public ObservableCollection<StoppageReportVm> List { get { return _list; } }
		private ObservableCollection<StoppageReportVm> _list = new ObservableCollection<StoppageReportVm>();
		
		public override void Reset()
		{
			List.Clear();
			base.Reset();
		}
	}
}
