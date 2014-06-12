using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ActivityGroupVm : ViewModelBase, IToolboxData
	{
		public Model.ActivityGroup Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}

		public ActivityGroupVm() { }
		public ActivityGroupVm(Model.ActivityGroup model)
		{
			Model = model;
			foreach (var act in model.Activities.Where(x=>x.RecordStatus == Common.Status.Active))
			{
				Activities.Add(new ActivityVm(act, this));
			}
		}
		//activities Observable Collection
		public ObservableCollection<ActivityVm> Activities { get { return _activities; } }
		private ObservableCollection<ActivityVm> _activities = new ObservableCollection<ActivityVm>();

		//IsExpanded Dependency Property
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ActivityGroupVm), new UIPropertyMetadata(false));
	}
}
