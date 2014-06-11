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
    public class ActivityDataService : DataServiceBase, IDataService<Activity>
    {
		public event EventHandler<ModelAddedEventArgs<Activity>> ActivityAdded;
		public event EventHandler<ModelAddedEventArgs<ActivitySkill>> OperatorAdded;
		public event EventHandler<ModelRemovedEventArgs> OperatorRemoved;
		Repository<Activity> _activityRepository;
		Repository<ActivityGroup> _activityGroupRepository;

		public ActivityDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ActivityDataService(SoheilEdmContext context)
		{
			this.context = context;
			_activityRepository = new Repository<Activity>(context);
			_activityGroupRepository = new Repository<ActivityGroup>(context);
		}

        #region IDataService<Activity> Members

        public Activity GetSingle(int id)
        {
			return _activityRepository.Single(activity => activity.Id == id);
        }

        public ObservableCollection<Activity> GetAll()
        {
			var entityList = _activityRepository.Find(activity => activity.Status != (decimal)Status.Deleted, "ActivityGroup");
			return new ObservableCollection<Activity>(entityList);
        }

        public int AddModel(Activity model)
        {
			int id;
			var activityGroup = _activityGroupRepository.Single(group => group.Id == model.ActivityGroup.Id);
			activityGroup.Activities.Add(model);
			context.Commit();
			if (ActivityAdded != null)
				ActivityAdded(this, new ModelAddedEventArgs<Activity>(model));
			id = model.Id;
            return id;
        }

        public int AddModel(Activity model, int groupId)
        {
			int id;
			var activityGroup = _activityGroupRepository.Single(group => group.Id == groupId);
			activityGroup.Activities.Add(model);
			context.Commit();
			if (ActivityAdded != null)
				ActivityAdded(this, new ModelAddedEventArgs<Activity>(model));
			id = model.Id;
            return id;
        }

		public void UpdateModel(Activity model)
        {
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
            context.Commit();
        }
		public void UpdateModel(Activity model, int groupId)
		{
			var group =
				_activityGroupRepository.Single(activityGroup => activityGroup.Id == groupId);

			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			model.ActivityGroup = group;

			context.Commit();
		}

        public void DeleteModel(Activity model)
        {
        }

        public void AttachModel(Activity model)
        {
			if (_activityRepository.Exists(activity => activity.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		public void AttachModel(Activity model, int groupId)
		{
			if (_activityRepository.Exists(activity => activity.Id == model.Id))
			{
				UpdateModel(model, groupId);
			}
			else
			{
				AddModel(model, groupId);
			}
		}

        #endregion


        /// <summary>
        /// Gets all active activitys as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Activity> GetActives()
        {
			var entityList = _activityRepository.Find(activity => activity.Status == (byte)Status.Active, "ActivityGroup");
			return new ObservableCollection<Activity>(entityList);
        }

        /// <summary>
        /// Gets all active activitys as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Activity> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Operators)
            {
				IEnumerable<Activity> entityList = _activityRepository.Find(activity => 
					activity.Status == (decimal)Status.Active 
					&& activity.ActivitySkills.All(item=>item.Operator.Id != linkId), "ActivityGroup");
				return new ObservableCollection<Activity>(entityList);
            }
            return GetActives();
        }

        public ObservableCollection<ActivitySkill> GetOperators(int activityId)
        {
			Activity entity = _activityRepository.FirstOrDefault(activity => activity.Id == activityId, "ActivitySkills.Activity", "ActivitySkills.Operator");
			return new ObservableCollection<ActivitySkill>(entity.ActivitySkills.Where(item => item.Operator.Status == (decimal)Status.Active));
        }


		public void AddOperator(int activityId, int operatorId)
		{
			var operatorRepository = new Repository<Operator>(context);
			var currentActivity = _activityRepository.Single(activity => activity.Id == activityId);
			var newOperator = operatorRepository.Single(opr => opr.Id == operatorId);
			if (currentActivity.ActivitySkills.Any(activityOperator => 
				activityOperator.Activity.Id == activityId 
				&& activityOperator.Operator.Id == operatorId))
				return;
			var newGeneralActivitySkill = new ActivitySkill { 
				Operator = newOperator, 
				Activity = currentActivity,
				CreatedDate = DateTime.Now,
				ModifiedBy = LoginInfo.Id,
				ModifiedDate = DateTime.Now,
			};
			currentActivity.ActivitySkills.Add(newGeneralActivitySkill);
			context.Commit();
			OperatorAdded(this, new ModelAddedEventArgs<ActivitySkill>(newGeneralActivitySkill));
		}

		public void RemoveOperator(int activityId, int operatorId)
		{
			var currentActivity = _activityRepository.Single(activity => activity.Id == activityId);
			var currentActivityOperator = currentActivity.ActivitySkills.First(activityOperator =>
					activityOperator.Activity.Id == activityId
					&& activityOperator.Id == operatorId);

			int id = currentActivityOperator.Id;
			var activityOperatorRepository = new Repository<ActivitySkill>(context);
			activityOperatorRepository.Delete(currentActivityOperator);
			context.Commit();
			OperatorRemoved(this, new ModelRemovedEventArgs(id));
		}
    }
}