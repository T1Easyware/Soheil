using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ActivityGroupDataService : IDataService<ActivityGroup>
    {
		public IEnumerable<ActivityGroup> GetAllWithActivities()
		{
			List<ActivityGroup> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<ActivityGroup>(context);
				models = new List<ActivityGroup>(repository.Find(group=> group.Status != (decimal)Status.Deleted, "Activities"));
			}
			return models;
		}

		#region IDataService<ActivityGroupVM> Members

        public ActivityGroup GetSingle(int id)
        {
            ActivityGroup entity;
            using (var context = new SoheilEdmContext())
            {
                var activityGroupRepository = new Repository<ActivityGroup>(context);
                entity = activityGroupRepository.Single(activityGroup => activityGroup.Id == id);
        }
            return entity;
        }

        public ObservableCollection<ActivityGroup> GetAll()
        {
            ObservableCollection<ActivityGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActivityGroup>(context);
                IEnumerable<ActivityGroup> entityList = repository.Find(group=> group.Status != (decimal)Status.Deleted, "Activities");
                models = new ObservableCollection<ActivityGroup>(entityList);
                }
            return models;
            }

        public ObservableCollection<ActivityGroup> GetActives()
        {
            ObservableCollection<ActivityGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActivityGroup>(context);
                IEnumerable<ActivityGroup> entityList = repository.Find(group => group.Status == (decimal)Status.Active, "Activities");
                models = new ObservableCollection<ActivityGroup>(entityList);
            }
            return models;
        }

        public int AddModel(ActivityGroup model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActivityGroup>(context);
                repository.Add(model);
                context.Commit();
                if (ActivityGroupAdded != null)
                    ActivityGroupAdded(this, new ModelAddedEventArgs<ActivityGroup>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(ActivityGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var activityGroupRepository = new Repository<ActivityGroup>(context);
                ActivityGroup entity = activityGroupRepository.Single(activityGroup => activityGroup.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(ActivityGroup model)
        {
        }

        public void AttachModel(ActivityGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActivityGroup>(context);
                if (repository.Exists(activityGroup => activityGroup.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<ActivityGroup>> ActivityGroupAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The activity group id.</param>
        /// <returns></returns>
        public ActivityGroup GetModel(int id)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActivityGroup>(context);
                return repository.Single(activity => activity.Id == id);
            }
        }
    }
}