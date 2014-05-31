using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class TaskEditorVm : DependencyObject
	{
		/// <summary>
		/// Gets the Task model
		/// </summary>
		internal Model.Task Model { get; private set; }

		Dal.SoheilEdmContext _uow;

		/// <summary>
		/// Occurs when delete task request is confirmed
		/// </summary>
		public event Action DeleteTaskConfirmed;

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

		#region Ctor and methods
		/// <summary>
		/// Must be called with an open connection
		/// </summary>
		/// <param name="model"></param>
		internal TaskEditorVm(Model.Task model, Dal.SoheilEdmContext uow)
		{
			Model = model;
			_uow = uow;
            initializeCommands();
			OperatorManager = new OperatorManagerVm(_uow);
			OperatorManager.SelectionChanged += (operatorVm, isSelected, role) =>
			{
				if (SelectedProcess == null) return;
				if (isSelected)
				{
					SelectedProcess.Model.ProcessOperators.Add(new Model.ProcessOperator
					{
						Process = SelectedProcess.Model,
						Operator = operatorVm.OperatorModel,
						Role = role,
						Code = model.Code + operatorVm.Code,
					});
					SelectedProcess.SelectedOperatorsCount++;
				}
				else
				{
					var po = SelectedProcess.Model.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operatorVm.OperatorId);
					if (po != null) SelectedProcess.Model.ProcessOperators.Remove(po);
					SelectedProcess.SelectedOperatorsCount--;
				}
			};

			StartDate = model.StartDateTime.Date;
			StartTime = model.StartDateTime.TimeOfDay;

            //subValues
			TaskTargetPoint = model.TaskTargetPoint;
			DurationSeconds = model.DurationSeconds;

			#region Auto check RadioButtons
			if (!model.Processes.Any())
			{
				IsDeferToActivitiesSelected = true;
			}
			else if (model.Processes.AreAllEqual(x => x.TargetCount))
			{
				SameQtyForActivities = model.Processes.FirstOrDefault().TargetCount;
				IsSameQtyForActivitiesSelected = true;
			}
			// if all processes have same duration then
			//		DurationOfProcess = TP * CT
			//		=> DurationOfTask = TP * CT + d, where (0 <= d < CT)
			//		=> d = DurationOfTask - TP * CT
			//		=> 0 <= (DurationOfTask - TP * CT)/CT < 1
			else if (model.Processes.Select(
				p => (DurationSeconds - p.TargetCount * p.StateStationActivity.CycleTime) / (float)p.StateStationActivity.CycleTime)
				.All(diff => 0 <= diff && diff < 1f))
			{
				SameTimeForActivities = TimeSpan.FromSeconds(model.Processes.Max(p => p.TargetCount * p.StateStationActivity.CycleTime));
				IsSameTimeForActivitiesSelected = true;
			}
			else
			{
				IsDeferToActivitiesSelected = true;
			} 
			#endregion

			RebuildProcesses();
		}
		/// <summary>
		/// Recreate all activities within this task
		/// </summary>
		internal void RebuildProcesses()
		{
			ProcessList.Clear();

			//convert and add each activity (ssaGroup) within current StateStation to ProcessList
            //note that: each group of SSA (grouped by activity) MUST be added ONCE (not 0 not 5)
			//			if SSA of one of processModels is in ssaGroup, consider that SSA
			//			else create a new processModel
			foreach (var ssaGroup in Model.Block.StateStation.StateStationActivities.GroupBy(ssa => ssa.Activity.Id))
			{
				ProcessEditorVm processVm = null;

				#region processVm = Create a new PPEditorProcess for this ssaGroup
				//processModel: existing process which its ssa is in ssaGroup
				var processModel = Model.Processes
					.Where(p => p.StateStationActivity != null)
					.FirstOrDefault(p => ssaGroup.Any(ssa => ssa.Id == p.StateStationActivity.Id));
				if (processModel != null)//a process exists matching a ssa in ssaGroup
				{
					processVm = new ProcessEditorVm(processModel, _uow);
				}

				else//no process matches any ssa in ssaGroup
				{
					processModel = new Model.Process();
					processModel.Task = Model;
					processVm = new ProcessEditorVm(processModel, _uow, ssaGroup);
				} 
				#endregion

				#region Set the event handlers of processVm
				processVm.ActivityChoiceChanged += (oldVal, newVal) =>
				{
					if (newVal == null) return;

					//update TargetPoint of process
					if (IsSameQtyForActivitiesSelected)
					{
						processVm.TargetPoint = SameQtyForActivities;
						var newDuration = SameQtyForActivities * (int)newVal.CycleTime;
						if (newDuration != processVm.DurationSeconds)
							processVm.DurationSeconds = newDuration;
					}
					else if (IsSameTimeForActivitiesSelected)
					{
						processVm.TargetPoint = (int)Math.Floor(SameTimeForActivities.TotalSeconds / newVal.CycleTime);
						var newDuration = processVm.TargetPoint * (int)newVal.CycleTime;
						if (newDuration != processVm.DurationSeconds)
							processVm.DurationSeconds = newDuration;
					}
					else
					{
						if (processVm.TargetPoint > 0)
							processVm.DurationSeconds = (int)Math.Floor(processVm.TargetPoint * newVal.CycleTime);
						else if (processVm.DurationSeconds > 0)
							processVm.TargetPoint = (int)Math.Floor(processVm.DurationSeconds / newVal.CycleTime);
					}
				};
				processVm.ProcessTargetPointChanged += (oldVal, newVal) =>
				{
				};
				processVm.ProcessDurationChanged += (oldVal, newVal) =>
				{
					DurationSeconds = ProcessList.Max(x => x.DurationSeconds);
				}; 
				#endregion

				//finally add it to ProcessList
				ProcessList.Add(processVm);
			}
		}
		internal void ForceCalculateDuration()
		{
			DurationSeconds = 0;
			foreach (var process in ProcessList)
			{
				DurationSeconds = Math.Max(DurationSeconds, process.SelectedChoice == null ? 0
					: (int)Math.Floor(process.TargetPoint * process.SelectedChoice.CycleTime));
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this task is selected in the parent block
		/// <remarks>If used with a Selector no need to deselect other tasks manually (i.e. in a tab control)</remarks>
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(TaskEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				(d.GetValue(OperatorManagerProperty) as OperatorManagerVm).
					Refresh(d as TaskEditorVm);
			}));

		/// <summary>
		/// Gets or sets the bindable operator manager to manager operators of this task
		/// </summary>
		public OperatorManagerVm OperatorManager
		{
			get { return (OperatorManagerVm)GetValue(OperatorManagerProperty); }
			set { SetValue(OperatorManagerProperty, value); }
		}
		public static readonly DependencyProperty OperatorManagerProperty =
			DependencyProperty.Register("OperatorManager", typeof(OperatorManagerVm), typeof(TaskEditorVm), new UIPropertyMetadata(null));

		#region Process
		/// <summary>
		/// Gets a bindable collection of processes in this task
		/// </summary>
		public ObservableCollection<ProcessEditorVm> ProcessList { get { return _processList; } }
		private ObservableCollection<ProcessEditorVm> _processList = new ObservableCollection<ProcessEditorVm>();
		
		/// <summary>
		/// Gets or sets the bindable process which is selected in the current task
		/// </summary>
		public ProcessEditorVm SelectedProcess
		{
			get { return (ProcessEditorVm)GetValue(SelectedProcessProperty); }
			set { SetValue(SelectedProcessProperty, value); }
		}
		public static readonly DependencyProperty SelectedProcessProperty =
			DependencyProperty.Register("SelectedProcess", typeof(ProcessEditorVm), typeof(TaskEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
				vm.OperatorManager.Refresh((ProcessEditorVm)e.NewValue);
			}));
		#endregion


		#region Time/Qty Propdps
		/// <summary>
		/// Gets or sets the bindable StartDate of this task
		/// <para>Changing the value updates StartDateTime of model and EndDateTime</para>
		/// </summary>
		public DateTime StartDate
		{
			get { return ((DateTime)GetValue(StartDateProperty)); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(TaskEditorVm),
			new PropertyMetadata(DateTime.Now.Date, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
				var val = (DateTime)e.NewValue;
				vm.Model.StartDateTime = val.Add(vm.StartTime);
				vm.EndDateTime = vm.Model.StartDateTime.AddSeconds(vm.DurationSeconds);
			}));
		/// <summary>
		/// Gets or sets the bindable StartTime of this task
		/// <para>Changing the value updates StartDateTime of model and EndDateTime</para>
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(TaskEditorVm),
			new PropertyMetadata(DateTime.Now.TimeOfDay, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
				var val = (TimeSpan)e.NewValue;
				vm.Model.StartDateTime = vm.StartDate.Add(val);
				vm.EndDateTime = vm.Model.StartDateTime.AddSeconds(vm.DurationSeconds);
			}));
		/// <summary>
		/// Gets or sets the bindable EndDateTime of this task
		/// <para>Changing the value updates EndDateTime of model</para>
		/// </summary>
		public DateTime EndDateTime
		{
			get { return (DateTime)GetValue(EndDateTimeProperty); }
			set { SetValue(EndDateTimeProperty, value); }
		}
		public static readonly DependencyProperty EndDateTimeProperty =
			DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(TaskEditorVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
				var val = (DateTime)e.NewValue;
				vm.Model.EndDateTime = val;
			}));

		/// <summary>
		/// Gets or sets the bindable DurationSeconds of this task
		/// <para>Changing the value updates DurationSeconds of model and EndDateTime, also fires TaskDurationChanged</para>
		/// </summary>
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TaskEditorVm),
			new UIPropertyMetadata(0, (d, e) => d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue))));
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TaskEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) => ((TaskEditorVm)d).durationChanged(e)));

		/// <summary>
		/// Gets or sets the bindable TaskTargetPoint of this task
		/// <para>Changing this value updates model's TP and also fires TaskTargetPointChanged event</para>
		/// </summary>
		public int TaskTargetPoint
		{
			get { return (int)GetValue(TaskTargetPointProperty); }
			set { SetValue(TaskTargetPointProperty, value); }
		}
		public static readonly DependencyProperty TaskTargetPointProperty =
			DependencyProperty.Register("TaskTargetPoint", typeof(int), typeof(TaskEditorVm),
			new PropertyMetadata(0, (d, e) => ((TaskEditorVm)d).taskTargetPointChanged(e)));
		
		//IsSameTimeForActivitiesSelected Dependency Property
		public bool IsSameTimeForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameTimeForActivitiesSelectedProperty); }
			set { SetValue(IsSameTimeForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameTimeForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameTimeForActivitiesSelected", typeof(bool), typeof(TaskEditorVm),
			new UIPropertyMetadata(false, (d, e) => ((TaskEditorVm)d).isSameTimeForActivitiesSelectedChanged((bool)e.NewValue)));

		//IsSameQtyForActivitiesSelected Dependency Property
		public bool IsSameQtyForActivitiesSelected
		{
			get { return (bool)GetValue(IsSameQtyForActivitiesSelectedProperty); }
			set { SetValue(IsSameQtyForActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSameQtyForActivitiesSelectedProperty =
			DependencyProperty.Register("IsSameQtyForActivitiesSelected", typeof(bool), typeof(TaskEditorVm),
			new UIPropertyMetadata(false, (d, e) => ((TaskEditorVm)d).isSameQtyForActivitiesSelectedChanged((bool)e.NewValue)));
		//IsDeferToActivitiesSelected Dependency Property
		public bool IsDeferToActivitiesSelected
		{
			get { return (bool)GetValue(IsDeferToActivitiesSelectedProperty); }
			set { SetValue(IsDeferToActivitiesSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsDeferToActivitiesSelectedProperty =
			DependencyProperty.Register("IsDeferToActivitiesSelected", typeof(bool), typeof(TaskEditorVm),
			new UIPropertyMetadata(false));
		//SameTimeForActivities Dependency Property
		public TimeSpan SameTimeForActivities
		{
			get { return (TimeSpan)GetValue(SameTimeForActivitiesProperty); }
			set { SetValue(SameTimeForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameTimeForActivitiesProperty =
			DependencyProperty.Register("SameTimeForActivities", typeof(TimeSpan), typeof(TaskEditorVm),
			new UIPropertyMetadata(TimeSpan.FromHours(1), (d, e) => ((TaskEditorVm)d).sameTimeForActivitiesChanged((TimeSpan)e.NewValue)));
		//SameQtyForActivities Dependency Property
		public int SameQtyForActivities
		{
			get { return (int)GetValue(SameQtyForActivitiesProperty); }
			set { SetValue(SameQtyForActivitiesProperty, value); }
		}
		public static readonly DependencyProperty SameQtyForActivitiesProperty =
			DependencyProperty.Register("SameQtyForActivities", typeof(int), typeof(TaskEditorVm),
			new UIPropertyMetadata(0, (d, e) => ((TaskEditorVm)d).sameQtyForActivitiesChanged((int)e.NewValue)));
		#endregion

		#region Time/Qty codes
		/// <summary>
		/// Updates DurationSeconds of model and EndDateTime, also fires TaskDurationChanged
		/// </summary>
		/// <param name="e"></param>
		void durationChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldVal = e.OldValue == DependencyProperty.UnsetValue ? TimeSpan.Zero : (TimeSpan)e.OldValue;
			var newVal = e.NewValue == DependencyProperty.UnsetValue ? TimeSpan.Zero : (TimeSpan)e.NewValue;

			//Update DurationSeconds of model
			Model.DurationSeconds = (int)newVal.TotalSeconds;

			//update EndDateTime
			EndDateTime = Model.StartDateTime.AddSeconds((int)newVal.TotalSeconds);

			//fire event
			if (TaskDurationChanged != null)
				TaskDurationChanged(oldVal, newVal);
		}
		/// <summary>
		/// Updates TaskTargetPoint of model and fires TaskTargetPointChanged
		/// </summary>
		/// <param name="e"></param>
		void taskTargetPointChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldVal = e.OldValue == DependencyProperty.UnsetValue ? 0 : (int)e.OldValue;
			var newVal = e.NewValue == DependencyProperty.UnsetValue ? 0 : (int)e.NewValue;

			//update TaskTargetPoint of model
			Model.TaskTargetPoint = newVal;

			//fire event
			if (TaskTargetPointChanged != null)
				TaskTargetPointChanged(oldVal, newVal);
		}
		/// <summary>
		/// Updates TargetPoint of all processes in which a valid choice is selected
		/// </summary>
		/// <param name="newVal"></param>
		void isSameTimeForActivitiesSelectedChanged(bool newVal)
		{
			if (newVal)
			{
				foreach (var process in ProcessList)
				{
					process.HoldEvents = true;
					if (process.SelectedChoice != null)
						process.TargetPoint = (int)Math.Floor(SameTimeForActivities.TotalSeconds / process.SelectedChoice.CycleTime);
					process.HoldEvents = false;
				}
				DurationSeconds = (int)SameTimeForActivities.TotalSeconds;
			}
		}
		/// <summary>
		/// Updates TargetPoint of all processes in which a valid choice is selected
		/// <para>Does not take effect unless IsSameTimeForActivitiesSelected is set to true</para>
		/// </summary>
		/// <param name="newVal"></param>
		void sameTimeForActivitiesChanged(TimeSpan newVal)
		{
			if (IsSameTimeForActivitiesSelected)
			{
				foreach (var process in ProcessList)
				{
					process.HoldEvents = true;
					if (process.SelectedChoice != null)
						process.TargetPoint = (int)Math.Floor(SameTimeForActivities.TotalSeconds / process.SelectedChoice.CycleTime);
					process.HoldEvents = false;
				}
				DurationSeconds = (int)newVal.TotalSeconds;
			}
		}


		/// <summary>
		/// Updates TargetPoint of all processes
		/// </summary>
		/// <param name="newVal"></param>
		void isSameQtyForActivitiesSelectedChanged(bool newVal)
		{
			if (newVal)
			{
				if (TaskTargetPoint > 0)
					SameQtyForActivities = TaskTargetPoint;
				else
					TaskTargetPoint = SameQtyForActivities;

				foreach (var process in ProcessList)
				{
					process.HoldEvents = true;
					process.TargetPoint = SameQtyForActivities;
					process.HoldEvents = false;
				}
			}
		}
		/// <summary>
		/// Updates TargetPoint of all processes
		/// <para>Does not take effect unless IsSameQtyForActivitiesSelected is set to true</para>
		/// </summary>
		/// <param name="newVal"></param>
		void sameQtyForActivitiesChanged(int newVal)
		{
			if (IsSameQtyForActivitiesSelected)
			{
				foreach (var process in ProcessList)
				{
					process.HoldEvents = true;
					process.TargetPoint = newVal;
					process.HoldEvents = false;
				}
				TaskTargetPoint = newVal;
			}
		}

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
				if (DeleteTaskConfirmed != null) DeleteTaskConfirmed();
			});
			CancelDeleteTaskCommand = new Commands.Command(o => IsDeleteTaskMessageVisible = false);
			//SetDurationMinutesCommand = new Commands.Command(min => SameTimeForActivities = TimeSpan.FromMinutes(Convert.ToDouble(min)));
		}
		//DeleteTaskCommand Dependency Property
		public Commands.Command DeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(DeleteTaskCommandProperty); }
			set { SetValue(DeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteTaskCommandProperty =
			DependencyProperty.Register("DeleteTaskCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//IsDeleteTaskMessageVisible Dependency Property
		public bool IsDeleteTaskMessageVisible
		{
			get { return (bool)GetValue(IsDeleteTaskMessageVisibleProperty); }
			set { SetValue(IsDeleteTaskMessageVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsDeleteTaskMessageVisibleProperty =
			DependencyProperty.Register("IsDeleteTaskMessageVisible", typeof(bool), typeof(TaskEditorVm), new UIPropertyMetadata(false));
		//ConfirmDeleteTaskCommand Dependency Property
		public Commands.Command ConfirmDeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(ConfirmDeleteTaskCommandProperty); }
			set { SetValue(ConfirmDeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty ConfirmDeleteTaskCommandProperty =
			DependencyProperty.Register("ConfirmDeleteTaskCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//CancelDeleteTaskCommand Dependency Property
		public Commands.Command CancelDeleteTaskCommand
		{
			get { return (Commands.Command)GetValue(CancelDeleteTaskCommandProperty); }
			set { SetValue(CancelDeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelDeleteTaskCommandProperty =
			DependencyProperty.Register("CancelDeleteTaskCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		#endregion

	}
}
