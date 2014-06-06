﻿using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class TaskReportVm : DependencyObject
	{
		public event Action<TaskReportVm> TaskReportDeleted;
		public event Action<TaskReportVm> EndDateTimeChanged;

		#region Members
		/// <summary>
		/// Gets TaskReport Id
		/// </summary>
		public int Id { get { return Model.Id; } }
		public Model.TaskReport Model { get; protected set; }
		public Dal.SoheilEdmContext UOW { get; protected set; }

		DataServices.TaskReportDataService _taskReportDataService;

		public TaskReportVm PreviousReport { get; set; }
		public TaskReportVm NextReport { get; set; }
		public DateTime LowerBound { get { return PreviousReport == null ? Model.Task.StartDateTime : PreviousReport.EndDateTime; } }
		public DateTime UpperBound { get { return NextReport == null ? Model.Task.EndDateTime : NextReport.StartDateTime; } }


		/// <summary>
		/// Gets or sets a value that indicates whether user is dragging thumbs to change datetimes
		/// </summary>
		public bool IsUserDrag { get; set; }

		protected bool _isInInitializingPhase = true;
		#endregion

		public TaskReportVm(Model.TaskReport model, Dal.SoheilEdmContext uow)
		{
			Model = model;
			UOW = uow;
			_taskReportDataService = new DataServices.TaskReportDataService(UOW);

			TargetPoint = Model.TaskReportTargetPoint;
			ProducedG1 = Model.TaskProducedG1;
			DurationSeconds = Model.ReportDurationSeconds;
			StartDateTime = Model.ReportStartDateTime;
			EndDateTime = Model.ReportEndDateTime;

			CanUserEditTaskTPAndG1 = true;
			_isInInitializingPhase = false;
			IsUserDrag = false;

			initializeCommands();
		}

		private void SaveG1(int g1)
		{
			if (_isInInitializingPhase) return;
			_taskReportDataService.UpdateG1(Id, g1);
		}
		public void SaveTargetPoint(int tp)
		{
			if (_isInInitializingPhase) return;
			_taskReportDataService.UpdateTargetPoint(Id, tp);
		}

		#region Start/End/Duration
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TaskReportVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;

				vm.EndDateTime = vm.StartDateTime.AddSeconds((int)e.NewValue);
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));
			}, (d, v) =>
			{
				var vm = (TaskReportVm)d;
				var val = (int)v;
				//if very small => set to smallest unit
				if (val < 300)
				{
					return 300;
				}
				//if very large => set to Whole space
				if (val > (vm.UpperBound - vm.LowerBound).TotalSeconds)
				{
					val = (int)(vm.UpperBound - vm.LowerBound).TotalSeconds;
				}
				return val;
			}));
		//Duration Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TaskReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		//StartDateTime Dependency Property
		public DateTime StartDateTime
		{
			get { return (DateTime)GetValue(StartDateTimeProperty); }
			set { SetValue(StartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty StartDateTimeProperty =
			DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(TaskReportVm),
			new UIPropertyMetadata(DateTime.Now,
				(d, e) => ((TaskReportVm)d).TaskReportVm_StartDateTimeChanged((DateTime)e.NewValue),
				(d, v) =>
				{
					var vm = d as TaskReportVm;
					var val = (DateTime)v;
					//check lower bound
					if (val < vm.LowerBound)
					{
						val = vm.LowerBound;
					}
					else if (val > vm.Model.ReportEndDateTime.AddSeconds(-300))
					{
						val = vm.Model.ReportEndDateTime.AddSeconds(-300);
					}

					if (val.AddSeconds(vm.Model.ReportDurationSeconds) > vm.UpperBound)
					{
						val = vm.UpperBound.AddSeconds(-vm.Model.ReportDurationSeconds);
					}

					if (vm.IsUserDrag)
					{
						return val.Date.Add(new TimeSpan(val.Hour, SoheilFunctions.RoundFiveMinutes(val.Minute), 0));
					}
					return val;
				}));
		void TaskReportVm_StartDateTimeChanged(DateTime newVal)
		{
			//update Model, EndDateTime, StartDate & StartTime
			if (!_isInInitializingPhase)
			{
				//update Model
				Model.ReportStartDateTime = newVal;

				//Set EndDateTime
				EndDateTime = newVal.AddSeconds(Model.ReportDurationSeconds);

				_isInInitializingPhase = true;
				SetValue(StartTimeProperty, newVal.TimeOfDay);
				SetValue(StartDateProperty, newVal.Date);
				_isInInitializingPhase = false;
			}
			else
			{
				//update only startTime & startDate
				SetValue(StartTimeProperty, newVal.TimeOfDay);
				SetValue(StartDateProperty, newVal.Date);
			}
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(TaskReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (DateTime)e.NewValue;
				vm.StartDateTime = val.Add((TimeSpan)d.GetValue(StartTimeProperty));
			}));
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(TaskReportVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (TimeSpan)e.NewValue;
				vm.StartDateTime = ((DateTime)d.GetValue(StartDateProperty)).Add(val);
			})); 
		//EndDateTime Dependency Property
		public DateTime EndDateTime
		{
			get { return (DateTime)GetValue(EndDateTimeProperty); }
			set { SetValue(EndDateTimeProperty, value); }
		}
		public static readonly DependencyProperty EndDateTimeProperty =
			DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(TaskReportVm),
			new UIPropertyMetadata(DateTime.Now,
				(d, e) => ((TaskReportVm)d).TaskReportVm_EndDateTimeChanged((DateTime)e.NewValue),
				(d, v) =>
				{
					var vm = d as TaskReportVm;
					var val = (DateTime)v;
					//check upper bound
					if (val > vm.UpperBound)
					{
						val = vm.UpperBound;
					}
					else if (val < vm.Model.ReportStartDateTime.AddSeconds(300))
					{
						val = vm.Model.ReportStartDateTime.AddSeconds(300);
					}

					if (vm.IsUserDrag)
					{
						var duration = (val - vm.StartDateTime).TotalSeconds;								//+--+--+--+-
						var fixedDuration = vm.TargetPoint * (int)Math.Floor(duration / vm.TargetPoint);	//+--+--+--+
						return vm.StartDateTime.AddSeconds(fixedDuration);
					}
					return val;
				}));
		void TaskReportVm_EndDateTimeChanged(DateTime newVal)
		{
			//update Model, DurationSeconds, EndDate & EndTime
			if (!_isInInitializingPhase)
			{
				//update Model
				Model.ReportEndDateTime = newVal;

				//update DurationSeconds
				DurationSeconds = (int)(newVal - Model.ReportStartDateTime).TotalSeconds;

				_isInInitializingPhase = true;
				SetValue(EndTimeProperty, newVal.TimeOfDay);
				SetValue(EndDateProperty, newVal.Date);
				_isInInitializingPhase = false;
			}
			else
			{
				//update only EndTime & EndDate
				SetValue(EndTimeProperty, newVal.TimeOfDay);
				SetValue(EndDateProperty, newVal.Date);
			}
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(TaskReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (DateTime)e.NewValue;
				vm.EndDateTime = val.Add((TimeSpan)d.GetValue(EndTimeProperty));
			}));
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(TaskReportVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				var val = (TimeSpan)e.NewValue;
				vm.EndDateTime = ((DateTime)d.GetValue(EndDateProperty)).Add(val);
			})); 
		#endregion

		#region Count
		/// <summary>
		/// Gets of sets the bindable proportional target point of task report
		/// <para>Changing the value saves database</para>
		/// </summary>
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(TaskReportVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				vm.SaveTargetPoint((int)e.NewValue);
			}));

		/// <summary>
		/// Gets of sets the bindable ProducedG1 of task report
		/// <para>Changing the value saves database</para>
		/// </summary>
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(TaskReportVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportVm;
				if (vm._isInInitializingPhase) return;
				vm.SaveG1((int)e.NewValue);
			}));

		/// <summary>
		/// Gets of sets a bindable value that indicates whether user can edit task target point and produced g1
		/// </summary>
		public bool CanUserEditTaskTPAndG1
		{
			get { return (bool)GetValue(CanUserEditTaskTPAndG1Property); }
			set { SetValue(CanUserEditTaskTPAndG1Property, value); }
		}
		public static readonly DependencyProperty CanUserEditTaskTPAndG1Property =
			DependencyProperty.Register("CanUserEditTaskTPAndG1", typeof(bool), typeof(TaskReportVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Gets a bindable width for grade 1 bar
		/// <para>May differ from ProducedG1 in some cases</para>
		/// </summary>
		public int G1WidthToFit
		{
			get { return (int)GetValue(G1WidthToFitProperty); }
			private set { SetValue(G1WidthToFitProperty, value); }
		}
		public static readonly DependencyProperty G1WidthToFitProperty =
			DependencyProperty.Register("G1WidthToFit", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets a bindable value that indicates the extra production count
		/// </summary>
		public int Excess
		{
			get { return (int)GetValue(ExcessProperty); }
			private set { SetValue(ExcessProperty, value); }
		}
		public static readonly DependencyProperty ExcessProperty =
			DependencyProperty.Register("Excess", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0)); 
		#endregion

		#region Commands
		void initializeCommands()
		{
			DeleteCommand = new Commands.Command(o =>
			{
				_taskReportDataService.DeleteModel(Model);
				if (TaskReportDeleted != null)
					TaskReportDeleted(this);
			});
		}
		/// <summary>
		/// Gets or sets a bindable command to delete task report
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
		#endregion
	}
}
