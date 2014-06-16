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
    public class PartWarehouseGroupDataService : DataServiceBase, IDataService<PartWarehouseGroup>
    {
        private readonly Repository<PartWarehouseGroup> _groupRepository;

        public PartWarehouseGroupDataService(SoheilEdmContext context)
        {
            Context = context;
            _groupRepository = new Repository<PartWarehouseGroup>(context);
        }

        #region IDataService<PartWarehouseGroup> Members

        public PartWarehouseGroup GetSingle(int id)
        {
            return _groupRepository.Single(costCenter => costCenter.Id == id);
        }

        public ObservableCollection<PartWarehouseGroup> GetAll()
        {
                IEnumerable<PartWarehouseGroup> entityList = _groupRepository.Find(group=>group.Status != (decimal)Status.Deleted);
                return new ObservableCollection<PartWarehouseGroup>(entityList);
        }

        public ObservableCollection<PartWarehouseGroup> GetActives()
        {
                IEnumerable<PartWarehouseGroup> entityList = _groupRepository.Find(group => group.Status == (decimal)Status.Active);
                return new ObservableCollection<PartWarehouseGroup>(entityList);
        }

        public int AddModel(PartWarehouseGroup model)
        {
                _groupRepository.Add(model);
                Context.Commit();
                if (PartWarehouseGroupAdded != null)
                    PartWarehouseGroupAdded(this, new ModelAddedEventArgs<PartWarehouseGroup>(model));
                return model.Id;
        }

        public void UpdateModel(PartWarehouseGroup model)
        {
            Context.Commit();
        }

        public void DeleteModel(PartWarehouseGroup model)
        {
        }

        public void AttachModel(PartWarehouseGroup model)
        {
            if (_groupRepository.Exists(costCenter => costCenter.Id == model.Id))
            {
                UpdateModel(model);
            }
            else
            {
                AddModel(model);
            }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<PartWarehouseGroup>> PartWarehouseGroupAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The cost group id.</param>
        /// <returns></returns>
        public PartWarehouseGroup GetModel(int id)
        {
            return _groupRepository.Single(cost => cost.Id == id);
        }
    }
}