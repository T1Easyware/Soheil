using System;
using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class UserAccessRuleDataService : DataServiceBase, IDataService<User_AccessRule>
    {
        private readonly Repository<User_AccessRule> _userAccessRuleRepository;
        public UserAccessRuleDataService(SoheilEdmContext context)
        {
            Context = context;
            _userAccessRuleRepository = new Repository<User_AccessRule>(context);
        }
        public event EventHandler<ModelAddedEventArgs<User_AccessRule>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public User_AccessRule GetSingle(int id)
        {
                return _userAccessRuleRepository.Single(userAccessRule => userAccessRule.Id == id);
        }

        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="accessRuleId">The accessRule id.</param>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public User_AccessRule GetSingle(int userId, int accessRuleId)
        {
                return _userAccessRuleRepository.FirstOrDefault(userAccessRule => userAccessRule.User.Id == userId && userAccessRule.AccessRule.Id == accessRuleId, "User.User_Positions.Position","AccessRule", "AccessRule.Parent");
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
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<User_AccessRule>(model));
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