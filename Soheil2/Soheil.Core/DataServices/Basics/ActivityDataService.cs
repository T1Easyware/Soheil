using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ActivityDataService : IDataService<Activity>
    {
        #region IDataService<Activity> Members

        public Activity GetSingle(int id)
        {
            Activity entity;
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                entity = activityRepository.Single(activity => activity.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Activity> GetAll()
        {
            ObservableCollection<Activity> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Activity>(context);
                IEnumerable<Activity> entityList =
                    repository.Find(
                        activity => activity.Status != (decimal)Status.Deleted, "ActivityGroup");
                models = new ObservableCollection<Activity>(entityList);
            }
            return models;
        }

        public int AddModel(Activity model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<ActivityGroup>(context);
                ActivityGroup activityGroup = groupRepository.Single(group => group.Id == model.ActivityGroup.Id);
                activityGroup.Activities.Add(model);
                context.Commit();
                if (ActivityAdded != null)
                    ActivityAdded(this, new ModelAddedEventArgs<Activity>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(Activity model, int groupId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<ActivityGroup>(context);
                ActivityGroup activityGroup = groupRepository.Single(group => group.Id == groupId);
                activityGroup.Activities.Add(model);
                context.Commit();
                if (ActivityAdded != null)
                    ActivityAdded(this, new ModelAddedEventArgs<Activity>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Activity model)
        {
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                var activityGroupRepository = new Repository<ActivityGroup>(context);
                Activity entity = activityRepository.Single(activity => activity.Id == model.Id);
                ActivityGroup group =
                    activityGroupRepository.Single(activityGroup => activityGroup.Id == model.ActivityGroup.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.ActivityGroup = group;

                context.Commit();
            }
        }
        public void UpdateModel(Activity model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                var activityGroupRepository = new Repository<ActivityGroup>(context);
                Activity entity = activityRepository.Single(activity => activity.Id == model.Id);
                ActivityGroup group =
                    activityGroupRepository.Single(activityGroup => activityGroup.Id == groupId);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.ActivityGroup = group;

                context.Commit();
            }
        }

        public void DeleteModel(Activity model)
        {
        }

        public void AttachModel(Activity model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Activity>(context);
                if (repository.Exists(activity => activity.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
            }
        }

        public void AttachModel(Activity model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Activity>(context);
                if (repository.Exists(activity => activity.Id == model.Id))
                {
                    UpdateModel(model,groupId);
                }
                else
                {
                    AddModel(model,groupId);
                }
            }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<Activity>> ActivityAdded;
        public event EventHandler<ModelAddedEventArgs<OperatorActivity>> OperatorAdded;
        public event EventHandler<ModelRemovedEventArgs> OperatorRemoved;

        /// <summary>
        /// Gets all active activitys as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Activity> GetActives()
        {
            ObservableCollection<Activity> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Activity>(context);
                IEnumerable<Activity> entityList =
                    repository.Find(
                        activity => activity.Status == (decimal)Status.Active, "ActivityGroup");
                models = new ObservableCollection<Activity>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active activitys as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Activity> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Operators)
            {
                ObservableCollection<Activity> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Activity>(context);
                    IEnumerable<Activity> entityList =
                        repository.Find(
                            activity => activity.Status == (decimal)Status.Active && activity.OperatorActivities.Count == 0, "ActivityGroup");
                    models = new ObservableCollection<Activity>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<OperatorActivity> GetOperators(int activityId)
        {
            ObservableCollection<OperatorActivity> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Activity>(context);
                Activity entity = repository.FirstOrDefault(activity => activity.Id == activityId, "OperatorActivities.Activity", "OperatorActivities.Operator");
				models = new ObservableCollection<OperatorActivity>(entity.OperatorActivities.Where(item=>item.Operator.Status == (decimal)Status.Active));
            }

            return models;
        }


        public void AddOperator(int activityId, int operatorId)
        {
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                var operatorRepository = new Repository<Operator>(context);
                Activity currentActivity = activityRepository.Single(activity => activity.Id == activityId);
                Operator newOperator = operatorRepository.Single(opr => opr.Id == operatorId);
				if (currentActivity.OperatorActivities.Any(activityOperator => activityOperator.Activity.Id == activityId && activityOperator.Operator.Id == operatorId))
                {
                    return;
                }
                var newOperatorActivity = new OperatorActivity { Operator = newOperator, Activity = currentActivity };
				currentActivity.OperatorActivities.Add(newOperatorActivity);
                context.Commit();
                OperatorAdded(this, new ModelAddedEventArgs<OperatorActivity>(newOperatorActivity));
            }
        }

        public void RemoveOperator(int activityId, int operatorId)
        {
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                var activityOperatorRepository = new Repository<OperatorActivity>(context);
                Activity currentActivity = activityRepository.Single(activity => activity.Id == activityId);
                OperatorActivity currentActivityOperator =
					currentActivity.OperatorActivities.First(
                        activityOperator =>
                        activityOperator.Activity.Id == activityId && activityOperator.Id == operatorId);
                int id = currentActivityOperator.Id;
                activityOperatorRepository.Delete(currentActivityOperator);
                context.Commit();
                OperatorRemoved(this, new ModelRemovedEventArgs(id));
            }
        }
    }
}