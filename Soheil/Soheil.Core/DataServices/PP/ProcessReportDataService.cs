﻿using System;
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
			var processReportRepository = new Repository<ProcessReport>(context);
			var processReportDataService = new ProcessReportDataService(context);
			var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);
			processReportDataService.ClearModel(
				model,
				processReportRepository,
				operatorProcessReportRepository,
				defectionReportRepository,
				operatorDefectionReportRepository,
				stoppageReportRepository,
				operatorStoppageReportRepository,
				context);

			model.Process.ProcessReports.Remove(model);
			_processReportRepository.Delete(model);
			context.Commit();
		}

		public void AttachModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public ProcessReport GetSingleFull(int id)
		{
			if (id < 1) return null;
			return _processReportRepository.FirstOrDefault(x => x.Id == id,
					"Process.StateStationActivity",
					"OperatorProcessReports.ProcessOperator.Operator",
					"StoppageReports",
					"StoppageReports.Cause.Parent.Parent",
					"StoppageReports.OperatorStoppageReports.Operator",
					"DefectionReports.OperatorDefectionReports.Operator",
					"DefectionReports.ProductDefection.Defection");
		}
		/// <summary>
		/// Adds missing OperatorProcessReports to the ProcessReport's OperatorProcessReport collection
		/// </summary>
		/// <param name="model"></param>
		public void CorrectOperatorReports(ProcessReport model)
		{
			foreach (var processOperator in model.Process.ProcessOperators)
			{
				if (!model.OperatorProcessReports.Any(x => x.ProcessOperator.Id == processOperator.Id))
				{
					processOperator.OperatorProcessReports.Add(new OperatorProcessReport
					{
						ProcessReport = model,
						ProcessOperator = processOperator,
						OperatorProducedG1 = 0,
						ModifiedBy = LoginInfo.Id,
					});
				}
			}
		}

		public void Save(ViewModels.PP.Report.ProcessReportVm vm)
		{
			var productDefectionRepository = new Repository<ProductDefection>(context);
			var causeRepository = new Repository<Cause>(context);
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorRepository = new Repository<Operator>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);


			vm.Model.ProducedG1 = vm.ProducedG1;
			vm.Model.ProcessReportTargetPoint = vm.TargetPoint;

			//delete defectionReports and their children
			var defectionReports = vm.Model.DefectionReports.ToArray();
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
				defectionReportModel.ProcessReport = vm.Model;
				defectionReportModel.ProductDefection = productDefectionRepository.FirstOrDefault(x => x.Id == defectionReportVm.ProductDefection.SelectedItem.Id);
				foreach (var guiltyOperVm in defectionReportVm.GuiltyOperators.FilterBoxes)
				{
					var operatorDefectionReportModel = new Model.OperatorDefectionReport();
					operatorDefectionReportModel.DefectionReport = defectionReportModel;
					operatorDefectionReportModel.Operator = operatorRepository.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
					defectionReportModel.OperatorDefectionReports.Add(operatorDefectionReportModel);
				}
				vm.Model.DefectionReports.Add(defectionReportModel);
			}

			//delete stoppageReports and their children
			var stoppageReports = vm.Model.StoppageReports.ToArray();
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
				stoppageReportModel.ProcessReport = vm.Model;
				int selid = stoppageReportVm.StoppageLevels.FilterBoxes[2].SelectedItem.Id;
				stoppageReportModel.Cause = causeRepository.FirstOrDefault(x => x.Id == selid);
				foreach (var guiltyOperVm in stoppageReportVm.GuiltyOperators.FilterBoxes)
				{
					var operatorStoppageReportModel = new Model.OperatorStoppageReport();
					operatorStoppageReportModel.StoppageReport = stoppageReportModel;
					operatorStoppageReportModel.Operator = operatorRepository.FirstOrDefault(x => x.Id == guiltyOperVm.SelectedItem.Id);
					stoppageReportModel.OperatorStoppageReports.Add(operatorStoppageReportModel);
				}
				vm.Model.StoppageReports.Add(stoppageReportModel);
			}

			context.Commit();
		}

		/// <summary>
		/// Resets a processReport to its original form
		/// </summary>
		/// <param name="id"></param>
		/// <param name="newTargetPoint">new automatically calculated targetpoint</param>
		internal void ResetById(int id, int newTargetPoint)
		{
			var processReportRepository = new Repository<ProcessReport>(context);
			var processReportDataService = new ProcessReportDataService(context);
			var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
			var defectionReportRepository = new Repository<DefectionReport>(context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(context);
			var stoppageReportRepository = new Repository<StoppageReport>(context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(context);
			var model = processReportRepository.Single(x => x.Id == id);
			processReportDataService.ClearModel(
				model,
				processReportRepository,
				operatorProcessReportRepository,
				defectionReportRepository,
				operatorDefectionReportRepository,
				stoppageReportRepository,
				operatorStoppageReportRepository,
				context);
			model.ProcessReportTargetPoint = newTargetPoint;
			model.ProducedG1 = 0;
			context.Commit();
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
			Repository<OperatorProcessReport> operatorProcessReportRepository, 
			Repository<DefectionReport> defectionReportRepository, 
			Repository<OperatorDefectionReport> operatorDefectionReportRepository, 
			Repository<StoppageReport> stoppageReportRepository, 
			Repository<OperatorStoppageReport> operatorStoppageReportRepository, 
			SoheilEdmContext context)
		{
			var operatorReports = processReportModel.OperatorProcessReports.ToArray();
			foreach (var operatorReport in operatorReports)
			{
				operatorProcessReportRepository.Delete(operatorReport);
			}
			var defectionReports = processReportModel.DefectionReports.ToArray();
			foreach (var defectionReportModel in defectionReports)
			{
				var operatorDefectionReports = defectionReportModel.OperatorDefectionReports.ToArray();
				foreach (var operatorDefectionReportModel in operatorDefectionReports)
					operatorDefectionReportRepository.Delete(operatorDefectionReportModel);
				defectionReportRepository.Delete(defectionReportModel);
			}
			var stoppageReports = processReportModel.StoppageReports.ToArray();
			foreach (var stoppageReportModel in stoppageReports)
			{
				var operatorStoppageReports = stoppageReportModel.OperatorStoppageReports.ToArray();
				foreach (var operatorStoppageReportModel in operatorStoppageReports)
					operatorStoppageReportRepository.Delete(operatorStoppageReportModel);
				stoppageReportRepository.Delete(stoppageReportModel);
			}
		}


	}
}
