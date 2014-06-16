using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class PositionDataService : DataServiceBase, IDataService<Position>
    {
        private readonly Repository<AccessRule> _accessRuleRepository;
        private readonly Repository<User> _userRepository;
        private readonly Repository<Position> _positionRepository;
        private readonly Repository<User_Position> _userPositionRepository;
        private readonly Repository<OrganizationChart> _orgChartRepository;
        private readonly Repository<OrganizationChart_Position> _orgChatPositionRepository;

        public PositionDataService(SoheilEdmContext context)
        {
            Context = context;
            _accessRuleRepository = new Repository<AccessRule>(context);
            _userRepository = new Repository<User>(context);
            _positionRepository = new Repository<Position>(context);
            _userPositionRepository = new Repository<User_Position>(context);
            _orgChatPositionRepository= new Repository<OrganizationChart_Position>(context);
            _orgChartRepository = new Repository<OrganizationChart>(context);
        }
        #region IDataService<Position> Members

        public Position GetSingle(int id)
        {
                return _positionRepository.Single(position => position.Id == id);
        }

        public ObservableCollection<Position> GetAll()
        {
            IEnumerable<Position> entityList =
                _positionRepository.Find(
                    position => position.Status != (decimal) Status.Deleted);
            return new ObservableCollection<Position>(entityList);
        }

        public int AddModel(Position model)
        {
                _positionRepository.Add(model);
                Context.Commit();
                if (PositionAdded != null)
                    PositionAdded(this, new ModelAddedEventArgs<Position>(model));
                return model.Id;
        }

        public void UpdateModel(Position model)
        {
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
        }

        public void DeleteModel(Position model)
        {
        }

        public void AttachModel(Position model)
        {
                if (_positionRepository.Exists(position => position.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
        }

        #endregion

        public ObservableCollection<Position> GetActives()
        {
                IEnumerable<Position> entityList =
                    _positionRepository.Find(
                        position => position.Status == (decimal)Status.Active);
                return new ObservableCollection<Position>(entityList);
        }

        public ObservableCollection<Position> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Users)
            {
                    IEnumerable<Position> entityList =
                        _positionRepository.Find(
                            position => position.Status == (decimal)Status.Active && position.User_Positions.All(item=>item.User.Id != linkId));
                    return new ObservableCollection<Position>(entityList);
            }
            if (linkType == SoheilEntityType.OrganizationCharts)
            {
                    IEnumerable<Position> entityList =
                        _positionRepository.Find(
                            position => position.Status == (decimal)Status.Active && position.OrganizationChart_Positions.All(item=> item.OrganizationChart.Id != linkId));
                    return new ObservableCollection<Position>(entityList);
            }
            return GetActives();
        }

        public event EventHandler<ModelAddedEventArgs<Position>> PositionAdded;
        public event EventHandler<ModelAddedEventArgs<User_Position>> UserAdded;
        public event EventHandler<ModelRemovedEventArgs> UserRemoved;
        public event EventHandler<ModelAddedEventArgs<Position_AccessRule>> AccessRuleChanged;
        public event EventHandler<ModelAddedEventArgs<OrganizationChart_Position>> OrganizationChartAdded;
        public event EventHandler<ModelRemovedEventArgs> OrganizationChartRemoved;


        public ObservableCollection<User_Position> GetUsers(int positionId)
        {
                Position entity = _positionRepository.FirstOrDefault(position => position.Id == positionId, "User_Positions.User", "User_Positions.Position");
                return new ObservableCollection<User_Position>(entity.User_Positions.Where(item=>item.User.Status == (decimal)Status.Active));
        }

        public void AddUser(int positionId, int userId)
        {
                Position currentPosition = _positionRepository.Single(position => position.Id == positionId);
                User newUser = _userRepository.Single(product => product.Id == userId);
                if (currentPosition.User_Positions.Any(positionUser => positionUser.Position.Id == positionId && positionUser.User.Id == userId))
                {
                    return;
                }
                var newPositionUser = new User_Position { User = newUser, Position = currentPosition };
                currentPosition.User_Positions.Add(newPositionUser);
                Context.Commit();
                UserAdded(this, new ModelAddedEventArgs<User_Position>(newPositionUser));
        }

        public void RemoveUser(int positionId, int userId)
        {
                Position currentPosition = _positionRepository.Single(position => position.Id == positionId);
                User_Position currentPositionUser =
                    currentPosition.User_Positions.First(
                        positionUser =>
                        positionUser.Position.Id == positionId && positionUser.Id == userId);
                int id = currentPositionUser.Id;
                _userPositionRepository.Delete(currentPositionUser);
                Context.Commit();
                UserRemoved(this, new ModelRemovedEventArgs(id));
        }

        public ObservableCollection<Position_AccessRule> GetAccessRules(int positionId)
        {
                Position entity = _positionRepository.FirstOrDefault(position => position.Id == positionId, "Position_AccessRules.Position", "Position_AccessRules.AccessRule", "Position_AccessRules.AccessRule.Parent");
                return new ObservableCollection<Position_AccessRule>(entity.Position_AccessRules);
        }

        public void AddRemoveAccessRule(int positionId, int accessRuleId, AccessType type)
        {
                Position currentPosition = _positionRepository.Single(position => position.Id == positionId);
                AccessRule newAccessRule = _accessRuleRepository.Single(accessRule => accessRule.Id == accessRuleId);
                var currentPositionAccessRule = currentPosition.Position_AccessRules.FirstOrDefault(positionAccessRule => positionAccessRule.Position.Id == positionId && positionAccessRule.AccessRule.Id == accessRuleId);

                Position_AccessRule newPositionAccessRule;
                if (currentPositionAccessRule != null)
                {
                    if (type == AccessType.None)
                    {
                        currentPositionAccessRule.Type = (byte?)AccessType.None;
                    }
                    else if (type == AccessType.Full)
                    {
                        currentPositionAccessRule.Type = (byte?) (AccessType.View | AccessType.Print | AccessType.Update | AccessType.Insert | AccessType.Full);
                    }
                    else
                    {
                        currentPositionAccessRule.Type |= (byte?) type;
                    }
                    newPositionAccessRule = currentPositionAccessRule;
                }
                else
                {
                    var newType = AccessType.None;
                    if (type == AccessType.Full) newType = AccessType.View | AccessType.Print | AccessType.Update | AccessType.Insert | AccessType.Full;
                    else if (type != AccessType.None) newType = type;
                    newPositionAccessRule = new Position_AccessRule { AccessRule = newAccessRule, Position = currentPosition, ModifiedDate = DateTime.Now, Type = (byte?)newType };
                    currentPosition.Position_AccessRules.Add(newPositionAccessRule);
                }
                Context.Commit();
                AccessRuleChanged(this, new ModelAddedEventArgs<Position_AccessRule>(newPositionAccessRule));
        }

        public ObservableCollection<OrganizationChart_Position> GetOrganizationCharts(int positionId)
        {
                Position entity = _positionRepository.First(position => position.Id == positionId);
                return new ObservableCollection<OrganizationChart_Position>(entity.OrganizationChart_Positions.Where(item=>item.OrganizationChart.Status == (decimal)Status.Active));
        }

        public void AddOrganizationChart(int positionId, int organizationChartId)
        {
                Position currentPosition = _positionRepository.Single(position => position.Id == positionId);
                OrganizationChart newOrganizationChart = _orgChartRepository.Single(product => product.Id == organizationChartId);
                if (currentPosition.OrganizationChart_Positions.Any(positionOrganizationChart => positionOrganizationChart.Position.Id == positionId && positionOrganizationChart.OrganizationChart.Id == organizationChartId))
                {
                    return;
                }
                var newPositionOrganizationChart = new OrganizationChart_Position { OrganizationChart = newOrganizationChart, Position = currentPosition };
                currentPosition.OrganizationChart_Positions.Add(newPositionOrganizationChart);
                Context.Commit();
                OrganizationChartAdded(this, new ModelAddedEventArgs<OrganizationChart_Position>(newPositionOrganizationChart));
        }

        public void RemoveOrganizationChart(int positionId, int organizationChartId)
        {
                Position currentPosition = _positionRepository.Single(position => position.Id == positionId);
                OrganizationChart_Position currentPositionOrganizationChart =
                    currentPosition.OrganizationChart_Positions.First(
                        positionOrganizationChart =>
                        positionOrganizationChart.Position.Id == positionId && positionOrganizationChart.Id == organizationChartId);
                int id = currentPositionOrganizationChart.Id;
                _orgChatPositionRepository.Delete(currentPositionOrganizationChart);
                Context.Commit();
                OrganizationChartRemoved(this, new ModelRemovedEventArgs(id));
        }
    }
}