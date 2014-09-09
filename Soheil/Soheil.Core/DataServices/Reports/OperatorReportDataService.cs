using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Soheil.Common;
using Soheil.Core.Reports;
using Soheil.Core.ViewModels.Reports;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class OperatorReportDataService
    {
		public IList<Record> GetAll(DateTimeIntervals intervalType, OperatorBarInfo barInfo)
		{
			switch (barInfo.Level)
			{
				case 0:
					return GetOperatorsEfficiency(barInfo);
				//case 1:
				//return GetOperatorProcessReport(barInfo);
			}
			return new List<Record>();
		}

        public int GetOperatorsCount()
        {
            int count;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Operator>(context);
                count = repository.GetAll().Count();
            }
            return count;
        }
		private IList<Record> GetOperatorsEfficiency(OperatorBarInfo oprInfo)
		{
			IList<Record> records = new List<Record>();


			using (var context = new SoheilEdmContext())
			{
				var workProfilePlanDs = new DataServices.WorkProfilePlanDataService(context);
				var start = workProfilePlanDs.GetShiftStartOn(oprInfo.StartDate);
				var end = workProfilePlanDs.GetShiftStartOn(oprInfo.EndDate);
				
				var operatorRepository = new Repository<Operator>(context);
				var oList = operatorRepository.Find(x => x.Status == (byte)Status.Active).ToArray();

				var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
				var oprList = operatorProcessReportRepository.Find(x => x != null
					&& x.ProcessOperator != null && x.ProcessOperator.Operator != null
					&& x.ProcessReport != null && x.ProcessReport.StartDateTime>=start && x.ProcessReport.EndDateTime <=end
					&& x.ProcessReport.Process != null && x.ProcessReport.Process.StateStationActivity != null).ToArray();

				var osrRepository = new Repository<OperatorStoppageReport>(context);
				var osrList = osrRepository.Find(x => x != null
					&& x.StoppageReport != null && x.StoppageReport.ProcessReport != null
					&& x.StoppageReport.ProcessReport.StartDateTime>=start && x.StoppageReport.ProcessReport.EndDateTime <=end
					&& x.Operator != null).ToArray();

				var odrRepository = new Repository<OperatorDefectionReport>(context);
				var odrList = odrRepository.Find(x => x != null
					&& x.DefectionReport != null && x.DefectionReport.ProcessReport != null
					&& x.DefectionReport.ProcessReport.StartDateTime>=start && x.DefectionReport.ProcessReport.EndDateTime <=end
					&& x.Operator != null).ToArray();


				var oprQuery = from opr in oprList
							   group opr by opr.ProcessOperator.Operator.Id into og
							   select new
							   {
								   operatorId = og.Key,
								   productionCount = og.Any() ? og.Sum(x => x.OperatorProducedG1) : 0,
								   productionTime = og.Any() ? og.Sum(x => x.OperatorProducedG1 * x.ProcessReport.Process.StateStationActivity.CycleTime) : 0,
								   targetCount = og.Any() ? og.Sum(x => x.ProcessReport.ProcessReportTargetPoint / x.ProcessReport.OperatorProcessReports.Count) : 0,
								   targetTime = og.Any() ? og.Sum(x => x.ProcessReport.ProcessReportTargetPoint * x.ProcessReport.Process.StateStationActivity.CycleTime / x.ProcessReport.OperatorProcessReports.Count) : 0,
							   };

				var osrQuery = from osr in osrList
							   where osr.StoppageReport.ProcessReport.Process.ProcessOperators.Any(o => o.Operator.Id == osr.Operator.Id)
							   group osr by osr.Operator.Id into og
							   select new
							   {
								   operatorId = og.Key,
								   stoppageTime = og.Any() ? og.Sum(x => x.StoppageReport.TimeEquivalence / x.StoppageReport.OperatorStoppageReports.Count) : 0,
								   stoppageCount = og.Any() ? og.Sum(x => x.StoppageReport.CountEquivalence / x.StoppageReport.OperatorStoppageReports.Count) : 0,
							   };

				var odrQuery = from odr in odrList
							   where odr.DefectionReport.ProcessReport.Process.ProcessOperators.Any(o => o.Operator.Id == odr.Operator.Id)
							   group odr by odr.Operator.Id into og
							   select new
							   {
								   operatorId = og.Key,
								   defectionTime = og.Any() ? og.Sum(x => x.DefectionReport.TimeEquivalence / x.DefectionReport.OperatorDefectionReports.Count) : 0,
								   defectionCount = og.Any() ? og.Sum(x => x.DefectionReport.CountEquivalence / x.DefectionReport.OperatorDefectionReports.Count) : 0,
							   };

				var oQuery = from o in oList
							 join opr in oprQuery on o.Id equals opr.operatorId into oprg 
							 join osr in osrQuery on o.Id equals osr.operatorId into osrg 
							 join odr in odrQuery on o.Id equals odr.operatorId into odrg
							 from oprgi in oprg.DefaultIfEmpty()
							 from osrgi in osrg.DefaultIfEmpty()
							 from odrgi in odrg.DefaultIfEmpty()
							 select new
							 {
								 o.Id,
								 o.Name,
								 targetTime = oprgi == null ? 0f : oprgi.targetTime,
								 targetCount = oprgi == null ? 0 : oprgi.targetCount,
								 productionTime = oprgi == null ? 0f : oprgi.productionTime,
								 productionCount = oprgi == null ? 0 : oprgi.productionCount,
								 stoppageTime = osrgi == null ? 0f : osrgi.stoppageTime,
								 stoppageCount = osrgi == null ? 0 : (int)osrgi.stoppageCount,
								 defectionTime = odrgi == null ? 0f : odrgi.defectionTime,
								 defectionCount = odrgi == null ? 0 : (int)odrgi.defectionCount,
							 };

				foreach (var line in oQuery)
				{
					var newRecord = new Record();
					newRecord.Data = new List<object>(8) { 0f, 0f, 0f, 0f, 0, 0, 0, 0 };
					newRecord.Id = line.Id;
					newRecord.Data = new List<object>
                        {
                            line.targetTime,
                            line.productionTime,
                            line.defectionTime,
                            line.stoppageTime,
                            line.targetCount,
                            line.productionCount,
                            line.defectionCount,
                            line.stoppageCount
                        };

					newRecord.StartDate = start;
					newRecord.EndDate = end;
					newRecord.Header = line.Name;
					records.Add(newRecord);
				}
			}
			return records;
		}


        public OperatorProcessReportVm GetOperatorProcessReport(int operatorId, DateTime startDate, DateTime endDate)
        {
            //IList<Record> records = new List<Record>();
            var result = new OperatorProcessReportVm();

            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<Operator>(context);
                var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var processRepository = new Repository<Process>(context);
                var ssaRepository = new Repository<StateStationActivity>(context);
                var ssRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);
                var productDefectionRepository = new Repository<ProductDefection>(context);
                var productReworkRepository = new Repository<ProductRework>(context);
                var activityRepository = new Repository<Activity>(context);
                var srRepository = new Repository<StoppageReport>(context);
                var osrRepository = new Repository<OperatorStoppageReport>(context);
                var drRepository = new Repository<DefectionReport>(context);
                var odrRepository = new Repository<OperatorDefectionReport>(context);

				var workProfilePlanDs = new DataServices.WorkProfilePlanDataService(context);
				var start = workProfilePlanDs.GetShiftStartOn(startDate);
				var end = workProfilePlanDs.GetShiftStartOn(endDate);

                var oprList = operatorProcessReportRepository.GetAll();
				var processReportList = processReportRepository.Find(pr => pr.StartDateTime >= start && pr.EndDateTime <= end).ToArray();
                var processList = processRepository.GetAll();
                var ssaList = ssaRepository.GetAll().ToArray();
				var ssList = ssRepository.GetAll().ToArray();
				var stationList = stationRepository.GetAll().ToArray();
				var stateList = stateRepository.GetAll().ToArray();
				var fpcList = fpcRepository.GetAll().ToArray();
				var productList = productRepository.GetAll().ToArray();
				var productDefectionList = productDefectionRepository.GetAll().ToArray();
				var productReworkList = productReworkRepository.GetAll().ToArray();
				var activityList = activityRepository.GetAll().ToArray();
                var srList = srRepository.GetAll();
                var osrList = osrRepository.GetAll();
                var drList = drRepository.GetAll();
                var odrList = odrRepository.GetAll();

				var genQuery = from processReport in processReportList
                               from process in processList.Where(p => processReport != null && processReport.Process != null && p.Id == processReport.Process.Id).DefaultIfEmpty()
                               from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                               from stateStation in ssList.Where(ss=> ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                               from state in stateList.Where(s=> stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                               from fpc in fpcList.Where(f=> state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                               from product in productList.Where(pd=> fpc != null && fpc.Product != null && pd.Id == fpc.Product.Id).DefaultIfEmpty()
                               from productRework in productReworkList.Where(r=> state != null && state.OnProductRework != null && state.OnProductRework.Id == r.Id).DefaultIfEmpty()
                               from station in stationList.Where(s=> stateStation != null && stateStation.Station != null && s.Id == stateStation.Station.Id).DefaultIfEmpty()
                               from activity in activityList.Where(a=> ssActivity != null && ssActivity.Activity != null && a.Id == ssActivity.Activity.Id).DefaultIfEmpty()
                               let prId = processReport == null ? -1 : processReport.Id
                               let pdId = product == null ? -1 : product.Id
                               let pdCode = product == null ? string.Empty : product.Code
                               let pdName = product == null ? string.Empty : product.Name
                               let snId = station == null ? -1 : station.Id
                               let snName = station == null ? string.Empty : station.Name
                               let aId = activity == null ? -1 : activity.Id
                               let aCode = activity == null ? string.Empty : activity.Code
                               let aName = activity == null ? string.Empty : activity.Name
							   let rId = productRework == null ? -1 : (productRework.Rework == null ? -1 : productRework.Rework.Id)
                               let tp = process == null ? 0 : process.TargetCount
                               let ct = ssActivity == null ? 0 : ssActivity.CycleTime
                               let date = processReport == null ? DateTime.MinValue : processReport.StartDateTime
                               select new { prId, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var oprQuery = from opr in oprList.Where(opr => opr.ProcessOperator.Operator != null && opr.ProcessOperator.Operator.Id == operatorId)
                               from gen in genQuery.Where(pr => opr.ProcessReport != null && opr.ProcessReport.Id == pr.prId)
                               let oprId = opr == null ? -1 : opr.Id
                               let prId = opr == null ? -1 : opr.ProcessReport == null ? -1 : opr.ProcessReport.Id
                               let oId = opr == null ? -1 : opr.ProcessOperator.Operator == null ? -1 : opr.ProcessOperator.Operator.Id
                               let pdId = gen == null ? -1 : gen.pdId
                               let pdCode = gen == null ? string.Empty : gen.pdCode
                               let pdName = gen == null ? string.Empty : gen.pdName
                               let snId = gen == null ? -1 : gen.snId
                               let snName = gen == null ? string.Empty : gen.snName
                               let aId = gen == null ? -1 : gen.aId
                               let aCode = gen == null ? string.Empty : gen.aCode
                               let aName = gen == null ? string.Empty : gen.aName
                               let rId = gen == null ? -1 : gen.rId
                               let tp = gen == null ? 0 : gen.tp
                               let ct = gen == null ? 0 : gen.ct
                               let date = gen == null ? DateTime.MinValue : gen.date
                               let productionTime = opr == null ? 0 : opr.OperatorProducedG1 * ct
                               let productionCount = opr == null ? 0 : opr.OperatorProducedG1
                               select new { oprId, prId, oId, productionTime, productionCount, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var srQuery = from sReport in srList
                              from osReport in osrList.Where(osr => sReport != null && osr.StoppageReport != null && osr.Operator != null && osr.Operator.Id == operatorId && osr.StoppageReport.Id == sReport.Id)
                              from pReport in processReportList.Where(pr => sReport != null && sReport.ProcessReport != null && sReport.ProcessReport.Id == pr.Id)
                              from opReport in oprList.Where(opr => pReport != null && opr.ProcessReport != null && opr.ProcessOperator != null && opr.ProcessOperator.Operator != null && osReport != null && osReport.Operator != null && opr.ProcessReport.Id == pReport.Id && opr.ProcessOperator.Operator.Id == osReport.Operator.Id)
                              let oId = osReport == null ? -1 : osReport.Operator == null ? -1 : osReport.Operator.Id
                              let prId = pReport == null ? -1 : pReport.Id
                              select new { sReport.Id, oId, prId, sReport.LostCount, sReport.LostTime };

                var sQuery = from opr in oprQuery
                             from sReport in srQuery.Where(sr => sr.oId == opr.oId && sr.prId == opr.prId).DefaultIfEmpty()
                             let oprId = opr == null ? -1 : opr.oprId
                             let ct = opr == null ? 0 : opr.ct
                             let lostTime = sReport == null ? 0 : sReport.LostCount * ct + sReport.LostTime
                             let lostCount = sReport == null ? 0 : sReport.LostCount + (sReport.LostTime / ct)
                             let prId = opr == null ? -1 : opr.prId
                             let oId = opr == null ? -1 : opr.oId
                             let productionTime = opr == null ? 0 : opr.productionTime
                             let productionCount = opr == null ? 0 : opr.productionCount
                             let pdId = opr == null ? -1 : opr.pdId
                             let pdCode = opr == null ? string.Empty : opr.pdCode
                             let pdName = opr == null ? string.Empty : opr.pdName
                             let snId = opr == null ? -1 : opr.snId
                             let snName = opr == null ? string.Empty : opr.snName
                             let aId = opr == null ? -1 : opr.aId
                             let aCode = opr == null ? string.Empty : opr.aCode
                             let aName = opr == null ? string.Empty : opr.aName
                             let rId = opr == null ? -1 : opr.rId
                             let tp = opr == null ? 0 : opr.tp
                             let date = opr == null ? DateTime.MinValue : opr.date
                             select new { oprId, oId, prId, lostTime, lostCount, productionTime, productionCount, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var sgQuery = from s in sQuery
                              group s by new { s.oId, s.oprId, s.prId, s.productionTime, s.productionCount, s.ct, s.pdId, s.pdCode, s.pdName, s.snId, s.snName, s.aId, s.aCode, s.aName, s.rId, s.tp, s.date }
                                  into g
                                  let stoppageTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                                  let stoppageCount = g.Any() ? g.Sum(item => item.lostCount) : 0
                                  select new { g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.productionCount, g.Key.ct, stoppageTime, stoppageCount, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date };

                var drQuery = from dReport in drList
                              from pdReport in productDefectionList.Where(pd=> dReport.ProductDefection != null && pd.Id == dReport.ProductDefection.Id)
                              from odReport in odrList.Where(odr => dReport != null && odr.DefectionReport != null && odr.Operator != null && odr.Operator.Id == operatorId && odr.DefectionReport.Id == dReport.Id)
                              from pReport in processReportList.Where(pr => dReport != null && dReport.ProcessReport != null && dReport.ProcessReport.Id == pr.Id)
                              from opReport in oprList.Where(opr => pReport != null && opr.ProcessReport != null && opr.ProcessOperator != null && opr.ProcessOperator.Operator != null && odReport != null && odReport.Operator != null && opr.ProcessReport.Id == pReport.Id && opr.ProcessOperator.Operator.Id == odReport.Operator.Id)
                              let pId = dReport == null ? -1 : dReport.ProductDefection == null ? -1 : dReport.ProductDefection.Product == null ? -1 : dReport.ProductDefection.Product.Id
                              let prId = pReport == null ? -1 : pReport.Id
                              select new { dReport.Id, pId, prId, dReport.LostCount, dReport.LostTime };

                var dQuery = from sg in sgQuery
                             from dReport in drQuery.Where(dr => sg.pdId == dr.pId && sg.prId == dr.prId).DefaultIfEmpty()
                             let oprId = sg == null ? -1 : sg.oprId
                             let ct = sg == null ? 0 : sg.ct
                             let lostTime = dReport == null ? 0 : dReport.LostCount * ct + dReport.LostTime
                             let lostCount = dReport == null ? 0 : dReport.LostCount + (dReport.LostTime / ct)
                             let prId = sg == null ? -1 : sg.prId
                             let oId = sg == null ? -1 : sg.oId
                             let pdId = sg == null ? -1 : sg.pdId
                             let pdCode = sg == null ? string.Empty : sg.pdCode
                             let pdName = sg == null ? string.Empty : sg.pdName
                             let snId = sg == null ? -1 : sg.snId
                             let snName = sg == null ? string.Empty : sg.snName
                             let aId = sg == null ? -1 : sg.aId
                             let aCode = sg == null ? string.Empty : sg.aCode
                             let aName = sg == null ? string.Empty : sg.aName
                             let rId = sg == null ? -1 : sg.rId
                             let tp = sg == null ? 0 : sg.tp
                             let productionTime = sg == null ? 0 : sg.productionTime
                             let stoppageTime = sg == null ? 0 : sg.stoppageTime
                             let productionCount = sg == null ? 0 : sg.productionCount
                             let stoppageCount = sg == null ? 0 : sg.stoppageCount
                             let date = sg == null ? DateTime.MinValue : sg.date
                             select new { oId, oprId, prId, lostTime, lostCount, stoppageTime, productionTime, stoppageCount, productionCount, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var dgQuery = from d in dQuery
                              group d by new {d.oId, d.pdId, d.oprId, d.prId, d.productionTime, d.stoppageTime, d.productionCount, d.stoppageCount, d.ct, d.pdCode, d.pdName, d.snId, d.snName, d.aId, d.aCode, d.aName, d.rId, d.tp, d.date } into g
                              let defectionTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                              let defectionCount = g.Any() ? g.Sum(item => item.lostCount) : 0
                              select new { g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.ct, g.Key.stoppageTime, defectionTime, g.Key.productionCount, g.Key.stoppageCount, defectionCount, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date };

                var mainQuery = from opr in dgQuery
                            from processReport in processReportList.Where(pr => opr.prId == pr.Id).DefaultIfEmpty()
                            group processReport by new { opr.oId, opr.oprId, operatorId = opr.oId, opr.pdId, opr.pdCode, opr.pdName, opr.snId, opr.snName, opr.aId, opr.aCode, opr.aName, opr.rId, opr.tp, opr.date, opr.productionTime, opr.stoppageTime, opr.defectionTime, opr.productionCount, opr.stoppageCount, opr.defectionCount } into g
                            let targetTime = g.Sum(item => item == null ? 0 : item.DurationSeconds / item.OperatorProcessReports.Count)
                            let targetCount = g.Sum(item => item == null ? 0 : item.ProcessReportTargetPoint / item.OperatorProcessReports.Count)
                            select new { g.Key.oprId, g.Key.operatorId, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date, g.Key.productionTime, g.Key.stoppageTime, g.Key.defectionTime, targetTime, g.Key.productionCount, g.Key.stoppageCount, g.Key.defectionCount, targetCount };


                var qualitiveQuery = from dReport in drList
                               from pdReport in productDefectionList.Where(pd => dReport.ProductDefection != null && pd.Id == dReport.ProductDefection.Id)
                               from odReport in odrList.Where(odr => dReport != null && odr.DefectionReport != null && odr.Operator != null && odr.Operator.Id == operatorId && odr.DefectionReport.Id == dReport.Id)
                               from gen in genQuery.Where(pr => dReport != null && dReport.ProcessReport != null && dReport.ProcessReport.Id == pr.prId)
                               let pdId = gen == null ? -1 : gen.pdId
                               let pdCode = gen == null ? string.Empty : gen.pdCode
                               let pdName = gen == null ? string.Empty : gen.pdName
                               let snId = gen == null ? -1 : gen.snId
                               let snName = gen == null ? string.Empty : gen.snName
                               let aId = gen == null ? -1 : gen.aId
                               let aCode = gen == null ? string.Empty : gen.aCode
                               let aName = gen == null ? string.Empty : gen.aName
                               let rId = gen == null ? -1 : gen.rId
                               let tp = gen == null ? 0 : gen.tp
                               let ct = gen == null ? 0 : gen.ct
                               let date = gen == null ? DateTime.MinValue : gen.date
                               let lostTime = dReport == null ? 0 : dReport.LostCount * ct + dReport.LostTime
                               let lostCount = dReport == null ? 0 : dReport.LostCount + (dReport.LostTime / ct)
                               let pStatus = dReport == null ? 0 : dReport.IsG2 ? 2 : 1
                               select new { dReport.Id, lostCount, lostTime, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date, pStatus };

                var technicalQuery = from sReport in srList
                                     from osReport in osrList.Where(osr => sReport != null && osr.StoppageReport != null && osr.Operator != null && osr.Operator.Id == operatorId && osr.StoppageReport.Id == sReport.Id)
                                     from pReport in processReportList.Where(pr => sReport != null && sReport.ProcessReport != null && sReport.ProcessReport.Id == pr.Id)
                                     from gen in genQuery.Where(pr => sReport != null && sReport.ProcessReport != null && sReport.ProcessReport.Id == pr.prId)
                                     let pdId = gen == null ? -1 : gen.pdId
                                     let pdCode = gen == null ? string.Empty : gen.pdCode
                                     let pdName = gen == null ? string.Empty : gen.pdName
                                     let snId = gen == null ? -1 : gen.snId
                                     let snName = gen == null ? string.Empty : gen.snName
                                     let aId = gen == null ? -1 : gen.aId
                                     let aCode = gen == null ? string.Empty : gen.aCode
                                     let aName = gen == null ? string.Empty : gen.aName
                                     let rId = gen == null ? -1 : gen.rId
                                     let tp = gen == null ? 0 : gen.tp
                                     let ct = gen == null ? 0 : gen.ct
                                     let date = gen == null ? DateTime.MinValue : gen.date
                                     let lostTime = sReport == null ? 0 : sReport.LostCount * ct + sReport.LostTime
                                     let lostCount = sReport == null ? 0 : sReport.LostCount + (sReport.LostTime / ct)
                                     select new { sReport.Id, lostCount, lostTime, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                result.Id = operatorId;
                result.Code = operatorRepository.FirstOrDefault(o => o.Id == operatorId).Code;
                result.Title = operatorRepository.FirstOrDefault(o => o.Id == operatorId).Name;

                var mainList = mainQuery.ToList();

                if (mainList.Any())
                {
                    result.TotalTargetTime = mainList.Sum(record => record.targetTime);
                    result.TotalProductionTime = mainList.Sum(record => record.productionTime);
                    result.TotalExtraTime = result.TotalProductionTime > result.TotalTargetTime
                        ? result.TotalProductionTime - result.TotalTargetTime
                        : 0;
                    result.TotalShortageTime = result.TotalTargetTime > result.TotalProductionTime
                        ? result.TotalTargetTime - result.TotalProductionTime
                        : 0;
                    result.TotalDefectionTime = mainList.Sum(record => record.defectionTime);
                    result.TotalStoppageTime = mainList.Sum(record => record.stoppageTime);

                    result.TotalTargetCount = mainList.Sum(record => record.targetCount);
                    result.TotalProductionCount = mainList.Sum(record => record.productionCount);
                    result.TotalExtraCount = result.TotalProductionCount > result.TotalTargetCount
                        ? result.TotalProductionCount - result.TotalTargetCount
                        : 0;
                    result.TotalShortageCount = result.TotalTargetCount > result.TotalProductionCount
                        ? result.TotalTargetCount - result.TotalProductionCount
                        : 0;
                    result.TotalDefectionCount = mainList.Sum(record => record.defectionCount);
                    result.TotalStoppageCount = mainList.Sum(record => record.stoppageCount);

                    foreach (var line in mainList)
                    {
                        var mainDetail = new OprActivityDetailVM
                        {
                            Id = line.oprId,
                            Date = line.date,
                            Product = line.pdCode + "-" + line.pdName,
                            Station = line.snName,
                            Activity = line.aCode + "-" + line.aName,
                            IsRework = line.rId == -1 ? string.Empty : "*",

                            TargetTime = Format.ConvertToHMS(line.targetTime),
                            DefectionTime = Format.ConvertToHMS((int)line.defectionTime),
                            ProductionTime = Format.ConvertToHMS((int)line.productionTime),
                            StoppageTime = Format.ConvertToHMS((int)line.stoppageTime),
                            TargetCount = line.targetCount.ToString(CultureInfo.InvariantCulture),
                            DefectionCount = line.defectionCount.ToString(CultureInfo.InvariantCulture),
                            ProductionCount = line.productionCount.ToString(CultureInfo.InvariantCulture),
                            StoppageCount = line.stoppageCount.ToString(CultureInfo.InvariantCulture)
                        };
                        result.ActivityItems.Add(mainDetail);
                    }
                }

                var qualitiveList = qualitiveQuery.ToList();
                result.TotalWaste = qualitiveList.Any()
                    ? qualitiveList.Sum(record => record.pStatus == (decimal) QualitiveStatus.Waste ? record.lostCount : 0)
                    : 0;
                result.TotalSecondGrade = qualitiveList.Any()
                    ? qualitiveList.Sum(record => record.pStatus == (decimal)QualitiveStatus.SecondGrade ? record.lostCount : 0)
                    : 0;

                foreach (var line in qualitiveList)
                {
                    var qualitiveDetail = new OprQualitativeDetailVM
                    {
                        Id = line.Id,
                        Date = line.date,
                        Product = line.pdCode + "-" + line.pdName,
                        Station = line.snName,
                        Activity = line.aCode + "-" + line.aName,
                        Status = (QualitiveStatus)line.pStatus,

                        DefectionTime = Format.ConvertToHMS((int)line.lostTime),
                        DefectionCount = line.lostCount.ToString(CultureInfo.InvariantCulture),
                    };
                    result.QualitiveItems.Add(qualitiveDetail);
                }

                foreach (var line in technicalQuery)
                {
                    var technicalDetail = new OprTechnicalDetailVM
                    {
                        Id = line.Id,
                        Date = line.date,
                        Product = line.pdCode + "-" + line.pdName,
                        Station = line.snName,
                        Activity = line.aCode + "-" + line.aName,

                        StoppageTime = Format.ConvertToHMS((int)line.lostTime),
                        StoppageCount = line.lostCount.ToString(CultureInfo.InvariantCulture),
                    };
                    result.TechnicalItems.Add(technicalDetail);
                }
            }
            return result;
        }

      
    }
}