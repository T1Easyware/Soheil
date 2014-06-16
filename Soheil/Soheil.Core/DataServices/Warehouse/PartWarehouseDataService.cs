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
        private readonly Repository<PartWarehouse> _partRepository;
        private readonly Repository<PartWarehouseGroup> _groupRepository;

        public PartWarehouseDataService(SoheilEdmContext context)
        {
            Context = context;
            _partRepository = new Repository<PartWarehouse>(context);
            _groupRepository = new Repository<PartWarehouseGroup>(context);
        }

        public event EventHandler<ModelAddedEventArgs<PartWarehouse>> PartWarehouseAdded;

        #region IDataService<PartWarehouse> Members

        public PartWarehouse GetSingle(int id)
        {
            return _partRepository.Single(cost => cost.Id == id);
        }

        public ObservableCollection<PartWarehouse> GetAll()
        {
            IEnumerable<PartWarehouse> entityList =
                _partRepository.Find(
                    cost => cost.Status != (decimal) Status.Deleted, "PartWarehouseGroup");
            return new ObservableCollection<PartWarehouse>(entityList);
        }

        public int AddModel(PartWarehouse model)
        {
                PartWarehouseGroup costCenter = _groupRepository.Single(group => group.Id == model.PartWarehouseGroup.Id);
                costCenter.PartWarehouses.Add(model);
                Context.Commit();
                if (PartWarehouseAdded != null)
                    PartWarehouseAdded(this, new ModelAddedEventArgs<PartWarehouse>(model));
                return model.Id;
        }

        public int AddModel(PartWarehouse model, int groupId)
        {
            PartWarehouseGroup costCenter = _groupRepository.Single(group => group.Id == groupId);
            model.PartWarehouseGroup = costCenter;
            costCenter.PartWarehouses.Add(model);
            Context.Commit();
            if (PartWarehouseAdded != null)
                PartWarehouseAdded(this, new ModelAddedEventArgs<PartWarehouse>(model));
            return model.Id;
        }

        public void UpdateModel(PartWarehouse model)
        {
            Context.Commit();
        }

        public void UpdateModel(PartWarehouse model, int groupId)
        {
            PartWarehouseGroup group =
                _groupRepository.Single(costCenter => costCenter.Id == groupId);
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            model.PartWarehouseGroup = group;
            Context.Commit();
        }

        public void DeleteModel(PartWarehouse model)
        {
        }

        public void AttachModel(PartWarehouse model)
        {
            if (_partRepository.Exists(cost => cost.Id == model.Id))
            {
                UpdateModel(model);
            }
            else
            {
                AddModel(model);
            }
        }

        public void AttachModel(PartWarehouse model, int groupId)
        {
            if (_partRepository.Exists(cost => cost.Id == model.Id))
            {
                UpdateModel(model, groupId);
            }
            else
            {
                AddModel(model, groupId);
            }
        }

        #endregion

        public ObservableCollection<PartWarehouse> GetActives()
        {
                IEnumerable<PartWarehouse> entityList =
                    _partRepository.Find(
                        cost => cost.Status == (decimal) Status.Active,"PartWarehouseGroup");
                return new ObservableCollection<PartWarehouse>(entityList);
        }

        public bool HasCost(int id)
        {
            return _partRepository.Exists(part => part.Id == id && part.Cost != null && part.Cost.Status != (decimal) Status.Deleted);
        }

    }
}