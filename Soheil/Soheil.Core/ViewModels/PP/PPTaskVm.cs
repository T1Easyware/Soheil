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
	public class PPTaskVm : PPItemVm
	{
		public Model.Task Model { get; private set; }
		public override int Id { get { return Model.Id; } }
		public BlockVm Block { get; private set; }

		public DataServices.TaskReportDataService TaskReportDataService { get; private set; }

		#region Ctor
		public PPTaskVm(Model.Task taskModel, BlockVm parentBlock)
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
			DependencyProperty.Register("TaskProducedG1", typeof(int), typeof(PPTaskVm), new UIPropertyMetadata(0));
		
		/// <summary>
		/// Gets or sets the bindable number of operators in this task
		/// </summary>
		public int TaskOperatorCount
		{
			get { return (int)GetValue(TaskOperatorCountProperty); }
			set { SetValue(TaskOperatorCountProperty, value); }
		}
		public static readonly DependencyProperty TaskOperatorCountProperty =
			DependencyProperty.Register("TaskOperatorCount", typeof(int), typeof(PPTaskVm), new UIPropertyMetadata(0));
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
				var models = TaskReportDataService.GetAllForTask(Id);
				int i = 0;
				int sumOfTP = 0;
				foreach (var model in models)
				{
					var vm = new Report.TaskReportVm(this, model);
					TaskReports.Add(vm);
					sumOfTP += vm.TargetPoint;
					i++;
				}

				//checks if there is any place for a holder
				//holder is put in the unreported space (if any)
				//sumOfDurations = sum of duration of all task reports (if equal to DurationSeconds => IsReportFilled=true)
				int sumOfDurations = models.Sum(x => x.ReportDurationSeconds);
				if (sumOfDurations < this.DurationSeconds)
				{
					var taskReportHolder = new Report.TaskReportHolderVm(this, sumOfDurations, sumOfTP);
					taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
					TaskReports.Add(taskReportHolder);

					//if any space is remained unreported IsReportFilled is false
					IsReportFilled = false;
					ReportFillPercent = string.Format("{0:D2}%", (100 * DurationSeconds / sumOfDurations));
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
		/// Reloads all process reports in all Tasks in the parent Block of this Task
		/// </summary>
		internal void ReloadAllProcessReports()
		{
			if (Block.BlockReport == null)
				Block.BlockReport = new Report.BlockReportVm(Block);
			else
				Block.BlockReport.ReloadProcessReportRows();
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
			DependencyProperty.Register("ReportFillPercent", typeof(string), typeof(PPTaskVm), new UIPropertyMetadata("0"));
		/// <summary>
		/// Gets a bindable value that indicates if all reports for this Task are filleds
		/// </summary>
		public bool IsReportFilled
		{
			get { return (bool)GetValue(IsReportFilledProperty); }
			protected set { SetValue(IsReportFilledProperty, value); }
		}
		public static readonly DependencyProperty IsReportFilledProperty =
			DependencyProperty.Register("IsReportFilled", typeof(bool), typeof(PPTaskVm), new UIPropertyMetadata(false));
		#endregion


	}
}
