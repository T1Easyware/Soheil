using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
    public class UnitSetDataService : DataServiceBase, IDataService<UnitSet>
    {
        public event EventHandler<ModelAddedEventArgs<UnitSet>> UnitSetAdded;
        private readonly Repository<UnitSet> _unitSetRepository;
        private readonly Repository<UnitGroup> _unitGroupRepository;

        public UnitSetDataService(SoheilEdmContext context)
        {
            Context = context ?? new SoheilEdmContext();
            _unitSetRepository = new Repository<UnitSet>(Context);
            _unitGroupRepository = new Repository<UnitGroup>(Context);
        }

        #region IDataService<UnitSet> Members

        public UnitSet GetSingle(int id)
        {
            return _unitSetRepository.Single(unitSet => unitSet.Id == id);
        }

        public ObservableCollection<UnitSet> GetAll()
        {
            var entityList = _unitSetRepository.Find(unitSet => unitSet.Status != (decimal) Status.Deleted, "UnitGroup");
            return new ObservableCollection<UnitSet>(entityList);
        }

        public int AddModel(UnitSet model)
        {
            var unitGroup = _unitGroupRepository.Single(group => group.Id == model.UnitGroup.Id);
            unitGroup.UnitSets.Add(model);
            Context.Commit();
            if (UnitSetAdded != null)
                UnitSetAdded(this, new ModelAddedEventArgs<UnitSet>(model));
            int id = model.Id;
            return id;
        }

        public int AddModel(UnitSet model, int groupId)
        {
            var unitGroup = _unitGroupRepository.Single(group => group.Id == groupId);
            unitGroup.UnitSets.Add(model);
            Context.Commit();
            if (UnitSetAdded != null)
                UnitSetAdded(this, new ModelAddedEventArgs<UnitSet>(model));
            int id = model.Id;
            return id;
        }

        public void UpdateModel(UnitSet model)
        {
            model.ModifiedBy = LoginInfo.Id;
            Context.Commit();
        }

        public void UpdateModel(UnitSet model, int groupId)
        {
            var group =
                _unitGroupRepository.Single(unitGroup => unitGroup.Id == groupId);

            model.ModifiedBy = LoginInfo.Id;
            model.UnitGroup = group;

            Context.Commit();
        }

        public void DeleteModel(UnitSet model)
        {
        }

        public void AttachModel(UnitSet model)
        {
            if (_unitSetRepository.Exists(unitSet => unitSet.Id == model.Id))
            {
                UpdateModel(model);
            }
            else
            {
                AddModel(model);
            }
        }

        public void AttachModel(UnitSet model, int groupId)
        {
            if (_unitSetRepository.Exists(unitSet => unitSet.Id == model.Id))
            {
                UpdateModel(model, groupId);
            }
            else
            {
                AddModel(model, groupId);
            }
        }

        #endregion


        /// <summary>
        /// Gets all active UnitSets as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UnitSet> GetActives()
        {
            var entityList = _unitSetRepository.Find(unitSet => unitSet.Status == (byte) Status.Active, "UnitGroup");
            return new ObservableCollection<UnitSet>(entityList);
        }

        /// <summary>
        /// Gets all active UnitSets as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UnitSet> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Operators)
            {
                IEnumerable<UnitSet> entityList = _unitSetRepository.Find(unitSet =>
                    unitSet.Status == (decimal) Status.Active
                    , "UnitGroup");
                return new ObservableCollection<UnitSet>(entityList);
            }
            return GetActives();
        }
    }
}