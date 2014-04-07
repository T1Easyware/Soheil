using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Core.Interfaces;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class ProcessReportDataService : DataServiceBase, IDataService<ProcessReport>
	{
		Repository<ProcessReport> _processReportRepository;
		public ProcessReportDataService()
			:this(new SoheilEdmContext())
		{
		}
		public ProcessReportDataService(SoheilEdmContext context)
		{
			this.context = context;
			_processReportRepository = new Repository<ProcessReport>(context);
		}

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
			if (id < 1) return null;
			return _processReportRepository.FirstOrDefault(x => x.Id == id,
					"Process",
					"Process.StateStationActivity",
					"ProcessOperatorReports",
					"ProcessOperatorReports.Operator",
					"ProcessOperatorReports.ProcessOperator",
					"ProcessOperatorReports.ProcessOperator.Operator",
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
		/// <summary>
		/// Adds missing ProcessOperatorReports to the ProcessReport's ProcessOperatorReport collection
		/// </summary>
		/// <param name="model"></param>
		public void CorrectOperatorReports(ProcessReport model)
		{
			foreach (var processOperator in model.Process.ProcessOperators)
			{
				if (!model.ProcessOperatorReports.Any(x => x.ProcessOperator.Id == processOperator.Id))
				{
					processOperator.ProcessOperatorReports.Add(new ProcessOperatorReport
					{
						ProcessReport = model,
						ProcessOperator = processOperator,
						OperatorProducedG1 = 0,
						ModifiedBy = LoginInfo.Id,
					});
				}
			}
		}

		public ProcessReport GetByTaskReportIdAndProcessId(int taskReportId, int processId)
		{
				return _processReportRepository.FirstOrDefault(
					x => x.Process.Id == processId && x.TaskReport.Id == taskReportId);
		}

		internal IEnumerable<ProcessReport> GetProcessReports(int taskId)
		{
			return _processReportRepository.Find(x => x.TaskReport.Task.Id == taskId);
		}

		public void Save(ViewModels.PP.Report.ProcessReportCellVm vm)
		{
			var productDefectionRepository = new Repository<ProductDefection>(context);
			var causeRepository = new Repository<Cause>(context);
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorRepository = new Repository<Operator>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);

			var model = _processReportRepository.FirstOrDefault(x => x.Id == vm.Id);
			if (model != null)
			{
				//overwrite ProcessReport
				model.ProducedG1 = vm.ProducedG1;
				model.ProcessReportTargetPoint = vm.ProcessReportTargetPoint;
			}
			else
			{
				//add ProcessReport
				model.Process = new Repository<Process>(context).FirstOrDefault(x => x.Id == vm.ProcessId);
				model.ProducedG1 = vm.ProducedG1;
				model.ProcessReportTargetPoint = vm.ProcessReportTargetPoint;
				model.TaskReport = new Repository<TaskReport>(context).FirstOrDefault(x => x.Id == vm.ParentColumn.Task.Id);
			}

			//delete defectionReports and their children
			var defectionReports = model.DefectionReports.ToArray();
			foreach (var defectionReport in defectionReports)
			{
				var odrs = defectionReport.OperatorDefectionReports.ToArray();
				foreach (var odr in odrs)
				{
					operatorDefectionReportRepository.Delete(odr);
				}
				defectionReportRepository.Delete(defectionReport);
			}

			//add defectionReports and their children
			foreach (var defectionReportVm in vm.DefectionReports.List)
			{
				var defectionReportModel = new Model.DefectionReport();
				defectionReportModel.AffectsTaskReport = defectionReportVm.AffectsTaskReport;
				defectionReportModel.LostCount = defectionReportVm.LostCount;
				defectionReportModel.LostTime = defectionReportVm.LostSeconds;
				defectionReportModel.ProcessReport = model;
				defectionReportModel.ProductDefection = productDefectionRepository.FirstOrDefault(x => x.Id == defectionReportVm.ProductDefection.SelectedItem.Id);
				foreach (var guiltyOperVm in defectionReportVm.GuiltyOperators.FilterBoxes)
				{
					var operatorDefectionReportModel = new Model.OperatorDefectionReport();
					operatorDefectionReportModel.DefectionReport = defectionReportModel;
					operatorDefectionReportModel.Operator = operatorRepository.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
					defectionReportModel.OperatorDefectionReports.Add(operatorDefectionReportModel);
				}
				model.DefectionReports.Add(defectionReportModel);
			}

			//delete stoppageReports and their children
			var stoppageReports = model.StoppageReports.ToArray();
			foreach (var stoppageReport in stoppageReports)
			{
				var operatorStoppageReports = stoppageReport.OperatorStoppageReports.ToArray();
				foreach (var operatorStoppageReport in operatorStoppageReports)
				{
					operatorStoppageReportRepository.Delete(operatorStoppageReport);
				}
				stoppageReportRepository.Delete(stoppageReport);
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
				stoppageReportModel.Cause = causeRepository.FirstOrDefault(x => x.Id == selid);
				foreach (var guiltyOperVm in stoppageReportVm.GuiltyOperators.FilterBoxes)
				{
					var operatorStoppageReportModel = new Model.OperatorStoppageReport();
					operatorStoppageReportModel.StoppageReport = stoppageReportModel;
					operatorStoppageReportModel.Operator = operatorRepository.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
					stoppageReportModel.OperatorStoppageReports.Add(operatorStoppageReportModel);
				}
				model.StoppageReports.Add(stoppageReportModel);
			}

			//correct task report produced G1 value
			var defections = model.TaskReport.ProcessReports.Sum(x =>
				x.DefectionReports.Where(d =>
					d.AffectsTaskReport)
				.Sum(d =>
					d.CountEquivalence));
			var stoppages = model.TaskReport.ProcessReports.Sum(x =>
				x.StoppageReports.Where(s =>
					s.AffectsTaskReport)
				.Sum(s =>
					s.CountEquivalence));
			var g1 = model.TaskReport.TaskReportTargetPoint - (int)(stoppages + defections);
			var vmvm = vm.ParentColumn as ViewModels.PP.Report.TaskReportVm;
			if (vmvm != null) vmvm.ProducedG1 = g1;
			model.TaskReport.TaskProducedG1 = g1;

			context.Commit();
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
				var processReportDataService = new ProcessReportDataService(context);
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
