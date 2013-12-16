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
    public class PartWarehouseDataService : DataServiceBase, IDataService<PartWarehouse>
    {
        public event EventHandler<ModelAddedEventArgs<PartWarehouse>> PartWarehouseAdded;

        #region IDataService<PartWarehouse> Members

        public PartWarehouse GetSingle(int id)
        {
            PartWarehouse entity;
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<PartWarehouse>(context);
                entity = costRepository.Single(cost => cost.Id == id);
            }
            return entity;
        }

        public ObservableCollection<PartWarehouse> GetAll()
        {
            ObservableCollection<PartWarehouse> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouse>(context);
                IEnumerable<PartWarehouse> entityList =
                    repository.Find(
                        cost => cost.Status != (decimal)Status.Deleted, "PartWarehouseGroup");
                models = new ObservableCollection<PartWarehouse>(entityList);
            }
            return models;
        }

        public int AddModel(PartWarehouse model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<PartWarehouseGroup>(context);
                PartWarehouseGroup costCenter = groupRepository.Single(group => group.Id == model.PartWarehouseGroup.Id);
                costCenter.PartWarehouses.Add(model);
                context.Commit();
                if (PartWarehouseAdded != null)
                    PartWarehouseAdded(this, new ModelAddedEventArgs<PartWarehouse>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(PartWarehouse model, int groupId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<PartWarehouseGroup>(context);
                PartWarehouseGroup costCenter = groupRepository.Single(group => group.Id == groupId);
                model.PartWarehouseGroup = costCenter;
                costCenter.PartWarehouses.Add(model);
                context.Commit();
                if (PartWarehouseAdded != null)
                    PartWarehouseAdded(this, new ModelAddedEventArgs<PartWarehouse>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(PartWarehouse model)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<PartWarehouse>(context);
                PartWarehouse entity = costRepository.FirstOrDefault(cost => cost.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                entity.Quantity = model.Quantity;
                entity.OriginalQuantity = model.OriginalQuantity;
                entity.TotalCost = model.TotalCost;

                context.Commit();
            }
        }

        public void UpdateModel(PartWarehouse model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<PartWarehouse>(context);
                var costCenterRepository = new Repository<PartWarehouseGroup>(context);
                PartWarehouse entity = costRepository.Single(cost => cost.Id == model.Id);
                PartWarehouseGroup group =
                    costCenterRepository.Single(costCenter => costCenter.Id == groupId);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                entity.Quantity = model.Quantity;
                entity.OriginalQuantity = model.OriginalQuantity;
                entity.TotalCost = model.TotalCost;

                entity.PartWarehouseGroup = group;

                context.Commit();
            }
        }

        public void DeleteModel(PartWarehouse model)
        {
        }

        public void AttachModel(PartWarehouse model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouse>(context);
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

        public void AttachModel(PartWarehouse model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouse>(context);
                if (repository.Exists(cost => cost.Id == model.Id))
                {
                    UpdateModel(model, groupId);
                }
                else
                {
                    AddModel(model, groupId);
                }
            }
        }

        #endregion

        public ObservableCollection<PartWarehouse> GetActives()
        {
            ObservableCollection<PartWarehouse> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouse>(context);
                IEnumerable<PartWarehouse> entityList =
                    repository.Find(
                        cost => cost.Status == (decimal) Status.Active,"PartWarehouseGroup");
                models = new ObservableCollection<PartWarehouse>(entityList);
            }
            return models;
        }
        public bool HasCost(int id)
        {
            bool hasCost;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouse>(context);
                hasCost = repository.Exists(part => part.Id == id && part.Cost != null && part.Cost.Status != (decimal)Status.Deleted);
            }
            return hasCost;
        }

    }
}