using System.Collections.ObjectModel;
using Soheil.Core.Base;
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

		public WarehouseTransactionDataService()
			: this(new SoheilEdmContext())
		{

		}
		public WarehouseTransactionDataService(SoheilEdmContext context)
		{
			Context = context;
			_repository = new Repository<WarehouseTransaction>(Context);
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
			if (model.Warehouse == null)
			{
				System.Windows.MessageBox.Show("No warehouse is selected.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				return 0;
			}
			Context.Commit();
			return model.Id;
		}

		public void UpdateModel(WarehouseTransaction model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.RecordDateTime = DateTime.Now;

			Context.Commit();
		}

		public void DeleteModel(WarehouseTransaction model)
		{
			if (model.WarehouseReceipt != null)
				new Repository<WarehouseReceipt>(Context).Delete(model.WarehouseReceipt);
			bool flag = model.Warehouse != null;
			model.TaskReport.WarehouseTransactions.Remove(model);
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

		internal WarehouseTransaction CreateTransactionFor(TaskReport model)
		{
			//Model
			var tr = new Repository<TaskReport>(Context).Single(x=>x.Id == model.Id);
			var wt = new WarehouseTransaction
			{
				Code = model.Code,
				ProductRework = new Repository<ProductRework>(Context).Single(x => x.Id == model.Task.Block.StateStation.State.OnProductRework.Id),
				TaskReport = tr,
				Quantity = model.TaskProducedG1,
				TransactionDateTime = model.ReportEndDateTime,
				Flow = 0,
			};
			tr.WarehouseTransactions.Add(wt);
			AddModel(wt);
			return wt;
		}

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
