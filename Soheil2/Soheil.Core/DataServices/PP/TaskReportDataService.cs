using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Model;
using Soheil.Dal;

namespace Soheil.Core.DataServices
{
	public class TaskReportDataService : IDataService<TaskReport>
	{
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

		public void DeleteModel(TaskReport model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(TaskReport model)
		{
			throw new NotImplementedException();
		} 
		#endregion

		public TaskReport AddReportToTask(Model.TaskReport report, int taskId)
		{
			TaskReport taskReport = null;
			using (var context = new SoheilEdmContext())
			{
				var task = new Repository<Task>(context).Single(x => x.Id == taskId);
				if (report.ReportEndDateTime > task.EndDateTime) return null; 
				if (report.ReportStartDateTime < task.StartDateTime) return null;
				if (report.ReportDurationSeconds > task.DurationSeconds) return null;
				if (report.TaskReportTargetPoint > task.TaskTargetPoint) return null; 

				taskReport = new TaskReport
				{
					ModifiedDate = DateTime.Now,
					CreatedDate = DateTime.Now,
					Task = task,
					TaskReportTargetPoint = report.TaskReportTargetPoint,
					ReportStartDateTime = report.ReportStartDateTime,
					ReportEndDateTime = report.ReportEndDateTime,
					ReportDurationSeconds = report.ReportDurationSeconds
				};
				var processRepository = new Repository<Process>(context);
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
			}
			return taskReport;
		}

		public IList<TaskReport> GetAllForTask(int taskId)
		{
			var models = new List<TaskReport>();
			using (var context = new SoheilEdmContext())
			{
				models.AddRange(new Repository<TaskReport>(context).Find(
					x => x.Task.Id == taskId));
			}
			return models;
		}

		internal void DeleteById(int Id)
		{
			using (var context = new SoheilEdmContext())
			{
				var taskReportRepository = new Repository<TaskReport>(context);
				var model = taskReportRepository.Single(x => x.Id == Id);
				DeleteModel(model, taskReportRepository, context);
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Delete all a taskReport with all its inner objects 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="taskReportRepository"></param>
		/// <param name="context"></param>
		public void DeleteModel(TaskReport model, Repository<TaskReport> taskReportRepository, SoheilEdmContext context)
		{
			var processReportRepository = new Repository<ProcessReport>(context);
			var processReportDataService=new ProcessReportDataService();
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);
			var processReports = model.ProcessReports.ToArray();
			foreach (var processReportModel in processReports)
			{
				processReportDataService.ClearModel(
					processReportModel,
					processReportRepository,
					defectionReportRepository,
					operatorDefectionReportRepository,
					stoppageReportRepository,
					operatorStoppageReportRepository,
					context);
				processReportRepository.Delete(processReportModel);
			}
			taskReportRepository.Delete(model);
		}

		/// <summary>
		/// Does not return those reports which aren't affecting station results
		/// </summary>
		/// <param name="taskReportId"></param>
		/// <returns></returns>
		internal int GetSumOfDefectionCounts(int taskReportId)
		{
			using (var context = new Dal.SoheilEdmContext())
			{
				var model = new Repository<TaskReport>(context).FirstOrDefault(x=>x.Id == taskReportId);
				return (int)Math.Round(model.ProcessReports.Sum(x => x.DefectionReports.Where(z => z.AffectsTaskReport).Sum(y => y.CountEquivalence)));
			}
		}
		/// <summary>
		/// Does not return those reports which aren't affecting station results
		/// </summary>
		/// <param name="taskReportId"></param>
		/// <returns></returns>
		internal int GetSumOfStoppageCounts(int taskReportId)
		{
			using (var context = new Dal.SoheilEdmContext())
			{
				var model = new Repository<TaskReport>(context).FirstOrDefault(x => x.Id == taskReportId);
				return (int)Math.Round(model.ProcessReports.Sum(x => x.StoppageReports.Where(z => z.AffectsTaskReport).Sum(y => y.CountEquivalence)));
			}
		}

		internal void UpdateG1(int Id, int g1)
		{
			using (var context = new Dal.SoheilEdmContext())
			{
				var model = new Repository<TaskReport>(context).FirstOrDefault(x => x.Id == Id);
				model.TaskProducedG1 = g1;
				context.SaveChanges();
			}	
		}

		internal void UpdateTargetPoint(int Id, int tp)
		{
			using (var context = new Dal.SoheilEdmContext())
			{
				var model = new Repository<TaskReport>(context).FirstOrDefault(x => x.Id == Id, "Task");
				model.Task.TaskTargetPoint = tp;
				context.SaveChanges();
			}
		}
	}
}
