﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class TaskReportHolderVm : TaskReportBaseVm
	{
		DateTime _start;
		int _durationSeconds;
		int _guessedTP;

		public event Action<TaskReportHolderVm> RequestForChangeOfCurrentTaskReportBuilder;

		public TaskReportHolderVm(TaskVm parent, DateTime start, int durationSeconds, int guessedTP)
			: base(parent)
		{
			Task = parent;
			_start = start;
			_durationSeconds = durationSeconds;
			_guessedTP = guessedTP;

			TargetPoint = guessedTP;
			DurationSeconds = durationSeconds;
			StartDateTime = start;
			EndDateTime = start.AddSeconds(durationSeconds);
			
			CanUserEditTaskTPAndG1 = false;
			initializeCommands();
		}

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

		#region Commands
		void initializeCommands()
		{
			//AddAndOpenCommand
			OpenReportCommand = new Commands.Command(o =>
			{
				var model = new Model.TaskReport();
				model.ReportDurationSeconds = DurationSeconds;
				model.ReportStartDateTime = StartDateTime;
				model.ReportEndDateTime = EndDateTime;
				model.TaskReportTargetPoint = TargetPoint;
				if (TaskReportDataService.AddReportToTask(model, Task.Model) == null)
				{
					//user should correct times
					AutoFillCommand.Execute(o);
				}
				else
				{
					Task.Block.ReloadReports();
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
				StartDateTime = _start;
				EndDateTime = _start.AddSeconds(_durationSeconds);
				DurationSeconds = _durationSeconds;
				TargetPoint = _guessedTP;
			});
			AutoFindTargetPoint = new Commands.Command(o =>
			{
				if (_durationSeconds == DurationSeconds)
					TargetPoint = _guessedTP;
				else
					TargetPoint = (int)Math.Round(_guessedTP * DurationSeconds / (float)_durationSeconds);
			});
			AutoFindDuration = new Commands.Command(o =>
			{
				if (TargetPoint == _guessedTP)
					DurationSeconds = _durationSeconds;
				else
					DurationSeconds = (int)Math.Round(_durationSeconds * TargetPoint / (float)_guessedTP);
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
		//AutoFindDuration Dependency Property
		public Commands.Command AutoFindDuration
		{
			get { return (Commands.Command)GetValue(AutoFindDurationProperty); }
			set { SetValue(AutoFindDurationProperty, value); }
		}
		public static readonly DependencyProperty AutoFindDurationProperty =
			DependencyProperty.Register("AutoFindDuration", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		#endregion
	}
}
