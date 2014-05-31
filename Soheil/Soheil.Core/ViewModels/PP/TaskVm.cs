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
		/// Partitions this Task into TaskReports and fills gaps with <see cref="TaskReportHolderVm"/>
		/// <para>Also reloads all process reports of the block, if asked to</para>
		/// </summary>
		/// <param name="reloadProcessReports">Calls ReloadProcessReportRows on Block.BlockReport</param>
		public void ReloadTaskReports(bool reloadProcessReports)
		{
			try
			{
				TaskReports.Clear();
				var taskReportModels = TaskReportDataService.GetAllForTask(Id).OrderBy(x => x.ReportStartDateTime);

				int gap = 0;
				int remainingTP = TaskTargetPoint - taskReportModels.Sum(x => x.TaskReportTargetPoint);
				int remainingDuration = DurationSeconds - taskReportModels.Sum(x => x.ReportDurationSeconds);
				if (remainingDuration > 0)
				{
					//if any space is remained unreported IsReportFilled is false
					IsReportFilled = false;
					ReportFillPercent = string.Format("{0:D2}%", (100 * taskReportModels.Sum(x => x.ReportDurationSeconds) / DurationSeconds));
				}
				else
				{
					IsReportFilled = true;
					ReportFillPercent = "100%";
				}

				//find all checkpoints in this task
				//Task:		---TaskReport----TaskReport-----
				//splits:	x  x         x   x         x   x
				//type:		dt model     dt  model     dt  dt
				List<object> splits = new List<object>();
				DateTime? previousEndDateTime = null;
				//add start of Task if gap at the start
				if (taskReportModels.Any())
				{
					if (taskReportModels.First().ReportStartDateTime > StartDateTime)
						splits.Add(StartDateTime);
				}
				else
					splits.Add(StartDateTime);
				//add taskReports and gaps between them if any
				foreach (var taskReportModel in taskReportModels)
				{
					if(previousEndDateTime.HasValue && previousEndDateTime.Value < taskReportModel.ReportStartDateTime)
					{
						splits.Add(previousEndDateTime.Value);
					}
					splits.Add(taskReportModel);
					previousEndDateTime = taskReportModel.ReportEndDateTime;
				}
				//add end of last report if gap in the end
				if (taskReportModels.Any() && taskReportModels.Last().ReportEndDateTime < Model.EndDateTime)
				{
					splits.Add(previousEndDateTime.Value);
				}
				//adds the end of range
				splits.Add(Model.EndDateTime);

				//put reports and holders
				for (int i = 0; i < splits.Count-1; i++)
				{
					if(splits[i] is Model.TaskReport)
					{
						var taskReportModel = splits[i] as Model.TaskReport;
						var taskReportVm = new Report.TaskReportVm(this, taskReportModel);
						TaskReports.Add(taskReportVm);
					}
					else
					{
						//find the gap
						var startDt = (DateTime)splits[i];
						var endDt = (splits[i + 1] is Model.TaskReport)
									? (splits[i + 1] as Model.TaskReport).ReportStartDateTime
									: (DateTime)splits[i + 1];
						gap = (int)endDt.Subtract(startDt).TotalSeconds;

						//guess the gapTP and modify remainings
						int gapTP = (int)Math.Round(gap * remainingTP / (float)remainingDuration);
						remainingTP -= gapTP;
						remainingDuration -= gap;

						//put the holder
						var taskReportHolder = new Report.TaskReportHolderVm(this, startDt, gap, gapTP);
						taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
						TaskReports.Add(taskReportHolder);
					}
				}

				//load process reports
				if(reloadProcessReports)
				{
					Block.BlockReport.ReloadProcessReportRows();
				}
			}
			catch (Exception ex) { Message.AddEmbeddedException(ex.Message); }
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
