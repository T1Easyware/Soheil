using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class ProcessReportCellTaskReportHolder : DependencyObject
	{
		public ProcessReportCellTaskReportHolder(ProcessReportCellVm cell, TaskReportHolderVm holder)
		{
			ProcessReportCell = cell;
			TaskReportHolder = holder;
			IsSelected = true;

			AddCommand = new Commands.Command(o =>
			{
				var model = new Model.TaskReport();
				model.ReportDurationSeconds = TaskReportHolder.DurationSeconds;
				model.ReportStartDateTime = TaskReportHolder.StartDateTime;
				model.ReportEndDateTime = TaskReportHolder.EndDateTime;
				model.TaskReportTargetPoint = TaskReportHolder.TargetPoint;
				if (TaskReportHolder.TaskReportDataService.AddReportToTask(model, TaskReportHolder.Task.Id) == null)
				{
					TaskReportHolder.DurationSeconds = model.ReportDurationSeconds;
					TaskReportHolder.StartDateTime = model.ReportStartDateTime;
					TaskReportHolder.EndDateTime = model.ReportEndDateTime;
					TaskReportHolder.TargetPoint = model.TaskReportTargetPoint;
				}
				else
				{
					IsSelected = false;
					TaskReportHolder.Task.ReloadTaskReports();
					TaskReportHolder.Task.ReloadProcessReportRows();
				}
			});
			CancelCommand = new Commands.Command(o => IsSelected = false);
		}
		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProcessReportCellTaskReportHolder), 
						new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (ProcessReportCellTaskReportHolder)d;
				if ((bool)e.NewValue)
					vm.ProcessReportCell.Parent.Parent.CurrentTaskReportBuilderInProcess = vm;
				else
					vm.ProcessReportCell.Parent.Parent.CurrentTaskReportBuilderInProcess = null;
			}));
		//Offset Dependency Property
		public Point Offset
		{
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(Point), typeof(ProcessReportCellTaskReportHolder), new UIPropertyMetadata(new Point()));

		//TaskReportHolder Dependency Property
		public TaskReportHolderVm TaskReportHolder
		{
			get { return (TaskReportHolderVm)GetValue(TaskReportHolderProperty); }
			set { SetValue(TaskReportHolderProperty, value); }
		}
		public static readonly DependencyProperty TaskReportHolderProperty =
			DependencyProperty.Register("TaskReportHolder", typeof(TaskReportHolderVm), typeof(ProcessReportCellTaskReportHolder), new UIPropertyMetadata(null));
		//ProcessReportCell Dependency Property
		public ProcessReportCellVm ProcessReportCell
		{
			get { return (ProcessReportCellVm)GetValue(ProcessReportCellProperty); }
			set { SetValue(ProcessReportCellProperty, value); }
		}
		public static readonly DependencyProperty ProcessReportCellProperty =
			DependencyProperty.Register("ProcessReportCell", typeof(ProcessReportCellVm), typeof(ProcessReportCellTaskReportHolder), new UIPropertyMetadata(null));

		//AddCommand Dependency Property
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(ProcessReportCellTaskReportHolder), new UIPropertyMetadata(null));
		//CancelCommand Dependency Property
		public Commands.Command CancelCommand
		{
			get { return (Commands.Command)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof(Commands.Command), typeof(ProcessReportCellTaskReportHolder), new UIPropertyMetadata(null));
	}
}
