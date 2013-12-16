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
    public class OperatorDataService : IDataService<Operator>
    {
        #region IDataService<Operator> Members

        public Operator GetSingle(int id)
        {
            Operator entity;
            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<Operator>(context);
                entity = operatorRepository.Single(opr => opr.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Operator> GetAll()
        {
            ObservableCollection<Operator> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                IEnumerable<Operator> entityList = repository.Find(opr=> opr.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<Operator>(entityList);
            }
            return models;
        }

        public ObservableCollection<Operator> GetActives()
        {
            ObservableCollection<Operator> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                IEnumerable<Operator> entityList = repository.Find(opr => opr.Status == (decimal)Status.Active);
                models = new ObservableCollection<Operator>(entityList);
            }
            return models;
        }

        public ObservableCollection<Operator> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Activities)
            {
                ObservableCollection<Operator> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Operator>(context);
                    IEnumerable<Operator> entityList = repository.Find(opr => opr.Status == (decimal)Status.Active && opr.OperatorActivities.Count == 0);
                    models = new ObservableCollection<Operator>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public int AddModel(Operator model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                repository.Add(model);
                context.Commit();
                if (OperatorAdded != null)
                    OperatorAdded(this, new ModelAddedEventArgs<Operator>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Operator model)
        {
            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<Operator>(context);
                Operator entity = operatorRepository.Single(opr => opr.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(Operator model)
        {
        }

        public void AttachModel(Operator model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                if (repository.Exists(opr => opr.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<Operator>> OperatorAdded;
        public event EventHandler<ModelAddedEventArgs<OperatorActivity>> ActivityAdded;
        public event EventHandler<ModelRemovedEventArgs> ActivityRemoved;


        public ObservableCollection<OperatorActivity> GetActivities(int oprId)
        {
            ObservableCollection<OperatorActivity> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                Operator entity = repository.FirstOrDefault(opr => opr.Id == oprId, "OperatorActivities.Operator", "OperatorActivities.Activity");
                models = new ObservableCollection<OperatorActivity>(entity.OperatorActivities.Where(item => item.Activity.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddActivity(int oprId, int activityId)
        {
            using (var context = new SoheilEdmContext())
            {
                var oprRepository = new Repository<Operator>(context);
                var operatorRepository = new Repository<Activity>(context);
                Operator currentOperator = oprRepository.Single(opr => opr.Id == oprId);
                Activity newActivity = operatorRepository.Single(opr => opr.Id == activityId);
                if (currentOperator.OperatorActivities.Any(oprActivity => oprActivity.Operator.Id == oprId && oprActivity.Activity.Id == activityId))
                {
                    return;
                }
                var newOperatorActivity = new OperatorActivity { Activity = newActivity, Operator = currentOperator };
				currentOperator.OperatorActivities.Add(newOperatorActivity);
                context.Commit();
                ActivityAdded(this, new ModelAddedEventArgs<OperatorActivity>(newOperatorActivity));
            }
        }

        public void RemoveActivity(int oprId, int activityId)
        {
            using (var context = new SoheilEdmContext())
            {
                var oprRepository = new Repository<Operator>(context);
                var oprActivityRepository = new Repository<OperatorActivity>(context);
                Operator currentOperator = oprRepository.Single(opr => opr.Id == oprId);
                OperatorActivity currentOperatorActivity =
					currentOperator.OperatorActivities.First(
                        oprActivity =>
                        oprActivity.Operator.Id == oprId && oprActivity.Id == activityId);
                int id = currentOperatorActivity.Id;
                oprActivityRepository.Delete(currentOperatorActivity);
                context.Commit();
                ActivityRemoved(this, new ModelRemovedEventArgs(id));
            }
        }
    }
}