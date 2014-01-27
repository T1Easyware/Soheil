using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Common.SoheilException;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.PP.Editor;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class TaskDataService : DataServiceBase, IDataService<Task>
	{
		Repository<Task> _taskRepository;
		Repository<TaskReport> _taskReportRepository;
		Repository<NonProductiveTask> _nptRepository;
		Repository<Process> _processRepository;
		Repository<ProcessOperator> _processOperatorRepository;
		Repository<SelectedMachine> _selectedMachineRepository;

		public event EventHandler<ModelAddedEventArgs<Task>> TaskAdded;
		public event EventHandler<ModelUpdatedEventArgs<Task>> TaskUpdated;
		public event EventHandler<ModelAddedEventArgs<TaskReport>> TaskReportAdded;
		public event EventHandler<ModelRemovedEventArgs> TaskReportRemoved;

		public TaskDataService()
			: this(new SoheilEdmContext())
		{
		}
		public TaskDataService(SoheilEdmContext context)
		{
			this.context = context;
			_taskRepository = new Repository<Task>(context);
			_taskReportRepository = new Repository<TaskReport>(context);
			_nptRepository = new Repository<NonProductiveTask>(context);
			_processRepository = new Repository<Process>(context);
			_processOperatorRepository = new Repository<ProcessOperator>(context);
			_selectedMachineRepository = new Repository<SelectedMachine>(context);
		}


		#region IDataService<Task> Members

		public Task GetSingle(int id)
		{
			return _taskRepository.Single(task => task.Id == id);
		}
		public Task GetSingleWithProcessData(int id)
		{
			return _taskRepository.FirstOrDefault(task => task.Id == id);
		}

		public ObservableCollection<Task> GetAll()
		{
			IEnumerable<Task> entityList = _taskRepository.GetAll();
			return new ObservableCollection<Task>(entityList);
		}

		public int AddModel(Task model)
		{
			_taskRepository.Add(model);
			context.Commit();
			if (TaskAdded != null)
				TaskAdded(this, new ModelAddedEventArgs<Task>(model));
			return model.Id;
		}
		/*public int AddModelAndCreateProcesses(Task model)
		{
			foreach (var ssa in model.Block.StateStation.StateStationActivities)
			{
				var process = new Process
				{
					Task = model,
					StateStationActivity = ssa,
					TargetCount = 0,//model.Duration.Ticks / ssa.CycleTime*3600000
				};
				foreach (var ssam in ssa.StateStationActivityMachines.Where(x => x.IsFixed))
				{
					process.SelectedMachines.Add(new SelectedMachine { Process = process, StateStationActivityMachine = ssam });
				}
			}
			_taskRepository.Add(model);
			context.Commit();
			if (TaskAdded != null)
				TaskAdded(this, new ModelAddedEventArgs<Task>(model));
			return model.Id;
		}*/

		public void UpdateModel(Task model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(int taskId)
		{
			var model = _taskRepository.FirstOrDefault(x => x.Id == taskId);
			DeleteModel(model);
			context.Commit();
		}
		/// <summary>
		/// No commit
		/// </summary>
		/// <param name="model"></param>
		/// <param name="context"></param>
		public void DeleteModel(Task model)
		{
			if(!_taskRepository.Exists(x => x.Id == model.Id))
			{
				//not saved at all (just remove it from its parent)
				model.Block.Tasks.Remove(model);
				return;
			}

			if (_taskReportRepository.Exists(x => x.Task.Id == model.Id))
				throw new RoutedException("You can't delete this Task. It has Reports", ExceptionLevel.Error, model);

			var taskReportDs = new TaskReportDataService(context);
			foreach (var taskReportEnt in model.TaskReports)
			{
				taskReportDs.DeleteModel(taskReportEnt);
			}
			foreach (var process in model.Processes.ToList())
			{
				DeleteModel(process);
			}
			_taskRepository.Delete(model);
		}
		//Recursive (sm & po)
		internal void DeleteModel(Process process)
		{
			foreach (var po in process.ProcessOperators.ToList())
			{
				DeleteModel(po);
			}
			foreach (var sm in process.SelectedMachines.ToList())
			{
				DeleteModel(sm);
			}
			_processRepository.Delete(process);
		}
		internal void DeleteModel(SelectedMachine sm)
		{
			_selectedMachineRepository.Delete(sm);
		}
		internal void DeleteModel(ProcessOperator po)
		{
			_processOperatorRepository.Delete(po);
		}

		public void AttachModel(Task model)
		{
			if (_taskRepository.Exists(x => x.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		#endregion


		/// <summary>
		/// Gets all active activitys as view models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Task> GetActives()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets existing processes for a task (not equal to all StateStationActivities for a StateStation)
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		public IEnumerable<Process> GetProcesses(int taskId)
		{
			return _taskRepository.FirstOrDefault(x => x.Id == taskId,
				"Processes",
				"Processes.StateStationActivity",
				"Processes.StateStationActivity.Activity").Processes;
		}

		public ObservableCollection<TaskReport> GetTaskReports(int taskId)
		{
			var repository = new Repository<Task>(context);
			Task entity = repository.FirstOrDefault(x => x.Id == taskId);
			return new ObservableCollection<TaskReport>(entity.TaskReports);
		}

		public KeyValuePair<int, TimeSpan> GetSumOfReportedData(int taskId)
		{
			int healthy = 0;
			TimeSpan duration = new TimeSpan();
			Task entity = _taskRepository.First(x => x.Id == taskId);
			foreach (var processEntity in entity.Processes)
			{
				foreach (var processReportEntity in processEntity.ProcessReports)
				{
					healthy += processReportEntity.ProducedG1;
				}
			}
			foreach (var taskReportEntity in entity.TaskReports)
			{
				duration.Add(new TimeSpan(taskReportEntity.ReportDurationSeconds * TimeSpan.TicksPerSecond));
			}
			return new KeyValuePair<int, TimeSpan>(healthy, duration);
		}

		public static bool HasCollision(TaskReport report1, TaskReport report2)
		{
			if (report1.ReportStartDateTime < report2.ReportStartDateTime)
			{
				return report1.ReportStartDateTime.AddSeconds(report1.ReportDurationSeconds) > report2.ReportStartDateTime;
			}
			else if (report1.ReportStartDateTime > report2.ReportStartDateTime)
			{
				return report2.ReportStartDateTime.AddSeconds(report2.ReportDurationSeconds) > report1.ReportStartDateTime;
			}
			return true;//else: both start at the same time so collide
		}

		public StateStationActivity GetStateStationActivity(int ssaId)
		{
			return new Repository<StateStationActivity>(context).Single(x => x.Id == ssaId);
		}

		public StateStationActivityMachine GetStateStationActivityMachine(int ssamId)
		{
			return new Repository<StateStationActivityMachine>(context).Single(x => x.Id == ssamId);
		}


	}
}