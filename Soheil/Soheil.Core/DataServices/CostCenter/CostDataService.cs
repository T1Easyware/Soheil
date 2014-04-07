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

        #region IDataService<Cost> Members

        public Cost GetSingle(int id)
        {
            Cost entity;
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                entity = costRepository.Single(cost => cost.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Cost> GetAll()
        {
            ObservableCollection<Cost> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cost>(context);
                IEnumerable<Cost> entityList = repository.Find(cost=>cost.Status != (decimal)Status.Deleted, "CostCenter","Operator","Machine","Station","PartWarehouse");
                models = new ObservableCollection<Cost>(entityList);
            }
            return models;
        }

        public int AddModel(Cost model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<CostCenter>(context);
                CostCenter costCenter = groupRepository.Single(group => group.Id == model.CostCenter.Id);
                costCenter.Costs.Add(model);
                context.Commit();
                if (CostAdded != null)
                    CostAdded(this, new ModelAddedEventArgs<Cost>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(Cost model, int groupId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<CostCenter>(context);
                CostCenter costCenter = groupRepository.Single(group => group.Id == groupId);
                model.CostCenter = costCenter;
                costCenter.Costs.Add(model);
                context.Commit();
                if (CostAdded != null)
                    CostAdded(this, new ModelAddedEventArgs<Cost>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Cost model)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);
                Cost entity = costRepository.Single(cost => cost.Id == model.Id);
                CostCenter group =
                    costCenterRepository.Single(costCenter => costCenter.Id == model.CostCenter.Id);

                entity.Quantity = model.Quantity;
                entity.CostType = model.CostType;
                entity.CostValue = model.CostValue;
                entity.Date = model.Date;
                entity.CostCenter = group;


                context.Commit();
            }
        }

        public void UpdateModel(Cost costModel, int groupId, PartWarehouse partModel = null, int costSourceId= -1)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);
                var partWarehouseRepository = new Repository<PartWarehouse>(context);

                Cost costEntity = costRepository.Single(cost => cost.Id == costModel.Id);
                CostCenter group =
                    costCenterRepository.Single(costCenter => costCenter.Id == groupId);

                costEntity.Quantity = costModel.Quantity;
                costEntity.CostValue = costModel.CostValue;
                costEntity.Date = costModel.Date;
                costEntity.CostCenter = group;
                costEntity.Description = costModel.Description;
                costEntity.Status = costModel.Status;
                costEntity.CostType = costModel.CostType;

                if (partModel != null)
                {
                    PartWarehouse partEntity =
                        partWarehouseRepository.Single(part => part.Id == partModel.Id);
                    partEntity.Quantity = partModel.Quantity;
                    costEntity.PartWarehouse = partEntity;
                }

                if (costSourceId > 0)
                {
                    switch ((CostSourceType)costEntity.CostCenter.SourceType)
                    {
                        case CostSourceType.Other:
                            break;
                        case CostSourceType.Machines:
                            var machineRepository = new Repository<Machine>(context);
                            Machine machine =
                                machineRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costEntity.Machine = machine;
                            break;
                        case CostSourceType.Operators:
                            var operatorRepository = new Repository<Operator>(context);
                            Operator opr =
                                operatorRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costEntity.Operator = opr;
                            break;
                        case CostSourceType.Stations:
                            var stationRepository = new Repository<Station>(context);
                            Station station =
                                stationRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costEntity.Station = station;
                            break;
                        case CostSourceType.Activities:
                            var activityRepository = new Repository<Activity>(context);
                            Activity activity =
                                activityRepository.Single(srcCost => srcCost.Id == costSourceId);
                            costEntity.Activity = activity;
                            break;
                    }
                }

                context.Commit();
            }
        }

        public void DeleteModel(Cost model)
        {
        }

        public void AttachModel(Cost model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cost>(context);
                if (repository.Exists(cost => cost.Id == model.Id))
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

        public ObservableCollection<Cost> GetActives()
        {
            ObservableCollection<Cost> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cost>(context);
                IEnumerable<Cost> entityList = repository.Find(cost => cost.Status == (decimal)Status.Active, "CostCenter", "Operator", "Machine", "Station", "PartWarehouse");
                models = new ObservableCollection<Cost>(entityList);
            }
            return models;
        }
    }
}