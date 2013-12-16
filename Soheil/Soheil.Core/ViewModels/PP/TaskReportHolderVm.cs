using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class TaskReportHolderVm : TaskReportBaseVm
	{
		public TaskReportHolderVm(PPTaskVm parent, int sumOfDurations, int sumOfTargetPoints, int index)
			: base(parent, index)
		{
			TargetPoint = parent.TaskTargetPoint - sumOfTargetPoints;
			DurationSeconds = parent.DurationSeconds - sumOfDurations;
			StartDateTime = parent.StartDateTime.AddSeconds(sumOfDurations);
			EndDateTime = parent.StartDateTime.AddSeconds(parent.DurationSeconds);
			
			CanUserEditTaskTPAndG1 = false;

			AddCommand = new Commands.Command(o =>
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
				StartDateTime = parent.StartDateTime.AddSeconds(sumOfDurations);
				EndDateTime = parent.StartDateTime.AddSeconds(parent.DurationSeconds);
				DurationSeconds = parent.DurationSeconds - sumOfDurations;
				TargetPoint = parent.TaskTargetPoint - sumOfTargetPoints;
			});
			AutoFindTargetPoint = new Commands.Command(o =>
			{
				if (parent.DurationSeconds - sumOfDurations - DurationSeconds == 0) TargetPoint = parent.TaskTargetPoint - sumOfTargetPoints;
				else TargetPoint = (int)Math.Round((parent.TaskTargetPoint - sumOfTargetPoints) * (float)DurationSeconds / (parent.DurationSeconds - sumOfDurations));
			});
		}

		
		#region Commands
		//AddCommand Dependency Property
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
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
				if ((bool)e.NewValue)
					vm.Task.Parent.Parent.CurrentTaskReportBuilder = vm;
				else
					vm.Task.Parent.Parent.CurrentTaskReportBuilder = null;
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
	}
}
