using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class TaskReportDataService : DataServiceBase, IDataService<TaskReport>
	{
		Repository<TaskReport> _taskReportRepository;
		Repository<Task> _taskRepository;
		Repository<Process> _processRepository;
		Repository<ProcessReport> _processReportRepository;

		public TaskReportDataService()
			: this(new SoheilEdmContext())
		{

		}
		public TaskReportDataService(SoheilEdmContext context)
		{
			this.context = context;
			_taskReportRepository = new Repository<TaskReport>(context);
			_taskRepository = new Repository<Task>(context);
			_processRepository = new Repository<Process>(context);
			_processReportRepository = new Repository<ProcessReport>(context);
		}

		#region IDataService
		public TaskReport GetSingle(int id)
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<TaskReport> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<TaskReport> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(TaskReport model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(TaskReport model)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete all a taskReport with all its inner objects 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="taskReportRepository"></param>
		/// <param name="context"></param>
		public void DeleteModel(TaskReport model)
		{
			var processReportDataService = new ProcessReportDataService(context);
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);
			var processReports = model.ProcessReports.ToArray();
			foreach (var processReportModel in processReports)
			{
				processReportDataService.ClearModel(
					processReportModel,
					_processReportRepository,
					defectionReportRepository,
					operatorDefectionReportRepository,
					stoppageReportRepository,
					operatorStoppageReportRepository,
					context);
				_processReportRepository.Delete(processReportModel);
			}
			_taskReportRepository.Delete(model);
		}

		public void AttachModel(TaskReport model)
		{
			throw new NotImplementedException();
		}
		#endregion

		public TaskReport AddReportToTask(Model.TaskReport report, Model.Task task)
		{
			if (report.ReportEndDateTime > task.EndDateTime) return null;
			if (report.ReportStartDateTime < task.StartDateTime) return null;
			if (report.ReportDurationSeconds > task.DurationSeconds) return null;
			if (report.TaskReportTargetPoint > task.TaskTargetPoint) return null;

			var taskReport = new TaskReport
			{
				ModifiedDate = DateTime.Now,
				CreatedDate = DateTime.Now,
				Task = task,
				TaskReportTargetPoint = report.TaskReportTargetPoint,
				ReportStartDateTime = report.ReportStartDateTime,
				ReportEndDateTime = report.ReportEndDateTime,
				ReportDurationSeconds = report.ReportDurationSeconds
			};
			foreach (var process in task.Processes)
			{
				int remainingPRTP = process.TargetCount - process.ProcessReports.Sum(x => x.ProcessReportTargetPoint);
				int guessedPRTP = (int)(report.ReportDurationSeconds / process.StateStationActivity.CycleTime);
				if (remainingPRTP < guessedPRTP) guessedPRTP = remainingPRTP;
				taskReport.ProcessReports.Add(new ProcessReport
				{
					Process = process,
					TaskReport = taskReport,
					ProcessReportTargetPoint = guessedPRTP,
				});
			}
			task.TaskReports.Add(taskReport);
			context.SaveChanges();
			return taskReport;
		}

		public IList<TaskReport> GetAllForTask(int taskId)
		{
			var models = new List<TaskReport>();
			models.AddRange(_taskReportRepository.Find(x => x.Task.Id == taskId));
			return models;
		}

		internal void DeleteById(int Id)
		{
			var model = _taskReportRepository.Single(x => x.Id == Id);
			DeleteModel(model);
			context.SaveChanges();
		}

		/// <summary>
		/// Does not return those reports which aren't affecting station results
		/// </summary>
		/// <param name="taskReportId"></param>
		/// <returns></returns>
		internal int GetSumOfDefectionCounts(int taskReportId)
		{
			var model = _taskReportRepository.FirstOrDefault(x => x.Id == taskReportId);
			return (int)Math.Round(model.ProcessReports.Sum(x => x.DefectionReports.Where(z => z.AffectsTaskReport).Sum(y => y.CountEquivalence)));
		}
		/// <summary>
		/// Does not return those reports which aren't affecting station results
		/// </summary>
		/// <param name="taskReportId"></param>
		/// <returns></returns>
		internal int GetSumOfStoppageCounts(int taskReportId)
		{
			var model = _taskReportRepository.FirstOrDefault(x => x.Id == taskReportId);
			return (int)Math.Round(model.ProcessReports.Sum(x => x.StoppageReports.Where(z => z.AffectsTaskReport).Sum(y => y.CountEquivalence)));
		}

		internal void UpdateG1(int Id, int g1)
		{
			var model = _taskReportRepository.FirstOrDefault(x => x.Id == Id);
			model.TaskProducedG1 = g1;
			context.SaveChanges();
		}

		internal void UpdateTargetPoint(int Id, int tp)
		{
			var model = _taskReportRepository.FirstOrDefault(x => x.Id == Id, "Task");
			model.Task.TaskTargetPoint = tp;
			context.SaveChanges();
		}
	}
}