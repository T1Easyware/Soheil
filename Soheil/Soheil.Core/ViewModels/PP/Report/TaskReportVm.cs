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

			WarehouseTransaction = new WarehouseTransactionVm(model);

			_isInInitializingPhase = false;
			IsUserDrag = false;

			initializeCommands();
		}

		public void Save()
		{
			if (_isInInitializingPhase || IsUserDrag) return;
			UOW.Commit();
		}

		#region DepProperties and callbacks

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

				vm.Model.TaskProducedG1 = (int)e.NewValue;
				vm.Save();
			}));

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

				vm.Model.TaskReportTargetPoint = (int)e.NewValue;
				vm.Save();
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates WarehouseTransaction
		/// </summary>
		public WarehouseTransactionVm WarehouseTransaction
		{
			get { return (WarehouseTransactionVm)GetValue(WarehouseTransactionProperty); }
			set { SetValue(WarehouseTransactionProperty, value); }
		}
		public static readonly DependencyProperty WarehouseTransactionProperty =
			DependencyProperty.Register("WarehouseTransaction", typeof(WarehouseTransactionVm), typeof(TaskReportVm), new UIPropertyMetadata(null));


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

				vm.Model.ReportDurationSeconds = (int)e.NewValue;
				vm.EndDateTime = vm.StartDateTime.AddSeconds((int)e.NewValue);
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));

				if (vm.IsUserDrag) vm.AutoTargetPointCommand.Execute(null);

				vm.Save();

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

					//if (vm.IsUserDrag)
					//{
					//	val = val.Date.Add(new TimeSpan(val.Hour, SoheilFunctions.RoundFiveMinutes(val.Minute), 0));
					//}

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

				Save();
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

		#endregion


		#region Commands
		void initializeCommands()
		{
			SaveCommand = new Commands.Command(o =>
			{
				UOW.Commit();
			});
			DeleteCommand = new Commands.Command(o =>
			{
				_taskReportDataService.DeleteModel(Model);
				if (TaskReportDeleted != null)
					TaskReportDeleted(this);
			});
			AutoDurationCommand = new Commands.Command(o =>
			{
				var otherReports = Model.Task.TaskReports.Where(x => x != Model);
				var remainingTp = Model.Task.TaskTargetPoint - (otherReports.Any() ? otherReports.Sum(x => x.TaskReportTargetPoint) : 0);
				var remainingDur = Model.Task.DurationSeconds - (otherReports.Any() ? otherReports.Sum(x => x.ReportDurationSeconds) : 0);
				if (remainingTp <= 0 || remainingDur <= 0) 
					DurationSeconds = 0;
				else
					DurationSeconds = (int)Math.Round(TargetPoint * remainingDur / (float)remainingTp);
			});
			AutoTargetPointCommand = new Commands.Command(o =>
			{
				var otherReports = Model.Task.TaskReports.Where(x => x != Model);
				var remainingTp = Model.Task.TaskTargetPoint - (otherReports.Any() ? otherReports.Sum(x => x.TaskReportTargetPoint) : 0);
				var remainingDur = Model.Task.DurationSeconds - (otherReports.Any() ? otherReports.Sum(x => x.ReportDurationSeconds) : 0);
				if (remainingTp <= 0 || remainingDur <= 0)
					TargetPoint = 0;
				else
					TargetPoint = (int)Math.Round(DurationSeconds * remainingTp / (float)remainingDur);
			});
		}

		/// <summary>
		/// Gets or sets a bindable command to save task report
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
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

		/// <summary>
		/// Gets or sets the bindable command to automatically set the TargetPoint of this report according to its DurationSeconds
		/// </summary>
		public Commands.Command AutoTargetPointCommand
		{
			get { return (Commands.Command)GetValue(AutoTargetPointCommandProperty); }
			set { SetValue(AutoTargetPointCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoTargetPointCommandProperty =
			DependencyProperty.Register("AutoTargetPointCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable command to automatically set the DurationSeconds of this report according to its TargetPoint
		/// </summary>
		public Commands.Command AutoDurationCommand
		{
			get { return (Commands.Command)GetValue(AutoDurationCommandProperty); }
			set { SetValue(AutoDurationCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoDurationCommandProperty =
			DependencyProperty.Register("AutoDurationCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));

		#endregion
	}
}
