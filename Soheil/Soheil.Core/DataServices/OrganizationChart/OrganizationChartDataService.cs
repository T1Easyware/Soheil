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
    public class OrganizationChartDataService :DataServiceBase, IDataService<OrganizationChart>
    {
        private readonly Repository<OrganizationChart> _orgChartRepository;
        private readonly Repository<Position> _productRepository;
        private readonly Repository<OrganizationChart_Position> _orgChatPositionRepository;

        public OrganizationChartDataService(SoheilEdmContext context)
        {
            Context = context;
            _orgChartRepository = new Repository<OrganizationChart>(context);
            _productRepository = new Repository<Position>(context);
            _orgChatPositionRepository = new Repository<OrganizationChart_Position>(context);
        }
        #region IDataService<OrganizationChart> Members

        public OrganizationChart GetSingle(int id)
        {
                return _orgChartRepository.Single(organizationChart => organizationChart.Id == id);
        }

        public ObservableCollection<OrganizationChart> GetAll()
        {
                IEnumerable<OrganizationChart> entityList =
                    _orgChartRepository.Find(
                        organizationChart => organizationChart.Status != (decimal)Status.Deleted);
                return new ObservableCollection<OrganizationChart>(entityList);
        }

        public int AddModel(OrganizationChart model)
        {
                _orgChartRepository.Add(model);
                Context.Commit();
                if (OrganizationChartAdded != null)
                    OrganizationChartAdded(this, new ModelAddedEventArgs<OrganizationChart>(model));
                return model.Id;
        }

        public void UpdateModel(OrganizationChart model)
        {
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
        }

        public void DeleteModel(OrganizationChart model)
        {
        }

        public void AttachModel(OrganizationChart model)
        {
                if (_orgChartRepository.Exists(organizationChart => organizationChart.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
        }

        #endregion

        public ObservableCollection<OrganizationChart> GetActives()
        {
                IEnumerable<OrganizationChart> entityList =
                    _orgChartRepository.Find(
                        organizationChart => organizationChart.Status == (decimal)Status.Active);
                return new ObservableCollection<OrganizationChart>(entityList);
        }

        public ObservableCollection<OrganizationChart> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Positions)
            {
                    IEnumerable<OrganizationChart> entityList =
                        _orgChartRepository.Find(
                            organizationChart => organizationChart.Status == (decimal)Status.Active 
								&& !organizationChart.OrganizationChart_Positions.Any(item=>item.Position.Id == linkId));
                    return new ObservableCollection<OrganizationChart>(entityList);
            }
            return GetActives();
        }

        public event EventHandler<ModelAddedEventArgs<OrganizationChart>> OrganizationChartAdded;
        public event EventHandler<ModelAddedEventArgs<OrganizationChart_Position>> PositionAdded;
        public event EventHandler<ModelRemovedEventArgs> PositionRemoved;


        public ObservableCollection<OrganizationChart_Position> GetPositions(int organizationChartId)
        {
                OrganizationChart entity = _orgChartRepository.FirstOrDefault(organizationChart => organizationChart.Id == organizationChartId, "OrganizationChart_Positions.Position", "OrganizationChart_Positions.Children");
                return new ObservableCollection<OrganizationChart_Position>(entity.OrganizationChart_Positions.Where(item=>item.Position.Status == (decimal)Status.Active));
        }

        public void AddPosition(int organizationChartId, int positionId, int positionParentId)
        {
                OrganizationChart currentOrganizationChart = _orgChartRepository.Single(organizationChart => organizationChart.Id == organizationChartId);
                Position newPosition = _productRepository.Single(product => product.Id == positionId);
                if (currentOrganizationChart.OrganizationChart_Positions.Any(organizationChartPosition => organizationChartPosition.OrganizationChart.Id == organizationChartId && organizationChartPosition.Position.Id == positionId))
                {
                    return;
                }
                var parent = _orgChatPositionRepository.FirstOrDefault(orgChartPos => orgChartPos.Id == positionParentId);
                var newOrganizationChartPosition = new OrganizationChart_Position { Position = newPosition, OrganizationChart = currentOrganizationChart, Parent = parent };
                currentOrganizationChart.OrganizationChart_Positions.Add(newOrganizationChartPosition);
                Context.Commit();
                PositionAdded(this, new ModelAddedEventArgs<OrganizationChart_Position>(newOrganizationChartPosition));
        }

        public void RemovePosition(int organizationChartId, int positionId)
        {
                OrganizationChart currentOrganizationChart = _orgChartRepository.Single(organizationChart => organizationChart.Id == organizationChartId);
                OrganizationChart_Position currentOrganizationChartPosition =
                    currentOrganizationChart.OrganizationChart_Positions.First(
                        organizationChartPosition =>
                        organizationChartPosition.OrganizationChart.Id == organizationChartId && organizationChartPosition.Position.Id == positionId);
                _orgChatPositionRepository.Delete(currentOrganizationChartPosition);
                Context.Commit();
                PositionRemoved(this, new ModelRemovedEventArgs(currentOrganizationChartPosition.Id));
        }


    }
}