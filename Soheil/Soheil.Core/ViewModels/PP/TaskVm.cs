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

namespace Soheil.Core.ViewModels.PP
{
	public class TaskVm : PPItemVm
	{
		public Model.Task Model { get; private set; }
		public override int Id { get { return Model.Id; } }
		public BlockVm Block { get; private set; }

		public DataServices.TaskReportDataService TaskReportDataService { get; private set; }

		#region Ctor
		public TaskVm(Model.Task taskModel, BlockVm parentBlock)
		{
			//data service
			UOW = parentBlock.UOW;
			TaskReportDataService = new DataServices.TaskReportDataService(UOW);

			Message = new EmbeddedException();
			Block = parentBlock;

			//update model data
			Model = taskModel;
			StartDateTime = taskModel.StartDateTime;
			StartDateTimeChanged += newVal => Model.StartDateTime = newVal;
			DurationSeconds = taskModel.DurationSeconds;
			DurationSecondsChanged += newVal => Model.DurationSeconds = newVal;
			TaskTargetPoint = taskModel.TaskTargetPoint;
			TaskProducedG1 = taskModel.TaskReports.Sum(x => x.TaskProducedG1);

			//this non-crucial block of code is likely to throw
			try
			{
				//calculate the number of distinct operators
				var ids = new List<int>();
				foreach (var item in taskModel.Processes)
				{
					ids.AddRange(item.ProcessOperators.Select(x => x.Id));
				}
				TaskOperatorCount = ids.Distinct().Count();
			}
			catch { }
		}

		#endregion

		#region Members
		public int TaskTargetPoint
		{
			get { return Model.TaskTargetPoint; }
			set { Model.TaskTargetPoint = value; OnPropertyChanged("TaskTargetPoint"); }
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
		/// Partitions a Task into TaskReports
		/// </summary>
		public void ReloadTaskReports()
		{
			try
			{
				TaskReports.Clear();
				var models = TaskReportDataService.GetAllForTask(Id).OrderBy(x => x.ReportStartDateTime);

				int remainingTP = TaskTargetPoint - models.Sum(x => x.TaskReportTargetPoint);
				int remainingDuration = DurationSeconds - models.Sum(x => x.ReportDurationSeconds);
				int gap = 0;

				//checks before the first report to see if there is any place for a holder
				DateTime first = Model.EndDateTime;
				if (models.Any()) first = models.First().ReportStartDateTime;
				gap = (int)first.Subtract(Model.StartDateTime).TotalSeconds;
				if (gap > 0)
				{
					//guess TP and modify remainings
					int guessedTP = (int)Math.Round(gap * remainingTP / (float)remainingDuration);
					remainingTP -= guessedTP;
					remainingDuration -= gap;

					//holder is put in the unreported space at the beginning of all reports
					var taskReportHolder = new Report.TaskReportHolderVm(this, StartDateTime, gap, guessedTP);
					taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
					TaskReports.Add(taskReportHolder);
				}

				Model.TaskReport previousModel = null;
				foreach (var model in models)
				{
					//if empty gap between two reports (previousModel & model)
					if (previousModel != null && remainingDuration > 0)
					{
						//put a holder in between
						gap = (int)model.ReportStartDateTime.Subtract(previousModel.ReportEndDateTime).TotalSeconds;
						if (gap > 0)
						{
							//guess TP and modify remainings
							int guessedTP = (int)Math.Round(gap * remainingTP / (float)remainingDuration);
							remainingTP -= guessedTP;
							remainingDuration -= gap;

							var taskReportHolder = new Report.TaskReportHolderVm(this, previousModel.ReportEndDateTime, gap, guessedTP);
							taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
							TaskReports.Add(taskReportHolder);
						}
					}
					var taskReportVm = new Report.TaskReportVm(this, model);
					TaskReports.Add(taskReportVm);
					previousModel = model;
				}

				//checks after the last report to see if there is any place for a holder
				DateTime last = StartDateTime;
				if (models.Any()) last = models.Last().ReportEndDateTime;
				gap = (int)Model.EndDateTime.Subtract(last).TotalSeconds;
				if (gap > 0)
				{
					//holder is put in the unreported space at the end of all reports
					var taskReportHolder = new Report.TaskReportHolderVm(this, last, gap, remainingTP);
					taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
					TaskReports.Add(taskReportHolder);

					//if any space is remained unreported IsReportFilled is false
					IsReportFilled = false;
					ReportFillPercent = string.Format("{0:D2}%", (100 * models.Sum(x => x.ReportDurationSeconds) / DurationSeconds));
				}
				else
				{
					IsReportFilled = true;
					ReportFillPercent = "100";
				}
			}
			catch { }
		}

		/// <summary>
		/// Gets a bindable collection of TaskReports in this Task
		/// </summary>
		public ObservableCollection<Report.TaskReportBaseVm> TaskReports { get { return _taskReports; } }
		private ObservableCollection<Report.TaskReportBaseVm> _taskReports = new ObservableCollection<Report.TaskReportBaseVm>();

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
		#endregion


	}
}
