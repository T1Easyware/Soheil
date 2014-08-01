using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Model;
using Soheil.Dal;

namespace Soheil.Core.DataServices.PM
{
	public class ReportDataService : Base.DataServiceBase, Interfaces.IDataService<Model.MaintenanceReport>
	{
		public ReportDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ReportDataService(SoheilEdmContext context)
		{
			Context = context;
			_reportRepository = new Repository<MaintenanceReport>(context);
		}

		Repository<MaintenanceReport> _reportRepository;

		public Model.MaintenanceReport GetSingle(int id)
		{
			return _reportRepository.FirstOrDefault(x => x.Id == id);
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.MaintenanceReport> GetAll()
		{
			return new System.Collections.ObjectModel.ObservableCollection<MaintenanceReport>(
				_reportRepository.GetAll()
				.OrderByDescending(x=>x.PerformedDate));
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.MaintenanceReport> GetActives()
		{
			return new System.Collections.ObjectModel.ObservableCollection<MaintenanceReport>(
				_reportRepository.GetAll()
				.OrderByDescending(x=>x.PerformedDate));
		}
		public IEnumerable<Model.MaintenanceReport> GetActivesForMachine(Machine machineModel)
		{
			return _reportRepository.Find(x => x.MachinePartMaintenance.MachinePart.Machine.Id == machineModel.Id)
				.OrderByDescending(x => x.PerformedDate);
		}

		public IEnumerable<Model.MaintenanceReport> GetActivesForMachinePart(MachinePart machinePartModel)
		{
			return _reportRepository.Find(x => x.MachinePartMaintenance.MachinePart.Id == machinePartModel.Id)
				.OrderByDescending(x=>x.PerformedDate);
		}
		
		public int AddModel(Model.MaintenanceReport model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			_reportRepository.Add(model);

			Context.Commit();
			return model.Id;
		}

		public void UpdateModel(Model.MaintenanceReport model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			Context.Commit();
		}

		public void DeleteModel(Model.MaintenanceReport model)
		{
			if (_reportRepository.Exists(x => x.Id == model.Id))
				_reportRepository.Delete(model);
			else Context.DeleteObject(model);
			Context.Commit();
		}

		public void AttachModel(Model.MaintenanceReport model)
		{
			if (_reportRepository.Exists(x => x.Id == model.Id))
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
