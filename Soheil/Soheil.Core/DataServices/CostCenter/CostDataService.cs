using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class CostDataService : DataServiceBase, IDataService<Cost>
    {
        public event EventHandler<ModelAddedEventArgs<Cost>> CostAdded;
        readonly Repository<Cost> _costRepository;
        readonly Repository<CostCenter> _costCenterRepository;

        public CostDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
            _costRepository = new Repository<Cost>(Context);
            _costCenterRepository = new Repository<CostCenter>(Context);
		}

        #region IDataService<Cost> Members

        public Cost GetSingle(int id)
        {
            return _costRepository.Single(cost => cost.Id == id);
        }

        public ObservableCollection<Cost> GetAll()
        {

            IEnumerable<Cost> entityList = _costRepository.Find(cost => cost.Status != (decimal) Status.Deleted,
                "CostCenter", "Operator", "Machine", "Station", "PartWarehouse");
            return new ObservableCollection<Cost>(entityList);
        }

        public int AddModel(Cost model)
        {
            CostCenter costCenter = _costCenterRepository.Single(group => group.Id == model.CostCenter.Id);
            costCenter.Costs.Add(model);
            Context.Commit();
            if (CostAdded != null)
                CostAdded(this, new ModelAddedEventArgs<Cost>(model));
            return model.Id;
        }

        public int AddModel(Cost model, int groupId)
        {

                CostCenter costCenter = _costCenterRepository.Single(group => group.Id == groupId);
                model.CostCenter = costCenter;
                costCenter.Costs.Add(model);
                Context.Commit();
                if (CostAdded != null)
                    CostAdded(this, new ModelAddedEventArgs<Cost>(model));
                return model.Id;
        }

        public void UpdateModel(Cost model)
        {
            model.Date = DateTime.Now;
            Context.Commit();
        }

        public void UpdateModel(Cost costModel, int groupId, PartWarehouse partModel = null, int costSourceId= -1)
        {
                if (partModel != null)
                {
                    costModel.PartWarehouse = partModel;
                }

                if (costSourceId > 0)
                {
                    switch ((CostSourceType)costModel.CostCenter.SourceType)
                    {
                        case CostSourceType.Other:
                            break;
                        case CostSourceType.Machines:
                            var machineRepository = new Repository<Machine>(Context);
                            Machine machine =
                                machineRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costModel.Machine = machine;
                            break;
                        case CostSourceType.Operators:
                            var operatorRepository = new Repository<Operator>(Context);
                            Operator opr =
                                operatorRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costModel.Operator = opr;
                            break;
                        case CostSourceType.Stations:
                            var stationRepository = new Repository<Station>(Context);
                            Station station =
                                stationRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costModel.Station = station;
                            break;
                        case CostSourceType.Activities:
                            var activityRepository = new Repository<Activity>(Context);
                            Activity activity =
                                activityRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costModel.Activity = activity;
                            break;
                    }
                }

                Context.Commit();
            
        }

        public void DeleteModel(Cost model)
        {
        }

        public void AttachModel(Cost model)
        {
            if (_costRepository.Exists(cost => cost.Id == model.Id))
            {
                UpdateModel(model);
            }
            else
            {
                AddModel(model);
            }
        }

        #endregion

        public ObservableCollection<Cost> GetActives()
        {
                IEnumerable<Cost> entityList = _costRepository.Find(cost => cost.Status == (decimal)Status.Active, "CostCenter", "Operator", "Machine", "Station", "PartWarehouse");
                return new ObservableCollection<Cost>(entityList);
        }
    }
}