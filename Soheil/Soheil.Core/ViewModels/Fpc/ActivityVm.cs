using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ActivityVm : ViewModelBase, IToolboxData
	{
		public Model.Activity Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}

		public ActivityVm(Model.Activity model, ActivityGroupVm groupVm)
		{
			Model = model;
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
