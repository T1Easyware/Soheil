using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ActivityGroupVm : NamedVM
	{
		public ActivityGroupVm() { }
		public ActivityGroupVm(Model.ActivityGroup model)
		{
			Id = model.Id;
			Name = model.Name;
			foreach (var act in model.Activities)
			{
				Activities.Add(new ActivityVm(act, this));
			}
		}
		//activities Observable Collection
		private ObservableCollection<ActivityVm> _activities = new ObservableCollection<ActivityVm>();
		public ObservableCollection<ActivityVm> Activities { get { return _activities; } }
	}
}
