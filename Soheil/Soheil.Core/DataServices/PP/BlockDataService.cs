using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Dal;
using Soheil.Core.Commands;
using Soheil.Common;

namespace Soheil.Core.DataServices
{
	public class BlockDataService : DataServiceBase, IDataService<Block>
	{
		Repository<Block> _blockRepository;
		Repository<NonProductiveTask> _nptRepository;
		Repository<Task> _taskRepository;

		public BlockDataService(SoheilEdmContext context)
		{
			this.context = context;
			_blockRepository = new Repository<Block>(context);
			_nptRepository = new Repository<NonProductiveTask>(context);
			_taskRepository = new Repository<Task>(context);
		}
		public BlockDataService()
			: this(new SoheilEdmContext())
		{
		}

		#region IDataService
		/// <summary>
		/// Gets the fully filled block model
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Block GetSingle(int id)
		{
			return _blockRepository.Single(x => x.Id == id);
		}
/*		public Block GetSingleFull(int id)
		{
			string qstr = 
@"SELECT VALUE block FROM AdventureWorksEntities.Blocks
WHERE block.Id = @id";
			var query = context.CreateQuery<Block>(qstr, new System.Data.Objects.ObjectParameter("id", id));
			return query.FirstOrDefault();
		}*/
		public Block GetSingleFull(int id)
		{
			return _blockRepository.Single(x => x.Id == id,
				"StateStation.State.OnProductRework.Rework",
				"StateStation.State.FPC.Product",
				"StateStation.StateStationActivities.StateStationActivityMachines.Machine",
				"StateStation.Station",
				"Job",
				"Tasks.Processes.ProcessReports",
				"Tasks.Processes.SelectedMachines",
				"Tasks.TaskReports.ProcessReports.ProcessOperatorReports.ProcessOperator.Operator",
				"Tasks.TaskReports.ProcessReports.DefectionReports",
				"Tasks.TaskReports.ProcessReports.StoppageReports"
				);
		}

		public System.Collections.ObjectModel.ObservableCollection<Block> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Block> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Block model)
		{
			_blockRepository.Add(model);
			return model.Id;
		}

		public void UpdateModel(Block model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Block model)
		{
			var taskDataService = new TaskDataService(context);
			var entity = _blockRepository.Single(x => x.Id == model.Id);
			foreach (var task in entity.Tasks.ToArray())
			{
				taskDataService.DeleteModel(task);
			}
			_blockRepository.Delete(entity);
			context.SaveChanges();
		}

		public void AttachModel(Block model)
		{
			throw new NotImplementedException();
		} 
		#endregion

		/// <summary>
		/// Returns all blocks which are completely or partially inside the given range
		/// <para>blocks touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<Block> GetInRange(DateTime startDate, DateTime endDate, int stationId)
		{
			//boundaries not included because otherwise a block won't be fitted in a well-fittable space (see reference: PPEditorBlock)
			return _blockRepository.Find(x => 
				x.StateStation.Station.Id == stationId 
				&&
				((x.StartDateTime < endDate && x.StartDateTime >= startDate)
				||
				(x.EndDateTime <= endDate && x.EndDateTime > startDate)
				||
				(x.StartDateTime <= startDate && x.EndDateTime >= endDate)), 
				y => y.StartDateTime);
		}

		/// <summary>
		/// Returns all block Ids which are completely or partially inside the given range
		/// <para>blocks touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<int> GetIdsInRange(DateTime startDate, DateTime endDate)
		{
			//boundaries not included because otherwise a block won't be fitted in a well-fittable space (see reference: PPEditorBlock)
			return _blockRepository.Find(x =>
				(x.StartDateTime < endDate && x.StartDateTime >= startDate)
				||
				(x.EndDateTime <= endDate && x.EndDateTime > startDate)
				||
				(x.StartDateTime <= startDate && x.EndDateTime >= endDate),
				y => y.StartDateTime)
				
				.Select(x => x.Id);
		}

		//blocks in specified station, after (or partially after) startDate
		public IEnumerable<Block> GetInRange(DateTime startDate, int stationId)
		{
			return _blockRepository.Find(
				x => x.StateStation.Station.Id == stationId
				&& x.EndDateTime >= startDate
				, y => y.StartDateTime);
		}

		internal IEnumerable<Block> GetJobBlocks(int jobId)
		{
			return _blockRepository.Find(x => x.Job != null && x.Job.Id == jobId).ToList();
		}

		internal void DeleteModelById(int blockId)
		{
			DeleteModel(_blockRepository.Single(x => x.Id == blockId));
			context.Commit();
		}

		/// <summary>
		/// Get infos of all taskReports associated with the given block
		/// <para>The info includes: the sum of TaskProducedG1's, % of reported targetpoints</para>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		internal int[] GetProductionReportData(Block model)
		{
			int g1 = 0;
			int taskTp = 0;
			int reportedTaskTp = 0;
			foreach (var task in model.Tasks)
			{
				g1 += task.TaskReports.Sum(x => x.TaskProducedG1);
				taskTp += task.TaskTargetPoint;
				reportedTaskTp += task.TaskReports.Sum(x => x.TaskReportTargetPoint);
			}
			return new int[] { g1, taskTp == 0 ? 0 : 100 * reportedTaskTp / taskTp };
		}

		internal void SaveBlock(Block block)
		{
			if (!_blockRepository.Exists(x => x.Id == block.Id))
				_blockRepository.Add(block);
			block.Code = string.Format("{0:D3}{1:D2}{2}.",
				(int)block.StartDateTime.GetPersianDayOfYear(),
				block.StartDateTime.Hour,
				block.StateStation.Station.Code);

			//fix tasks
			var time = block.StartDateTime;
			foreach (var task in block.Tasks)
			{
				//fix time
				task.StartDateTime = time;
				time = time.AddSeconds(task.DurationSeconds);
				task.EndDateTime = time;

				var invalidProcessGroup = task.Processes.GroupBy(p => p.StateStationActivity.Activity.Id).FirstOrDefault(ag => ag.Count() > 1);
				if (invalidProcessGroup != null)
					throw new Soheil.Common.SoheilException.RoutedException(
						"یک فعالیت نمی تواند بیش از یک بار استفاده شود",
						Common.SoheilException.ExceptionLevel.Error,
						invalidProcessGroup.First());
			}
			context.Commit();
		}

/*		internal void AttachModelFromVm(PPEditorTask vm)
		{
			Task oldModel = null;
			var model = _taskRepository.FirstOrDefault(x => x.Id == vm.TaskId);
			if (model == null)
			{
				model = new Model.Task();
				model.Block.StateStation = new Repository<StateStation>(context).FirstOrDefault(
					x => x.State.Id == vm.Block.StateId && x.Station.Id == vm.Block.StationId);
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
			var smartManager = new PP.Smart.SmartManager(this, new NPTDataService(context));

			if (model.Block.StateStation.State.OnProductRework == null)
				throw new Exception("نیاز به بازنگری تنظیمات این مرحله از FPC وجود دارد\nدوباره کاری نامشخص است");

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

			if (!_taskRepository.Exists(x => x.Id == model.Id))
				_taskRepository.Add(model);

			//update other items of the seq
			var setupDs = new SetupDataService();
			foreach (var item in seq.Where(x => x.Type == PP.Smart.SmartRange.RangeType.DeleteSetup))
			{
				setupDs.DeleteModelById(item.SetupId, context);
			}
			foreach (var item in seq.Where(x => x.Type == PP.Smart.SmartRange.RangeType.NewSetup))
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

		/// <summary>
		/// Returns a value indicating whether or not this task still exists
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		internal bool UpdateViewModel(ViewModels.PP.PPTaskVm vm)
		{
			var model = _taskRepository.FirstOrDefault(x => x.Id == vm.Id);
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
			var prev = findPreviousPPItem(
				_taskRepository,
				_nptRepository,
				model.StateStation.Station.Id,
				model.StartDateTime);

			if (prev.Value1 == null)
				vm.CanAddSetupBefore = true;//if no tasks before then can add setup
			else
			{
				if (prev.Value2 == null)
				{
					//if prev task is same => can't add setup
					//if prev task differs => can add setup
					vm.CanAddSetupBefore =
						prev.Value1.StateStation.State.OnProductRework.Id
						!= model.StateStation.State.OnProductRework.Id;
				}
				else
				{
					//if prev setup's 'to' is same => can't add setup
					//if prev setup is going to a different PR => can add setup
					vm.CanAddSetupBefore =
						prev.Value2.Changeover.ToProductRework.Id
						!= model.StateStation.State.OnProductRework.Id;
				}
			}
			return true;
		}

		private void UpdateVmIdRecursive(ViewModels.PP.Editor.PPEditorTask vm, Model.Task model)
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
		}*/

		#region NPT
		/// <summary>
		/// <para>Returns previousBlock and previousSetup which start before start</para>
		/// <para>if previousSetup is before previousBlock it is considered as null</para>
		/// <para>This method is for auto setup-time add</para>
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public Tuple<Block, Setup> FindPreviousBlock(int stationId, DateTime start)
		{
			return findPreviousPPItem(stationId, start);
		}
		/// <summary>
		///Checks if a setup can be added before this block (due to its previous block's state)
		///<para> free space is not considered, i.e. if there's not enough space before block it still may return true</para>
		///<para> if previous item is a setup, returns false</para>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public bool CanAddSetupBeforeBlock(Model.Block model)
		{
			var previousBlock = FindPreviousBlock(model.StateStation.Station.Id, model.StartDateTime);
			if (previousBlock.Item2 == null)
			{
				if (previousBlock.Item1 == null) return true;
				return (previousBlock.Item1.StateStation.Id != model.StateStation.Id);
			}
			return false;
		}
		/// <summary>
		/// <para>Returns nextBlock and nextSetup which start after (or at the) end</para>
		/// <para>if nextSetup is after nextBlock it is considered as null</para>
		/// <para>This method is for auto setup-time add</para>
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Tuple<Block, Setup> FindNextBlock(int stationId, DateTime end)
		{
			return findNextPPItem(stationId, end);
		}
		/// <summary>
		/// See FindPreviousBlock
		/// </summary>
		/// <param name="taskRepository"></param>
		/// <param name="nptRepository"></param>
		/// <param name="stationId"></param>
		/// <param name="start"></param>
		/// <returns>Task or Setup</returns>
		private Tuple<Block, Setup> findPreviousPPItem(int stationId, DateTime start)
		{
			var previousTask = _blockRepository.LastOrDefault(x =>
				x.StateStation.Station.Id == stationId
				&& x.StartDateTime < start,
				dt => dt.StartDateTime,
				"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

			var previousSetup = _nptRepository
				.OfType<Setup>("Warmup", "Warmup.Station", "Warmup.ProductRework", "Changeover", "Changeover.FromProductRework", "Changeover.Station")
				.OrderByDescending(x => x.StartDateTime)
				.FirstOrDefault(x =>
					x.Warmup.Station.Id == stationId
					&& x.StartDateTime < start);

			if (previousSetup == null || previousTask == null)
				return new Tuple<Block, Setup>(previousTask, previousSetup);
			return new Tuple<Block, Setup>(previousTask,
				(previousSetup.StartDateTime >= previousTask.EndDateTime) ? previousSetup : null);
		}
		private Tuple<Block, Setup> findNextPPItem(int stationId, DateTime end)
		{
			var nextTask = _blockRepository.FirstOrDefault(x =>
				x.StateStation.Station.Id == stationId
				&& x.StartDateTime >= end,
				dt => dt.StartDateTime,
				"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

			var nextSetup = _nptRepository
				.OfType<Setup>()
				.OrderBy(x => x.StartDateTime)
				.FirstOrDefault(x =>
					x.Warmup.Station.Id == stationId
					&& x.StartDateTime >= end);

			if (nextSetup == null || nextTask == null)
				return new Tuple<Block, Setup>(nextTask, nextSetup);
			return new Tuple<Block, Setup>(nextTask,
				(nextSetup.EndDateTime <= nextTask.StartDateTime) ? nextSetup : null);
		}


		public Changeover FindChangeover(int stationId, int fromProductReworkId, int toProductReworkId)
		{
			Changeover changeover = null;

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

			return changeover;
		}
		private Changeover findChangeover(Block block, Block previousBlock, InsertSetupBeforeBlockErrors result)
		{
			Changeover changeover = null;
			var changeoverRepository = new Repository<Changeover>(context);
			if (previousBlock != null)
			{
				changeover = changeoverRepository.FirstOrDefault(x =>
					x.Station.Id == block.StateStation.Station.Id
					&& x.FromProductRework.Id == previousBlock.StateStation.State.OnProductRework.Id
					&& x.ToProductRework.Id == block.StateStation.State.OnProductRework.Id);
				if (changeover == null)
				{
					changeover = new Changeover
					{
						Station = block.StateStation.Station,
						FromProductRework = previousBlock.StateStation.State.OnProductRework,
						ToProductRework = block.StateStation.State.OnProductRework,
						Seconds = 0
					};
					changeoverRepository.Add(changeover);
					if (result != null)
					{
						result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
							InsertSetupBeforeBlockErrors.ErrorSource.This,
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

			var warmupRepository = new Repository<Warmup>(context);
			warmup = warmupRepository.FirstOrDefault(x =>
				x.Station.Id == stationId
				&& x.ProductRework.Id == productReworkId);
			if (warmup == null)
			{
				warmup = new Warmup
				{
					Station = new Repository<Station>(context).Single(x => x.Id == stationId),
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

			return warmup;
		}
		private Warmup findWarmup(Block block, InsertSetupBeforeBlockErrors result)
		{
			var warmupRepository = new Repository<Warmup>(context);
			var warmup = warmupRepository.FirstOrDefault(x =>
				x.Station.Id == block.StateStation.Station.Id
				&& x.ProductRework.Id == block.StateStation.State.OnProductRework.Id);
			if (warmup == null)
			{
				warmup = new Warmup
				{
					Station = block.StateStation.Station,
					ProductRework = block.StateStation.State.OnProductRework,
					Seconds = 0
				};
				warmupRepository.Add(warmup);
				if (result != null)
				{
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.This,
						"زمان آماده سازی مربوطه پیدا نشد در نتیجه زمان آماده سازی جدید تعریف و برابر صفر در نظر گرفته شد\n",
						0));
					result.IsSaved = false;
				}
			}
			return warmup;
		}

		public class InsertSetupBeforeBlockErrors
		{
			public List<Tuple<ErrorSource, string, int>> Errors = new List<Tuple<ErrorSource, string, int>>();
			public enum ErrorSource { Task, NPT, This }
			public bool IsSaved = false;
		}
		public InsertSetupBeforeBlockErrors InsertSetupBeforeBlock(int blockId)
		{
			var result = new InsertSetupBeforeBlockErrors();

			try
			{
				var block = _blockRepository.FirstOrDefault(x => x.Id == blockId,
					"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

				var setupStartDateTime = block.StartDateTime;

				var movingBlocks = _blockRepository.Find(x =>
					x.StateStation.Station.Id == block.StateStation.Station.Id
					&& x.StartDateTime >= setupStartDateTime)
					.ToArray();
				var movingNpts = _nptRepository.Find(x => x.StartDateTime >= setupStartDateTime);//this has no station
				var movingSetups = movingNpts.OfType<Setup>().Where(x => x.Warmup.Station.Id == block.StateStation.Station.Id);
				//var movingEducations = movingNpts.OfType<Education>().Where(x => x.Task.StateStation.Station.Id == task.StateStation.Station.Id);
				//etc...

				//if any of them have report, quit
				if (movingBlocks.Any(x => x.Tasks.Any(t => t.TaskReports.Any())))
				{
					//find the task with report
					int errorousId = 0;
					foreach (var movingBlock in movingBlocks)
					{
						var tmp = movingBlock.Tasks.FirstOrDefault(x => x.TaskReports.Any());
						if (tmp != null) { errorousId = tmp.Id; break; }
					}
					//create error
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.Task,
						"این Task گزارش دارد و قابل جابجایی نیست.",
						errorousId));
					result.IsSaved = false;
					return result;
				}
				if (movingSetups.Any(x => x.NonProductiveTaskReport != null))
				{
					//find the setup with report
					int errorousId = movingSetups.First(x => x.NonProductiveTaskReport != null).Id;
					//create error
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.NPT,
						"این Task گزارش دارد و قابل جابجایی نیست.",
						errorousId));
					result.IsSaved = false;
					return result;
				}
				//etc...

				//find previous thing
				var previousItem = findPreviousPPItem(block.StateStation.Station.Id, setupStartDateTime);

				int delaySeconds = 0;

				//check changeover
				var changeover = findChangeover(block, previousItem.Item1, result);
				if (changeover != null)
					delaySeconds += changeover.Seconds;

				//check warmup
				var warmup = findWarmup(block, result);
				if (warmup != null)
					delaySeconds += warmup.Seconds;

				//check if previousSetup needs to be removed
				bool needToDeletePreviousSetup = false;
				if (previousItem.Item2 != null)
				{
					if (previousItem.Item2.Warmup.Id == warmup.Id && previousItem.Item2.Changeover.Id == changeover.Id)
					{
						result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
							InsertSetupBeforeBlockErrors.ErrorSource.This,
							"برای این Task راه اندازی وجود دارد لذا راه اندازی جدید افزوده نشد",
							-1));
						result.IsSaved = false;
						return result;
					}
					else//it is a wrong setup
					{
						_nptRepository.Delete(previousItem.Item2);
						needToDeletePreviousSetup = true;
					}
				}

				//if it's zero seconds
				if (delaySeconds == 0)
				{
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.This,
						"زمان کل برابر با صفر است، لذا راه اندازی افزوده نشد",
						0));
					result.IsSaved = needToDeletePreviousSetup;
					if (needToDeletePreviousSetup) context.SaveChanges();
					return result;
				}

				//add setuptime
				_nptRepository.Add(new Setup
				{
					Changeover = changeover,
					Warmup = warmup,
					StartDateTime = setupStartDateTime,
					DurationSeconds = delaySeconds,
					EndDateTime = setupStartDateTime.AddSeconds(delaySeconds),
				});

				//move...
				foreach (var movingTask in movingBlocks)
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
			catch (Exception exp)
			{
				result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
					InsertSetupBeforeBlockErrors.ErrorSource.This,
					exp.Message,
					0));
				return result;
			}
			return result;
		}

		#endregion
	}
}
