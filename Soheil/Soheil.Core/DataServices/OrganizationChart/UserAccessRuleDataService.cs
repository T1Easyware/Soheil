using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class UserAccessRuleDataService : IDataService<User_AccessRule>
    {
        public event EventHandler<ModelAddedEventArgs<User_AccessRule>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public User_AccessRule GetSingle(int id)
        {
            User_AccessRule entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User_AccessRule>(context);
                entity = repository.Single(userAccessRule => userAccessRule.Id == id);
            }
            return entity;
        }

        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="accessRuleId">The accessRule id.</param>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public User_AccessRule GetSingle(int userId, int accessRuleId)
        {
            User_AccessRule entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User_AccessRule>(context);
                entity = repository.FirstOrDefault(userAccessRule => userAccessRule.User.Id == userId && userAccessRule.AccessRule.Id == accessRuleId, "User.User_Positions.Position","AccessRule", "AccessRule.Parent");
            }
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<User_AccessRule> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<User_AccessRule> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(User_AccessRule model)
        {
            throw new NotImplementedException();
        }

        public void UpdateModel(User_AccessRule model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User_AccessRule>(context);
                User_AccessRule entity = repository.Single(userAccessRule => userAccessRule.Id == model.Id);

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<User_AccessRule>(entity));
            }
        }

        public void DeleteModel(User_AccessRule model)
        {
            throw new NotImplementedException();
        }

        public void AttachModel(User_AccessRule model)
        {
            throw new NotImplementedException();
        }
    }
}