using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
    public class ActivityGroupDataService : DataServiceBase, IDataService<ActivityGroup>
    {
		public event EventHandler<ModelAddedEventArgs<ActivityGroup>> ActivityGroupAdded;
		Repository<ActivityGroup> _activityGroupRepository;


		public ActivityGroupDataService(SoheilEdmContext context)
		{
			this.Context = context ?? new SoheilEdmContext();
            _activityGroupRepository = new Repository<ActivityGroup>(Context);
		}


		#region IDataService<ActivityGroupVM> Members

		public ActivityGroup GetSingle(int id)
		{
			return _activityGroupRepository.Single(activityGroup => activityGroup.Id == id);
		}

		public ObservableCollection<ActivityGroup> GetAll()
		{
			IEnumerable<ActivityGroup> entityList = _activityGroupRepository.Find(group => group.Status != (byte)Status.Deleted, "Activities");
			return new ObservableCollection<ActivityGroup>(entityList);
		}

		public ObservableCollection<ActivityGroup> GetActives()
		{
			IEnumerable<ActivityGroup> entityList = _activityGroupRepository.Find(group =>
				(group.Status == (byte)Status.Active) &&
				group.Activities.Any(x => x.Status == (byte)Status.Active),
				"Activities");
			return new ObservableCollection<ActivityGroup>(entityList);
		}

		public int AddModel(ActivityGroup model)
		{
			int id;
			_activityGroupRepository.Add(model);
			Context.Commit();
			if (ActivityGroupAdded != null)
				ActivityGroupAdded(this, new ModelAddedEventArgs<ActivityGroup>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(ActivityGroup model)
		{
			model.CreatedDate = model.CreatedDate;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
		}

        public void DeleteModel(ActivityGroup model)
        {
        }

		public void AttachModel(ActivityGroup model)
		{
			if (_activityGroupRepository.Exists(activityGroup => activityGroup.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion


        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The activity group id.</param>
        /// <returns></returns>
		public ActivityGroup GetModel(int id)
		{
			return _activityGroupRepository.Single(activity => activity.Id == id);
		}
    }
}