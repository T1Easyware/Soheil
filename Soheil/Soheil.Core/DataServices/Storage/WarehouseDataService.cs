using System;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;
using System.Collections.Generic;

namespace Soheil.Core.DataServices
{
    public class WarehouseDataService : DataServiceBase, IDataService<Warehouse>
    {
		public event EventHandler<ModelAddedEventArgs<Warehouse>> WarehouseAdded;
        readonly Repository<Warehouse> _warehouseRepository;

		public WarehouseDataService()
			: this(new SoheilEdmContext())
		{
		}

		public WarehouseDataService(SoheilEdmContext context)
		{
			Context = context;
			_warehouseRepository = new Repository<Warehouse>(context);
		}



		#region IDataService<WarehouseVM> Members

        public Warehouse GetSingle(int id)
        {
			return _warehouseRepository.Single(warehouse => warehouse.Id == id);
        }

		public ObservableCollection<Warehouse> GetAll()
		{
            var entityList = _warehouseRepository.Find(activity => activity.Status != (decimal)Status.Deleted );
            return new ObservableCollection<Warehouse>(entityList);
		}

		/// <summary>
		/// Gets all active Warehouse models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Warehouse> GetActives()
		{
            var entityList = _warehouseRepository.Find(activity => activity.Status == (byte)Status.Active);
            return new ObservableCollection<Warehouse>(entityList);
		}

		public int AddModel(Warehouse model)
		{
		    _warehouseRepository.Add(model);
			Context.Commit();
			if (WarehouseAdded != null)
				WarehouseAdded(this, new ModelAddedEventArgs<Warehouse>(model));
			int id = model.Id;
			return id;
		}

		public void UpdateModel(Warehouse model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            Context.Commit();
		}

        public void DeleteModel(Warehouse model)
        {
        }

		public void AttachModel(Warehouse model)
		{
			if (_warehouseRepository.Exists(warehouse => warehouse.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion


		internal IEnumerable<Warehouse> GetActivesForPP()
		{
			return _warehouseRepository.Find(x => 
				x.Status == (byte)Common.Status.Active
				&& (x.HasWIP || x.HasFinalProduct));
		}

		internal IEnumerable<Warehouse> GetActivesForMaterials()
		{
			return _warehouseRepository.Find(x => x.Status == (byte)Common.Status.Active && x.HasRawMaterial);
		}
	}
}