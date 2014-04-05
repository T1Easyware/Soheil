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
    public class OrganizationChartDataService : IDataService<OrganizationChart>
    {
        #region IDataService<OrganizationChart> Members

        public OrganizationChart GetSingle(int id)
        {
            OrganizationChart entity;
            using (var context = new SoheilEdmContext())
            {
                var organizationChartRepository = new Repository<OrganizationChart>(context);
                entity = organizationChartRepository.Single(organizationChart => organizationChart.Id == id);
            }
            return entity;
        }

        public ObservableCollection<OrganizationChart> GetAll()
        {
            ObservableCollection<OrganizationChart> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart>(context);
                IEnumerable<OrganizationChart> entityList =
                    repository.Find(
                        organizationChart => organizationChart.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<OrganizationChart>(entityList);
            }
            return models;
        }

        public int AddModel(OrganizationChart model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart>(context);
                repository.Add(model);
                context.Commit();
                if (OrganizationChartAdded != null)
                    OrganizationChartAdded(this, new ModelAddedEventArgs<OrganizationChart>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(OrganizationChart model)
        {
            using (var context = new SoheilEdmContext())
            {
                var organizationChartRepository = new Repository<OrganizationChart>(context);
                OrganizationChart entity = organizationChartRepository.Single(organizationChart => organizationChart.Id == model.Id);

                entity.Name = model.Name;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(OrganizationChart model)
        {
        }

        public void AttachModel(OrganizationChart model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart>(context);
                if (repository.Exists(organizationChart => organizationChart.Id == model.Id))
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

        public ObservableCollection<OrganizationChart> GetActives()
        {
            ObservableCollection<OrganizationChart> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart>(context);
                IEnumerable<OrganizationChart> entityList =
                    repository.Find(
                        organizationChart => organizationChart.Status == (decimal)Status.Active);
                models = new ObservableCollection<OrganizationChart>(entityList);
            }
            return models;
        }

        public ObservableCollection<OrganizationChart> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Positions)
            {
                ObservableCollection<OrganizationChart> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<OrganizationChart>(context);
                    IEnumerable<OrganizationChart> entityList =
                        repository.Find(
                            organizationChart => organizationChart.Status == (decimal)Status.Active && organizationChart.OrganizationChart_Positions.All(item=>item.Position.Id != linkId));
                    models = new ObservableCollection<OrganizationChart>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public event EventHandler<ModelAddedEventArgs<OrganizationChart>> OrganizationChartAdded;
        public event EventHandler<ModelAddedEventArgs<OrganizationChart_Position>> PositionAdded;
        public event EventHandler<ModelRemovedEventArgs> PositionRemoved;


        public ObservableCollection<OrganizationChart_Position> GetPositions(int organizationChartId)
        {
            ObservableCollection<OrganizationChart_Position> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart>(context);
                OrganizationChart entity = repository.FirstOrDefault(organizationChart => organizationChart.Id == organizationChartId, "OrganizationChart_Positions.Position", "OrganizationChart_Positions.Children");
                models = new ObservableCollection<OrganizationChart_Position>(entity.OrganizationChart_Positions.Where(item=>item.Position.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddPosition(int organizationChartId, int positionId, int positionParentId)
        {
            using (var context = new SoheilEdmContext())
            {
                var organizationChartRepository = new Repository<OrganizationChart>(context);
                var productRepository = new Repository<Position>(context);
                var parentRepository = new Repository<OrganizationChart_Position>(context);
                OrganizationChart currentOrganizationChart = organizationChartRepository.Single(organizationChart => organizationChart.Id == organizationChartId);
                Position newPosition = productRepository.Single(product => product.Id == positionId);
                if (currentOrganizationChart.OrganizationChart_Positions.Any(organizationChartPosition => organizationChartPosition.OrganizationChart.Id == organizationChartId && organizationChartPosition.Position.Id == positionId))
                {
                    return;
                }
                var parent = parentRepository.FirstOrDefault(orgChartPos => orgChartPos.Id == positionParentId);
                var newOrganizationChartPosition = new OrganizationChart_Position { Position = newPosition, OrganizationChart = currentOrganizationChart, Parent = parent };
                currentOrganizationChart.OrganizationChart_Positions.Add(newOrganizationChartPosition);
                context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<OrganizationChart_Position>(newOrganizationChartPosition));
            }
        }

        public void RemovePosition(int organizationChartId, int positionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var organizationChartRepository = new Repository<OrganizationChart>(context);
                var organizationChartPositionRepository = new Repository<OrganizationChart_Position>(context);
                OrganizationChart currentOrganizationChart = organizationChartRepository.Single(organizationChart => organizationChart.Id == organizationChartId);
                OrganizationChart_Position currentOrganizationChartPosition =
                    currentOrganizationChart.OrganizationChart_Positions.First(
                        organizationChartPosition =>
                        organizationChartPosition.OrganizationChart.Id == organizationChartId && organizationChartPosition.Position.Id == positionId);
                organizationChartPositionRepository.Delete(currentOrganizationChartPosition);
                context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(currentOrganizationChartPosition.Id));
            }
        }


    }
}