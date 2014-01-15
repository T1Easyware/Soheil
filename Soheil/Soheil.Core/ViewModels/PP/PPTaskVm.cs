using System;
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
		Model.Task _model;
		public override int Id { get { return _model.Id; } }
		public BlockVm Block { get; private set; }

		public DataServices.TaskDataService TaskDataService { get { return Block.Parent.PPTable.TaskDataService; } }
		public DataServices.JobDataService JobDataService { get { return Block.Parent.PPTable.JobDataService; } }
		public DataServices.TaskReportDataService TaskReportDataService { get { return Block.Parent.PPTable.TaskReportDataService; } }

		#region Ctor
		public PPTaskVm(Model.Task taskModel, BlockVm parentBlock)
		{
			Block = parentBlock;
			StartDateTime = taskModel.StartDateTime;
			DurationSeconds = taskModel.DurationSeconds;
			TaskTargetPoint = taskModel.TaskTargetPoint;
			Message = new EmbeddedException();

			//TaskProducedG1 = 
			var ids = new List<int>();
			foreach (var item in taskModel.Processes)
			{
				ids.AddRange(item.ProcessOperators.Select(x => x.Id));
			}
			TaskOperatorCount = ids.Distinct().Count();
		}

		#endregion

		#region Members
		public int TaskTargetPoint
		{
			get { return _model.TaskTargetPoint; }
			set { _model.TaskTargetPoint = value; OnPropertyChanged("TaskTargetPoint"); }
		}
		public new DateTime StartDateTime
		{
			get { return _model.StartDateTime; }
			set { _model.StartDateTime = value; OnPropertyChanged("StartDateTime"); }
		}
		public new int DurationSeconds
		{
			get { return _model.DurationSeconds; }
			set
			{
				_model.DurationSeconds = value;
				SetValue(DurationProperty, new TimeSpan(value * TimeSpan.TicksPerSecond));
				OnPropertyChanged("DurationSeconds");
			}
		}
		//DurationSeconds Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPTaskVm), new UIPropertyMetadata(TimeSpan.Zero));
		
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
			TaskReports.Clear();
			var models = TaskReportDataService.GetAllForTask(Id);
			int i = 0;
			int sumOfTP = 0;
			foreach (var model in models)
			{
				var vm = new TaskReportVm(this, model);
				TaskReports.Add(vm);
				sumOfTP += vm.TargetPoint;
				i++;
			}
			int sumOfDurations = models.Sum(x => x.ReportDurationSeconds);
			if (sumOfDurations < this.DurationSeconds)
			{
				var taskReportHolder = new TaskReportHolderVm(this, sumOfDurations, sumOfTP);
				taskReportHolder.RequestForChangeOfCurrentTaskReportBuilder += vm => Block.Parent.PPTable.CurrentTaskReportBuilder = vm;
				TaskReports.Add(taskReportHolder);
			}
			SumOfReportedHours = sumOfDurations / 3600d;
		}

		public void ClearTaskReports()
		{
			TaskReports.Clear();
		}
		//TaskReports Observable Collection
		public ObservableCollection<TaskReportBaseVm> TaskReports { get { return _taskReports; } }
		private ObservableCollection<TaskReportBaseVm> _taskReports = new ObservableCollection<TaskReportBaseVm>();

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
