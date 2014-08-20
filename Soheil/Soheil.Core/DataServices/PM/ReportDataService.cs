using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Common;

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
				.OrderByDescending(x => x.PerformedDate));
		}
		public Core.Reports.PMReportData GetAllReportsInRange(DateTime start, DateTime end)
		{
			var repairRepository = new Repository<Repair>(Context);
			var data = new Core.Reports.PMReportData
			{
				PMList = _reportRepository
					.Find(x => (x.MaintenanceDate >= start && x.MaintenanceDate <= end) || (x.PerformedDate >= start && x.PerformedDate <= end))
					.OrderBy(x => x.MaintenanceDate)
					.Select(x => new Core.Reports.PMReportData.PM
					{
						Machine = x.MachinePartMaintenance.MachinePart.Machine.Name,
						Part = x.MachinePartMaintenance.MachinePart.IsMachine ? "---" : x.MachinePartMaintenance.MachinePart.Part.Name,
						Maintenance = x.MachinePartMaintenance.Maintenance.Name,
						MaintenanceDate = x.MaintenanceDate,
						PerformedDate = x.PerformedDate,
						LastMaintenanceDate = x.MachinePartMaintenance.LastMaintenanceDate,
						Period = x.MachinePartMaintenance.IsOnDemand ? "---" : x.MachinePartMaintenance.PeriodDays.ToString(),
						Delay = (int)x.MachinePartMaintenance.CalculatedDiffDays,
						IsPerformed = x.PerformedDate.HasValue,
						Description = x.Description,
					}),
				RepairList = repairRepository
					.Find(x => (x.CreatedDate >= start && x.CreatedDate <= end) || (x.AcquiredDate >= start && x.AcquiredDate <= end) || (x.DeliveredDate >= start && x.DeliveredDate <= end))
					.OrderBy(x => x.CreatedDate)
					.Select(x => new Core.Reports.PMReportData.Repair
					{
						Machine = x.MachinePart.Machine.Name,
						Part = x.MachinePart.IsMachine ? "---" : x.MachinePart.Part.Name,
						CreatedDate = x.CreatedDate,
						AcquiredDate = x.AcquiredDate,
						DeliveredDate = x.DeliveredDate,
						RepairStatus = x.RepairStatus,
						Description = x.Description,
					}),
			};
			return data;
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
