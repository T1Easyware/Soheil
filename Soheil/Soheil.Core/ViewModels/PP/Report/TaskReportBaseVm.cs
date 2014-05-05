﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class TaskReportBaseVm : DependencyObject
	{
		public Dal.SoheilEdmContext UOW { get; protected set; }

		public DataServices.TaskReportDataService TaskReportDataService { get { return Task.TaskReportDataService; } }

		protected TaskReportBaseVm(TaskVm parent)
		{
			Task = parent;
			UOW = parent.UOW;
		}

		//Task Dependency Property
		public TaskVm Task
		{
			get { return (TaskVm)GetValue(TaskProperty); }
			set { SetValue(TaskProperty, value); }
		}
		public static readonly DependencyProperty TaskProperty =
			DependencyProperty.Register("Task", typeof(TaskVm), typeof(TaskReportBaseVm), new PropertyMetadata(null));
		
		//TargetPoint Dependency Property
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(TaskReportBaseVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm != null) vm.SaveTargetPoint((int)e.NewValue);
			}));

		//OpenReportCommand Dependency Property
		public Commands.Command OpenReportCommand
		{
			get { return (Commands.Command)GetValue(OpenReportCommandProperty); }
			set { SetValue(OpenReportCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenReportCommandProperty =
			DependencyProperty.Register("OpenReportCommand", typeof(Commands.Command), typeof(TaskReportBaseVm), new UIPropertyMetadata(null));
		//CanUserEditTaskTPAndG1 Dependency Property
		public bool CanUserEditTaskTPAndG1
		{
			get { return (bool)GetValue(CanUserEditTaskTPAndG1Property); }
			set { SetValue(CanUserEditTaskTPAndG1Property, value); }
		}
		public static readonly DependencyProperty CanUserEditTaskTPAndG1Property =
			DependencyProperty.Register("CanUserEditTaskTPAndG1", typeof(bool), typeof(TaskReportBaseVm), new UIPropertyMetadata(true));

		#region Start/End/Duration
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TaskReportBaseVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (!vm.ByEndDate)
						vm.EndDateTime = vm.StartDateTime.AddSeconds((int)e.NewValue);
				}
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));
			}));
		//Duration Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TaskReportBaseVm), new UIPropertyMetadata(TimeSpan.Zero));

		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(TaskReportBaseVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)vm.EndDateTime.Subtract(((DateTime)e.NewValue).Add(vm.StartTime)).TotalSeconds;
					else
						vm.EndDateTime = ((DateTime)e.NewValue).Add(vm.StartTime).AddSeconds(vm.DurationSeconds);
				}
			}));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(TaskReportBaseVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)vm.EndDateTime.Subtract(vm.StartDate.Add((TimeSpan)e.NewValue)).TotalSeconds;
					else
						vm.EndDateTime = vm.StartDate.Add((TimeSpan)e.NewValue).AddSeconds(vm.DurationSeconds);
				}
			}));
		public DateTime StartDateTime
		{
			get { return StartDate.Add(StartTime); }
			set { StartDate = value.Date; StartTime = value.TimeOfDay; }
		}
		//EndDate Dependency Property
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(TaskReportBaseVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)((DateTime)e.NewValue).Add(vm.EndTime).Subtract(vm.StartDateTime).TotalSeconds;
				}
			}));
		//EndTime Dependency Property
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(TaskReportBaseVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)vm.EndDate.Add((TimeSpan)e.NewValue).Subtract(vm.StartDateTime).TotalSeconds;
				}
			}));
		public DateTime EndDateTime
		{
			get { return EndDate.Add(EndTime); }
			set { EndDate = value.Date; EndTime = value.TimeOfDay; }
		} 
		#endregion
	}
}
