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
			:this(new SoheilEdmContext())
		{

		}
		public OperatorDataService(SoheilEdmContext context)
		{
			this.context = context;
			_operatorRepository = new Repository<Operator>(context);
		}

        #region IDataService<Operator> Members

        public Operator GetSingle(int id)
        {
			return _operatorRepository.Single(opr => opr.Id == id);
        }

        public ObservableCollection<Operator> GetAll()
        {
                IEnumerable<Operator> entityList = _operatorRepository.Find(opr=> opr.Status != (decimal)Status.Deleted);
                return new ObservableCollection<Operator>(entityList);
        }

        public ObservableCollection<Operator> GetActives()
        {
                IEnumerable<Operator> entityList = _operatorRepository.Find(opr => opr.Status == (decimal)Status.Active);
                return new ObservableCollection<Operator>(entityList);
        }

        public ObservableCollection<Operator> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Activities)
            {
                    IEnumerable<Operator> entityList = _operatorRepository.Find(opr => opr.Status == (decimal)Status.Active && opr.OperatorActivities.Count == 0);
                    return new ObservableCollection<Operator>(entityList);
            }
            return GetActives();
        }

        public int AddModel(Operator model)
        {
                _operatorRepository.Add(model);
                context.Commit();
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
                context.Commit();
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
        public event EventHandler<ModelAddedEventArgs<GeneralActivitySkill>> ActivityAdded;
        public event EventHandler<ModelRemovedEventArgs> ActivityRemoved;


		public ObservableCollection<GeneralActivitySkill> GetActivities(int oprId)
		{
			Operator entity = _operatorRepository.FirstOrDefault(opr => opr.Id == oprId,
				"OperatorActivities.Operator",
				"OperatorActivities.Activity");
			return new ObservableCollection<GeneralActivitySkill>(
				entity.OperatorActivities.Where(item => item.Activity.Status == (decimal)Status.Active).ToList());
		}

        public void AddActivity(int oprId, int activityId)
        {
                var operatorRepository = new Repository<Activity>(context);
                Operator currentOperator = _operatorRepository.Single(opr => opr.Id == oprId);
                Activity newActivity = operatorRepository.Single(opr => opr.Id == activityId);
                if (currentOperator.OperatorActivities.Any(oprActivity => oprActivity.Operator.Id == oprId && oprActivity.Activity.Id == activityId))
                {
                    return;
                }
                var newGeneralActivitySkill = new GeneralActivitySkill { Activity = newActivity, Operator = currentOperator };
				currentOperator.OperatorActivities.Add(newGeneralActivitySkill);
                context.Commit();
                ActivityAdded(this, new ModelAddedEventArgs<GeneralActivitySkill>(newGeneralActivitySkill));
        }

        public void RemoveActivity(int oprId, int activityId)
        {
                var oprActivityRepository = new Repository<GeneralActivitySkill>(context);
                Operator currentOperator = _operatorRepository.Single(opr => opr.Id == oprId);
                GeneralActivitySkill currentGeneralActivitySkill =
					currentOperator.OperatorActivities.First(
                        oprActivity =>
                        oprActivity.Operator.Id == oprId && oprActivity.Id == activityId);
                int id = currentGeneralActivitySkill.Id;
                oprActivityRepository.Delete(currentGeneralActivitySkill);
                context.Commit();
                ActivityRemoved(this, new ModelRemovedEventArgs(id));
        }
    }
}