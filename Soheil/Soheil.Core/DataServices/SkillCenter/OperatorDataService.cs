using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class OperatorDataService : DataServiceBase, IDataService<Operator>
	{
		Repository<Operator> _operatorRepository;
		public OperatorDataService()
			: this(new SoheilEdmContext())
		{

		}
		public OperatorDataService(SoheilEdmContext context)
		{
			this.Context = context;
			_operatorRepository = new Repository<Operator>(context);
		}

		#region IDataService<Operator> Members

		public Operator GetSingle(int id)
		{
			return _operatorRepository.Single(opr => opr.Id == id);
		}

		public ObservableCollection<Operator> GetAll()
		{
			IEnumerable<Operator> entityList = _operatorRepository.Find(opr => opr.Status != (byte)Status.Deleted);
			return new ObservableCollection<Operator>(entityList);
		}

		public ObservableCollection<Operator> GetActives()
		{
			IEnumerable<Operator> entityList = _operatorRepository.Find(opr => opr.Status == (byte)Status.Active);
			return new ObservableCollection<Operator>(entityList);
		}

		public ObservableCollection<Operator> GetActives(SoheilEntityType linkType, int linkId = 0)
		{
			if (linkType == SoheilEntityType.Activities)
			{
				IEnumerable<Operator> entityList = _operatorRepository.Find(opr => opr.Status == (byte)Status.Active && opr.ActivitySkills.All(item => item.Activity.Id != linkId));
				return new ObservableCollection<Operator>(entityList);
			}
			return GetActives();
		}

		public int AddModel(Operator model)
		{
			_operatorRepository.Add(model);
			Context.Commit();
			if (OperatorAdded != null)
				OperatorAdded(this, new ModelAddedEventArgs<Operator>(model));
			return model.Id;
		}

		public void UpdateModel(Operator model)
		{
			Operator entity = _operatorRepository.Single(opr => opr.Id == model.Id);

			entity.Code = model.Code;
			entity.Name = model.Name;
			entity.Status = model.Status;
			entity.CreatedDate = model.CreatedDate;
			entity.ModifiedBy = LoginInfo.Id;
			entity.ModifiedDate = DateTime.Now;
			Context.Commit();
		}

		public void DeleteModel(Operator model)
		{
		}

		public void AttachModel(Operator model)
		{
			if (_operatorRepository.Exists(opr => opr.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		#endregion

		public event EventHandler<ModelAddedEventArgs<Operator>> OperatorAdded;
		public event EventHandler<ModelAddedEventArgs<ActivitySkill>> ActivityAdded;
		public event EventHandler<ModelRemovedEventArgs> ActivityRemoved;


		public ObservableCollection<ActivitySkill> GetActivities(int oprId)
		{
			Operator entity = _operatorRepository.FirstOrDefault(opr => opr.Id == oprId,
				"ActivitySkills.Operator",
				"ActivitySkills.Activity");
			return new ObservableCollection<ActivitySkill>(
				entity.ActivitySkills.Where(item => item.Activity.Status == (decimal)Status.Active).ToList());
		}

		public void AddActivity(int oprId, int activityId)
		{
			var operatorRepository = new Repository<Activity>(Context);
			Operator currentOperator = _operatorRepository.Single(opr => opr.Id == oprId);
			Activity newActivity = operatorRepository.Single(opr => opr.Id == activityId);
			if (currentOperator.ActivitySkills.Any(oprActivity => oprActivity.Operator.Id == oprId && oprActivity.Activity.Id == activityId))
			{
				return;
			}
			var newGeneralActivitySkill = new ActivitySkill
			{
				Activity = newActivity,
				Operator = currentOperator,
				CreatedDate = DateTime.Now,
				ModifiedDate = DateTime.Now,
				ModifiedBy = LoginInfo.Id,
			};
			currentOperator.ActivitySkills.Add(newGeneralActivitySkill);
			Context.Commit();
			ActivityAdded(this, new ModelAddedEventArgs<ActivitySkill>(newGeneralActivitySkill));
		}

		public void RemoveActivity(int oprId, int activityId)
		{
			var oprActivityRepository = new Repository<ActivitySkill>(Context);
			Operator currentOperator = _operatorRepository.Single(opr => opr.Id == oprId);
			ActivitySkill currentGeneralActivitySkill =
				currentOperator.ActivitySkills.First(
					oprActivity =>
					oprActivity.Operator.Id == oprId && oprActivity.Id == activityId);
			int id = currentGeneralActivitySkill.Id;
			oprActivityRepository.Delete(currentGeneralActivitySkill);
			Context.Commit();
			ActivityRemoved(this, new ModelRemovedEventArgs(id));
		}

		/// <summary>
		/// Returns working status of an operator relative to the given process and time range
		/// </summary>
		/// <param name="model">model of operator to use</param>
		/// <param name="process">model of process to compare with (can't be null)</param>
		/// <returns>bool[3] => [0]:IsSelected in process, [1]:IsInTask (other processes of task) [2]:IsInTimeRange (other tasks or stations)</returns>
		public bool[] GetOperatorStatus(Model.Operator model, Model.Process process)
		{
			if (process == null) return new bool[] { false, false, false };

			var procOpers = model.ProcessOperators.Where(x => x.Process.Task != null).Where(x =>
				x.Process.Task.StartDateTime < process.EndDateTime &&
				x.Process.Task.EndDateTime > process.StartDateTime);

			return new bool[]{
				process.ProcessOperators.Any(x => x.Operator.Id == model.Id),
				process.Task.Processes.Any(p => 
					p != process &&
					p.ProcessOperators.Any(po => po.Operator.Id == model.Id)),
				procOpers.Any(x => x.Process.Task != process.Task),
			};
		}

		/// <summary>
		/// Returns working status of an operator relative to the given process and time range
		/// </summary>
		/// <param name="model">model of operator to use</param>
		/// <param name="process">model of task to compare with (can't be null)</param>
		/// <returns>bool[3] => [0]:IsSelected in process, [1]:IsInTask (other processes of task) [2]:IsInTimeRange (other tasks or stations)</returns>
		public bool[] GetOperatorStatus(Model.Operator model, Model.Task task)
		{
			return new bool[]{
				false,
				task.Processes.Any(p => p.ProcessOperators.Any(po => po.Operator.Id == model.Id)),
				model.ProcessOperators.Where(x => x.Process.Task != null).Any(x =>
					x.Process.Task != task &&
					x.Process.Task.StartDateTime < task.EndDateTime &&
					x.Process.Task.EndDateTime > task.StartDateTime),
			};
		}
	}
}