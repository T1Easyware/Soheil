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
		Repository<ActivitySkill> _activitySkillRepository;

		public ActivitySkillDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ActivitySkillDataService(SoheilEdmContext context)
		{
			this.Context = context;
			_activitySkillRepository = new Repository<ActivitySkill>(Context);
		}

        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActivitySkill GetSingle(int id)
        {
            ActivitySkill entity;
			entity = _activitySkillRepository.FirstOrDefault(model => model.Id == id, "Operator", "Activity");
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
			_activitySkillRepository.Add(model);
			Context.Commit();
			return model.Id;
        }

		public void UpdateModel(ActivitySkill model)
        {
			ActivitySkill entity = _activitySkillRepository.Single(generalActivitySkill => generalActivitySkill.Id == model.Id);

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

		public ActivitySkill TryFind(int operatorId, int activityId)
		{
			var model = _activitySkillRepository.FirstOrDefault(x =>
				x.Activity.Id == activityId
				&& x.Operator.Id == operatorId, "Operator", "Activity");
			return model;
		}

		internal void AddOrUpdateSkill(ViewModels.SkillCenter.ActivitySkillVm vm)
		{
			if(vm.Model == null)
			{
				vm.Model = _activitySkillRepository.FirstOrDefault(x =>
					x.Activity.Id == vm.ActivityId
					&& x.Operator.Id == vm.OperatorId);
				if (vm.Model == null)
				{
					var actv = new Repository<Activity>(Context).Single(x => x.Id == vm.ActivityId);
					var oper = new Repository<Operator>(Context).Single(x => x.Id == vm.OperatorId);
					vm.Model = new ActivitySkill
					{
						Activity = actv,
						Operator = oper,
						CreatedDate = DateTime.Now,
						ModifiedDate = DateTime.Now,
						ModifiedBy = LoginInfo.Id,
					};
				}
			}
			vm.Model.IluoNr = (byte)vm.Data;
			Context.Commit();
		}
	}
}