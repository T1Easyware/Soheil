using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class PartWarehouseGroupDataService : IDataService<PartWarehouseGroup>
    {
        #region IDataService<PartWarehouseGroup> Members

        public PartWarehouseGroup GetSingle(int id)
        {
            PartWarehouseGroup entity;
            using (var context = new SoheilEdmContext())
            {
                var costCenterRepository = new Repository<PartWarehouseGroup>(context);
                entity = costCenterRepository.Single(costCenter => costCenter.Id == id);
            }
            return entity;
        }

        public ObservableCollection<PartWarehouseGroup> GetAll()
        {
            ObservableCollection<PartWarehouseGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouseGroup>(context);
                IEnumerable<PartWarehouseGroup> entityList = repository.Find(group=>group.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<PartWarehouseGroup>(entityList);
            }
            return models;
        }

        public ObservableCollection<PartWarehouseGroup> GetActives()
        {
            ObservableCollection<PartWarehouseGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouseGroup>(context);
                IEnumerable<PartWarehouseGroup> entityList = repository.Find(group => group.Status == (decimal)Status.Active);
                models = new ObservableCollection<PartWarehouseGroup>(entityList);
            }
            return models;
        }

        public int AddModel(PartWarehouseGroup model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouseGroup>(context);
                repository.Add(model);
                context.Commit();
                if (PartWarehouseGroupAdded != null)
                    PartWarehouseGroupAdded(this, new ModelAddedEventArgs<PartWarehouseGroup>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(PartWarehouseGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var costCenterRepository = new Repository<PartWarehouseGroup>(context);
                PartWarehouseGroup entity = costCenterRepository.Single(costCenter => costCenter.Id == model.Id);

                entity.Name = model.Name;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(PartWarehouseGroup model)
        {
        }

        public void AttachModel(PartWarehouseGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouseGroup>(context);
                if (repository.Exists(costCenter => costCenter.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<PartWarehouseGroup>> PartWarehouseGroupAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The cost group id.</param>
        /// <returns></returns>
        public PartWarehouseGroup GetModel(int id)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PartWarehouseGroup>(context);
                return repository.Single(cost => cost.Id == id);
            }
        }
    }
}