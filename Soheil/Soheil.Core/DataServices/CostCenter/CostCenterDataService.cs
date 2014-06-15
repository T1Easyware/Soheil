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
    public class CostCenterDataService :  DataServiceBase, IDataService<CostCenter>
    {
        readonly Repository<CostCenter> _costCenterRepository;

        public CostCenterDataService(SoheilEdmContext context)
        {
            Context = context;
            _costCenterRepository = new Repository<CostCenter>(context);
        }

        #region IDataService<CostCenter> Members

        public CostCenter GetSingle(int id)
        {
            return _costCenterRepository.Single(costCenter => costCenter.Id == id);
        }

        public ObservableCollection<CostCenter> GetAll()
        {

            IEnumerable<CostCenter> entityList =
                _costCenterRepository.Find(costCenter => costCenter.Status != (decimal) Status.Deleted);
            return new ObservableCollection<CostCenter>(entityList);
        }

        public ObservableCollection<CostCenter> GetActives()
        {
            IEnumerable<CostCenter> entityList =
                _costCenterRepository.Find(costCenter => costCenter.Status == (decimal) Status.Active);
            return new ObservableCollection<CostCenter>(entityList);
        }

        public int AddModel(CostCenter model)
        {
                _costCenterRepository.Add(model);
                Context.Commit();
                if (CostCenterAdded != null)
                    CostCenterAdded(this, new ModelAddedEventArgs<CostCenter>(model));
                return model.Id;
        }

        public void UpdateModel(CostCenter model)
        {
            Context.Commit();
        }

        public void DeleteModel(CostCenter model)
        {
        }

        public void AttachModel(CostCenter model)
        {
            if (_costCenterRepository.Exists(costCenter => costCenter.Id == model.Id))
            {
                UpdateModel(model);
            }
            else
            {
                AddModel(model);
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
            return _costCenterRepository.Single(cost => cost.Id == id);
        }
    }
}