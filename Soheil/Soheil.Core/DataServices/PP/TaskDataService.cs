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

namespace Soheil.Core.DataServices
{
	public class TaskDataService : IDataService<Task>
	{
		#region IDataService<Task> Members

		public Task GetSingle(int id)
		{
			Task entity;
			using (var context = new SoheilEdmContext())
			{
				var taskRepository = new Repository<Task>(context);
				entity = taskRepository.Single(task => task.Id == id);
			}
			return entity;
		}
		public Task GetSingleWithProcessData(int id)
		{
			Task entity;
			using (var context = new SoheilEdmContext())
			{
				var taskRepository = new Repository<Task>(context);
				entity = taskRepository.FirstOrDefault(task => task.Id == id, new string[]{

				});
			}
			return entity;
		}

		public ObservableCollection<Task> GetAll()
		{
			ObservableCollection<Task> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				IEnumerable<Task> entityList = repository.GetAll();
				models = new ObservableCollection<Task>(entityList);
			}
			return models;
		}

		public int AddModel(Task model)
		{
			int id;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				repository.Add(model);
				context.Commit();
				if (TaskAdded != null)
					TaskAdded(this, new ModelAddedEventArgs<Task>(model));
				id = model.Id;
			}
			return id;
		}
		public int AddModelAndCreateProcesses(Task model)
		{
			int id;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				foreach (var ssa in model.StateStation.StateStationActivities)
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
				repository.Add(model);
				context.Commit();
				if (TaskAdded != null)
					TaskAdded(this, new ModelAddedEventArgs<Task>(model));
				id = model.Id;
			}
			return id;
		}

		public void UpdateModel(Task model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Task model)
		{
			using (var context = new SoheilEdmContext())
			{
				DeleteModel(model, context);
				context.Commit();
			}
		}
		public void DeleteModel(int taskId)
		{
			using (var context = new SoheilEdmContext())
			{
				var model = new Repository<Task>(context).FirstOrDefault(x => x.Id == taskId);
				DeleteModel(model, context);
				context.Commit();
			}
		}
		/// <summary>
		/// No commit
		/// </summary>
		/// <param name="model"></param>
		/// <param name="context"></param>
		internal void DeleteModel(Task model, SoheilEdmContext context)
		{
			var taskRepository = new Repository<Task>(context);
			var entity = taskRepository.First(x => x.Id == model.Id);
			if (new Repository<TaskReport>(context).Exists(x => x.Task.Id == model.Id))
				throw new RoutedException("You can't delete this Task. It has Reports", ExceptionLevel.Error, model);

			var taskReportDs = new TaskReportDataService();
			var taskReportRepos = new Repository<TaskReport>(context);
			foreach (var taskReportEnt in entity.TaskReports)
			{
				taskReportDs.DeleteModel(taskReportEnt, taskReportRepos, context);
			}
			foreach (var process in entity.Processes.ToList())
			{
				foreach (var po in process.ProcessOperators.ToList())
				{
					new Repository<ProcessOperator>(context).Delete(po);
				}
				foreach (var sm in process.SelectedMachines.ToList())
				{
					new Repository<SelectedMachine>(context).Delete(sm);
				}
				new Repository<Process>(context).Delete(process);
			}
			taskRepository.Delete(entity);
		}

		public void AttachModel(Task model)
		{
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				if (repository.Exists(x => x.Id == model.Id))
				{
					UpdateModel(model);
				}
				else
				{
					AddModel(model);
				}
			}
		}

		#endregion

		public event EventHandler<ModelAddedEventArgs<Task>> TaskAdded;
		public event EventHandler<ModelUpdatedEventArgs<Task>> TaskUpdated;
		public event EventHandler<ModelAddedEventArgs<TaskReport>> TaskReportAdded;
		public event EventHandler<ModelRemovedEventArgs> TaskReportRemoved;

		/// <summary>
		/// Gets all active activitys as view models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Task> GetActives()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Task> GetInRange(DateTime startDate, DateTime endDate, SoheilEdmContext context)
		{
			var repository = new Repository<Task>(context);
			IEnumerable<Task> entityList = repository.Find(x => x.StartDateTime < endDate && x.EndDateTime >= startDate, y => y.StartDateTime);
			return entityList;
		}

		//tasks in specified station, after (or partially after) startDate
		public IEnumerable<Task> GetInRange(DateTime startDate, int stationId, SoheilEdmContext context)
		{
			var inRangeTasks = new Repository<Task>(context).Find(
				x => x.StateStation.Station.Id == stationId
				&& x.EndDateTime >= startDate
				, y => y.StartDateTime);
			return inRangeTasks;
		}

		public IEnumerable<Process> GetProcesses(int taskId)
		{
			List<Process> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				Task entity = repository.FirstOrDefault(x => x.Id == taskId,
					"Processes",
					"Processes.StateStationActivity",
					"Processes.StateStationActivity.Activity");
				models = new List<Process>(entity.Processes);
			}

			return models;
		}

		public ObservableCollection<TaskReport> GetTaskReports(int taskId)
		{
			ObservableCollection<TaskReport> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				Task entity = repository.FirstOrDefault(x => x.Id == taskId);
				models = new ObservableCollection<TaskReport>(entity.TaskReports);
			}

			return models;
		}

		public KeyValuePair<int, TimeSpan> GetSumOfReportedData(int taskId)
		{
			int healthy = 0;
			TimeSpan duration = new TimeSpan();
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				Task entity = repository.First(x => x.Id == taskId);
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

		#region ViewModel Links
		internal void AttachModelFromVm(PPEditorStation vm)
		{
			Task oldModel = null;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Task>(context);
				var model = repository.FirstOrDefault(x => x.Id == vm.TaskId);
				if (model == null)
				{
					model = new Model.Task();
					model.StateStation = new Repository<StateStation>(context).FirstOrDefault(
						x => x.State.Id == vm.StateId && x.Station.Id == vm.StationId);
				}
				else
				{
					//oldModel stores only those properties which can help PPEditor find and remove the corresponding old VM
					oldModel = new Task
					{
						Id = model.Id,
						StartDateTime = model.StartDateTime,
						DurationSeconds = model.DurationSeconds,
						EndDateTime = model.StartDateTime.AddSeconds(model.DurationSeconds),
					};
				}

				//find and update start
				var startDT = (vm.IsAutoStart) ? 
					DateTime.Now : 
					vm.StartDate.Date.AddTicks(vm.StartTime.Ticks);
				var smartManager = new PP.Smart.SmartManager(this, context);
				
				if (model.StateStation.State.OnProductRework == null)
					throw new Exception("نیاز به بازنگری تنظیمات این مرحله از FPC وجود دارد");

				var seq = smartManager.FindNextFreeSpace(
					vm.StationId, model.StateStation.State.OnProductRework.Id, startDT, model.DurationSeconds);
				var taskseq = seq.FirstOrDefault(x => x.Type == PP.Smart.SmartRange.RangeType.NewTask);
				model.StartDateTime = taskseq.StartDT;

				//update duration, end & taskTP
				model.DurationSeconds = (int)Math.Ceiling(vm.ActivityList.Max(x => x.CycleTime * x.TargetPoint));
				model.EndDateTime = model.StartDateTime.AddSeconds(model.DurationSeconds);
				model.TaskTargetPoint = vm.TaskTargetPoint;

				//inspect processes
				foreach (var processVm in vm.ActivityList)
				{
					Process processModel = null;
					if (processVm.ProcessId > 0)
						processModel = model.Processes.FirstOrDefault(x => x.Id == processVm.ProcessId);
					if (processModel == null)
					{
						#region ======[ New Task ]======
						processModel = new Process();
						processModel.Task = model;
						processModel.TargetCount = processVm.TargetPoint;
						processModel.StateStationActivity = model.StateStation.StateStationActivities.FirstOrDefault(
							x => x.Activity.Id == processVm.ActivityId);
						//	processModel.Code = string.Format("{0}/{1}", processModel.StateStationActivity.Activity.Code, model.Id);
						#region Add Machines
						foreach (var machineVm in processVm.MachineList)
						{
							var selectedMachineModel = new SelectedMachine
							{
								Process = processModel,
								StateStationActivityMachine = processModel.StateStationActivity.StateStationActivityMachines.FirstOrDefault(
								x => x.Machine.Id == machineVm.MachineId),
							};
							processModel.SelectedMachines.Add(selectedMachineModel);
						}
						#endregion
						#region Add operator
						foreach (var operatorVm in processVm.OperatorList.Where(x => x.IsSelected))
						{
							var processOperatorModel = new ProcessOperator
							{
								Process = processModel,
								Code = operatorVm.Code,
								Role = operatorVm.Role,
								Operator = new Repository<Operator>(context).FirstOrDefault(x => x.Id == operatorVm.OperatorId)
							};
						}
						#endregion
						model.Processes.Add(processModel);
						#endregion
					}
					else
					{
						#region ======[ Edit Task ]======
						processModel.TargetCount = processVm.TargetPoint;
						//inspect Machines
						foreach (var ssamModel in processModel.StateStationActivity.StateStationActivityMachines)
						{
							var machineVm = processVm.MachineList
								.FirstOrDefault(x => x.MachineId == ssamModel.Machine.Id);
							SelectedMachine selectedMachineModel = null;
							if (machineVm.SelectedMachineId > 0)
								selectedMachineModel = processModel.SelectedMachines
									.FirstOrDefault(x => x.StateStationActivityMachine.Machine.Id == machineVm.MachineId);
							if (machineVm != null)
							{
								//ssam should be in selectedMachines (if it is not, add it)
								if (selectedMachineModel == null)
									processModel.SelectedMachines.Add(new SelectedMachine
									{
										Process = processModel,
										StateStationActivityMachine = ssamModel,
									});
							}
							else
							{
								//ssam should NOT be in selectedMachines (if it is, remove it)
								if (selectedMachineModel != null)
								{
									processModel.SelectedMachines.Remove(selectedMachineModel);
									new Repository<SelectedMachine>(context).Delete(selectedMachineModel);
								}
							}
						}

						//inspect processOperators
						foreach (var operatorVm in processVm.OperatorList)
						{
							var processOperatorModel = processModel.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operatorVm.OperatorId);
							if (operatorVm.IsSelected)
							{
								//operator should be in processOperators (if it is not, add it)
								if (processOperatorModel == null)
									processModel.ProcessOperators.Add(new ProcessOperator
									{
										Operator = new Repository<Operator>(context).FirstOrDefault(x => x.Id == operatorVm.OperatorId),
										Process = processModel,
										Role = operatorVm.Role,
									});
								else
									//just update role
									processOperatorModel.Role = operatorVm.Role;
							}
							else
							{
								//operator should NOT be in processOperators (if it is, remove it)
								if (processOperatorModel != null)
								{
									processModel.ProcessOperators.Remove(processOperatorModel);
									new Repository<ProcessOperator>(context).Delete(processOperatorModel);
								}
							}
						}
						#endregion
					}
				}

				if (!repository.Exists(x => x.Id == model.Id))
					repository.Add(model);

				//update other items of the seq
				var setupDs = new SetupDataService();
				foreach (var item in seq.Where(x => x.Type == PP.Smart.SmartRange.RangeType.DeleteSetup))
				{
					setupDs.DeleteModelById(item.SetupId, context);
				}
				foreach (var item in seq.Where(x=>x.Type == PP.Smart.SmartRange.RangeType.NewSetup))
				{
					setupDs.AddModelBySmart(item, context);
				}

				//save and ...
				context.Commit();

				UpdateVmIdRecursive(vm, model);

				if (oldModel == null)
				{
					if (TaskAdded != null)
						TaskAdded(this, new ModelAddedEventArgs<Task>(model));
				}
				else
				{
					if (TaskUpdated != null)
						TaskUpdated(this, new ModelUpdatedEventArgs<Task>(model, oldModel));
				}
			}
		}

		/// <summary>
		/// Returns a value indicating whether or not this task still exists
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		internal bool UpdateViewModel(ViewModels.PP.PPTaskVm vm)
		{
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Model.Task>(context);
				var model = repository.FirstOrDefault(x => x.Id == vm.Id);
				if (model == null) return false;
				vm.Job = new ViewModels.PP.PPJobVm(model.Job);
				vm.ProductCode = model.StateStation.State.FPC.Product.Code;
				vm.ProductName = model.StateStation.State.FPC.Product.Name;
				vm.ProductColor = model.StateStation.State.FPC.Product.Color;
				vm.IsRework = model.IsRework;
				vm.StateCode = model.StateStation.State.Code;
				vm.StartDateTime = model.StartDateTime;
				vm.DurationSeconds = model.DurationSeconds;

				//check canAddSetupBefore status
				var prev = findPreviousPPItem(repository, new Repository<NonProductiveTask>(context), model.StateStation.Station.Id, model.StartDateTime);
				if (prev.Value1 == null) 
					vm.CanAddSetupBefore = true;//if no tasks before then can add setup
				else
				{
					if (prev.Value2 == null)
					{
						//if prev task is same => can't add setup
						//if prev task differs => can add setup
						vm.CanAddSetupBefore = prev.Value1.StateStation.State.OnProductRework.Id != model.StateStation.State.OnProductRework.Id;
					}
					else
					{
						//if prev setup's 'to' is same => can't add setup
						//if prev setup is going to a different PR => can add setup
						vm.CanAddSetupBefore = prev.Value2.Changeover.ToProductRework.Id != model.StateStation.State.OnProductRework.Id;
					}
				}
			}
			return true;
		}

		private void UpdateVmIdRecursive(ViewModels.PP.Editor.PPEditorStation vm, Model.Task model)
		{
			vm.TaskId = model.Id;
			foreach (var processModel in model.Processes)
			{
				var act = vm.ActivityList.FirstOrDefault(x => x.ActivityId == processModel.StateStationActivity.Activity.Id);
				act.StateStationActivityId = processModel.StateStationActivity.Id;
				act.ProcessId = processModel.Id;
				foreach (var processOperatorModel in processModel.ProcessOperators)
				{
					var oper = act.OperatorList.FirstOrDefault(x => x.OperatorId == processOperatorModel.Operator.Id);
					oper.ProcessOperatorId = processOperatorModel.Id;
				}
				foreach (var selectedMachineModel in processModel.SelectedMachines)
				{
					var mac = act.MachineList.FirstOrDefault(x => x.MachineId == selectedMachineModel.StateStationActivityMachine.Machine.Id);
					mac.SelectedMachineId = selectedMachineModel.Id;
				}
			}
		}

		internal IList<PPEditorState> GetEditorStateList(IList<int> taskIds)
		{
			List<PPEditorState> stateList = new List<PPEditorState>();
			using (var context = new SoheilEdmContext())
			{
				foreach (var taskId in taskIds)
				{
					var model = new Repository<Task>(context).Single(x => x.Id == taskId);
					var taskMate = stateList.FirstOrDefault(x => x.StateId == model.StateStation.State.Id);
					if (taskMate == null)
						stateList.Add(new PPEditorState(model));
					else
						//Add the Task to 'taskMate' as a new StateStation (or PPEditorStation)
						taskMate.StationList.Add(new PPEditorStation(taskMate, model));
				}
			}
			return stateList;
		}
		#endregion

		internal IEnumerable<Task> GetJobTasks(int jobId)
		{
			using (var context = new SoheilEdmContext())
			{
				return new Repository<Task>(context).Find(x => x.Job != null && x.Job.Id == jobId).ToList();
			}
		}

		public class InsertSetupBeforeTaskErrors
		{
			public List<Pair<ErrorSource, string, int>> Errors = new List<Pair<ErrorSource, string, int>>();
			public enum ErrorSource { Task, NPT, This }
			public bool IsSaved = false;
		}
		public InsertSetupBeforeTaskErrors InsertSetupBeforeTask(int Id)
		{
			var result = new InsertSetupBeforeTaskErrors();

			try
			{
				using (var context = new SoheilEdmContext())
				{
					var taskRepository = new Repository<Task>(context);
					var nptRepository = new Repository<NonProductiveTask>(context);

					var task = taskRepository.FirstOrDefault(x => x.Id == Id,
						"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");
					
					var setupStartDateTime = task.StartDateTime;

					var movingTasks = taskRepository.Find(x =>
						x.StateStation.Station.Id == task.StateStation.Station.Id
						&& x.StartDateTime >= setupStartDateTime)
						.ToArray();
					var movingNpts = nptRepository.Find(x => x.StartDateTime >= setupStartDateTime);//this has no station
					var movingSetups = movingNpts.OfType<Setup>().Where(x => x.Warmup.Station.Id == task.StateStation.Station.Id);
					//var movingEducations = movingNpts.OfType<Education>().Where(x => x.Task.StateStation.Station.Id == task.StateStation.Station.Id);
					//etc...

					//if any of them have report, quit
					if (movingTasks.Any(x => x.TaskReports.Any()))
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource,string,int>(
							InsertSetupBeforeTaskErrors.ErrorSource.Task,
							"این Task گزارش دارد و قابل جابجایی نیست.",
							movingTasks.First(x => x.TaskReports.Any()).Id));
						result.IsSaved = false;
						return result;
					}
					if (movingSetups.Any(x => x.NonProductiveTaskReport != null))
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
							InsertSetupBeforeTaskErrors.ErrorSource.NPT,
							"این Task گزارش دارد و قابل جابجایی نیست.",
							movingSetups.First(x => x.NonProductiveTaskReport != null).Id));
						result.IsSaved = false;
						return result;
					}
					//etc...

					//find previous thing
					var previousItem = findPreviousPPItem(taskRepository, nptRepository, task.StateStation.Station.Id, setupStartDateTime);
				
					int delaySeconds = 0;

					//check changeover
					var changeover = findChangeover(task, previousItem.Value1, context, result);
					if (changeover != null)
						delaySeconds += changeover.Seconds;

					//check warmup
					var warmup = findWarmup(task, context, result);
					if (warmup != null)
						delaySeconds += warmup.Seconds;

					//check if previousSetup needs to be removed
					bool needToDeletePreviousSetup = false;
					if (previousItem.Value2 != null)
					{
						if (previousItem.Value2.Warmup.Id == warmup.Id && previousItem.Value2.Changeover.Id == changeover.Id)
						{
							result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
								InsertSetupBeforeTaskErrors.ErrorSource.This,
								"برای این Task راه اندازی وجود دارد لذا راه اندازی جدید افزوده نشد",
								-1));
							result.IsSaved = false;
							return result;
						}
						else//it is a wrong setup
						{
							nptRepository.Delete(previousItem.Value2);
							needToDeletePreviousSetup = true;
						}
					}

					//if it's zero seconds
					if (delaySeconds == 0)
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
							InsertSetupBeforeTaskErrors.ErrorSource.This,
							"زمان کل برابر با صفر است، لذا راه اندازی افزوده نشد",
							0));
						result.IsSaved = needToDeletePreviousSetup;
						if(needToDeletePreviousSetup) context.SaveChanges();
						return result;
					}

					//add setuptime
					nptRepository.Add(new Setup
					{
						Changeover = changeover,
						Warmup = warmup,
						StartDateTime = setupStartDateTime,
						DurationSeconds = delaySeconds,
						EndDateTime = setupStartDateTime.AddSeconds(delaySeconds),
					});

					//move...
					foreach (var movingTask in movingTasks)
					{
						movingTask.StartDateTime = movingTask.StartDateTime.AddSeconds(delaySeconds);
						movingTask.EndDateTime = movingTask.EndDateTime.AddSeconds(delaySeconds);
					}
					foreach (var movingSetup in movingSetups)
					{
						movingSetup.StartDateTime = movingSetup.StartDateTime.AddSeconds(delaySeconds);
						movingSetup.EndDateTime = movingSetup.EndDateTime.AddSeconds(delaySeconds);
					}
					//etc...
					context.SaveChanges();
					result.IsSaved = true;
				}
			}
			catch (Exception exp)
			{
				result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
					InsertSetupBeforeTaskErrors.ErrorSource.This,
					exp.Message,
					0));
				return result;
			}
			return result;
		}


		/// <summary>
		/// <para>Returns previousTask and previousSetup which start before (or at the) start</para>
		/// <para>if previousSetup is before previousTask it is considered as null</para>
		/// <para>This method is for auto setup-time add</para>
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public Pair<Task, Setup> FindPreviousTask(int stationId, DateTime start)
		{
			using (var context = new SoheilEdmContext())
			{
				var taskRepository = new Repository<Task>(context);
				var nptRepository = new Repository<NonProductiveTask>(context);
				return findPreviousPPItem(taskRepository, nptRepository, stationId, start);
			}
		}
		/// <summary>
		/// <para>Returns nextTask and nextSetup which start after (or at the) end</para>
		/// <para>if nextSetup is after nextTask it is considered as null</para>
		/// <para>This method is for auto setup-time add</para>
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="productReworkId"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Pair<Task, Setup> FindNextTask(int stationId, int productReworkId, DateTime end)
		{
			using (var context = new SoheilEdmContext())
			{
				var taskRepository = new Repository<Task>(context);
				var nptRepository = new Repository<NonProductiveTask>(context);
				return findNextPPItem(taskRepository, nptRepository, stationId, end);
			}
		}
		/// <summary>
		/// See FindPreviousTask
		/// </summary>
		/// <param name="taskRepository"></param>
		/// <param name="nptRepository"></param>
		/// <param name="stationId"></param>
		/// <param name="start"></param>
		/// <returns>Task or Setup</returns>
		private Pair<Task, Setup> findPreviousPPItem(Repository<Task> taskRepository,
			Repository<NonProductiveTask> nptRepository, int stationId, DateTime start)
		{
			var previousTask = taskRepository.LastOrDefault(x =>
				x.StateStation.Station.Id == stationId
				&& x.StartDateTime <= start,
				dt => dt.StartDateTime,
				"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

			var previousSetup = nptRepository
				.OfType<Setup>("Warmup","Warmup.Station","Warmup.ProductRework","Changeover","Changeover.FromProductRework","Changeover.Station")
				.OrderByDescending(x => x.StartDateTime)
				.FirstOrDefault(x =>
					x.Warmup.Station.Id == stationId
					&& x.StartDateTime <= start);

			if (previousSetup == null || previousTask == null)
				return new Pair<Task, Setup>(previousTask, previousSetup);
			return new Pair<Task, Setup>(previousTask, 
				(previousSetup.StartDateTime >= previousTask.EndDateTime) ? previousSetup : null);
		}
		private Pair<Task, Setup> findNextPPItem(Repository<Task> taskRepository,
			Repository<NonProductiveTask> nptRepository, int stationId, DateTime end)
		{
			var nextTask = taskRepository.FirstOrDefault(x =>
				x.StateStation.Station.Id == stationId
				&& x.StartDateTime >= end,
				dt => dt.StartDateTime,
				"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

			var nextSetup = nptRepository
				.OfType<Setup>()
				.OrderBy(x => x.StartDateTime)
				.FirstOrDefault(x =>
					x.Warmup.Station.Id == stationId
					&& x.StartDateTime >= end);

			if (nextSetup == null || nextTask == null)
				return new Pair<Task, Setup>(nextTask, nextSetup);
			return new Pair<Task, Setup>(nextTask, 
				(nextSetup.EndDateTime <= nextTask.StartDateTime) ? nextSetup : null);
		}
		public Changeover FindChangeover(int stationId, int fromProductReworkId, int toProductReworkId)
		{
			Changeover changeover = null;
			using (var context = new SoheilEdmContext())
			{
				var changeoverRepository = new Repository<Changeover>(context);
				changeover = changeoverRepository.FirstOrDefault(x =>
					x.Station.Id == stationId
					&& x.FromProductRework.Id == fromProductReworkId
					&& x.ToProductRework.Id == toProductReworkId);
				if (changeover == null)
				{
					changeover = new Changeover
					{
						Station = new Repository<Station>(context).Single(x => x.Id == stationId),
						FromProductRework = new Repository<ProductRework>(context).Single(x => x.Id == stationId),
						ToProductRework = new Repository<ProductRework>(context).Single(x => x.Id == stationId),
						Seconds = 0
					};
					changeoverRepository.Add(changeover);
					context.SaveChanges();
					/*if (result != null)
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
							InsertSetupBeforeTaskErrors.ErrorSource.NPT,
							"زمان تعویض مربوطه پیدا نشد در نتیجه زمان تعویض جدید تعریف و برابر صفر در نظر گرفته شد\n",
							0));
						result.IsSaved = true;
					}*/
				}
			}
			return changeover;
		}
		private Changeover findChangeover(Task task, Task previousTask, SoheilEdmContext context, InsertSetupBeforeTaskErrors result)
		{
			Changeover changeover = null;
			var changeoverRepository = new Repository<Changeover>(context);
			if (previousTask != null)
			{
				changeover = changeoverRepository.FirstOrDefault(x =>
					x.Station.Id == task.StateStation.Station.Id
					&& x.FromProductRework.Id == previousTask.StateStation.State.OnProductRework.Id
					&& x.ToProductRework.Id == task.StateStation.State.OnProductRework.Id);
				if(changeover == null)
				{
					changeover = new Changeover
					{
						Station = task.StateStation.Station,
						FromProductRework = previousTask.StateStation.State.OnProductRework,
						ToProductRework = task.StateStation.State.OnProductRework,
						Seconds = 0
					};
					changeoverRepository.Add(changeover);
					if (result != null)
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
							InsertSetupBeforeTaskErrors.ErrorSource.This,
							"زمان تعویض مربوطه پیدا نشد در نتیجه زمان تعویض جدید تعریف و برابر صفر در نظر گرفته شد\n",
							0));
						result.IsSaved = false;
					}
				}
			}
			return changeover;
		}
		public Warmup FindWarmup(int stationId, int productReworkId)
		{
			Warmup warmup = null;
			using (var context = new SoheilEdmContext())
			{
				var warmupRepository = new Repository<Warmup>(context);
				warmup = warmupRepository.FirstOrDefault(x =>
					x.Station.Id == stationId
					&& x.ProductRework.Id == productReworkId);
				if (warmup == null)
				{
					warmup = new Warmup
					{
						Station = new Repository<Station>(context).Single(x=>x.Id == stationId),
						ProductRework = new Repository<ProductRework>(context).Single(x => x.Id == productReworkId),
						Seconds = 0
					};
					warmupRepository.Add(warmup);
					context.SaveChanges();
					/*if (result != null)
					{
						result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
							InsertSetupBeforeTaskErrors.ErrorSource.NPT,
							"زمان آماده سازی مربوطه پیدا نشد در نتیجه زمان آماده سازی جدید تعریف و برابر صفر در نظر گرفته شد\n",
							warmup.Id));
						result.IsSaved = false;
					}*/
				}
			}
			return warmup;
		}
		private Warmup findWarmup(Task task, SoheilEdmContext context, InsertSetupBeforeTaskErrors result)
		{
			var warmupRepository = new Repository<Warmup>(context);
			var warmup = warmupRepository.FirstOrDefault(x =>
				x.Station.Id == task.StateStation.Station.Id
				&& x.ProductRework.Id == task.StateStation.State.OnProductRework.Id);
			if (warmup == null)
			{
				warmup = new Warmup
				{
					Station = task.StateStation.Station,
					ProductRework = task.StateStation.State.OnProductRework,
					Seconds = 0
				};
				warmupRepository.Add(warmup);
				if (result != null)
				{
					result.Errors.Add(new Pair<InsertSetupBeforeTaskErrors.ErrorSource, string, int>(
						InsertSetupBeforeTaskErrors.ErrorSource.This,
						"زمان آماده سازی مربوطه پیدا نشد در نتیجه زمان آماده سازی جدید تعریف و برابر صفر در نظر گرفته شد\n",
						0));
					result.IsSaved = false;
				}
			}
			return warmup;
		}
	}
}