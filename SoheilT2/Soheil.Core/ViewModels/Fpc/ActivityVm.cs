using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ActivityVm : NamedVM
	{
		public ActivityVm()
		{

		}
		public ActivityVm(Model.Activity model, ActivityGroupVm groupVm)
		{
			Id = model.Id;
			Name = model.Name;
			Group = groupVm;
		}
		//Group Dependency Property
		public ActivityGroupVm Group
		{
			get { return (ActivityGroupVm)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public static readonly DependencyProperty GroupProperty =
			DependencyProperty.Register("Group", typeof(ActivityGroupVm), typeof(ActivityVm), new UIPropertyMetadata(null));
	}
}
