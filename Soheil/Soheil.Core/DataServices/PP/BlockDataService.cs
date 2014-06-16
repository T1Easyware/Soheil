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
				"Tasks.Processes.ProcessReports.OperatorProcessReports.ProcessOperator.Operator",
				"Tasks.Processes.ProcessReports.DefectionReports",
				"Tasks.Processes.ProcessReports.StoppageReports"
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
			if (entity == null) return;

			foreach (var task in entity.Tasks.ToArray())
			{
				taskDataService.DeleteModel(task);
			}
			_blockRepository.Delete(entity);
			context.Commit();
		}
		/// <summary>
		/// Delete a block with all its reports. No questions asked!
		/// </summary>
		/// <param name="model"></param>
		public void DeleteModelRecursive(Block model)
		{
			var taskDataService = new TaskDataService(context);
			var taskReportDataService = new TaskReportDataService(context);
			var processReportDataService = new ProcessReportDataService(context);
			var entity = _blockRepository.Single(x => x.Id == model.Id);
			if (entity == null) return;

			foreach (var task in entity.Tasks.ToArray())
			{
				taskDataService.DeleteModelRecursive(task);
			}
			_blockRepository.Delete(entity);
			context.Commit();
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
		/// Returns all block which are completely or partially inside the given range
		/// <para>blocks touching the range from outside are not counted</para>
		/// <para>only Ids, StationIndex and ModifiedDates are returned</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<Soheil.Core.PP.ItemMetaInfo> GetInRange(DateTime startDate, DateTime endDate)
		{
			//boundaries not included because otherwise a block won't be fitted in a well-fittable space (see reference: PPEditorBlock)
			var tmp = _blockRepository
				.Find(x => !(x.EndDateTime <= startDate || x.StartDateTime >= endDate))
				.OrderBy(y => y.StartDateTime)
				.Select(x => new object[] { x.Id, x.StateStation.Station.Index, x.ModifiedDate });

			return tmp.Select(x => new Soheil.Core.PP.ItemMetaInfo((int)x[0], (int)x[1], (DateTime)x[2]));
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
			block.ModifiedBy = LoginInfo.Id;
			block.ModifiedDate = DateTime.Now;

			//fix tasks
			var time = block.StartDateTime;
			foreach (var task in block.Tasks)
			{
				//check for report
				if (task.TaskReports.Any())
				{
					var diff = task.StartDateTime - task.TaskReports.First().ReportStartDateTime;
					foreach (var taskReport in task.TaskReports)
					{
						taskReport.ReportStartDateTime += diff;
						taskReport.ReportEndDateTime += diff;
						if(taskReport.ReportEndDateTime > task.EndDateTime)
							throw new Soheil.Common.SoheilException.RoutedException(
										"مدت گزارش ایستگاه از مدت برنامه طولانی تر است",
										Common.SoheilException.ExceptionLevel.Error,
										task);
					}
				}

				//fix time
				task.StartDateTime = time;
				time = time.AddSeconds(task.DurationSeconds);
				task.EndDateTime = time;

				//delete empty processes
				foreach (var process in task.Processes.ToArray())
				{
					if (process.TargetCount == 0 || process.StateStationActivity == null)
					{
						foreach (var po in process.ProcessOperators.ToArray())
						{
							process.ProcessOperators.Remove(po);
							new Repository<ProcessOperator>(context).Delete(po);
						}
						foreach (var sm in process.SelectedMachines.ToArray())
						{
							process.SelectedMachines.Remove(sm);
							new Repository<SelectedMachine>(context).Delete(sm);
						}
						task.Processes.Remove(process);
						new Repository<Process>(context).Delete(process);
					}
					else if (!process.StateStationActivity.IsMany)
					{
						if (task.Processes.Any(p =>
							p != process &&
							p.StateStationActivity.Id == process.StateStationActivity.Id &&
							((p.EndDateTime > process.StartDateTime && p.EndDateTime <= process.EndDateTime)||
							(p.StartDateTime >= process.StartDateTime && p.StartDateTime < process.EndDateTime))))
							throw new Soheil.Common.SoheilException.RoutedException(
								string.Format("فعالیت {0} نمی تواند بیش از یک بار استفاده شود", process.StateStationActivity.Activity.Name),
								Common.SoheilException.ExceptionLevel.Error,
								process);
					}
				}
			}
			context.Commit();
		}

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
		/// <para>if nextSetup ends after nextBlock it is considered as null</para>
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
			var tmp = start.AddSeconds(1);
			var previousTask = _blockRepository.LastOrDefault(x =>
				x.StateStation.Station.Id == stationId
				&& x.EndDateTime <= tmp,
				dt => dt.EndDateTime,
				"StateStation.Station", "StateStation.State.OnProductRework");

			var previousSetup = _nptRepository
				.OfType<Setup>("Warmup.Station", "Warmup.ProductRework", "Changeover.FromProductRework", "Changeover.Station")
				.OrderByDescending(x => x.EndDateTime)
				.FirstOrDefault(x =>
					x.Warmup.Station.Id == stationId
					&& x.EndDateTime <= tmp);

			if (previousSetup == null || previousTask == null)
				return new Tuple<Block, Setup>(previousTask, previousSetup);
			return new Tuple<Block, Setup>(previousTask,
				(previousSetup.EndDateTime >= previousTask.EndDateTime) ? previousSetup : null);
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
				context.Commit();
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
				context.Commit();
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

				#region Prev
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
				#endregion

				bool moveNeeded = false;
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
					if (needToDeletePreviousSetup) context.Commit();
					return result;
				}
				//prev is block
				if (previousItem.Item2 == null && previousItem.Item1 != null)
				{
					if ((block.StartDateTime - previousItem.Item1.EndDateTime).Seconds >= delaySeconds)
					{
						//no need to move anything
						setupStartDateTime = block.StartDateTime.AddSeconds(-delaySeconds);
					}
					else moveNeeded = true;
				}


				//re-evaluate movingItems
				var movingBlocks = _blockRepository.Find(x =>
					x.StateStation.Station.Id == block.StateStation.Station.Id
					&& x.StartDateTime >= setupStartDateTime)
					.OrderBy(x => x.StartDateTime).ToArray();
				var movingNpts = _nptRepository.Find(x =>
					x.StartDateTime >= setupStartDateTime)
					.OrderBy(x => x.StartDateTime);//this has no station
				var movingSetups = movingNpts.OfType<Setup>().Where(x =>
					x.Warmup.Station.Id == block.StateStation.Station.Id);
				//var movingEducations = movingNpts.OfType<Education>().Where(x => x.Task.StateStation.Station.Id == task.StateStation.Station.Id);
				//etc...


				#region Problems



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
				#endregion

				#region Move
				//move...
				foreach (var movingBlock in movingBlocks)
				{
					movingBlock.StartDateTime = movingBlock.StartDateTime.AddSeconds(delaySeconds);
					movingBlock.EndDateTime = movingBlock.EndDateTime.AddSeconds(delaySeconds);
					if (movingBlock.Tasks.Any())
					{
						foreach (var task in movingBlock.Tasks)
						{
							task.StartDateTime = task.StartDateTime.AddSeconds(delaySeconds);
							task.EndDateTime = task.EndDateTime.AddSeconds(delaySeconds);
						}
					}
					movingBlock.ModifiedDate = DateTime.Now;
				}
				foreach (var movingSetup in movingSetups)
				{
					movingSetup.StartDateTime = movingSetup.StartDateTime.AddSeconds(delaySeconds);
					movingSetup.EndDateTime = movingSetup.EndDateTime.AddSeconds(delaySeconds);
				}
				//etc... 
				#endregion

				#region Add & Save Setup
				//add setuptime
				_nptRepository.Add(new Setup
				{
					Changeover = changeover,
					Warmup = warmup,
					StartDateTime = setupStartDateTime,
					DurationSeconds = delaySeconds,
					EndDateTime = setupStartDateTime.AddSeconds(delaySeconds),
				});
				context.Commit();
				#endregion

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
		public InsertSetupBeforeBlockErrors InsertSetupBetweenBlocks(int fromId, int blockId)
		{
			var result = new InsertSetupBeforeBlockErrors();

			try
			{
				var fromBlock = _blockRepository.FirstOrDefault(x => x.Id == fromId,
					"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");
				var toBlock = _blockRepository.FirstOrDefault(x => x.Id == blockId,
					"StateStation", "StateStation.Station", "StateStation.State", "StateStation.State.OnProductRework");

				if(fromBlock == null || toBlock == null)
				{
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.This,
						"Task انتخاب شده وجود ندارد",
						fromId));
					result.IsSaved = false;
					return result;
				}

				var setupStartDateTime = fromBlock.EndDateTime;

				int delaySeconds = 0;

				//check changeover
				var changeover = findChangeover(toBlock, fromBlock, result);
				if (changeover != null)
					delaySeconds += changeover.Seconds;

				//check warmup
				var warmup = findWarmup(toBlock, result);
				if (warmup != null)
					delaySeconds += warmup.Seconds; 

				//if it's zero seconds
				if (delaySeconds == 0)
				{
					result.Errors.Add(new Tuple<InsertSetupBeforeBlockErrors.ErrorSource, string, int>(
						InsertSetupBeforeBlockErrors.ErrorSource.This,
						"زمان کل برابر با صفر است، لذا راه اندازی افزوده نشد",
						0));
					result.IsSaved = false;
					return result;
				}
				if ((toBlock.StartDateTime - fromBlock.EndDateTime).Seconds >= delaySeconds)
				{
					//no need to move anything
					setupStartDateTime = toBlock.StartDateTime.AddSeconds(-delaySeconds);
				}


				//re-evaluate movingItems
				var movingBlocks = _blockRepository.Find(x =>
					x.StateStation.Station.Id == toBlock.StateStation.Station.Id
					&& x.StartDateTime >= setupStartDateTime)
					.OrderBy(x => x.StartDateTime).ToArray();
				var movingNpts = _nptRepository.Find(x =>
					x.StartDateTime >= setupStartDateTime)
					.OrderBy(x => x.StartDateTime);//this has no station
				var movingSetups = movingNpts.OfType<Setup>().Where(x =>
					x.Warmup.Station.Id == toBlock.StateStation.Station.Id);
				//var movingEducations = movingNpts.OfType<Education>().Where(x => x.Task.StateStation.Station.Id == task.StateStation.Station.Id);
				//etc...


				#region Problems



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
				#endregion

				#region Move
				//move...
				foreach (var movingBlock in movingBlocks)
				{
					movingBlock.StartDateTime = movingBlock.StartDateTime.AddSeconds(delaySeconds);
					movingBlock.EndDateTime = movingBlock.EndDateTime.AddSeconds(delaySeconds);
					if (movingBlock.Tasks.Any())
					{
						foreach (var task in movingBlock.Tasks)
						{
							task.StartDateTime = task.StartDateTime.AddSeconds(delaySeconds);
							task.EndDateTime = task.EndDateTime.AddSeconds(delaySeconds);
						}
					}
					movingBlock.ModifiedDate = DateTime.Now;
				}
				foreach (var movingSetup in movingSetups)
				{
					movingSetup.StartDateTime = movingSetup.StartDateTime.AddSeconds(delaySeconds);
					movingSetup.EndDateTime = movingSetup.EndDateTime.AddSeconds(delaySeconds);
				}
				//etc... 
				#endregion

				#region Add & Save Setup
				//add setuptime
				_nptRepository.Add(new Setup
				{
					Changeover = changeover,
					Warmup = warmup,
					StartDateTime = setupStartDateTime,
					DurationSeconds = delaySeconds,
					EndDateTime = setupStartDateTime.AddSeconds(delaySeconds),
				});
				context.Commit();
				#endregion

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
