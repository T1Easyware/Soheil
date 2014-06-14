﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Soheil.Common.SoheilException;
using Soheil.Core.Base;
using Soheil.Core.ViewModels.PP.Report;

namespace Soheil.Core.ViewModels.PP
{
	public class TaskVm : PPItemVm
	{
		/// <summary>
		/// Gets Task Model
		/// </summary>
		public Model.Task Model { get; private set; }
		/// <summary>
		/// Gets Task Id
		/// </summary>
		public override int Id { get { return Model.Id; } }
		/// <summary>
		/// Gets parent BlockVm
		/// </summary>
		public BlockVm Block { get; private set; }

		DataServices.TaskReportDataService _taskReportDataService;

		#region Ctor
		public TaskVm(Model.Task taskModel, Dal.SoheilEdmContext uow)
		{
			//data service
			UOW = uow;
			_taskReportDataService = new DataServices.TaskReportDataService(UOW);
			
			Message = new EmbeddedException();

			//update model data
			Model = taskModel;
			StartDateTime = taskModel.StartDateTime;
			StartDateTimeChanged += newVal => Model.StartDateTime = newVal;
			DurationSeconds = taskModel.DurationSeconds;
			DurationSecondsChanged += newVal => Model.DurationSeconds = newVal;
			//TaskTargetPoint = taskModel.TaskTargetPoint;
			TaskProducedG1 = taskModel.TaskReports.Sum(x => x.TaskProducedG1);

			//this non-crucial block of code is likely to throw
			try
			{
				//calculate the number of distinct operators
				var ids = new List<int>();
				foreach (var item in taskModel.Processes)
				{
					ids.AddRange(item.ProcessOperators.Select(x => x.Operator.Id));
				}
				TaskOperatorCount = ids.Distinct().Count();
			}
			catch { TaskOperatorCount = -1; }///??? does happen?

			#region FillEmptySpaces Command
			FillEmptySpacesCommand = new Commands.Command(o =>
			{
				var models = Model.TaskReports.OrderBy(x => x.ReportStartDateTime).ToArray();
				var dt = StartDateTime;
				var tp = Model.TaskTargetPoint;
				if (models.Any()) tp -= models.Sum(x => x.TaskReportTargetPoint);

				//insert before each task report
				foreach (var model in models)
				{
					if (model.ReportStartDateTime - dt > TimeSpan.FromSeconds(1))
					{
						//insert taskReport newModel before model
						var duration = model.ReportStartDateTime - dt;
						var newModel = new Model.TaskReport
						{
							Task = Model,
							Code = Model.Code,
							ReportStartDateTime = dt,
							ReportEndDateTime = model.ReportStartDateTime,
							ReportDurationSeconds = (int)duration.TotalSeconds,
							TaskProducedG1 = 0,
							TaskReportTargetPoint = (int)(duration.TotalSeconds * Model.TaskTargetPoint / Model.DurationSeconds),
							CreatedDate = DateTime.Now,
							ModifiedDate = DateTime.Now,
							ModifiedBy = LoginInfo.Id,
						};
						Model.TaskReports.Add(newModel);
					}

					dt = model.ReportEndDateTime;
				}

				//insert after last task report
				if (Model.EndDateTime - dt > TimeSpan.FromSeconds(1))
				{
					//insert taskReport after last model
					var duration = Model.EndDateTime - dt;
					var newModel = new Model.TaskReport
					{
						Task = Model,
						Code = Model.Code,
						ReportStartDateTime = dt,
						ReportEndDateTime = Model.EndDateTime,
						ReportDurationSeconds = (int)duration.TotalSeconds,
						TaskProducedG1 = 0,
						TaskReportTargetPoint = (int)(duration.TotalSeconds * Model.TaskTargetPoint / Model.DurationSeconds),
						CreatedDate = DateTime.Now,
						ModifiedDate = DateTime.Now,
						ModifiedBy = LoginInfo.Id,
					};
					Model.TaskReports.Add(newModel);
				}

				//reload reports
				ReloadTaskReports();
			});
			#endregion

		}

		#endregion

		#region Props
		public int TaskTargetPoint
		{
			get { return Model.TaskTargetPoint; }
			set { Model.TaskTargetPoint = value; OnPropertyChanged("TaskTargetPoint"); }
		}
		public DateTime EndDateTime
		{
			get { return Model.EndDateTime; }
			set { Model.EndDateTime = value; OnPropertyChanged("EndDateTime"); }
		}
		
		/// <summary>
		/// Gets or sets the bindable output of Grade-1 for this task
		/// </summary>
		public int TaskProducedG1
		{
			get { return (int)GetValue(TaskProducedG1Property); }
			set { SetValue(TaskProducedG1Property, value); }
		}
		public static readonly DependencyProperty TaskProducedG1Property =
			DependencyProperty.Register("TaskProducedG1", typeof(int), typeof(TaskVm), new UIPropertyMetadata(0));
		
		/// <summary>
		/// Gets or sets the bindable number of operators in this task
		/// </summary>
		public int TaskOperatorCount
		{
			get { return (int)GetValue(TaskOperatorCountProperty); }
			set { SetValue(TaskOperatorCountProperty, value); }
		}
		public static readonly DependencyProperty TaskOperatorCountProperty =
			DependencyProperty.Register("TaskOperatorCount", typeof(int), typeof(TaskVm), new UIPropertyMetadata(0));
		#endregion

		#region TaskReports
		/// <summary>
		/// Partitions this Task into <see cref="TaskReportVm"/>s
		/// </summary>
		public void ReloadTaskReports()
		{
			//reload taskreport models
			TaskReports.Clear();
			var taskReportModels = Model.TaskReports.OrderBy(x => x.ReportStartDateTime);

			//add current reports & set limits
			TaskReportVm prev = null;
			foreach (var taskReportModel in taskReportModels)
			{
				var taskReportVm = new Report.TaskReportVm(taskReportModel, UOW);
				taskReportVm.TaskReportDeleted += TaskReport_TaskReportDeleted;
				if(prev!=null)
				{
					prev.NextReport = taskReportVm;
					taskReportVm.PreviousReport = prev;
				}
				prev = taskReportVm;
				TaskReports.Add(taskReportVm);
			}
		}

		void TaskReport_TaskReportDeleted(Report.TaskReportVm vm)
		{
			if (vm.NextReport != null) vm.NextReport.PreviousReport = vm.PreviousReport;
			if (vm.PreviousReport != null) vm.PreviousReport.NextReport = vm.NextReport;

			TaskReports.Remove(vm);
		}

		/// <summary>
		/// Gets a bindable collection of TaskReports in this Task
		/// </summary>
		public ObservableCollection<Report.TaskReportVm> TaskReports { get { return _taskReports; } }
		private ObservableCollection<Report.TaskReportVm> _taskReports = new ObservableCollection<Report.TaskReportVm>();

		/// <summary>
		/// Gets a bindable value that shows the % of reports for this Task that are filled
		/// <para>value is between 0 and 100</para>
		/// </summary>
		public string ReportFillPercent
		{
			get { return (string)GetValue(ReportFillPercentProperty); }
			protected set { SetValue(ReportFillPercentProperty, value); }
		}
		public static readonly DependencyProperty ReportFillPercentProperty =
			DependencyProperty.Register("ReportFillPercent", typeof(string), typeof(TaskVm), new UIPropertyMetadata("0"));
		/// <summary>
		/// Gets a bindable value that indicates if all reports for this Task are filleds
		/// </summary>
		public bool IsReportFilled
		{
			get { return (bool)GetValue(IsReportFilledProperty); }
			protected set { SetValue(IsReportFilledProperty, value); }
		}
		public static readonly DependencyProperty IsReportFilledProperty =
			DependencyProperty.Register("IsReportFilled", typeof(bool), typeof(TaskVm), new UIPropertyMetadata(false));

		/// <summary>
		/// Bindable command to fill all empty gaps among TaskReports
		/// </summary>
		public Commands.Command FillEmptySpacesCommand
		{
			get { return (Commands.Command)GetValue(FillEmptySpacesCommandProperty); }
			set { SetValue(FillEmptySpacesCommandProperty, value); }
		}
		public static readonly DependencyProperty FillEmptySpacesCommandProperty =
			DependencyProperty.Register("FillEmptySpacesCommand", typeof(Commands.Command), typeof(TaskVm), new UIPropertyMetadata(null));
		#endregion
	}
}
