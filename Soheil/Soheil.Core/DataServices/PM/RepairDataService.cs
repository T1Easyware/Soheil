using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Common;

namespace Soheil.Core.DataServices.PM
{
	public class RepairDataService : Base.DataServiceBase, Interfaces.IDataService<Repair>
	{
		Repository<Repair> _repairRepository;
		public RepairDataService():this(new SoheilEdmContext())
		{
		}
		public RepairDataService(SoheilEdmContext context)
		{
			Context = context;
			_repairRepository = new Repository<Repair>(context);
		}
		public Repair GetSingle(int id)
		{
			return _repairRepository.Single(x => x.Id == id);
		}

		public System.Collections.ObjectModel.ObservableCollection<Repair> GetAll()
		{
			return new System.Collections.ObjectModel.ObservableCollection<Repair>(
				_repairRepository.GetAll()
				.OrderByDescending(x => x.CreatedDate));
		}

		public System.Collections.ObjectModel.ObservableCollection<Repair> GetActives()
		{
			return new System.Collections.ObjectModel.ObservableCollection<Repair>(
				_repairRepository.GetAll()
				.OrderByDescending(x => x.CreatedDate));
		}
		public IEnumerable<Model.Repair> GetActivesForMachine(Machine machineModel)
		{
			return _repairRepository.Find(x => x.MachinePart.Machine.Id == machineModel.Id)
				.OrderByDescending(x => x.CreatedDate);
		}

		public IEnumerable<Model.Repair> GetActivesForMachinePart(MachinePart machinePartModel)
		{
			return _repairRepository.Find(x => x.MachinePart.Id == machinePartModel.Id)
				.OrderByDescending(x => x.CreatedDate);
		}
		public int AddModel(Repair model)
		{
			model.ModifiedBy = LoginInfo.Id;
			_repairRepository.Add(model);

			Context.Commit();
			return model.Id;
		}

		public void UpdateModel(Repair model)
		{
			model.ModifiedBy = LoginInfo.Id;
			Context.Commit();
		}

		public void DeleteModel(Repair model)
		{
			if (_repairRepository.Exists(x => x.Id == model.Id))
				_repairRepository.Delete(model);
			else Context.DeleteObject(model);
			Context.Commit();
		}
		/// <summary>
		/// No commit
		/// </summary>
		/// <param name="model"></param>
		public void Delete(Repair model)
		{
			if (_repairRepository.Exists(x => x.Id == model.Id))
				_repairRepository.Delete(model);
			else Context.DeleteObject(model);
		}

		public void AttachModel(Repair model)
		{
			if (_repairRepository.Exists(x => x.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		internal IEnumerable<Model.Repair> GetAlarms()
		{
			return _repairRepository.Find(x =>
				x.RepairStatus == (byte)RepairStatus.NotDone ||
				x.RepairStatus == (byte)RepairStatus.Reported).OrderBy(x => x.CreatedDate);
		}
	}
}
