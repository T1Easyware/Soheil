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
    public class WarehouseReceiptDataService : DataServiceBase, IDataService<WarehouseReceipt>
    {
		public event EventHandler<ModelAddedEventArgs<WarehouseReceipt>> WarehouseReceiptAdded;
        readonly Repository<WarehouseReceipt> _warehouseReceiptRepository;

		public WarehouseReceiptDataService()
			: this(new SoheilEdmContext())
		{
		}

		public WarehouseReceiptDataService(SoheilEdmContext context)
		{
			Context = context;
			_warehouseReceiptRepository = new Repository<WarehouseReceipt>(context);
		}



		#region IDataService<WarehouseReceiptVM> Members

        public WarehouseReceipt GetSingle(int id)
        {
			return _warehouseReceiptRepository.Single(warehouseReceipt => warehouseReceipt.Id == id);
        }

		public ObservableCollection<WarehouseReceipt> GetAll()
		{
            var entityList = _warehouseReceiptRepository.Find(receipt => receipt.Status != (decimal)Status.Deleted );
            return new ObservableCollection<WarehouseReceipt>(entityList);
		}

        public ObservableCollection<WarehouseReceipt> GetAll(WarehouseReceiptType type)
        {
            var entityList = _warehouseReceiptRepository.Find(receipt => receipt.Status != (decimal)Status.Deleted && receipt.Type == (decimal) type);
            return new ObservableCollection<WarehouseReceipt>(entityList);
        }

		/// <summary>
		/// Gets all active WarehouseReceipt models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<WarehouseReceipt> GetActives()
		{
            var entityList = _warehouseReceiptRepository.Find(receipt => receipt.Status == (byte)Status.Active);
            return new ObservableCollection<WarehouseReceipt>(entityList);
		}

		public int AddModel(WarehouseReceipt model)
		{
		    _warehouseReceiptRepository.Add(model);
			Context.Commit();
			if (WarehouseReceiptAdded != null)
				WarehouseReceiptAdded(this, new ModelAddedEventArgs<WarehouseReceipt>(model));
			int id = model.Id;
			return id;
		}

		public void UpdateModel(WarehouseReceipt model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            Context.Commit();
		}

        public void DeleteModel(WarehouseReceipt model)
        {
        }

		public void AttachModel(WarehouseReceipt model)
		{
			if (_warehouseReceiptRepository.Exists(warehouseReceipt => warehouseReceipt.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}
        #endregion
        
	}
}