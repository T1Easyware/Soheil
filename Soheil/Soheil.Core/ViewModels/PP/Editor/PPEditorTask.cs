using Soheil.Common;
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
		public int TaskId { get { return _model.Id; } }

		/// <summary>
		/// Use this event to notify Block about changes to Duration of this Task
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<TimeSpan, TimeSpan> TaskDurationChanged;

		/// <summary>
		/// Use this event to notify Block about changes to TargetPoint of this Task
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<int, int> TaskTargetPointChanged;

		#region Ctor
		/// <summary>
		/// Must be called with an open connection
		/// </summary>
		/// <param name="model"></param>
		internal PPEditorTask(Model.Task model, PPEditorBlock editorParent)
		{
			_model = model;
			Block = editorParent;
			StartDate = model.StartDateTime.Date;
			StartTime = model.StartDateTime.TimeOfDay;
			IsDeferToActivitiesSelected = true;
			initProcesses();
			initializeCommands();
		}
		void initProcesses()
		{
			ProcessList.Clear();
			_model.Processes.Clear();
			//?

			//convert and add each activity (ssaGroup) within current StateStation to ProcessList
			foreach (var ssaGroup in _model.Block.StateStation.StateStationActivities.GroupBy(ssa => ssa.Activity.Id))
			{
				PPEditorProcess processVm = null;
				//processModel: existing process which its ssa is in ssaGroup
				/*var processModel = _model.Processes.FirstOrDefault(p =>
					ssaGroup.Any(ssa => ssa.Id == p.StateStationActivity.Id));
				if (processModel != null)//a process exists matching a ssa in ssaGroup
				{
					processVm = new PPEditorProcess(this, processModel);
				}
				else//no process matches any ssa in ssaGroup
				{
					//processModel: existing process which its activity is in ssaGroup
					processModel = _model.Processes.FirstOrDefault(p => 
						ssaGroup.Any(ssa => ssa.Activity.Id == p.StateStationActivity.Activity.Id));
					if (processModel != null)//a process exists matching an activity in ssaGroup
					{
						//create new process as close as possible to ssaModel
						var ssaModel = ssaGroup.FirstOrDefault(ssa => ssa.ManHour == processModel.StateStationActivity.ManHour);
						if (ssaModel == null) ssaModel = ssaGroup.FirstOrDefault();
						processModel.StateStationActivity = ssaModel;
						processModel.Task = _model;
						processModel.Code = _model.Code + ssaModel.Activity.Code;
						processVm = new PPEditorProcess(this, processModel);
					}
					else
					{*/
						//create new process
						/*|||processVm = new PPEditorProcess(this, new Model.Process
						{
							StateStationActivity = ssaGroup.First(),
							Task = _model,
							Code = _model.Code + ssaGroup.First().Activity.Code,
						});*/
				processVm = new PPEditorProcess(this, ssaGroup);
					//}
				//}
				//finally add it to ProcessList
				ProcessList.Add(processVm);
			}
			//add leftover processes
			/*foreach (var processModel in _model.Processes.Where(x => !ProcessList.Any(y => y.ActivityId == x.StateStationActivity.Activity.Id)))
			{
				ProcessList.Add(new PPEditorProcess(this, processModel));
			}*/
			//set the event handlers
			foreach (var process in ProcessList)
			{
				process.ActivityChoiceChanged += (oldVal, newVal) =>
				{
					if (newVal == null) DurationSeconds = 0;
					else if (IsSameTimeForActivitiesSelected)
						process.TargetPoint = newVal.CycleTime == 0 ? 0 :
							(int)Math.Floor(SameTimeForActivities.TotalSeconds / newVal.CycleTime);
					else if (IsSameQtyForActivitiesSelected)
					{
						DurationSeconds = (int)Math.Floor(TaskTargetPoint * newVal.CycleTime);
					}
					else if (IsDeferToActivitiesSelected)
					{
						DurationSeconds = (int)Math.Floor(process.TargetPoint * newVal.CycleTime);
					}
					validateDuration(process);
				};
				process.ProcessTargetPointChanged += (oldVal, newVal) =>
				{
					if (newVal != SameQtyForActivities) IsDeferToActivitiesSelected = true;
					if (process.SelectedChoice == null) return;
					DurationSeconds = (int)Math.Floor(newVal * process.SelectedChoice.CycleTime);
					validateDuration(process);
				};
			}
		}
		/// <summary>
		/// Recreate all activities within this task
		/// </summary>
		public void Reset()
		{
			_model.Block = Block.Model;
			initProcesses();
		}
		#endregion
		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(PPEditorTask), new UIPropertyMetadata(true));
		//Block Dependency Property
		public PPEditorBlock Block
		{
			get { return (PPEditorBlock)GetValue(BlockProperty); }
			set { SetValue(BlockProperty, value); }
		}
		public static readonly DependencyProperty BlockProperty =
			DependencyProperty.Register("Block", typeof(PPEditorBlock), typeof(PPEditorTask), new UIPropertyMetadata(null));

		#region Process
		//ProcessList Observable Collection
		private ObservableCollection<PPEditorProcess> _processList = new ObservableCollection<PPEditorProcess>();
		public ObservableCollection<PPEditorProcess> ProcessList { get { return _processList; } }
		//SelectedProcess Dependency Property
		public PPEditorProcess SelectedProcess
		{
			get { return (PPEditorProcess)GetValue(SelectedProcessProperty); }
			set { SetValue(SelectedProcessProperty, value); }
		}
		public static readonly DependencyProperty SelectedProcessProperty =
			DependencyProperty.Register("SelectedProcess", typeof(PPEditorProcess), typeof(PPEditorTask), new UIPropertyMetadata(null));
		#endregion

		#region Time/Qty codes
		void validateDuration(PPEditorProcess except)
		{
			foreach (var process in ProcessList.Except(new PPEditorProcess[] { except }))
			{
				DurationSeconds = Math.Max(DurationSeconds, process.DurationSeconds);
			}
		}
		void durationChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldVal = e.OldValue == DependencyProperty.UnsetValue ? TimeSpan.Zero : (TimeSpan)e.OldValue;
			var newVal = e.NewValue == DependencyProperty.UnsetValue ? TimeSpan.Zero : (TimeSpan)e.NewValue;
			if (TaskDurationChanged != null)
				TaskDurationChanged(oldVal, newVal);
		}
		void taskTargetPointChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldVal = e.OldValue == DependencyProperty.UnsetValue ? 0 : (int)e.OldValue;
			var newVal = e.NewValue == DependencyProperty.UnsetValue ? 0 : (int)e.NewValue;
			if (TaskTargetPointChanged != null)
				TaskTargetPointChanged(oldVal, newVal);
			if (IsSameQtyForActivitiesSelected)
			{
				foreach (var process in ProcessList)
				{
					process.TargetPoint = newVal;
				}
			}
		}
		void isSameTimeForActivitiesSelectedChanged(bool newVal)
		{
			if (newVal)
			{
				IsSameQtyForActivitiesSelected = false;
				IsDeferToActivitiesSelected = false;
				foreach (var process in ProcessList)
				{
					process.DurationSeconds = (int)SameTimeForActivities.TotalSeconds;
				}
			}
		}
		void isSameQtyForActivitiesSelectedChanged(bool newVal)
		{
			if (newVal)
			{
				IsSameTimeForActivitiesSelected = false;
				IsDeferToActivitiesSelected = false;
				SameQtyForActivities = TaskTargetPoint;
				foreach (var process in ProcessList)
				{
					process.TargetPoint = SameQtyForActivities;
				}
			}
		}
		void isDeferToActivitiesSelectedChanged(bool newVal)
		{
			if (newVal)
			{
				IsSameTimeForActivitiesSelected = false;
				IsSameQtyForActivitiesSelected = false;
			}
			foreach (var process in ProcessList)
			{
				process.DoesParentDeferToActivities = newVal;
			}
		}
		void sameTimeForActivitiesChanged(TimeSpan newVal)
		{
			foreach (var process in ProcessList)
			{
				process.DurationSeconds = (int)newVal.TotalSeconds;
			}
			DurationSeconds = (int)newVal.TotalSeconds;
		}
		void sameQtyForActivitiesChanged(int newVal)
		{
			foreach (var process in ProcessList)
			{
				process.TargetPoint = newVal;
			}
		}

		#endregion

		#region Time/Qty Propdps (no code)
		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return ((DateTime)GetValue(StartDateProperty)); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(PPEditorTask), new PropertyMetadata(DateTime.Now.Date));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(PPEditorTask), new UIPropertyMetadata(DateTime.Now.TimeOfDay));
		//durations
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(PPEditorTask),
			new UIPropertyMetadata(0, (d, e) => d.SetValue(DurationProperty, new TimeSpan((int)e.NewValue * TimeSpan.TicksPerSecond))));
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPEditorTask),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) => ((PPEditorTask)d).durationChanged(e)));
		//End DateTime
		public DateTime EndDateTime { get { return StartDate.Add(StartTime).AddSeconds(DurationSeconds); } }
		//TaskTargetPoint Dependency Property
		public int TaskTargetPoint
		{
			get { return (int)GetValue(TaskTargetPointProperty); }
			set { SetValue(TaskTargetPointProperty, value); }
		}
		public static readonly DependencyProperty TaskTargetPointProperty =
			DependencyProperty.Register("TaskTargetPoint", typeof(int), typeof(PPEditorTask),
			new PropertyMetadata(0, (d, e) => ((PPEditorTask)d).taskTargetPointChanged(e)));
		//IsSameTimeForActivitiesSelected Dependency Property
		public bool IsSameTimeForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameTimeForActivitiesSelectedProperty); }
			set { SetValue(IsSameTimeForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameTimeForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameTimeForActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) => ((PPEditorTask)d).isSameTimeForActivitiesSelectedChanged((bool)e.NewValue)));

		//IsSameQtyForActivitiesSelected Dependency Property
		public bool IsSameQtyForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameQtyForActivitiesSelectedProperty); }
			set { SetValue(IsSameQtyForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameQtyForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameQtyForActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) => ((PPEditorTask)d).isSameQtyForActivitiesSelectedChanged((bool)e.NewValue)));
		//IsDeferToActivitiesSelected Dependency Property
		public bool IsDeferToActivitiesSelected
		{
			get { return (bool)GetValue(IsDeferToActivitiesSelectedProperty); }
			set { SetValue(IsDeferToActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsDeferToActivitiesSelectedProperty =
			DependencyProperty.Register("IsDeferToActivitiesSelected", typeof(bool), typeof(PPEditorTask),
			new UIPropertyMetadata(false, (d, e) => ((PPEditorTask)d).isDeferToActivitiesSelectedChanged((bool)e.NewValue)));
		//SameTimeForActivities Dependency Property
		public TimeSpan SameTimeForActivities
		{
			get { return (TimeSpan)GetValue(SameTimeForActivitiesProperty); }
			set { SetValue(SameTimeForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameTimeForActivitiesProperty =
			DependencyProperty.Register("SameTimeForActivities", typeof(TimeSpan), typeof(PPEditorTask),
			new UIPropertyMetadata(new TimeSpan(1, 0, 0), (d, e) => ((PPEditorTask)d).sameTimeForActivitiesChanged((TimeSpan)e.NewValue)));
		//SameQtyForActivities Dependency Property
		public int SameQtyForActivities
		{
			get { return (int)GetValue(SameQtyForActivitiesProperty); }
			set { SetValue(SameQtyForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameQtyForActivitiesProperty =
			DependencyProperty.Register("SameQtyForActivities", typeof(int), typeof(PPEditorTask),
			new UIPropertyMetadata(0, (d, e) => ((PPEditorTask)d).sameQtyForActivitiesChanged((int)e.NewValue)));
		#endregion

		#region Commands
		void initializeCommands()
		{
			DeleteTaskCommand = new Commands.Command(o => 
			{
				IsSelected = true;
				IsDeleteTaskMessageVisible = true;
			});
			ConfirmDeleteTaskCommand = new Commands.Command(o =>
			{
				try
				{
					Block.TaskDataService.DeleteModel(_model);
					Block.TaskList.Remove(this);
				}
				catch (Exception ex)
				{
					Block.Message.AddEmbeddedException(ex.Message);
				}
			});
			CancelDeleteTaskCommand = new Commands.Command(o => IsDeleteTaskMessageVisible = false);
		}
		//DeleteTaskCommand Dependency Property
		public Commands.Command DeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(DeleteTaskCommandProperty); }
			set { SetValue(DeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteTaskCommandProperty =
			DependencyProperty.Register("DeleteTaskCommand", typeof(Commands.Command), typeof(PPEditorTask), new UIPropertyMetadata(null));
		//IsDeleteTaskMessageVisible Dependency Property
		public bool IsDeleteTaskMessageVisible
		{
			get { return (bool)GetValue(IsDeleteTaskMessageVisibleProperty); }
			set { SetValue(IsDeleteTaskMessageVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsDeleteTaskMessageVisibleProperty =
			DependencyProperty.Register("IsDeleteTaskMessageVisible", typeof(bool), typeof(PPEditorTask), new UIPropertyMetadata(false));
		//ConfirmDeleteTaskCommand Dependency Property
		public Commands.Command ConfirmDeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(ConfirmDeleteTaskCommandProperty); }
			set { SetValue(ConfirmDeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty ConfirmDeleteTaskCommandProperty =
			DependencyProperty.Register("ConfirmDeleteTaskCommand", typeof(Commands.Command), typeof(PPEditorTask), new UIPropertyMetadata(null));
		//CancelDeleteTaskCommand Dependency Property
		public Commands.Command CancelDeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(CancelDeleteTaskCommandProperty); }
			set { SetValue(CancelDeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelDeleteTaskCommandProperty =
			DependencyProperty.Register("CancelDeleteTaskCommand", typeof(Commands.Command), typeof(PPEditorTask), new UIPropertyMetadata(null)); 
		#endregion

		internal void ForceCalculateDuration()
		{
			DurationSeconds = 0;
			foreach (var process in ProcessList)
			{
				DurationSeconds = Math.Max(DurationSeconds, process.SelectedChoice == null ? 0
					: (int)Math.Floor(process.TargetPoint * process.SelectedChoice.CycleTime));
			}
		}
	}
}
