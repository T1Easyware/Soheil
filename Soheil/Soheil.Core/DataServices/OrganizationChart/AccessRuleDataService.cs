using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class AccessRuleDataService : RecursiveDataServiceBase, IDataService<AccessRule>
    {
        #region IDataService<AccessRule> Members

        public AccessRule GetSingle(int id)
        {
            AccessRule entity;
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                entity = accessRuleRepository.FirstOrDefault(accessRule => accessRule.Id == id, "Parent", "Children");
            }
            return entity;
        }

        public ObservableCollection<AccessRule> GetAll()
        {
            ObservableCollection<AccessRule> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<AccessRule>(context);
                IEnumerable<AccessRule> entityList = repository.GetAll("Children");
                models = new ObservableCollection<AccessRule>(entityList);
            }
            return models;
        }

        public ObservableCollection<AccessRule> GetActives()
        {
            return GetAll();
        }

        public int AddModel(AccessRule model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<AccessRule>(context);
                repository.Add(model);
                context.Commit();
                if (AccessRuleAdded != null)
                    AccessRuleAdded(this, new ModelAddedEventArgs<AccessRule>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(AccessRule model)
        {
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                AccessRule entity = accessRuleRepository.Single(accessRule => accessRule.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                context.Commit();
            }
        }

        public void DeleteModel(AccessRule model)
        {
        }

        public void AttachModel(AccessRule model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<AccessRule>(context);
                if (repository.Exists(accessRule => accessRule.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<AccessRule>> AccessRuleAdded;
        public event EventHandler<ModelAddedEventArgs<Position_AccessRule>> PositionAdded;
        public event EventHandler<ModelRemovedEventArgs> PositionRemoved;
        public event EventHandler<ModelAddedEventArgs<User_AccessRule>> UserAdded;
        public event EventHandler<ModelRemovedEventArgs> UserRemoved;


        public ObservableCollection<Position_AccessRule> GetPositions(int accessRuleId)
        {
            ObservableCollection<Position_AccessRule> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<AccessRule>(context);
                AccessRule entity = repository.First(accessRule => accessRule.Id == accessRuleId);
                models = new ObservableCollection<Position_AccessRule>(entity.Position_AccessRules.Where(item=>item.Position.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddPosition(int accessRuleId, int positionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                var positionRepository = new Repository<Position>(context);
                AccessRule currentAccessRule = accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                Position newPosition = positionRepository.Single(position => position.Id == positionId);
                if (currentAccessRule.Position_AccessRules.Any(accessRulePosition => accessRulePosition.AccessRule.Id == accessRuleId && accessRulePosition.Position.Id == positionId))
                {
                    return;
                }
                var newAccessRulePosition = new Position_AccessRule { Position = newPosition, AccessRule = currentAccessRule };
                currentAccessRule.Position_AccessRules.Add(newAccessRulePosition);
                context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<Position_AccessRule>(newAccessRulePosition));
            }
        }

        public void RemovePosition(int accessRuleId, int positionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                var accessRulePositionRepository = new Repository<Position_AccessRule>(context);
                AccessRule currentAccessRule = accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                Position_AccessRule currentAccessRulePosition =
                    currentAccessRule.Position_AccessRules.First(
                        accessRulePosition =>
                        accessRulePosition.AccessRule.Id == accessRuleId && accessRulePosition.Id == positionId);
                int removedId = currentAccessRulePosition.Id;
                accessRulePositionRepository.Delete(currentAccessRulePosition);
                context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(removedId));
            }
        }

        public ObservableCollection<User_AccessRule> GetUsers(int accessRuleId)
        {
            ObservableCollection<User_AccessRule> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<AccessRule>(context);
                AccessRule entity = repository.First(accessRule => accessRule.Id == accessRuleId);
                models = new ObservableCollection<User_AccessRule>(entity.User_AccessRules.Where(item=>item.User.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddUser(int accessRuleId, int userId)
        {
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                var userRepository = new Repository<User>(context);
                AccessRule currentAccessRule = accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                User newUser = userRepository.Single(user => user.Id == userId);
                if (currentAccessRule.User_AccessRules.Any(accessRuleUser => accessRuleUser.AccessRule.Id == accessRuleId && accessRuleUser.User.Id == userId))
                {
                    return;
                }
                var newAccessRuleUser = new User_AccessRule { User = newUser, AccessRule = currentAccessRule };
                currentAccessRule.User_AccessRules.Add(newAccessRuleUser);
                context.Commit();
                UserAdded(this, new ModelAddedEventArgs<User_AccessRule>(newAccessRuleUser));
            }
        }

        public void RemoveUser(int accessRuleId, int userId)
        {
            using (var context = new SoheilEdmContext())
            {
                var accessRuleRepository = new Repository<AccessRule>(context);
                var accessRuleUserRepository = new Repository<User_AccessRule>(context);
                AccessRule currentAccessRule = accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                User_AccessRule currentAccessRuleUser =
                    currentAccessRule.User_AccessRules.First(
                        accessRuleUser =>
                        accessRuleUser.AccessRule.Id == accessRuleId && accessRuleUser.Id == userId);
                int removedId = currentAccessRuleUser.Id;
                accessRuleUserRepository.Delete(currentAccessRuleUser);
                context.Commit();
                UserRemoved(this, new ModelRemovedEventArgs(removedId));
            }
        }

        public List<Tuple<int, AccessType>> GetPositionsAccessOfUser(int userId)
        {
            var list = new List<Tuple<int, AccessType>>();
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var currentUser = userRepository.FirstOrDefault(user => user.Id == userId);
                if (currentUser.BypassPositionAccess ?? false) return list;

                var userPositionRepository = new Repository<User_Position>(context);
                var positionRepository = new Repository<Position>(context);
                var positionAccessRepository = new Repository<Position_AccessRule>(context);

                var positionAccessList = positionAccessRepository.Find(item => item.Type != (byte?)AccessType.None);
                var userPositionList = userPositionRepository.Find(item => item.User.Id == userId);
                var positionList = positionRepository.GetAll();

                var query = from pa in positionAccessList 
                            join p in positionList on pa.Position.Id equals p.Id
                            join up in userPositionList on p.Id equals up.Position.Id
                            where p.Status == (decimal) Status.Active
                            select new { pa.AccessRule.Id, pa.Type };

                list.AddRange(query.Select(record => new Tuple<int, AccessType>(record.Id, (AccessType) record.Type)));
            }
            return list;
        }

        public Tuple<int,string> VerifyLogin(string username, string password)
        {
            Tuple<int, string> userInfo;
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var currentUser = userRepository.FirstOrDefault(user => user.Status == (decimal)Status.Active && user.Username == username && user.Password == password);
                userInfo = currentUser == null ? new Tuple<int, string>(-1,string.Empty) 
                    : new Tuple<int, string>(currentUser.Id, currentUser.Title);
            }
            return userInfo;
        }

        public List<Tuple<string, AccessType>> GetAccessOfUser(int userId)
        {
            var list = new List<Tuple<string, AccessType>>();
            using (var context = new SoheilEdmContext())
            {
                var userRepository = new Repository<User>(context);
                var accessRepository = new Repository<AccessRule>(context);
                var userPositionRepository = new Repository<User_Position>(context);
                var positionAccessRepository = new Repository<Position_AccessRule>(context);
                var userAccessRepository = new Repository<User_AccessRule>(context);

                var userList = userRepository.GetAll();
                var accessList = accessRepository.GetAll();
                var userAccessList = userAccessRepository.GetAll();
                var positionAccessList = positionAccessRepository.GetAll();
                var userPositionList = userPositionRepository.GetAll();

                var userQuery = from user in userList.Where(u => u.Id == userId)
                                from userAccess in userAccessList.Where(ua => ua.User.Id == user.Id).DefaultIfEmpty()
                                from access in accessList.Where(a => (userAccess != null && a.Id == userAccess.AccessRule.Id))
                                let type = userAccess.Type
                                where type != null
                                select new
                            {
                                access.Id,
                                access.Code,
                                AccessType = (AccessType)type
                            };

                var positionQuery = from user in userList.Where(u => u.Id == userId)
                                    from userPosition in userPositionList.Where(up => up.User.Id == user.Id  && (user.BypassPositionAccess == null || user.BypassPositionAccess == false)).DefaultIfEmpty()
                                    from positionAccess in positionAccessList.Where(pa => (userPosition != null && userPosition.Position != null && pa.Position.Id == userPosition.Position.Id && userPosition.Position.Status == (decimal)Status.Active)).DefaultIfEmpty()
                                    from access in accessList.Where(a => ((positionAccess != null && a.Id == positionAccess.AccessRule.Id)))
                                    let type = positionAccess == null ? 0 : access.Position_AccessRules.Select(item => item.Type).Aggregate((a, b) => (byte?)(a | b))
                                    where type != null
                                    select new
                                   {
                                       access.Id,
                                       access.Code,
                                       AccessType = (AccessType)type
                                   };

                var query = from access in accessList
                            from user in userQuery.Where(u => u.Id == access.Id).DefaultIfEmpty().Distinct()
                            from position in positionQuery.Where(p => p.Id == access.Id).DefaultIfEmpty().Distinct()
                            let uType = user == null ? 0 : user.AccessType
                            let pType = position == null ? 0 : position.AccessType
                            select new
                            {
                                access.Id,
                                access.Code,
                                AccessType = uType | pType
                            };
                list.AddRange(query.Select(record => new Tuple<string, AccessType>(record.Code, record.AccessType)));
            }
            return list;
        }
        #region Overrides of RecursiveDataServiceBase

        public override ObservableCollection<IEntityNode> GetChildren(int id)
        {
            //var allNodes = GetAll();
            //return GetChildrenNodes(allNodes,GetSingle(id));
            return null;
        }
        #endregion
    }
}