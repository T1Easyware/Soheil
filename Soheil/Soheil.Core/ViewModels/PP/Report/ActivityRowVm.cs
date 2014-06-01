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
				ProcessRowList.Add(new ProcessRowVm(ssa));
			}
		}
		public ObservableCollection<ProcessRowVm> ProcessRowList { get { return _processRowList; } }
		private ObservableCollection<ProcessRowVm> _processRowList = new ObservableCollection<ProcessRowVm>();
	}
}
