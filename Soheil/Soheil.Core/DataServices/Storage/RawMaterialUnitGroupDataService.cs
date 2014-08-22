using System;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class RawMaterialUnitGroupDataService :DataServiceBase, IDataService<RawMaterialUnitGroup>
    {
        public event EventHandler<ModelAddedEventArgs<RawMaterialUnitGroup>> ModelUpdated;
        private readonly Repository<RawMaterialUnitGroup> _rawMaterialUnitGroupRepository;

        public RawMaterialUnitGroupDataService(SoheilEdmContext context)
        {
            Context = context;
            _rawMaterialUnitGroupRepository = new Repository<RawMaterialUnitGroup>(context);
        }
        public RawMaterialUnitGroupDataService()
        {
            Context = new SoheilEdmContext();
            _rawMaterialUnitGroupRepository = new Repository<RawMaterialUnitGroup>(Context);
        }
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public RawMaterialUnitGroup GetSingle(int id)
        {
               return _rawMaterialUnitGroupRepository.Single(rawMaterialUnitGroup => rawMaterialUnitGroup.Id == id);
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<RawMaterialUnitGroup> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<RawMaterialUnitGroup> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(RawMaterialUnitGroup model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(RawMaterialUnitGroup model)
        {
            Context.Commit();
            if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<RawMaterialUnitGroup>(model));
        }

        public void DeleteModel(RawMaterialUnitGroup model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(RawMaterialUnitGroup model)
        {
            throw new System.NotImplementedException();
        }

		public RawMaterialUnitGroup[] GetActivesForRawMaterial(int rawMaterialId)
		{
				return _rawMaterialUnitGroupRepository.Find(
					x => x.RawMaterial.Id == rawMaterialId && x.UnitGroup.Status == (byte)Status.Active,
					"UnitGroup").ToArray();
		}
	}
}