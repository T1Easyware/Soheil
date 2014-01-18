using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorTask : DependencyObject
	{
		Model.Task _model;
		public int TaskId { get; set; }
		
		#region Ctor
		/// <summary>
		/// Must be called with an open connection
		/// </summary>
		/// <param name="model"></param>
		internal PPEditorTask(Model.Task model, PPEditorBlock editorParent)
		{
			_model = model;
			Block = editorParent;
			TaskId = model.Id;
			StartDate = model.StartDateTime.Date;
			StartTime = model.StartDateTime.TimeOfDay;
			foreach (var processModel in model.Processes)
			{
                //!!!
                //ActivityList.Add(new PPEditorProcess(this, processModel));
			}
			IsDeferToActivitiesSelected = true;
		}
		/// <summary>
		/// resets all activities within this task
		/// </summary>
		public void Reset()
		{
			foreach (var act in ActivityList)
			{
                //!!!
                //act.Reset();
			}
		}
		#endregion

		//Block Dependency Property
		public PPEditorBlock Block
		{
			get { return (PPEditorBlock)GetValue(BlockProperty); }
			set { SetValue(BlockProperty, value); }
		}
		public static readonly DependencyProperty BlockProperty =
			DependencyProperty.Register("Block", typeof(PPEditorBlock), typeof(PPEditorTask), new UIPropertyMetadata(null));

		#region Activity
		//ActivityList Observable Collection
		private ObservableCollection<PPEditorProcess> _activityList = new ObservableCollection<PPEditorProcess>();
		public ObservableCollection<PPEditorProcess> ActivityList { get { return _activityList; } }
		#endregion

		#region StartTime Issues
		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return ((Arash.PersianDate)GetValue(StartDateProperty)).ToDateTime(); }
			set { SetValue(StartDateProperty, new Arash.PersianDate(value)); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(Arash.PersianDate), typeof(PPEditorTask), new PropertyMetadata(Arash.PersianDate.Today));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(PPEditorTask), new UIPropertyMetadata(DateTime.Now.TimeOfDay));
		#endregion

		#region Qty Issues
		//TaskTargetPoint Dependency Property
		public int TaskTargetPoint
		{
			get { return (int)GetValue(TaskTargetPointProperty); }
			set { SetValue(TaskTargetPointProperty, value); }
		}
		public static readonly DependencyProperty TaskTargetPointProperty =
			DependencyProperty.Register("TaskTargetPoint", typeof(int), typeof(PPEditorTask), new PropertyMetadata(0));

		//IsSameTimeForActivitiesSelected Dependency Property
		public bool IsSameTimeForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameTimeForActivitiesSelectedProperty); }
			set { SetValue(IsSameTimeForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameTimeForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameTimeForActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PPEditorTask)d;
				if ((bool)e.NewValue)
				{
					vm.IsSameQtyForActivitiesSelected = false;
					vm.IsDeferToActivitiesSelected = false;
					foreach (var act in vm.ActivityList)
					{
                        //!!!
                        //act.TargetPoint = act.SelectedChoice == null ? 0 :
                        //    (int)(vm.SameTimeForActivities.TotalSeconds / act.SelectedChoice.CycleTime);
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
			DependencyProperty.Register("IsSameQtyForActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PPEditorTask)d;
				if ((bool)e.NewValue)
				{
					vm.IsSameTimeForActivitiesSelected = false;
					vm.IsDeferToActivitiesSelected = false;
					vm.SameQtyForActivities = vm.TaskTargetPoint;
					foreach (var act in vm.ActivityList)
					{
                        //!!!
                        //act.TargetPoint = vm.SameQtyForActivities;
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
			DependencyProperty.Register("IsDeferToActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = ((PPEditorTask)d);
				if ((bool)e.NewValue)
				{
					vm.IsSameTimeForActivitiesSelected = false;
					vm.IsSameQtyForActivitiesSelected = false;
				}
				foreach (var act in vm.ActivityList)
				{
                    //!!!
                    //act.DoesParentDeferToActivities = (bool)e.NewValue;
				}
			}));
		//SameTimeForActivities Dependency Property
		public TimeSpan SameTimeForActivities
		{
			get { return (TimeSpan)GetValue(SameTimeForActivitiesProperty); }
			set { SetValue(SameTimeForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameTimeForActivitiesProperty =
			DependencyProperty.Register("SameTimeForActivities", typeof(TimeSpan), typeof(PPEditorTask),
			new UIPropertyMetadata(new TimeSpan(1, 0, 0), (d, e) =>
			{
				var vm = (PPEditorTask)d;
				foreach (var act in vm.ActivityList)
				{
                    //!!!
                    //act.TargetPoint = act.SelectedChoice == null ? 0 :
                    //     (int)(((TimeSpan)e.NewValue).TotalSeconds / act.SelectedChoice.CycleTime);
				}
			}));
		//SameQtyForActivities Dependency Property
		public int SameQtyForActivities
		{
			get { return (int)GetValue(SameQtyForActivitiesProperty); }
			set { SetValue(SameQtyForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameQtyForActivitiesProperty =
			DependencyProperty.Register("SameQtyForActivities", typeof(int), typeof(PPEditorTask),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorTask)d;
				foreach (var act in vm.ActivityList)
				{
                    //!!!
                    //act.TargetPoint = (int)e.NewValue;
				}
			}));
		#endregion
	}
}
