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
        private readonly Repository<AccessRule> _accessRuleRepository;
        private readonly Repository<User> _userRepository;
        private readonly Repository<Position> _positionRepository;
        private readonly Repository<User_Position> _userPositionRepository;
        private readonly Repository<User_AccessRule> _userAccessRuleRepository;
        private readonly Repository<Position_AccessRule> _positionAccessRuleRepository;

        public AccessRuleDataService()
        {
            Context = new SoheilEdmContext();
            _accessRuleRepository = new Repository<AccessRule>(Context);
            _userRepository = new Repository<User>(Context);
            _positionRepository = new Repository<Position>(Context);
            _userPositionRepository = new Repository<User_Position>(Context);
            _userAccessRuleRepository = new Repository<User_AccessRule>(Context);
            _positionAccessRuleRepository = new Repository<Position_AccessRule>(Context);
        }

        public AccessRuleDataService(SoheilEdmContext context)
        {
            Context = context;
            _accessRuleRepository = new Repository<AccessRule>(context);
            _userRepository = new Repository<User>(context);
            _positionRepository = new Repository<Position>(context);
            _userPositionRepository = new Repository<User_Position>(context);
            _userAccessRuleRepository = new Repository<User_AccessRule>(context);
            _positionAccessRuleRepository = new Repository<Position_AccessRule>(context);
        }

        #region IDataService<AccessRule> Members

        public AccessRule GetSingle(int id)
        {
                return _accessRuleRepository.FirstOrDefault(accessRule => accessRule.Id == id, "Parent", "Children");
        }

        public ObservableCollection<AccessRule> GetAll()
        {
                IEnumerable<AccessRule> entityList = _accessRuleRepository.GetAll("Children");
                return new ObservableCollection<AccessRule>(entityList);
        }

        public ObservableCollection<AccessRule> GetActives()
        {
            return GetAll();
        }

        public int AddModel(AccessRule model)
        {
                _accessRuleRepository.Add(model);
                Context.Commit();
                if (AccessRuleAdded != null)
                    AccessRuleAdded(this, new ModelAddedEventArgs<AccessRule>(model));
                return model.Id;
        }

        public void UpdateModel(AccessRule model)
        {
            Context.Commit();
        }

        public void DeleteModel(AccessRule model)
        {
        }

        public void AttachModel(AccessRule model)
        {
                if (_accessRuleRepository.Exists(accessRule => accessRule.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
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
                AccessRule entity = _accessRuleRepository.First(accessRule => accessRule.Id == accessRuleId);
                return new ObservableCollection<Position_AccessRule>(entity.Position_AccessRules.Where(item=>item.Position.Status == (decimal)Status.Active));
        }

        public void AddPosition(int accessRuleId, int positionId)
        {
                AccessRule currentAccessRule = _accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                Position newPosition = _positionRepository.Single(position => position.Id == positionId);
                if (currentAccessRule.Position_AccessRules.Any(accessRulePosition => accessRulePosition.AccessRule.Id == accessRuleId && accessRulePosition.Position.Id == positionId))
                {
                    return;
                }
                var newAccessRulePosition = new Position_AccessRule { Position = newPosition, AccessRule = currentAccessRule };
                currentAccessRule.Position_AccessRules.Add(newAccessRulePosition);
                Context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<Position_AccessRule>(newAccessRulePosition));
            
        }

        public void RemovePosition(int accessRuleId, int positionId)
        {
                AccessRule currentAccessRule = _accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                Position_AccessRule currentAccessRulePosition =
                    currentAccessRule.Position_AccessRules.First(
                        accessRulePosition =>
                        accessRulePosition.AccessRule.Id == accessRuleId && accessRulePosition.Id == positionId);
                int removedId = currentAccessRulePosition.Id;
                _positionAccessRuleRepository.Delete(currentAccessRulePosition);
                Context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(removedId));
        }

        public ObservableCollection<User_AccessRule> GetUsers(int accessRuleId)
        {
                AccessRule entity = _accessRuleRepository.First(accessRule => accessRule.Id == accessRuleId);
                return new ObservableCollection<User_AccessRule>(entity.User_AccessRules.Where(item=>item.User.Status == (decimal)Status.Active));
        }

        public void AddUser(int accessRuleId, int userId)
        {
                AccessRule currentAccessRule = _accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                User newUser = _userRepository.Single(user => user.Id == userId);
                if (currentAccessRule.User_AccessRules.Any(accessRuleUser => accessRuleUser.AccessRule.Id == accessRuleId && accessRuleUser.User.Id == userId))
                {
                    return;
                }
                var newAccessRuleUser = new User_AccessRule { User = newUser, AccessRule = currentAccessRule };
                currentAccessRule.User_AccessRules.Add(newAccessRuleUser);
                Context.Commit();
                UserAdded(this, new ModelAddedEventArgs<User_AccessRule>(newAccessRuleUser));
        }

        public void RemoveUser(int accessRuleId, int userId)
        {
                AccessRule currentAccessRule = _accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                User_AccessRule currentAccessRuleUser =
                    currentAccessRule.User_AccessRules.First(
                        accessRuleUser =>
                        accessRuleUser.AccessRule.Id == accessRuleId && accessRuleUser.Id == userId);
                int removedId = currentAccessRuleUser.Id;
                _userAccessRuleRepository.Delete(currentAccessRuleUser);
                Context.Commit();
                UserRemoved(this, new ModelRemovedEventArgs(removedId));
        }

        public List<Tuple<int, AccessType>> GetPositionsAccessOfUser(int userId)
        {
            var list = new List<Tuple<int, AccessType>>();
                var currentUser = _userRepository.FirstOrDefault(user => user.Id == userId);
                if (currentUser.BypassPositionAccess ?? false) return list;

                var positionAccessList = _positionAccessRuleRepository.Find(item => item.Type != (byte?)AccessType.None);
                var userPositionList = _userPositionRepository.Find(item => item.User.Id == userId);
                var positionList = _positionRepository.GetAll();

                var query = from pa in positionAccessList 
                            join p in positionList on pa.Position.Id equals p.Id
                            join up in userPositionList on p.Id equals up.Position.Id
                            where p.Status == (decimal) Status.Active
                            select new { pa.AccessRule.Id, pa.Type };

                list.AddRange(query.Select(record => new Tuple<int, AccessType>(record.Id, (AccessType) record.Type)));
            
            return list;
        }

        public Tuple<int,string> VerifyLogin(string username, string password)
        {
            Tuple<int, string> userInfo;
                var currentUser = _userRepository.FirstOrDefault(user => user.Status == (decimal)Status.Active && user.Username == username && user.Password == password);
                return currentUser == null ? new Tuple<int, string>(-1, string.Empty) 
                    : new Tuple<int, string>(currentUser.Id, currentUser.Title);
        }

		public List<Tuple<string, AccessType>> GetAccessOfUser(int userId)
		{
			var userList = _userRepository.GetAll();
			var accessList = _accessRuleRepository.GetAll();
			var userAccessList = _userAccessRuleRepository.GetAll();
			var positionAccessList = _positionAccessRuleRepository.GetAll();
			var userPositionList = _userPositionRepository.GetAll();

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
								from userPosition in userPositionList.Where(up => up.User.Id == user.Id && (user.BypassPositionAccess == null || user.BypassPositionAccess == false)).DefaultIfEmpty()
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
			var list = query.Select(record => new Tuple<string, AccessType>(record.Code, record.AccessType)).ToList();
			return list;
		}
		public List<Tuple<string, AccessType>> GetAccessOfAdmin(int userId)
		{
			var accessList = _accessRuleRepository.GetAll();
			var userAccessList = _userAccessRuleRepository.GetAll();

			var userQuery = from userAccess in userAccessList.Where(ua => ua.User.Id == userId).DefaultIfEmpty()
							from access in accessList.Where(a => (userAccess != null && a.Id == userAccess.AccessRule.Id))
							let type = userAccess.Type
							where type != null
							select new
							{
								access.Id,
								access.Code,
								AccessType = (AccessType)type
							};

			var query = from access in accessList
						from userResult in userQuery.Where(u => u.Id == access.Id).DefaultIfEmpty().Distinct()
						let uType = userResult == null ? 0 : userResult.AccessType
						select new
						{
							access.Id,
							access.Code,
							AccessType = uType
						};
			var list = query.Select(record => new Tuple<string, AccessType>(record.Code, record.AccessType)).ToList();
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