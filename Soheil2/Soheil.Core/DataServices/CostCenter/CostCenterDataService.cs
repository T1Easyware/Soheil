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
    public class CostCenterDataService : IDataService<CostCenter>
    {
        #region IDataService<CostCenter> Members

        public CostCenter GetSingle(int id)
        {
            CostCenter entity;
            using (var context = new SoheilEdmContext())
            {
                var costCenterRepository = new Repository<CostCenter>(context);
                entity = costCenterRepository.Single(costCenter => costCenter.Id == id);
            }
            return entity;
        }

        public ObservableCollection<CostCenter> GetAll()
        {
            ObservableCollection<CostCenter> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
                IEnumerable<CostCenter> entityList = repository.Find(costCenter=> costCenter.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<CostCenter>(entityList);
            }
            return models;
        }

        public ObservableCollection<CostCenter> GetActives()
        {
            ObservableCollection<CostCenter> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
                IEnumerable<CostCenter> entityList = repository.Find(costCenter => costCenter.Status == (decimal)Status.Active);
                models = new ObservableCollection<CostCenter>(entityList);
            }
            return models;
        }

        public int AddModel(CostCenter model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
                repository.Add(model);
                context.Commit();
                if (CostCenterAdded != null)
                    CostCenterAdded(this, new ModelAddedEventArgs<CostCenter>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(CostCenter model)
        {
            using (var context = new SoheilEdmContext())
            {
                var costCenterRepository = new Repository<CostCenter>(context);
                CostCenter entity = costCenterRepository.FirstOrDefault(costCenter => costCenter.Id == model.Id);

                entity.Description = model.Description;
                entity.Name = model.Name;
                entity.SourceType = model.SourceType;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(CostCenter model)
        {
        }

        public void AttachModel(CostCenter model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
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

        public event EventHandler<ModelAddedEventArgs<CostCenter>> CostCenterAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The cost group id.</param>
        /// <returns></returns>
        public CostCenter GetModel(int id)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
                return repository.Single(cost => cost.Id == id);
            }
        }
    }
}