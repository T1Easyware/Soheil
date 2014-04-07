using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class PositionDataService : IDataService<Position>
    {
        #region IDataService<Position> Members

        public Position GetSingle(int id)
        {
            Position entity;
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                entity = positionRepository.Single(position => position.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Position> GetAll()
        {
            ObservableCollection<Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                IEnumerable<Position> entityList =
                    repository.Find(
                        position => position.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<Position>(entityList);
            }
            return models;
        }

        public int AddModel(Position model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                repository.Add(model);
                context.Commit();
                if (PositionAdded != null)
                    PositionAdded(this, new ModelAddedEventArgs<Position>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Position model)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                Position entity = positionRepository.Single(position => position.Id == model.Id);

                entity.Name = model.Name;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(Position model)
        {
        }

        public void AttachModel(Position model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                if (repository.Exists(position => position.Id == model.Id))
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

        public ObservableCollection<Position> GetActives()
        {
            ObservableCollection<Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                IEnumerable<Position> entityList =
                    repository.Find(
                        position => position.Status == (decimal)Status.Active);
                models = new ObservableCollection<Position>(entityList);
            }
            return models;
        }

        public ObservableCollection<Position> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Users)
            {
                ObservableCollection<Position> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Position>(context);
                    IEnumerable<Position> entityList =
                        repository.Find(
                            position => position.Status == (decimal)Status.Active && position.User_Positions.All(item=>item.User.Id != linkId));
                    models = new ObservableCollection<Position>(entityList);
                }
                return models;
            }
            if (linkType == SoheilEntityType.OrganizationCharts)
            {
                ObservableCollection<Position> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Position>(context);
                    IEnumerable<Position> entityList =
                        repository.Find(
                            position => position.Status == (decimal)Status.Active && position.OrganizationChart_Positions.All(item=> item.OrganizationChart.Id != linkId));
                    models = new ObservableCollection<Position>(entityList);
                }
                return models;
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
            ObservableCollection<User_Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                Position entity = repository.FirstOrDefault(position => position.Id == positionId, "User_Positions.User", "User_Positions.Position");
                models = new ObservableCollection<User_Position>(entity.User_Positions.Where(item=>item.User.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddUser(int positionId, int userId)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                var productRepository = new Repository<User>(context);
                Position currentPosition = positionRepository.Single(position => position.Id == positionId);
                User newUser = productRepository.Single(product => product.Id == userId);
                if (currentPosition.User_Positions.Any(positionUser => positionUser.Position.Id == positionId && positionUser.User.Id == userId))
                {
                    return;
                }
                var newPositionUser = new User_Position { User = newUser, Position = currentPosition };
                currentPosition.User_Positions.Add(newPositionUser);
                context.Commit();
                UserAdded(this, new ModelAddedEventArgs<User_Position>(newPositionUser));
            }
        }

        public void RemoveUser(int positionId, int userId)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                var positionUserRepository = new Repository<User_Position>(context);
                Position currentPosition = positionRepository.Single(position => position.Id == positionId);
                User_Position currentPositionUser =
                    currentPosition.User_Positions.First(
                        positionUser =>
                        positionUser.Position.Id == positionId && positionUser.Id == userId);
                int id = currentPositionUser.Id;
                positionUserRepository.Delete(currentPositionUser);
                context.Commit();
                UserRemoved(this, new ModelRemovedEventArgs(id));
            }
        }

        public ObservableCollection<Position_AccessRule> GetAccessRules(int positionId)
        {
            ObservableCollection<Position_AccessRule> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                Position entity = repository.FirstOrDefault(position => position.Id == positionId, "Position_AccessRules.Position", "Position_AccessRules.AccessRule", "Position_AccessRules.AccessRule.Parent");
                models = new ObservableCollection<Position_AccessRule>(entity.Position_AccessRules);
            }

            return models;
        }

        public void AddRemoveAccessRule(int positionId, int accessRuleId, AccessType type)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                var productRepository = new Repository<AccessRule>(context);
                Position currentPosition = positionRepository.Single(position => position.Id == positionId);
                AccessRule newAccessRule = productRepository.Single(product => product.Id == accessRuleId);
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
                context.Commit();
                AccessRuleChanged(this, new ModelAddedEventArgs<Position_AccessRule>(newPositionAccessRule));
            }
        }

        public ObservableCollection<OrganizationChart_Position> GetOrganizationCharts(int positionId)
        {
            ObservableCollection<OrganizationChart_Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position>(context);
                Position entity = repository.First(position => position.Id == positionId);
                models = new ObservableCollection<OrganizationChart_Position>(entity.OrganizationChart_Positions.Where(item=>item.OrganizationChart.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddOrganizationChart(int positionId, int organizationChartId)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                var productRepository = new Repository<OrganizationChart>(context);
                Position currentPosition = positionRepository.Single(position => position.Id == positionId);
                OrganizationChart newOrganizationChart = productRepository.Single(product => product.Id == organizationChartId);
                if (currentPosition.OrganizationChart_Positions.Any(positionOrganizationChart => positionOrganizationChart.Position.Id == positionId && positionOrganizationChart.OrganizationChart.Id == organizationChartId))
                {
                    return;
                }
                var newPositionOrganizationChart = new OrganizationChart_Position { OrganizationChart = newOrganizationChart, Position = currentPosition };
                currentPosition.OrganizationChart_Positions.Add(newPositionOrganizationChart);
                context.Commit();
                OrganizationChartAdded(this, new ModelAddedEventArgs<OrganizationChart_Position>(newPositionOrganizationChart));
            }
        }

        public void RemoveOrganizationChart(int positionId, int organizationChartId)
        {
            using (var context = new SoheilEdmContext())
            {
                var positionRepository = new Repository<Position>(context);
                var positionOrganizationChartRepository = new Repository<OrganizationChart_Position>(context);
                Position currentPosition = positionRepository.Single(position => position.Id == positionId);
                OrganizationChart_Position currentPositionOrganizationChart =
                    currentPosition.OrganizationChart_Positions.First(
                        positionOrganizationChart =>
                        positionOrganizationChart.Position.Id == positionId && positionOrganizationChart.Id == organizationChartId);
                int id = currentPositionOrganizationChart.Id;
                positionOrganizationChartRepository.Delete(currentPositionOrganizationChart);
                context.Commit();
                OrganizationChartRemoved(this, new ModelRemovedEventArgs(id));
            }
        }
    }
}