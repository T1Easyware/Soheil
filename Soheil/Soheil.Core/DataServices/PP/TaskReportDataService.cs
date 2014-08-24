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
			this.Context = context;
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
		/// Deletes a taskReport with all its inner objects 
		/// </summary>
		/// <param name="model">TaskReport Model to delete from its Task</param>
		public void DeleteModel(TaskReport model)
		{
			model.Task.TaskReports.Remove(model);
			_taskReportRepository.Delete(model);
			Context.Commit();
		}

		public void AttachModel(TaskReport model)
		{
			throw new NotImplementedException();
		}
		#endregion

		/// <summary>
		/// Uses a new UOW to guess producedG1 of a task (Station Output)
		/// </summary>
		/// <param name="model_"></param>
		/// <returns></returns>
		internal int GuessG1(TaskReport model_)
		{
			var model = new Repository<TaskReport>(new SoheilEdmContext()).Single(x=>x.Id == model_.Id);
			var processes = model.Task.Processes.Where(x => x.StateStationActivity.IsPrimaryOutput && x.TargetCount > 0);
			if (!processes.Any()) return 0;
			double guess = double.NaN;
			foreach (var process in processes)
			{
				var prQuery = from processReport in process.ProcessReports.Where(x => x.StartDateTime < model.ReportEndDateTime && x.EndDateTime > model.ReportStartDateTime)
							  let start = (processReport.StartDateTime < model.ReportStartDateTime) ? model.ReportStartDateTime : processReport.StartDateTime
							  let end = (processReport.EndDateTime > model.ReportEndDateTime) ? model.ReportEndDateTime : processReport.EndDateTime
							  let ratio = (end - start).TotalSeconds / processReport.DurationSeconds
							  //let sumOfStp = processReport.StoppageReports.Where(x => x.AffectsTaskReport).Sum(x => x.CountEquivalence)
							  //let sumOfDef = processReport.DefectionReports.Where(x => x.AffectsTaskReport).Sum(x => x.CountEquivalence)
							  select (processReport.ProducedG1 /*- sumOfStp - sumOfDef*/) * ratio;
				var tmp = prQuery.Sum();
				if(double.IsNaN(guess))
					guess = tmp;
				else if (tmp < guess)
					guess = tmp;
			}
			return (int)Math.Round(guess);
		}
	}
}