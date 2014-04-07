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
    public class UserDataService : IDataService<User>
    {
        #region IDataService<User> Members

        public User GetSingle(int id)
        {
            User entity;
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                entity = userRepository.Single(user => user.Id == id);
            }
            return entity;
        }

        public ObservableCollection<User> GetAll()
        {
            ObservableCollection<User> users;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                IEnumerable<User> entityList =
                    repository.Find(
                        user => user.Status != (decimal)Status.Deleted);
                users = new ObservableCollection<User>(entityList);
            }
            return users;
        }

        public int AddModel(User model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                repository.Add(model);
                context.Commit();
                if (UserAdded != null)
                    UserAdded(this, new ModelAddedEventArgs<User>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(User model)
        {
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                User entity = userRepository.Single(user => user.Id == model.Id);

                entity.Code = model.Code;
                entity.Title = model.Title;
                entity.Username = model.Username;
                entity.Password = model.Password;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                entity.BypassPositionAccess = model.BypassPositionAccess;
                context.Commit();
            }
        }
        public int GenerateCode()
        {
            int maxCode;
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var allUsers = userRepository.GetAll();
                if (!allUsers.Any()) return 1;
                maxCode = allUsers.Max(user => user.Code);
            }
            return maxCode + 1;
        }

        public bool IsCodeUnique(int code)
        {
            bool isUnique;
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                isUnique = !userRepository.Exists(user => user.Code == code);
            }
            return isUnique;
        }

        public void DeleteModel(User model)
        {
        }

        public void AttachModel(User model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                if (repository.Exists(user => user.Id == model.Id))
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

        public ObservableCollection<User> GetActives()
        {
            ObservableCollection<User> users;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                IEnumerable<User> entityList =
                    repository.Find(
                        user => user.Status == (decimal)Status.Active);
                users = new ObservableCollection<User>(entityList);
            }
            return users;
        }

        public ObservableCollection<User> GetActives(SoheilEntityType linkType, int linkId = 0)
        {
            if (linkType == SoheilEntityType.Positions)
            {
                ObservableCollection<User> users;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<User>(context);
                    IEnumerable<User> entityList =
                        repository.Find(
                            user => user.Status == (decimal)Status.Active && user.User_Positions.All(up => up.Position.Id != linkId));
                    users = new ObservableCollection<User>(entityList);
                }
                return users;
            }
            return GetActives();
        }

        public event EventHandler<ModelAddedEventArgs<User>> UserAdded;
        public event EventHandler<ModelAddedEventArgs<User_Position>> PositionAdded;
        public event EventHandler<ModelRemovedEventArgs> PositionRemoved;
        public event EventHandler<ModelAddedEventArgs<User_AccessRule>> AccessRuleChanged;


        public ObservableCollection<User_Position> GetPositions(int userId)
        {
            ObservableCollection<User_Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                User entity = repository.FirstOrDefault(user => user.Id == userId,"User_Positions.User","User_Positions.Position");
                models = new ObservableCollection<User_Position>(entity.User_Positions.Where(item=>item.Position.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddPosition(int userId, int positionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var productRepository = new Repository<Position>(context);
                User currentUser = userRepository.Single(user => user.Id == userId);
                Position newPosition = productRepository.Single(product => product.Id == positionId);
                if (currentUser.User_Positions.Any(userPosition => userPosition.User.Id == userId && userPosition.Position.Id == positionId))
                {
                    return;
                }
                var newUserPosition = new User_Position { Position = newPosition, User = currentUser };
                currentUser.User_Positions.Add(newUserPosition);
                context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<User_Position>(newUserPosition));
            }
        }

        public void RemovePosition(int userId, int positionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var userPositionRepository = new Repository<User_Position>(context);
                User currentUser = userRepository.Single(user => user.Id == userId);
                User_Position currentUserPosition =
                    currentUser.User_Positions.First(
                        userPosition =>
                        userPosition.User.Id == userId && userPosition.Id == positionId);
                int removedId = currentUserPosition.Id;
                userPositionRepository.Delete(currentUserPosition);
                context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(removedId));
            }
        }

        public ObservableCollection<User_AccessRule> GetAccessRules(int userId)
        {
            ObservableCollection<User_AccessRule> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<User>(context);
                User entity = repository.First(user => user.Id == userId);
                models = new ObservableCollection<User_AccessRule>(entity.User_AccessRules);
            }

            return models;
        }

        public void AddRemoveAccessRule(int userId, int accessRuleId, AccessType type)
        {
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var productRepository = new Repository<AccessRule>(context);
                User currentUser = userRepository.Single(user => user.Id == userId);
                AccessRule newAccessRule = productRepository.Single(product => product.Id == accessRuleId);
                var currentUserAccessRule = currentUser.User_AccessRules.FirstOrDefault(userAccessRule => userAccessRule.User.Id == userId && userAccessRule.AccessRule.Id == accessRuleId);

                User_AccessRule newUserAccessRule;
                if (currentUserAccessRule != null)
                {
                    if (type == AccessType.None)
                    {
                        currentUserAccessRule.Type = (byte?)AccessType.None;
                    }
                    else if (type == AccessType.Full)
                    {
                        currentUserAccessRule.Type = (byte?)(AccessType.View | AccessType.Print | AccessType.Update | AccessType.Insert | AccessType.Full);
                    }
                    else
                    {
                        currentUserAccessRule.Type |= (byte?)type;
                    }
                    newUserAccessRule = currentUserAccessRule;
                }
                else
                {
                    var newType = AccessType.None;
                    if (type == AccessType.Full) newType = AccessType.View | AccessType.Print | AccessType.Update | AccessType.Insert | AccessType.Full;
                    else if (type != AccessType.None) newType = type;
                    newUserAccessRule = new User_AccessRule { AccessRule = newAccessRule, User = currentUser, ModifiedDate = DateTime.Now, Type = (byte?)newType };
                    currentUser.User_AccessRules.Add(newUserAccessRule);
                }
                context.Commit();
                AccessRuleChanged(this, new ModelAddedEventArgs<User_AccessRule>(newUserAccessRule));
            }
        }

    }
}