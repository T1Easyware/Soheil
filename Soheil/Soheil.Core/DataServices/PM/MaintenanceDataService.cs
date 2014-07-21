using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Common;

namespace Soheil.Core.DataServices.PM
{
	public class MaintenanceDataService : Base.DataServiceBase, Interfaces.IDataService<Model.Maintenance>
	{
		public MaintenanceDataService()
			: this(new Dal.SoheilEdmContext())
		{

		}
		public MaintenanceDataService(Dal.SoheilEdmContext context)
		{
			Context = context;
			_maintenanceRepository = new Repository<Maintenance>(context);
			_maintenanceReportRepository = new Repository<MaintenanceReport>(context);
			_machinePartMaintenanceRepository = new Repository<MachinePartMaintenance>(context);
		}

		Repository<Maintenance> _maintenanceRepository;
		Repository<MaintenanceReport> _maintenanceReportRepository;
		Repository<MachinePartMaintenance> _machinePartMaintenanceRepository;


		public Model.Maintenance GetSingle(int id)
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.Maintenance> GetAll()
		{
			var list = _maintenanceRepository.GetAll().ToList();
			return new System.Collections.ObjectModel.ObservableCollection<Maintenance>(list);
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.Maintenance> GetActives()
		{
			var list = _maintenanceRepository.Find(x=>x.Status == (int)Status.Active).ToList();
			return new System.Collections.ObjectModel.ObservableCollection<Maintenance>(list);
		}

		public int AddModel(Model.Maintenance model)
		{
			_maintenanceRepository.Add(model);
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
			return model.Id;
		}
		public int AddModel(MaintenanceReport model)
		{
			_maintenanceReportRepository.Add(model);
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
			return model.Id;
		}
		public int AddModel(MachinePartMaintenance model)
		{
			_machinePartMaintenanceRepository.Add(model);
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
			return model.Id;
		}


		public void UpdateModel(Model.Maintenance model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
		}

		public void DeleteModel(Model.Maintenance model)
		{
			if (model.MachinePartMaintenances.Any())
			{
				model.RecordStatus = Status.Deleted;
				model.ModifiedDate = DateTime.Now;
			}
			else
			{
				_maintenanceRepository.Delete(model);
			}

			Context.Commit();
		}
		public void DeleteModel(MaintenanceReport model)
		{
			_maintenanceReportRepository.Delete(model);

			Context.Commit();
		}
		public void DeleteModel(MachinePartMaintenance model)
		{
			if (model.MaintenanceReports.Any())
			{
				model.RecordStatus = Status.Deleted;
				model.ModifiedDate = DateTime.Now;
			}
			else
			{
				_machinePartMaintenanceRepository.Delete(model);
			}

			Context.Commit();
		}

		public void AttachModel(Model.Maintenance model)
		{
			if (_maintenanceRepository.Exists(x => x.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

	}
}
