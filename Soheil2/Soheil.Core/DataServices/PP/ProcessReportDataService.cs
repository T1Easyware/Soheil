using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Core.Interfaces;

namespace Soheil.Core.DataServices
{
	public class ProcessReportDataService : IDataService<ProcessReport>
	{
		public ProcessReport GetSingle(int id)
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<ProcessReport> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<ProcessReport> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public ProcessReport GetSingleFull(int id)
		{
			ProcessReport model = null;
			using (var context = new SoheilEdmContext())
			{
				var repos = new Repository<ProcessReport>(context);
				model = repos.FirstOrDefault(x => x.Id == id,
					"Process",
					"Process.StateStationActivity",
					"StoppageReports",
					"StoppageReports.Cause",
					"StoppageReports.Cause.Parent",
					"StoppageReports.Cause.Parent.Parent",
					"StoppageReports.OperatorStoppageReports",
					"StoppageReports.OperatorStoppageReports.Operator",
					"DefectionReports",
					"DefectionReports.OperatorDefectionReports",
					"DefectionReports.OperatorDefectionReports.Operator",
					"DefectionReports.ProductDefection",
					"DefectionReports.ProductDefection.Defection");
			}
			return model;
		}

		public ProcessReport GetByTaskReportIdAndProcessId(int taskReportId, int processId)
		{
			using (var context = new SoheilEdmContext())
			{
				var model = new Repository<ProcessReport>(context).FirstOrDefault(
					x => x.Process.Id == processId && x.TaskReport.Id == taskReportId);
				return model;
			}
		}

		public void Save(ViewModels.PP.ProcessReportCellVm vm)
		{
			using (var context = new SoheilEdmContext())
			{
				var processReportDs = new Repository<ProcessReport>(context);
				var productDefectionDs = new Repository<ProductDefection>(context);
				var causeDs = new Repository<Cause>(context);
				var defectionReportDs = new Repository<DefectionReport>(context);
				var stoppageReportDs = new Repository<StoppageReport>(context);
				var operatorDs = new Repository<Operator>(context);
				var odrDs = new Repository<OperatorDefectionReport>(context);
				var osrDs = new Repository<OperatorStoppageReport>(context);

				var model = processReportDs.FirstOrDefault(x => x.Id == vm.Id);
				if (model != null)
				{
					//overwrite ProcessReport
					model.ProducedG1 = vm.ProducedG1;
					model.ProcessReportTargetPoint = vm.ProcessReportTargetPoint;
				}
				else
				{
					//add ProcessReport
					model.Process = new Repository<Process>(context).FirstOrDefault(x => x.Id == vm.Parent.ProcessId);
					model.ProducedG1 = vm.ProducedG1;
					model.ProcessReportTargetPoint = vm.ProcessReportTargetPoint;
					model.TaskReport = new Repository<TaskReport>(context).FirstOrDefault(x => x.Id == vm.Parent.Process.Task.Id);
				}

				//delete defectionReports and their children
				var defectionReports = model.DefectionReports.ToArray();
				foreach (var defectionReport in defectionReports)
				{
					var odrs = defectionReport.OperatorDefectionReports.ToArray();
					foreach (var odr in odrs)
					{
						odrDs.Delete(odr);
					}
					defectionReportDs.Delete(defectionReport);
				}

				//add defectionReports and their children
				foreach (var defectionReportVm in vm.DefectionReports.List)
				{
					var defectionReportModel = new Model.DefectionReport();
					defectionReportModel.AffectsTaskReport = defectionReportVm.AffectsTaskReport;
					defectionReportModel.LostCount = defectionReportVm.LostCount;
					defectionReportModel.LostTime = defectionReportVm.LostSeconds;
					defectionReportModel.ProcessReport = model;
					defectionReportModel.ProductDefection = productDefectionDs.FirstOrDefault(x => x.Id == defectionReportVm.ProductDefection.SelectedItem.Id);
					foreach (var guiltyOperVm in defectionReportVm.GuiltyOperators.FilterBoxes)
					{
						var odrModel = new Model.OperatorDefectionReport();
						odrModel.DefectionReport = defectionReportModel;
						odrModel.Operator = operatorDs.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
						defectionReportModel.OperatorDefectionReports.Add(odrModel);
					}
					model.DefectionReports.Add(defectionReportModel);
				}

				//delete stoppageReports and their children
				var stoppageReports = model.StoppageReports.ToArray();
				foreach (var stoppageReport in stoppageReports)
				{
					var osrs = stoppageReport.OperatorStoppageReports.ToArray();
					foreach (var osr in osrs)
					{
						osrDs.Delete(osr);
					}
					stoppageReportDs.Delete(stoppageReport);
				}

				//add stoppageReports and their children
				foreach (var stoppageReportVm in vm.StoppageReports.List)
				{
					var stoppageReportModel = new Model.StoppageReport();
					stoppageReportModel.AffectsTaskReport = stoppageReportVm.AffectsTaskReport;
					stoppageReportModel.LostCount = stoppageReportVm.LostCount;
					stoppageReportModel.LostTime = stoppageReportVm.LostSeconds;
					stoppageReportModel.ProcessReport = model;
					int selid = stoppageReportVm.StoppageLevels.FilterBoxes[2].SelectedItem.Id;
					stoppageReportModel.Cause = causeDs.FirstOrDefault(x => x.Id == selid);
					foreach (var guiltyOperVm in stoppageReportVm.GuiltyOperators.FilterBoxes)
					{
						var odrModel = new Model.OperatorStoppageReport();
						odrModel.StoppageReport = stoppageReportModel;
						odrModel.Operator = operatorDs.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
						stoppageReportModel.OperatorStoppageReports.Add(odrModel);
					}
					model.StoppageReports.Add(stoppageReportModel);
				}
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Resets a processReport to its original form
		/// </summary>
		/// <param name="id"></param>
		/// <param name="newTargetPoint">new automatically calculated targetpoint</param>
		internal void ResetById(int id, int newTargetPoint)
		{
			using (var context = new SoheilEdmContext())
			{
				var processReportRepository = new Repository<ProcessReport>(context);
				var processReportDataService = new ProcessReportDataService();
				var defectionReportRepository = new Repository<DefectionReport>(context);
				var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
				var stoppageReportRepository = new Repository<StoppageReport>(context);
				var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);
				var model = processReportRepository.Single(x => x.Id == id);
				processReportDataService.ClearModel(
					model,
					processReportRepository,
					defectionReportRepository,
					operatorDefectionReportRepository,
					stoppageReportRepository,
					operatorStoppageReportRepository,
					context);
				model.ProcessReportTargetPoint = newTargetPoint;
				model.ProducedG1 = 0;
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Delete all inner objects of a processReport but leave itself intact
		/// </summary>
		/// <param name="processReportModel"></param>
		/// <param name="processReportRepository"></param>
		/// <param name="defectionReportRepository"></param>
		/// <param name="operatorDefectionReportRepository"></param>
		/// <param name="stoppageReportRepository"></param>
		/// <param name="operatorStoppageReportRepository"></param>
		/// <param name="context"></param>
		internal void ClearModel(
			ProcessReport processReportModel, 
			Repository<ProcessReport> processReportRepository, 
			Repository<DefectionReport> defectionReportRepository, 
			Repository<OperatorDefectionReport> operatorDefectionReportRepository, 
			Repository<StoppageReport> stoppageReportRepository, 
			Repository<OperatorStoppageReport> operatorStoppageReportRepository, 
			SoheilEdmContext context)
		{
			var defectionReports = processReportModel.DefectionReports.ToArray();
			foreach (var defectionReportModel in defectionReports)
			{
				defectionReportRepository.Delete(defectionReportModel);
				var operatorDefectionReports = defectionReportModel.OperatorDefectionReports.ToArray();
				foreach (var operatorDefectionReportModel in operatorDefectionReports)
					operatorDefectionReportRepository.Delete(operatorDefectionReportModel);
			}
			var stoppageReports = processReportModel.StoppageReports.ToArray();
			foreach (var stoppageReportModel in stoppageReports)
			{
				stoppageReportRepository.Delete(stoppageReportModel);
				var operatorStoppageReports = stoppageReportModel.OperatorStoppageReports.ToArray();
				foreach (var operatorStoppageReportModel in operatorStoppageReports)
					operatorStoppageReportRepository.Delete(operatorStoppageReportModel);
			}
		}
	}
}
