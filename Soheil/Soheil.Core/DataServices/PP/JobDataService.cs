using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.PP.Smart;
using Soheil.Common;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class JobDataService : DataServiceBase, IDataService<Job>
	{
		Repository<Job> _jobRepository;

		public JobDataService()
			: this(new SoheilEdmContext())
		{

		}
		public JobDataService(SoheilEdmContext context)
		{
			this.Context = context;
			_jobRepository = new Repository<Job>(context);
		}

		#region IDataService<Job> Members
		public Job GetSingle(int id)
		{
			return _jobRepository.FirstOrDefault(x => x.Id == id, "ProductRework", "FPC", "ProductRework.Product");
		}

		public System.Collections.ObjectModel.ObservableCollection<Job> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Job> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Job model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(Job model)
		{
			throw new NotImplementedException();
		}
		public void UpdateDescriptionOnly(Job model)
		{
			var entity = _jobRepository.Single(x => x.Id == model.Id);
			entity.Description = model.Description;
			Context.Commit();
		}

		public void DeleteModel(Job model)
		{
			var ent = _jobRepository.FirstOrDefault(x => x.Id == model.Id);
			if (ent == null) return;
			_jobRepository.Delete(ent);
			Context.Commit();
		}
		public void DeleteModel(int jobId)
		{
			var ent = _jobRepository.FirstOrDefault(x => x.Id == jobId);
			if (ent == null) throw new Soheil.Common.SoheilException.SoheilExceptionBase("هیچ Jobای برای این Task پیدا نشد", Common.SoheilException.ExceptionLevel.Warning);
			var blockDs = new BlockDataService(Context);
			foreach (var block in ent.Blocks.ToList())
			{
				blockDs.DeleteModel(block);
			}
			_jobRepository.Delete(ent);
			Context.Commit();
		}

		public void AttachModel(Job model)
		{
			throw new NotImplementedException();
		}
		#endregion

		/// <summary>
		/// Given a job, creates tasks for it and does the other stuff too
		/// </summary>
		/// <param name="jobVms"></param>
		internal void SaveAndGenerateTasks(ViewModels.PP.Editor.PPEditorJob job)
		{
			SaveAndGenerateTasks(new List<ViewModels.PP.Editor.PPEditorJob> { job });
		}
		/// <summary>
		/// Given a list of jobs, creates tasks for all of them and does the other stuff too
		/// </summary>
		/// <param name="jobVms"></param>
		internal void SaveAndGenerateTasks(IList<ViewModels.PP.Editor.PPEditorJob> jobVms)
		{
			var taskDs = new TaskDataService(Context);

			//for each replication happens the following:
			//	create (or update) jobs
			//	create a smart job
			//	create tasks
			//	do other stuff (setups...)
			var blockDs = new BlockDataService(Context);
			var taskRepos = new Repository<Task>(Context);
			var setupDs = new SetupDataService();
			SmartManager manager = new SmartManager(blockDs, new NPTDataService(Context));
			List<SmartRange> toAddSetups = new List<SmartRange>();

			foreach (var jobVm in jobVms.OrderBy(x => 1 / x.Weight))
			{
				foreach (var replica in jobVm.Replications)
				{
					#region Auto Create/Edit Job
					Job jobModel = _jobRepository.FirstOrDefault(x => x.Id == replica.Id);
					if (jobModel == null)
					{
						#region Create Job
						jobModel = new Job
						{
							Code = jobVm.Code,
							Quantity = jobVm.Quantity,
							Weight = jobVm.Weight,
							Deadline = jobVm.Deadline,
							ReleaseTime = jobVm.ReleaseDT,
							Description = jobVm.Description,
							FPC = new Repository<FPC>(Context).FirstOrDefault(x => x.Id == jobVm.FpcId),
							ProductRework = new Repository<ProductRework>(Context).FirstOrDefault(x => x.Id == jobVm.ProductRework.Id),
						};
						_jobRepository.Add(jobModel);
						#endregion
					}
					else
					{
						#region Delete unused Tasks
						foreach (var block in jobModel.Blocks.ToList())//???
						{
							blockDs.DeleteModel(block);
						}
						#endregion

						#region Update Job
						jobModel.Code = jobVm.Code;
						jobModel.Quantity = jobVm.Quantity;
						jobModel.Weight = jobVm.Weight;
						jobModel.Deadline = jobVm.Deadline;
						jobModel.ReleaseTime = jobVm.ReleaseDT;
						jobModel.Description = jobVm.Description;
						jobModel.FPC = new Repository<FPC>(Context).FirstOrDefault(x => x.Id == jobVm.FpcId);
						jobModel.ProductRework = new Repository<ProductRework>(Context)
							.FirstOrDefault(x => x.Id == jobVm.ProductRework.Id);
						#endregion
					}
					#endregion

					SmartJob smartJob = new SmartJob(manager, jobVm);
					manager.SmartJobs.Add(smartJob);

					#region Auto Create Tasks
					//Build Layers of FPC
					foreach (var layer in smartJob.Layers)
					{
						foreach (var step in layer)
						{
							//Set the time
							//first reload State (in smartJob it's loaded by DataService instead of this context)
							step.State = new Repository<State>(Context).FirstOrDefault(x => x.Id == step.State.Id);
							step.MakeTheBestFit();

							//Make the task
							step.BestStateStation = new Repository<StateStation>(Context)
								.FirstOrDefault(x => x.Id == step.BestStateStation.Id);
							var block = smartJob.MakeBlockFrom(step, jobModel);
							blockDs.AddModel(block);

							//other changes
							foreach (var item in step.ChosenSequence.Where(x => x.Type == SmartRange.RangeType.NewSetup))
							{
								toAddSetups.Add(item);
							}
							foreach (var item in step.ChosenSequence.Where(x => x.Type == SmartRange.RangeType.DeleteSetup))
							{
								if (!setupDs.DeleteModelById(item.SetupId, Context))
									toAddSetups.RemoveWhere(x =>
										x.StartDT == item.StartDT
										&& x.EndDT == item.EndDT
										&& x.StationId == item.StationId);
							}
						}
					}
					#endregion
				}
			}
			foreach (var item in toAddSetups)
			{
				setupDs.AddModelBySmart(item, Context);
			}
			Context.Commit();
		}



		internal int GetFpcIdForProductId(int productId)
		{
			var fpc = new Repository<FPC>(Context).FirstOrDefault(x => x.Product.Id == productId && x.IsDefault);
			if (fpc == null) return -1;
			return fpc.Id;
		}

		/// <summary>
		/// Returns all jobs which are completely or partially inside the given range
		/// <para>jobs touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="StartDate"></param>
		/// <param name="EndDate"></param>
		/// <param name="byDefinition">[default = true] if set to false, change the criteria to the
		/// <para>time range of "any block in the job" in the specified range</para></param>
		/// <returns></returns>
		internal IEnumerable<Job> GetInRange(DateTime StartDate, DateTime EndDate, bool byDefinition = true)
		{
			if (byDefinition)
				return _jobRepository.Find(job =>
					(job.ReleaseTime < EndDate && job.ReleaseTime >= StartDate)
					||
					(job.Deadline <= EndDate && job.Deadline > StartDate)
					||
					(job.ReleaseTime <= StartDate && job.Deadline >= EndDate)
					);
			else
				return _jobRepository.Find(job => job.Blocks.Any(b =>
					(b.StartDateTime < EndDate && b.StartDateTime >= StartDate)
					||
					(b.EndDateTime <= EndDate && b.EndDateTime > StartDate)
					||
					(b.StartDateTime <= StartDate && b.EndDateTime >= EndDate)
					));
		}
	}
}
