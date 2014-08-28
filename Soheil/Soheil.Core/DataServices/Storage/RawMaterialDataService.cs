using System;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
    public class RawMaterialDataService : DataServiceBase, IDataService<RawMaterial>
    {
		public event EventHandler<ModelAddedEventArgs<RawMaterial>> RawMaterialAdded;
        public event EventHandler<ModelAddedEventArgs<RawMaterialUnitGroup>> UnitGroupAdded;
        public event EventHandler<ModelRemovedEventArgs> UnitGroupRemoved;
        readonly Repository<RawMaterial> _rawMaterialRepository;

		public RawMaterialDataService()
			: this(new SoheilEdmContext())
		{
		}

		public RawMaterialDataService(SoheilEdmContext context)
		{
			Context = context;
			_rawMaterialRepository = new Repository<RawMaterial>(context);
		}



		#region IDataService<RawMaterialVM> Members

        public RawMaterial GetSingle(int id)
        {
			return _rawMaterialRepository.Single(rawMaterial => rawMaterial.Id == id);
        }

		public ObservableCollection<RawMaterial> GetAll()
		{
            var entityList = _rawMaterialRepository.Find(activity => activity.Status != (decimal)Status.Deleted );
            return new ObservableCollection<RawMaterial>(entityList);
		}

		/// <summary>
		/// Gets all active RawMaterial models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<RawMaterial> GetActives()
		{
            var entityList = _rawMaterialRepository.Find(activity => activity.Status == (byte)Status.Active);
            return new ObservableCollection<RawMaterial>(entityList);
		}

		public int AddModel(RawMaterial model)
		{
		    _rawMaterialRepository.Add(model);
			Context.Commit();
			if (RawMaterialAdded != null)
				RawMaterialAdded(this, new ModelAddedEventArgs<RawMaterial>(model));
			int id = model.Id;
			return id;
		}

		public void UpdateModel(RawMaterial model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            Context.Commit();
		}

        public void DeleteModel(RawMaterial model)
        {
        }

		public void AttachModel(RawMaterial model)
		{
			if (_rawMaterialRepository.Exists(rawMaterial => rawMaterial.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        public ObservableCollection<RawMaterialUnitGroup> GetUnitGroups(int rawMaterialId)
        {
            RawMaterial entity = _rawMaterialRepository.FirstOrDefault(rawMaterial => rawMaterial.Id == rawMaterialId, "RawMaterialUnitGroups.UnitGroup", "RawMaterialUnitGroups.RawMaterial");
            return new ObservableCollection<RawMaterialUnitGroup>(entity.RawMaterialUnitGroups.Where(item => item.UnitGroup.Status == (decimal)Status.Active));
        }
        public void AddUnitGroup(int rawMaterialId, int unitGroupId)
        {

            RawMaterial currentRawMaterial = _rawMaterialRepository.Single(rawMaterial => rawMaterial.Id == rawMaterialId);
            if (currentRawMaterial.RawMaterialUnitGroups.Any(rawMaterialUnitGroup =>
                rawMaterialUnitGroup.RawMaterial.Id == rawMaterialId
                && rawMaterialUnitGroup.UnitGroup.Id == unitGroupId))
                return;

            var unitGroupRepository = new Repository<UnitGroup>(Context);
            UnitGroup newUnitGroup = unitGroupRepository.Single(unitGroup => unitGroup.Id == unitGroupId);

            var newRawMaterialUnitGroup = new RawMaterialUnitGroup { UnitGroup = newUnitGroup, RawMaterial = currentRawMaterial };
            currentRawMaterial.RawMaterialUnitGroups.Add(newRawMaterialUnitGroup);
            Context.Commit();
            UnitGroupAdded(this, new ModelAddedEventArgs<RawMaterialUnitGroup>(newRawMaterialUnitGroup));
        }

        public void RemoveUnitGroup(int rawMaterialId, int unitGroupId)
        {
            var rawMaterialUnitGroupRepository = new Repository<RawMaterialUnitGroup>(Context);
            RawMaterial currentRawMaterial = _rawMaterialRepository.Single(rawMaterial => rawMaterial.Id == rawMaterialId);
            RawMaterialUnitGroup currentRawMaterialUnitGroup =
                currentRawMaterial.RawMaterialUnitGroups.First(
                    rawMaterialUnitGroup =>
                    rawMaterialUnitGroup.RawMaterial.Id == rawMaterialId && rawMaterialUnitGroup.Id == unitGroupId);
            int id = currentRawMaterialUnitGroup.Id;
            rawMaterialUnitGroupRepository.Delete(currentRawMaterialUnitGroup);
            Context.Commit();
            UnitGroupRemoved(this, new ModelRemovedEventArgs(id));
        }
        #endregion

		//???don't mind this
		internal RawMaterial GetRawMaterial__(int p)
		{
			return _rawMaterialRepository.Single(x => x.Id == p);
		}
	}
}