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
		/// Deletes a taskReport with all its inner objects 
		/// </summary>
		/// <param name="model">TaskReport Model to delete from its Task</param>
		public void DeleteModel(TaskReport model)
		{
			model.Task.TaskReports.Remove(model);
			_taskReportRepository.Delete(model);
			context.Commit();
		}

		public void AttachModel(TaskReport model)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}