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
			_processReportRepository.Add(model);
			return 0;
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

		public void Save(ProcessReport model)
		{
			//correct Defections
			var drRepo = new Repository<DefectionReport>(context);
			var odrRepo = new Repository<OperatorDefectionReport>(context);
			foreach (var dr in model.DefectionReports.ToArray())
			{
				foreach (var odr in dr.OperatorDefectionReports.ToArray())
				{
					if(odr.Operator == null)
					{
						dr.OperatorDefectionReports.Remove(odr);
						odrRepo.Delete(odr);
					}
				}
				if (dr.ProductDefection == null)
				{
					model.DefectionReports.Remove(dr);
					drRepo.Delete(dr);
				}
			}

			//correct Stoppages
			var srRepo = new Repository<StoppageReport>(context);
			var osrRepo = new Repository<OperatorStoppageReport>(context);
			foreach (var sr in model.StoppageReports.ToArray())
			{
				foreach (var osr in sr.OperatorStoppageReports.ToArray())
				{
					if (osr.Operator == null)
					{
						sr.OperatorStoppageReports.Remove(osr);
						osrRepo.Delete(osr);
					}
				}
				if (sr.Cause == null)
				{
					model.StoppageReports.Remove(sr);
					srRepo.Delete(sr);
				}
			}

			//correct G1
			if (model.OperatorProcessReports.Any())
				model.ProducedG1 = model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);
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
