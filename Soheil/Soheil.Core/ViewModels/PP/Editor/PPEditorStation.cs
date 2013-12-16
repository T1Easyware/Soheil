using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorStation : DependencyObject
	{
		#region Ctor
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		internal PPEditorStation(PPEditorState parent, Model.Task model)
		{
			_model = model;
			_parent = parent;
			StationId = model.StateStation.Station.Id;
			StationIndex = model.StateStation.Station.Index;
			TaskId = model.Id;
			StateStationId = model.StateStation.Id;
			StartDate = model.StartDateTime.Date;
			StartTime = model.StartDateTime.TimeOfDay;
			Name = model.StateStation.Station.Name;
			foreach (var processModel in model.Processes)
			{
				ActivityList.Add(new PPEditorActivity(this, processModel));
			}
			IsDeferToActivitiesSelected = true;
		}
		internal PPEditorStation(PPEditorState parent, Fpc.StateStationVm ss)
		{
			_parent = parent;
			StationId = ss.Containment.Id;
			StationIndex = ss.Containment.Id;//???
			StateId = ss.Container.Id;
			StateStationId = ss.Id;
			TaskId = -1;
			Name = ss.Name;
			foreach (Fpc.StateStationActivityVm ssa in ss.ContentsList)
			{
				ActivityList.Add(new PPEditorActivity(this, ssa));
			}
			IsDeferToActivitiesSelected = true;
		}
		public void Reset()
		{
			foreach (var act in ActivityList)
			{
				act.Reset();
			}
			HasUnsavedChanges = false;
		}
		#endregion

		PPEditorState _parent;
		Model.Task _model;
		public int StationId { get; set; }
		public int StationIndex { get; set; }
		public int StateId { get; set; }
		public int StateStationId { get; set; }
		public int TaskId { get; set; }



		#region Name And ActivityList
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorStation), new UIPropertyMetadata(null));

		//ActivityList Observable Collection
		private ObservableCollection<PPEditorActivity> _activityList = new ObservableCollection<PPEditorActivity>();
		public ObservableCollection<PPEditorActivity> ActivityList { get { return _activityList; } }
		#endregion

		#region StartTime Issues
		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return ((Arash.PersianDate)GetValue(StartDateProperty)).ToDateTime(); }
			set { SetValue(StartDateProperty, new Arash.PersianDate(value)); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(Arash.PersianDate), typeof(PPEditorStation), new PropertyMetadata(Arash.PersianDate.Today));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(PPEditorStation), new UIPropertyMetadata(DateTime.Now.TimeOfDay));
		//IsAutoStart Dependency Property
		public bool IsAutoStart
		{
			get { return (bool)GetValue(IsAutoStartProperty); }
			set { SetValue(IsAutoStartProperty, value); }
		}
		public static readonly DependencyProperty IsAutoStartProperty =
			DependencyProperty.Register("IsAutoStart", typeof(bool), typeof(PPEditorStation), new PropertyMetadata(false));

		public void SetToToday()
		{
			StartDate = DateTime.Now.Date;
		}
		public void SetToTomorrow()
		{
			StartDate = DateTime.Now.AddDays(1).Date;
		}
		public void SetToNextHour()
		{
			StartTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
		}

		#endregion

		#region Qty Issues
		//TaskTargetPoint Dependency Property
		public int TaskTargetPoint
		{
			get { return (int)GetValue(TaskTargetPointProperty); }
			set { SetValue(TaskTargetPointProperty, value); }
		}
		public static readonly DependencyProperty TaskTargetPointProperty =
			DependencyProperty.Register("TaskTargetPoint", typeof(int), typeof(PPEditorStation), new PropertyMetadata(0));

		//IsSameTimeForActivitiesSelected Dependency Property
		public bool IsSameTimeForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameTimeForActivitiesSelectedProperty); }
			set { SetValue(IsSameTimeForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameTimeForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameTimeForActivitiesSelected", typeof(bool), typeof(PPEditorStation),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PPEditorStation)d;
				if ((bool)e.NewValue)
				{
					vm.IsSameQtyForActivitiesSelected = false;
					vm.IsDeferToActivitiesSelected = false;
					foreach (var act in vm.ActivityList)
					{
						act.TargetPoint = (int)(vm.SameTimeForActivities.TotalSeconds / act.CycleTime);
					}
				}
			}));
		//IsSameQtyForActivitiesSelected Dependency Property
		public bool IsSameQtyForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameQtyForActivitiesSelectedProperty); }
			set { SetValue(IsSameQtyForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameQtyForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameQtyForActivitiesSelected", typeof(bool), typeof(PPEditorStation),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PPEditorStation)d;
				if ((bool)e.NewValue)
				{
					vm.IsSameTimeForActivitiesSelected = false;
					vm.IsDeferToActivitiesSelected = false;
					vm.SameQtyForActivities = vm.TaskTargetPoint;
					foreach (var act in vm.ActivityList)
					{
						act.TargetPoint = vm.SameQtyForActivities;
					}
				}
			}));
		//IsDeferToActivitiesSelected Dependency Property
		public bool IsDeferToActivitiesSelected
		{
			get { return (bool)GetValue(IsDeferToActivitiesSelectedProperty); }
			set { SetValue(IsDeferToActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsDeferToActivitiesSelectedProperty =
			DependencyProperty.Register("IsDeferToActivitiesSelected", typeof(bool), typeof(PPEditorStation),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = ((PPEditorStation)d);
				if ((bool)e.NewValue)
				{
					vm.IsSameTimeForActivitiesSelected = false;
					vm.IsSameQtyForActivitiesSelected = false;
				}
				foreach (var act in vm.ActivityList)
				{
					act.DoesParentDeferToActivities = (bool)e.NewValue;
				}
			}));
		//SameTimeForActivities Dependency Property
		public TimeSpan SameTimeForActivities
		{
			get { return (TimeSpan)GetValue(SameTimeForActivitiesProperty); }
			set { SetValue(SameTimeForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameTimeForActivitiesProperty =
			DependencyProperty.Register("SameTimeForActivities", typeof(TimeSpan), typeof(PPEditorStation),
			new UIPropertyMetadata(new TimeSpan(1, 0, 0), (d, e) =>
			{
				var vm = (PPEditorStation)d;
				foreach (var act in vm.ActivityList)
				{
					act.TargetPoint = (int)(((TimeSpan)e.NewValue).TotalSeconds / act.CycleTime);
				}
			}));
		//SameQtyForActivities Dependency Property
		public int SameQtyForActivities
		{
			get { return (int)GetValue(SameQtyForActivitiesProperty); }
			set { SetValue(SameQtyForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameQtyForActivitiesProperty =
			DependencyProperty.Register("SameQtyForActivities", typeof(int), typeof(PPEditorStation),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorStation)d;
				foreach (var act in vm.ActivityList)
				{
					act.TargetPoint = (int)e.NewValue;
				}
			}));
		#endregion

		//HasUnsavedChanges Dependency Property
		public bool HasUnsavedChanges
		{
			get { return (bool)GetValue(HasUnsavedChangesProperty); }
			set { SetValue(HasUnsavedChangesProperty, value); }
		}
		public static readonly DependencyProperty HasUnsavedChangesProperty =
			DependencyProperty.Register("HasUnsavedChanges", typeof(bool), typeof(PPEditorStation),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
					((PPEditorStation)d)._parent.HasUnsavedChanges = true;
			}));
	}
}
