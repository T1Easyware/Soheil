using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class UserDataService : DataServiceBase, IDataService<User>
    {
        private readonly Repository<AccessRule> _accessRuleRepository;
        private readonly Repository<User> _userRepository;
        private readonly Repository<Position> _positionRepository;
        private readonly Repository<User_Position> _userPositionRepository;

        public UserDataService()
        {
            Context = new SoheilEdmContext();
            _accessRuleRepository = new Repository<AccessRule>(Context);
            _userRepository = new Repository<User>(Context);
            _positionRepository = new Repository<Position>(Context);
            _userPositionRepository = new Repository<User_Position>(Context);
        }
        public UserDataService(SoheilEdmContext context)
        {
            Context = context;
            _accessRuleRepository = new Repository<AccessRule>(context);
            _userRepository = new Repository<User>(context);
            _positionRepository = new Repository<Position>(context);
            _userPositionRepository = new Repository<User_Position>(context);
        }

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
                IEnumerable<User> entityList =
                    _userRepository.Find(
                        user => user.Status == (decimal)Status.Active);
                return new ObservableCollection<User>(entityList);
        }

        public ObservableCollection<User> GetActives(SoheilEntityType linkType, int linkId = 0)
        {
            if (linkType == SoheilEntityType.Positions)
            {
                    IEnumerable<User> entityList =
                        _userRepository.Find(
                            user => user.Status == (decimal)Status.Active && user.User_Positions.All(up => up.Position.Id != linkId));
                    return new ObservableCollection<User>(entityList);
            }
            return GetActives();
        }

        public event EventHandler<ModelAddedEventArgs<User>> UserAdded;
        public event EventHandler<ModelAddedEventArgs<User_Position>> PositionAdded;
        public event EventHandler<ModelRemovedEventArgs> PositionRemoved;
        public event EventHandler<ModelAddedEventArgs<User_AccessRule>> AccessRuleChanged;


        public ObservableCollection<User_Position> GetPositions(int userId)
        {
                User entity = _userRepository.FirstOrDefault(user => user.Id == userId,"User_Positions.User","User_Positions.Position");
                return new ObservableCollection<User_Position>(entity.User_Positions.Where(item=>item.Position.Status == (decimal)Status.Active));
        }

        public void AddPosition(int userId, int positionId)
        {
                User currentUser = _userRepository.Single(user => user.Id == userId);
                Position newPosition = _positionRepository.Single(position => position.Id == positionId);
                if (currentUser.User_Positions.Any(userPosition => userPosition.User.Id == userId && userPosition.Position.Id == positionId))
                {
                    return;
                }
                var newUserPosition = new User_Position { Position = newPosition, User = currentUser };
                currentUser.User_Positions.Add(newUserPosition);
                Context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<User_Position>(newUserPosition));
        }

        public void RemovePosition(int userId, int positionId)
        {
                User currentUser = _userRepository.Single(user => user.Id == userId);
                User_Position currentUserPosition =
                    currentUser.User_Positions.First(
                        userPosition =>
                        userPosition.User.Id == userId && userPosition.Id == positionId);
                int removedId = currentUserPosition.Id;
                _userPositionRepository.Delete(currentUserPosition);
                Context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(removedId));
        }

        public ObservableCollection<User_AccessRule> GetAccessRules(int userId)
        {
                User entity = _userRepository.First(user => user.Id == userId);
                return new ObservableCollection<User_AccessRule>(entity.User_AccessRules);
        }

        public void AddRemoveAccessRule(int userId, int accessRuleId, AccessType type)
        {
                User currentUser = _userRepository.Single(user => user.Id == userId);
                AccessRule newAccessRule = _accessRuleRepository.Single(product => product.Id == accessRuleId);
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
                Context.Commit();
                AccessRuleChanged(this, new ModelAddedEventArgs<User_AccessRule>(newUserAccessRule));
        }

    }
}