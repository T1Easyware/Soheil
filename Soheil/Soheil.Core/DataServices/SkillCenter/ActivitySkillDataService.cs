using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class ActivitySkillDataService : DataServiceBase, IDataService<ActivitySkill>
    {
		public event EventHandler<ModelAddedEventArgs<ActivitySkill>> ModelUpdated;

		public ActivitySkillDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ActivitySkillDataService(SoheilEdmContext context)
		{
			this.Context = context;
		}

        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActivitySkill GetSingle(int id)
        {
            ActivitySkill entity;
			var operatorRepository = new Repository<ActivitySkill>(Context);
            entity = operatorRepository.FirstOrDefault(model => model.Id == id, "Operator","Activity");
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<ActivitySkill> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<ActivitySkill> GetActives()
        {
            throw new NotImplementedException();
        }

		public int AddModel(ActivitySkill model)
        {
			var repository = new Repository<ActivitySkill>(Context);
			repository.Add(model);
			Context.Commit();
			return model.Id;
        }

		public void UpdateModel(ActivitySkill model)
        {
			var repository = new Repository<ActivitySkill>(Context);
			ActivitySkill entity = repository.Single(generalActivitySkill => generalActivitySkill.Id == model.Id);

            entity.IluoNr = model.IluoNr;

            Context.Commit();
			if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<ActivitySkill>(entity));
        }

		public void DeleteModel(ActivitySkill model)
        {
            throw new System.NotImplementedException();
        }

		public void AttachModel(ActivitySkill model)
        {
            throw new System.NotImplementedException();
        }

		public ActivitySkill FindOrAdd(int operatorId, int activityId)
		{
			var operatorRepository = new Repository<ActivitySkill>(Context);
			var model = operatorRepository.FirstOrDefault(x => 
				x.Activity.Id == activityId 
				&& x.Operator.Id == operatorId, "Operator", "Activity");
			if(model == null)
			{
				var actv = new Repository<Activity>(Context).Single(x => x.Id == activityId);
				var oper = new Repository<Operator>(Context).Single(x => x.Id == operatorId);
				model = new ActivitySkill
				{
					Activity = actv,
					Operator = oper,
					CreatedDate = DateTime.Now,
					ModifiedDate = DateTime.Now,
					ModifiedBy = LoginInfo.Id,
					IluoNr = 0,
				};
				Context.Commit();
			}
			return model;
		}
    }
}