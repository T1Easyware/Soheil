using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;
using System;
using System.Collections.Generic;

namespace Soheil.Core.DataServices.Storage
{
	public class WarehouseTransactionDataService : DataServiceBase, IDataService<WarehouseTransaction>
    {
        private readonly Repository<WarehouseTransaction> _repository;
        private readonly Repository<RawMaterial> _materialRepository;
        public event EventHandler<ModelAddedEventArgs<WarehouseTransaction>> TransactionAdded;

		public WarehouseTransactionDataService()
			: this(new SoheilEdmContext())
		{

		}
		public WarehouseTransactionDataService(SoheilEdmContext context)
		{
			Context = context;
            _repository = new Repository<WarehouseTransaction>(Context);
            _materialRepository = new Repository<RawMaterial>(Context);
		}

		#region IDataService
		public WarehouseTransaction GetSingle(int id)
		{
			return _repository.Single(x=>x.Id == id);
		}

		public ObservableCollection<WarehouseTransaction> GetAll()
		{
			return new ObservableCollection<WarehouseTransaction>(_repository.GetAll());
		}

		public ObservableCollection<WarehouseTransaction> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(WarehouseTransaction model)
		{
			_repository.Add(model);
			model.ModifiedBy = LoginInfo.Id;
			model.RecordDateTime = DateTime.Now;
			if (model.DestWarehouse == null&&model.SrcWarehouse==null)
			{
				System.Windows.MessageBox.Show("No warehouse is selected.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				return 0;
			}
		    CalculateInventory(model);
			Context.Commit();
            if (TransactionAdded != null)
                TransactionAdded(this, new ModelAddedEventArgs<WarehouseTransaction>(model));
			return model.Id;
		}
		//???
	    public int AddModel(WarehouseTransaction model, bool warehouseCheck)
        {
            _repository.Add(model);
            model.ModifiedBy = LoginInfo.Id;
            model.RecordDateTime = DateTime.Now;
            if (warehouseCheck && model.DestWarehouse == null)
            {
                System.Windows.MessageBox.Show("No warehouse is selected.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return 0;
            }
            CalculateInventory(model);
            Context.Commit();
            if (TransactionAdded != null)
                TransactionAdded(this, new ModelAddedEventArgs<WarehouseTransaction>(model));
            return model.Id;
        }

		public void UpdateModel(WarehouseTransaction model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.RecordDateTime = DateTime.Now;
            CalculateInventory(model);

			Context.Commit();
		}

		public void DeleteModel(WarehouseTransaction model)
		{
			bool flag = (model.DestWarehouse != null || model.SrcWarehouse != null);

			if (model.WarehouseReceipt != null)
				new Repository<WarehouseReceipt>(Context).Delete(model.WarehouseReceipt);
			if(model.TaskReport!=null) 
				model.TaskReport.WarehouseTransactions.Remove(model);
			if (model.ProductRework != null)
			{
				if(model.Flow == 0)//was stored
					model.ProductRework.Inventory -= (int)model.Quantity;
				else if(model.Flow == 1)//was brought out
					model.ProductRework.Inventory += (int)model.Quantity;
			}
			if (model.RawMaterial != null)
			{
				if (model.Flow == 0)//was stored
					model.RawMaterial.Inventory -= (int)model.Quantity;
				else if (model.Flow == 1)//was brought out
					model.RawMaterial.Inventory += (int)model.Quantity;
			}
			
			_repository.Delete(model);

			if (flag)
				Context.Commit();
		}

		public void AttachModel(WarehouseTransaction model)
		{
			if (_repository.Exists(x => x.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		#endregion

	    private void CalculateInventory(WarehouseTransaction model)
	    {
	        int sign = (WarehouseTransactionFlow) model.Flow == WarehouseTransactionFlow.In ? 1 : -1;
	        var convRepository = new Repository<UnitConversion>(Context);
	        double factor = 1;

            var prevContext = new SoheilEdmContext();
	        var prevRepository = new Repository<WarehouseTransaction>(prevContext);
	        var prevModel = prevRepository.Single(t => t.Id == model.Id);
	        if (prevModel == null)
	            return;
            if (model.UnitSet.Id == model.RawMaterial.BaseUnit.Id)
                return;

            double prevQuantity = prevModel.Quantity;
	        double reletiveQuantity = model.Quantity - prevQuantity;

	        switch ((WarehouseTransactionType) model.Type)
	        {
	            case WarehouseTransactionType.None:
	                break;
	            case WarehouseTransactionType.RawMaterial:
	                var query = convRepository.Find(c => c.Status != (decimal) Status.Deleted)
	                    .FirstOrDefault(
	                        c => c.MajorUnit.Id == model.UnitSet.Id && c.MinorUnit.Id == model.RawMaterial.BaseUnit.Id);
	                if (query == null)
	                {
	                    query = convRepository.Find(c => c.Status != (decimal) Status.Deleted)
	                        .FirstOrDefault(
	                            c => c.MinorUnit.Id == model.UnitSet.Id && c.MajorUnit.Id == model.RawMaterial.BaseUnit.Id);
                        factor = 1 / query.Factor;
	                }
	                else
	                {
	                    factor = query.Factor;
	                }
                    model.RawMaterial.Inventory += reletiveQuantity * factor * sign;
	                break;
	            case WarehouseTransactionType.Product:
	                break;
	            case WarehouseTransactionType.Good:
	                break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }

		//internal WarehouseTransaction CreateTransactionFor(TaskReport model)
		//{
		//	//Model
		//	var tr = new Repository<TaskReport>(Context).Single(x=>x.Id == model.Id);
		//	var wt = new WarehouseTransaction
		//	{
		//		Code = model.Code,
		//		ProductRework = new Repository<ProductRework>(Context).Single(x => x.Id == model.Task.Block.StateStation.State.OnProductRework.Id),
		//		TaskReport = tr,
		//		Quantity = model.TaskProducedG1,
		//		TransactionDateTime = model.ReportEndDateTime,
		//		Flow = 0,
		//	};
		//	tr.WarehouseTransactions.Add(wt);
		//	AddModel(wt);
		//	return wt;
		//}

	    public ObservableCollection<WarehouseTransaction> GetActives(int receiptId)
	    {
            IEnumerable<WarehouseTransaction> entityList = _repository.Find(transaction =>
                transaction.WarehouseReceipt != null && transaction.WarehouseReceipt.Id == receiptId);
            return new ObservableCollection<WarehouseTransaction>(entityList);
	    }

		internal string Save()
		{
			try
			{
				Context.Commit();
			}
			catch(Exception ex)
			{
				return ex.Message;
			}
			return null;
		}
	}
}
