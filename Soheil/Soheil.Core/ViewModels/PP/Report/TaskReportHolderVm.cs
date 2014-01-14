using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class TaskReportHolderVm : TaskReportBaseVm
	{
		int _sumOfDurations;
		int _sumOfTargetPoints;
		public event Action<TaskReportHolderVm> RequestForChangeOfCurrentTaskReportBuilder;

		public TaskReportHolderVm(PPTaskVm parent, int sumOfDurations, int sumOfTargetPoints)
			: base(parent)
		{
			Task = parent;
			_sumOfDurations = sumOfDurations;
			_sumOfTargetPoints = sumOfTargetPoints;

			TargetPoint = parent.TaskTargetPoint - sumOfTargetPoints;
			DurationSeconds = parent.DurationSeconds - sumOfDurations;
			StartDateTime = parent.StartDateTime.AddSeconds(sumOfDurations);
			EndDateTime = parent.StartDateTime.AddSeconds(parent.DurationSeconds);
			
			CanUserEditTaskTPAndG1 = false;
			initializeCommands();
		}

		#region Other PropDp
		//ByEndDate Dependency Property
		public bool ByEndDate
		{
			get { return (bool)GetValue(ByEndDateProperty); }
			set { SetValue(ByEndDateProperty, value); }
		}
		public static readonly DependencyProperty ByEndDateProperty =
			DependencyProperty.Register("ByEndDate", typeof(bool), typeof(TaskReportHolderVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = ((TaskReportHolderVm)d);
				if ((bool)e.NewValue)
					vm.DurationSeconds = (int)vm.EndDateTime.Subtract(vm.StartDateTime).TotalSeconds;
				else
				{
					var endDt = vm.StartDateTime.AddSeconds(vm.DurationSeconds);
					vm.EndDate = endDt.Date;
					vm.EndTime = endDt.TimeOfDay;
				}
			}));
		//IsSelected Dependency Property
		/// <summary>
		/// Also Sets CurrentTaskReportBuilder in PPTableVm
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(TaskReportHolderVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (TaskReportHolderVm)d;
				if (vm.RequestForChangeOfCurrentTaskReportBuilder != null)
				{
					if ((bool)e.NewValue)
						vm.RequestForChangeOfCurrentTaskReportBuilder(vm);
					else
						vm.RequestForChangeOfCurrentTaskReportBuilder(null);
				}
			}));
		//Offset Dependency Property
		public Point Offset
		{
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(Point), typeof(TaskReportHolderVm), new UIPropertyMetadata(new Point()));
		#endregion

		#region Commands
		void initializeCommands()
		{
			OpenCommand = new Commands.Command(o =>
			{
				var model = new Model.TaskReport();
				model.ReportDurationSeconds = DurationSeconds;
				model.ReportStartDateTime = StartDateTime;
				model.ReportEndDateTime = EndDateTime;
				model.TaskReportTargetPoint = TargetPoint;
				if (TaskReportDataService.AddReportToTask(model, Task.Id) == null)
				{
					AutoFillCommand.Execute(o);
				}
				else
				{
					Task.ReloadTaskReports();
					IsSelected = false;
				}
			});
			FocusCommand = new Commands.Command(o =>
			{
				IsSelected = true;
			});
			CancelCommand = new Commands.Command(o =>
			{
				IsSelected = false;
			});
			AutoFillCommand = new Commands.Command(o =>
			{
				StartDateTime = Task.StartDateTime.AddSeconds(_sumOfDurations);
				EndDateTime = Task.StartDateTime.AddSeconds(Task.DurationSeconds);
				DurationSeconds = Task.DurationSeconds - _sumOfDurations;
				TargetPoint = Task.TaskTargetPoint - _sumOfTargetPoints;
			});
			AutoFindTargetPoint = new Commands.Command(o =>
			{
				if (Task.DurationSeconds - _sumOfDurations - DurationSeconds == 0)
					TargetPoint = Task.TaskTargetPoint - _sumOfTargetPoints;
				else
					TargetPoint = (int)Math.Round(
						(Task.TaskTargetPoint - _sumOfTargetPoints) * (float)DurationSeconds 
						/ 
						(Task.DurationSeconds - _sumOfDurations));
			});
		}
		//FocusCommand Dependency Property
		public Commands.Command FocusCommand
		{
			get { return (Commands.Command)GetValue(FocusCommandProperty); }
			set { SetValue(FocusCommandProperty, value); }
		}
		public static readonly DependencyProperty FocusCommandProperty =
			DependencyProperty.Register("FocusCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
		//CancelCommand Dependency Property
		public Commands.Command CancelCommand
		{
			get { return (Commands.Command)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		//AutoFillCommand Dependency Property
		public Commands.Command AutoFillCommand
		{
			get { return (Commands.Command)GetValue(AutoFillCommandProperty); }
			set { SetValue(AutoFillCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoFillCommandProperty =
			DependencyProperty.Register("AutoFillCommand", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		//AutoFindTargetPoint Dependency Property
		public Commands.Command AutoFindTargetPoint
		{
			get { return (Commands.Command)GetValue(AutoFindTargetPointProperty); }
			set { SetValue(AutoFindTargetPointProperty, value); }
		}
		public static readonly DependencyProperty AutoFindTargetPointProperty =
			DependencyProperty.Register("AutoFindTargetPoint", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		#endregion
	}
}
