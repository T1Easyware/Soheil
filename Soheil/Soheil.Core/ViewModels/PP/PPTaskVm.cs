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

		public DataServices.TaskDataService TaskDataService { get { return Block.Parent.PPTable.TaskDataService; } }
		public DataServices.JobDataService JobDataService { get { return Block.Parent.PPTable.JobDataService; } }
		public DataServices.TaskReportDataService TaskReportDataService { get { return Block.Parent.PPTable.TaskReportDataService; } }

		#region Ctor
		public PPTaskVm(Model.Task taskModel, BlockVm parentBlock)
		{
			Block = parentBlock;
			Model = taskModel;
			StartDateTime = taskModel.StartDateTime;
			DurationSeconds = taskModel.DurationSeconds;
			TaskTargetPoint = taskModel.TaskTargetPoint;
			TaskProducedG1 = taskModel.TaskReports.Sum(x => x.TaskProducedG1);
			Message = new EmbeddedException();

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
		public override DateTime StartDateTime
		{
			get { return Model.StartDateTime; }
			set { Model.StartDateTime = value; OnPropertyChanged("StartDateTime"); }
		}
		public override int DurationSeconds
		{
			get { return Model.DurationSeconds; }
			set
			{
				Model.DurationSeconds = value;
				SetValue(DurationProperty, new TimeSpan(value * TimeSpan.TicksPerSecond));
				OnPropertyChanged("DurationSeconds");
			}
		}
		//TaskProducedG1 Dependency Property
		public int TaskProducedG1
		{
			get { return (int)GetValue(TaskProducedG1Property); }
			set { SetValue(TaskProducedG1Property, value); }
		}
		public static readonly DependencyProperty TaskProducedG1Property =
			DependencyProperty.Register("TaskProducedG1", typeof(int), typeof(PPTaskVm), new UIPropertyMetadata(0));
		//TaskOperatorCount Dependency Property
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
		/// Partitions a Task into TaskReports from PPTable's Simple Mode
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
				int sumOfDurations = models.Sum(x => x.ReportDurationSeconds);
				if (sumOfDurations < this.DurationSeconds)
				{
					var taskReportHolder = new Report.TaskReportHolderVm(this, sumOfDurations, sumOfTP);
					taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
					TaskReports.Add(taskReportHolder);
				}
				SumOfReportedHours = sumOfDurations / 3600d;
			}
			catch { }
		}

		public void ClearTaskReports()
		{
			TaskReports.Clear();
		}
		/// <summary>
		/// Reloads all process reports in all tasks in the parent block of this task
		/// </summary>
		internal void ReloadAllProcessReports()
		{
			if (Block.BlockReport == null)
				Block.BlockReport = new Report.BlockReportVm(Block);
			else
				Block.BlockReport.ReloadProcessReportRows();
		}

		//TaskReports Observable Collection
		public ObservableCollection<Report.TaskReportBaseVm> TaskReports { get { return _taskReports; } }
		private ObservableCollection<Report.TaskReportBaseVm> _taskReports = new ObservableCollection<Report.TaskReportBaseVm>();

		//SumOfReportedHours Dependency Property
		public double SumOfReportedHours
		{
			get { return (double)GetValue(SumOfReportedHoursProperty); }
			set { SetValue(SumOfReportedHoursProperty, value); }
		}
		public static readonly DependencyProperty SumOfReportedHoursProperty =
			DependencyProperty.Register("SumOfReportedHours", typeof(double), typeof(PPTaskVm), new UIPropertyMetadata(0d));
		#endregion


	}
}
