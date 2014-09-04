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
		//List Observable Collection
		public ObservableCollection<DefectionReportVm> List { get { return _list; } }
		private ObservableCollection<DefectionReportVm> _list = new ObservableCollection<DefectionReportVm>();

		public override void Reset()
		{
			List.Clear();
			base.Reset();
		}
	}
}
