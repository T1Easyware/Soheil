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
    public class UnitGroupDataService : DataServiceBase, IDataService<UnitGroup>
    {
		public event EventHandler<ModelAddedEventArgs<UnitGroup>> UnitGroupAdded;
        readonly Repository<UnitGroup> _unitGroupRepository;


		public UnitGroupDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
            _unitGroupRepository = new Repository<UnitGroup>(Context);
		}


		#region IDataService<UnitGroupVM> Members

		public UnitGroup GetSingle(int id)
		{
			return _unitGroupRepository.Single(unitGroup => unitGroup.Id == id);
		}

		public ObservableCollection<UnitGroup> GetAll()
		{
			IEnumerable<UnitGroup> entityList = _unitGroupRepository.Find(group => group.Status != (byte)Status.Deleted, "UnitSets");
			return new ObservableCollection<UnitGroup>(entityList);
		}

		public ObservableCollection<UnitGroup> GetActives()
		{
			IEnumerable<UnitGroup> entityList = _unitGroupRepository.Find(group =>
				(group.Status == (byte)Status.Active),"UnitSets");
			return new ObservableCollection<UnitGroup>(entityList);
		}

		public int AddModel(UnitGroup model)
		{
		    _unitGroupRepository.Add(model);
			Context.Commit();
			if (UnitGroupAdded != null)
				UnitGroupAdded(this, new ModelAddedEventArgs<UnitGroup>(model));
			int id = model.Id;
			return id;
		}

		public void UpdateModel(UnitGroup model)
		{
            model.ModifiedBy = LoginInfo.Id;
            Context.Commit();
		}

        public void DeleteModel(UnitGroup model)
        {
        }

		public void AttachModel(UnitGroup model)
		{
			if (_unitGroupRepository.Exists(unitGroup => unitGroup.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion


        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The UnitSet group id.</param>
        /// <returns></returns>
		public UnitGroup GetModel(int id)
		{
			return _unitGroupRepository.Single(unitSet => unitSet.Id == id);
		}
    }
}