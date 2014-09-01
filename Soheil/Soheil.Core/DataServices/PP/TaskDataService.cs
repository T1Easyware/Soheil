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
		TaskReportDataService _taskReportDataService;
		ProcessReportDataService _processReportDataService;

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
			this.Context = context;
			_taskRepository = new Repository<Task>(context);
			_taskReportRepository = new Repository<TaskReport>(context);
			_nptRepository = new Repository<NonProductiveTask>(context);
			_processRepository = new Repository<Process>(context);
			_processOperatorRepository = new Repository<ProcessOperator>(context);
			_selectedMachineRepository = new Repository<SelectedMachine>(context);

			_taskReportDataService = new TaskReportDataService(context);
			_processReportDataService = new ProcessReportDataService(context);
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
			Context.Commit();
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

		/// <summary>
		/// No commit
		/// </summary>
		/// <param name="model"></param>
		/// <param name="context"></param>
		public void DeleteModel(Task model)
		{
			if (model.Processes.Any(x => x.ProcessReports.Any()))
				throw new Soheil.Common.SoheilException.RoutedException(
					"You can't delete this Task. It has Reports",
					Soheil.Common.SoheilException.ExceptionLevel.Error, model.Block);

			foreach (var taskReportEnt in model.TaskReports.ToArray())
			{
				_taskReportDataService.DeleteModel(taskReportEnt);
			}
			foreach (var process in model.Processes.ToArray())
			{
				DeleteModel(process);
			}
			model.Block.Tasks.Remove(model);
			_taskRepository.Delete(model);
			Context.Commit();
		}
		public void DeleteModelRecursive(Task model)
		{
			foreach (var taskReportEnt in model.TaskReports)
			{
				_taskReportDataService.DeleteModel(taskReportEnt);
			}
			foreach (var process in model.Processes.ToList())
			{
				DeleteModel(process, true);
			}
			model.Block.Tasks.Remove(model);
			_taskRepository.Delete(model);
			Context.Commit();
		}
		//Recursive (sm & po)
		/// <summary>
		/// Deletes a process and returns a value indicating success
		/// <para>No commit</para>
		/// </summary>
		/// <param name="process"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		internal bool DeleteModel(Process process, bool force = false)
		{
			if (!force && !process.IsReportEmpty)
				return false;

			foreach (var po in process.ProcessOperators.ToArray())
			{
				DeleteModel(po);
			}
			foreach (var sm in process.SelectedMachines.ToArray())
			{
				DeleteModel(sm);
			}
			foreach (var processReport in process.ProcessReports.ToArray())
			{
				_processReportDataService.DeleteModel(processReport);
			}
			if (process.Task != null)
				process.Task.Processes.Remove(process);
			_processRepository.Delete(process);
			return true;
		}

		internal void DeleteModel(SelectedMachine sm)
		{
			sm.Process.SelectedMachines.Remove(sm);
			_selectedMachineRepository.Delete(sm);
		}
		internal void DeleteModel(ProcessOperator po)
		{
			po.Process.ProcessOperators.Remove(po);
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
			var repository = new Repository<Task>(Context);
			Task entity = repository.FirstOrDefault(x => x.Id == taskId);
			return new ObservableCollection<TaskReport>(entity.TaskReports);
		}

		public KeyValuePair<int, TimeSpan> GetSumOfReportedData(int taskId)
		{
			int healthy = 0;
			TimeSpan duration = TimeSpan.Zero;
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
				duration.Add(TimeSpan.FromSeconds(taskReportEntity.ReportDurationSeconds));
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
			return new Repository<StateStationActivity>(Context).Single(x => x.Id == ssaId);
		}

		public StateStationActivityMachine GetStateStationActivityMachine(int ssamId)
		{
			return new Repository<StateStationActivityMachine>(Context).Single(x => x.Id == ssamId);
		}



		internal IEnumerable<Reports.DailyStationPlanData> GetDailyStationsPlan(DateTime StartDateTime, DateTime EndDateTime)
		{
			IEnumerable<Core.Reports.DailyStationPlanData> data;

			//find shifts
			var tmp = new WorkProfilePlanDataService(Context).GetShiftsInRange(StartDateTime, EndDateTime);
			if (tmp.Any())
			{
				var shifts = tmp.Where(x => x.Item2.Date < EndDateTime.Date).Select(x => new
				{
					date = x.Item2,
					start = x.Item2.AddSeconds(x.Item1.StartSeconds),
					end = x.Item2.AddSeconds(x.Item1.EndSeconds),
					code = x.Item1.WorkShiftPrototype.Name
				});

				var processes = _processRepository.GetAll();
				//main query (with shift)
				var prQuery = from shift in shifts
							  from process in processes.Where(x=>x.StartDateTime < shift.end && x.EndDateTime > shift.start)

							  let station = process.Task.Block.StateStation.Station
							  let start = (process.StartDateTime < shift.start) ? shift.start : process.StartDateTime
							  let end = (process.EndDateTime > shift.end) ? shift.end : process.EndDateTime
							  let durationSeconds = (end - start).TotalSeconds
							  let ratio = durationSeconds / process.DurationSeconds
							  where ratio > 0

							  group new
							  {
								  process,
								  activity = process.StateStationActivity.Activity.Name,
								  product = process.Task.Block.StateStation.State.OnProductRework.Name,
								  ratio,
							  } by new
							  {
								  station,
								  shift
							  } into sGroup
							  select new
							  {
								  station = sGroup.Key.station,
								  shiftCode = sGroup.Key.shift.code,
								  items = sGroup,
							  };
				data = prQuery
					.OrderBy(x => x.station.Index)
					.ThenBy(x => x.shiftCode)
					.Select(x => new Reports.DailyStationPlanData
					{
						Activities = x.items
							.OrderBy(y=>y.activity)
							.ThenBy(y=>y.product)
							.Select(y=>new Reports.DailyStationPlanData.MainData
							{
								Activity = y.activity,
								Product = y.product,
								Shift = x.shiftCode,
								Start = y.process.StartDateTime.TimeOfDay,
								End = y.process.EndDateTime.TimeOfDay,
								TargetValue = (y.process.TargetCount * y.ratio).ToString("0.#"),
								Operators = y.process.ProcessOperators.Select(o=>o.Operator.Name),
							}),
						StationName = x.station.Name,
						StationId = x.station.Id,
						ShiftCode = x.shiftCode
					});
			}
			//No shift is available
			else
			{
				var processes = _processRepository.Find(x => x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime);

				//main query (without shift)
				var prQuery = from process in processes

							  let station = process.Task.Block.StateStation.Station
							  let start = (process.StartDateTime < StartDateTime) ? StartDateTime : process.StartDateTime
							  let end = (process.EndDateTime > EndDateTime) ? EndDateTime : process.EndDateTime
							  let durationSeconds = (end - start).TotalSeconds
							  let ratio = durationSeconds / process.DurationSeconds

							  group new
							  {
								  process,
								  activity = process.StateStationActivity.Activity.Name,
								  product = process.Task.Block.StateStation.State.OnProductRework.Name,
								  ratio,
							  } by station
							  into sGroup
							  select new
							  {
								  station = sGroup.Key,
								  items = sGroup,
							  };
				data = prQuery
					.OrderBy(x => x.station.Index)
					.Select(x => new Reports.DailyStationPlanData
					{
						Activities = x.items
							.OrderBy(y => y.activity)
							.ThenBy(y => y.product)
							.Select(y => new Reports.DailyStationPlanData.MainData
							{
								Activity = y.activity,
								Product = y.product,
								Shift = "-",
								Start = y.process.StartDateTime.TimeOfDay,
								End = y.process.EndDateTime.TimeOfDay,
								TargetValue = y.process.TargetCount.ToString(),
								Operators = y.process.ProcessOperators.Select(o => o.Operator.Name),
							}),
						StationName = x.station.Name,
						StationId = x.station.Id,
						ShiftCode = "-"
					});

			}

			return data;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startDt">Start date and time of the first shift of the desired Date</param>
		/// <returns></returns>
		internal Reports.MaterialRequestHour[] GetDailyMaterialPlan(DateTime StartDateTime, DateTime EndDateTime)
		{
			var data = new Reports.MaterialRequestHour[24];

			var tasks = _taskRepository.Find(x => 
				x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime 
				&& x.Block.StateStation.State.BOMs.Any());

			//itemQuery: collection of BOMs in specified date
			var itemQuery = from task in tasks
							from bom in task.Block.StateStation.State.BOMs
							select new
							{
								task,
								bom
							};

			EndDateTime = StartDateTime.AddHours(1);
			for (int i = 0; i < 24; i++)
			{
				//rawmatQuery: collection of requests grouped by raw material
				var rawmatQuery = from item in itemQuery
								  let station = item.task.Block.StateStation.Station
								  let start = (item.task.StartDateTime < StartDateTime) ? StartDateTime : item.task.StartDateTime
								  let end = (item.task.EndDateTime > EndDateTime) ? EndDateTime : item.task.EndDateTime
								  let durationSeconds = (end - start).TotalSeconds
								  let ratio = durationSeconds / item.task.DurationSeconds
								  group new
								  {
									  station,
									  item.bom,
									  qty = item.bom.Quantity * item.task.TaskTargetPoint * ratio
								  } by item.bom.RawMaterial;

				data[i] = new Core.Reports.MaterialRequestHour
				{
					Materials = rawmatQuery.Select(x => new Core.Reports.MaterialRequestMaterial
					{
						RawMaterial = x.Key,
						Stations = x.Select(y => new Core.Reports.MaterialRequestStation
						{
							Bom = y.bom,
							Station = y.station,
							Quantity = y.qty
						})
					}).ToArray(),
				};
				StartDateTime = StartDateTime.AddHours(1);
				EndDateTime = EndDateTime.AddHours(1);
			}
			return data;
		}
	}
}