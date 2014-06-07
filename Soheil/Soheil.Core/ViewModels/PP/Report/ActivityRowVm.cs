using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ActivityRowVm : ActivityVm
	{
		public ActivityRowVm(Model.Activity model)
			: base(model)
		{
			foreach (var ssa in model.StateStationActivities.OrderBy(x => x.ManHour))
			{
				SsaRowList.Add(new SsaRowVm(ssa));
			}
		}
		public ObservableCollection<SsaRowVm> SsaRowList { get { return _ssaRowList; } }
		private ObservableCollection<SsaRowVm> _ssaRowList = new ObservableCollection<SsaRowVm>();
	}
}
