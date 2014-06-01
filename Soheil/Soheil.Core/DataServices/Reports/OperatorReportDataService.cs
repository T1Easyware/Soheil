
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
        public IList<Record> GetInRange(DateTimeIntervals intervalType, OperatorBarInfo barInfo, int startIdx, int count)
        {
            switch (barInfo.Level)
            {
                case 0:
                    return GetOperatorsEfficiency(barInfo, startIdx, count);
                //case 1:
                    //return GetOperatorEfficiency(barInfo);
            }
            return new List<Record>();
        }

        public double GetMax(DateTimeIntervals intervalType, OperatorBarInfo barInfo, int count)
        {
            return GetMaxOperatorByDate(barInfo);
        }

        public int GetMachineCount()
        {
            int count;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                count = repository.GetAll().Count();
            }
            return count;
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

        private IList<Record> GetOperatorsEfficiency(OperatorBarInfo oprInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();

            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<Operator>(context);
                var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var processRepository = new Repository<Process>(context);
                var ssaRepository = new Repository<StateStationActivity>(context);
                var srRepository = new Repository<StoppageReport>(context);
                var osrRepository = new Repository<OperatorStoppageReport>(context);
                var drRepository = new Repository<DefectionReport>(context);
                var odrRepository = new Repository<OperatorDefectionReport>(context);

                var operatorList = operatorRepository.GetAll();
                var oprList = operatorProcessReportRepository.GetAll(); 
                var processReportList = processReportRepository.GetAll();
                var processList = processRepository.GetAll();
                var ssaList = ssaRepository.GetAll();
                var srList = srRepository.GetAll();
                var osrList = osrRepository.GetAll();
                var drList = drRepository.GetAll();
                var odrList = odrRepository.GetAll();

                var indexList = operatorList.Skip(startIndex).Take(count).Select((o, index) => new { interval = index, o.Id, o.Code, o.Name });

                var oprQuery = from opr in oprList
                               from processReport in processReportList.Where(pr=> opr.ProcessReport != null && opr.ProcessReport.Id == pr.Id && pr.StartDateTime >= oprInfo.StartDate && pr.EndDateTime < oprInfo.EndDate).DefaultIfEmpty()
                               from process in processList.Where(p=> processReport != null && processReport.Process != null && p.Id == processReport.Process.Id).DefaultIfEmpty()
                               from ssActivity in ssaList.Where(ssa => process!=null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                               let oprId = opr == null ? -1 : opr.Id
                               let prId = opr == null ? -1 : opr.ProcessReport == null ? -1 : opr.ProcessReport.Id
                               let oId = opr == null ? -1 : opr.Operator == null ? -1 : opr.Operator.Id
                               let ct = ssActivity == null ? 0 : ssActivity.CycleTime
                               let productionTime = opr == null || ssActivity == null ? 0 : opr.OperatorProducedG1 * ct
                               select new { oprId, prId, oId, productionTime, ct };

                var sQuery = from opr in oprQuery
                             from osReport in osrList.Where(osr=> osr.Operator != null && osr.Operator.Id == opr.oId).DefaultIfEmpty()
                             from sReport in srList.Where(sr=> osReport != null && osReport.StoppageReport != null && sr.Id == osReport.StoppageReport.Id).DefaultIfEmpty()
                             let oprId = opr == null ? -1 : opr.oprId
                             let ct = opr == null ? 0 : opr.ct
                             let lostTime = sReport == null ? 0: sReport.LostCount * ct + sReport.LostTime
                             let prId = opr == null ? -1 : opr.prId
                             let oId = opr == null ? -1 : opr.oId
                             let productionTime = opr == null? 0 : opr.productionTime
                             select new {oprId, oId, prId, lostTime, productionTime, ct };

                var sgQuery = from s in sQuery
                            group s by new {s.oId, s.oprId, s.prId, s.productionTime, s.ct}
                            into g
                            let stoppageTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                            select new {g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.ct, stoppageTime};

                var dQuery = from sg in sgQuery
                             from odReport in odrList.Where(odr=> odr.Operator != null && odr.Operator.Id == sg.oId).DefaultIfEmpty()
                             from dReport in drList.Where(dr=> odReport != null && odReport.DefectionReport != null && odReport.DefectionReport.Id == dr.Id).DefaultIfEmpty()
                             let oprId = sg == null ? -1 : sg.oprId
                             let ct = sg == null ? 0 : sg.ct
                             let lostTime = dReport == null ? 0: dReport.LostCount * ct + dReport.LostTime
                             let prId = sg == null ? -1 : sg.prId
                             let oId = sg == null ? -1 : sg.oId
                             let productionTime = sg == null? 0 : sg.productionTime
                             let stoppageTime = sg == null ? 0 : sg.stoppageTime
                             select new { oId, oprId, prId, lostTime, stoppageTime, productionTime, ct };

                var dgQuery = from d in dQuery
                              group d by new { d.oId, d.oprId, d.prId, d.productionTime, d.stoppageTime, d.ct }
                                  into g
                                  let defectionTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                                  select new { g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.ct, g.Key.stoppageTime, defectionTime };

                var prQuery = from opr in dgQuery
                              from processReport in processReportList.Where(pr=> opr.prId == pr.Id).DefaultIfEmpty()
                              group processReport by new { opr.oId, opr.oprId, operatorId = opr.oId, opr.productionTime, opr.stoppageTime, opr.defectionTime} into g
                              let duration = g.Sum(item => item == null ? 0 : item.DurationSeconds/item.OperatorProcessReports.Count)
                              select new { g.Key.oprId, g.Key.operatorId, g.Key.productionTime, g.Key.stoppageTime, g.Key.defectionTime, duration };

                var query = from oprt in indexList
                            from pr in prQuery.Where(p => p.operatorId == oprt.Id).DefaultIfEmpty()
                            group pr by new {oprt.interval, oprt.Id, oprt.Code, oprt.Name} into g
                            let productionTime = g.Any()? 85 : g.Sum(item=> item.productionTime)
                            let duration = g.Any() ? 100 : g.Sum(item => item.duration)
                            let stoppageTime = g.Any() ? 5 : g.Max(item=>item.stoppageTime)
                            let defectionTime = g.Any() ? 10 : g.Max(item => item.defectionTime)
                            select new { g.Key.interval, g.Key.Id, g.Key.Code, g.Key.Name, stoppageTime, defectionTime, productionTime, duration };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record();
                    foreach (var line in query)
                    {
                        if (line.interval + startIndex == i)
                        {
                            newRecord.Id = line.Id;
                            newRecord.Value = line.duration;
                            newRecord.Value1 = line.productionTime;
                            newRecord.Value2 = line.defectionTime;
                            newRecord.Value3 = line.stoppageTime;
                            newRecord.StartDate = oprInfo.StartDate;
                            newRecord.EndDate = oprInfo.EndDate;
                            newRecord.Header = line.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }

        public OperatorProcessReportVm GetOperatorEfficiency(int operatorId, DateTime startDate, DateTime endDate)
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
                var productReworkRepository = new Repository<ProductRework>(context);
                var activityRepository = new Repository<Activity>(context);
                var srRepository = new Repository<StoppageReport>(context);
                var osrRepository = new Repository<OperatorStoppageReport>(context);
                var drRepository = new Repository<DefectionReport>(context);
                var odrRepository = new Repository<OperatorDefectionReport>(context);

                var oprList = operatorProcessReportRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var processList = processRepository.GetAll();
                var ssaList = ssaRepository.GetAll();
                var ssList = ssRepository.GetAll();
                var stationList = stationRepository.GetAll();
                var stateList = stateRepository.GetAll();
                var fpcList = fpcRepository.GetAll();
                var productList = productRepository.GetAll();
                var productReworkList = productReworkRepository.GetAll();
                var activityList = activityRepository.GetAll();
                var srList = srRepository.GetAll();
                var osrList = osrRepository.GetAll();
                var drList = drRepository.GetAll();
                var odrList = odrRepository.GetAll();


                var oprQuery = from opr in oprList.Where(opr => opr.Operator != null && opr.Operator.Id == operatorId)
                               from processReport in processReportList.Where(pr => opr.ProcessReport != null && opr.ProcessReport.Id == pr.Id && pr.StartDateTime >= startDate && pr.EndDateTime < endDate).DefaultIfEmpty()
                               from process in processList.Where(p => processReport != null && processReport.Process != null && p.Id == processReport.Process.Id).DefaultIfEmpty()
                               from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                               from stateStation in ssList.Where(ss=> ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                               from state in stateList.Where(s=> stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                               from fpc in fpcList.Where(f=> state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                               from product in productList.Where(pd=> fpc != null && fpc.Product != null && pd.Id == fpc.Product.Id).DefaultIfEmpty()
                               from rework in productReworkList.Where(r=> state != null && state.OnProductRework != null && state.OnProductRework.Id == r.Id).DefaultIfEmpty()
                               from station in stationList.Where(s=> stateStation != null && stateStation.Station != null && s.Id == stateStation.Station.Id).DefaultIfEmpty()
                               from activity in activityList.Where(a=> ssActivity != null && ssActivity.Activity != null && a.Id == ssActivity.Activity.Id).DefaultIfEmpty()
                               let oprId = opr == null ? -1 : opr.Id
                               let prId = opr == null ? -1 : opr.ProcessReport == null ? -1 : opr.ProcessReport.Id
                               let oId = opr == null ? -1 : opr.Operator == null ? -1 : opr.Operator.Id
                               let pdId = product == null ? -1 : product.Id
                               let pdCode = product == null ? string.Empty : product.Code
                               let pdName = product == null ? string.Empty : product.Name
                               let snId = station == null ? -1 : station.Id
                               let snName = station == null ? string.Empty : station.Name
                               let aId = activity == null ? -1 : activity.Id
                               let aCode = activity == null ? string.Empty : activity.Code
                               let aName = activity == null ? string.Empty : activity.Name
                               let rId = rework == null ? -1 : rework.Id
                               let tp = process == null ? 0 : process.TargetCount
                               let ct = ssActivity == null ? 0 : ssActivity.CycleTime
                               let productionTime = opr == null || ssActivity == null ? 0 : opr.OperatorProducedG1 * ct
                               let date = processReport == null ? DateTime.MinValue : processReport.StartDateTime
                               select new { oprId, prId, oId, productionTime, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var sQuery = from opr in oprQuery
                             from osReport in osrList.Where(osr => osr.Operator != null && osr.Operator.Id == opr.oId).DefaultIfEmpty()
                             from sReport in srList.Where(sr => osReport != null && osReport.StoppageReport != null && sr.Id == osReport.StoppageReport.Id).DefaultIfEmpty()
                             let oprId = opr == null ? -1 : opr.oprId
                             let ct = opr == null ? 0 : opr.ct
                             let lostTime = sReport == null ? 0 : sReport.LostCount * ct + sReport.LostTime
                             let prId = opr == null ? -1 : opr.prId
                             let oId = opr == null ? -1 : opr.oId
                             let productionTime = opr == null ? 0 : opr.productionTime
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
                             select new { oprId, oId, prId, lostTime, productionTime, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var sgQuery = from s in sQuery
                              group s by new { s.oId, s.oprId, s.prId, s.productionTime, s.ct, s.pdId, s.pdCode, s.pdName, s.snId, s.snName, s.aId, s.aCode, s.aName, s.rId, s.tp, s.date }
                                  into g
                                  let stoppageTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                                  select new { g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.ct, stoppageTime, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date };

                var dQuery = from sg in sgQuery
                             from odReport in odrList.Where(odr => odr.Operator != null && odr.Operator.Id == sg.oId).DefaultIfEmpty()
                             from dReport in drList.Where(dr => odReport != null && odReport.DefectionReport != null && odReport.DefectionReport.Id == dr.Id).DefaultIfEmpty()
                             let oprId = sg == null ? -1 : sg.oprId
                             let ct = sg == null ? 0 : sg.ct
                             let lostTime = dReport == null ? 0 : dReport.LostCount * ct + dReport.LostTime
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
                             let date = sg == null ? DateTime.MinValue : sg.date
                             select new { oId, oprId, prId, lostTime, stoppageTime, productionTime, ct, pdId, pdCode, pdName, snId, snName, aId, aCode, aName, rId, tp, date };

                var dgQuery = from d in dQuery
                              group d by new { d.oId, d.oprId, d.prId, d.productionTime, d.stoppageTime, d.ct, d.pdId, d.pdCode, d.pdName, d.snId, d.snName, d.aId, d.aCode, d.aName, d.rId, d.tp, d.date } into g
                                  let defectionTime = g.Any() ? g.Sum(item => item.lostTime) : 0
                              select new { g.Key.oId, g.Key.oprId, g.Key.prId, g.Key.productionTime, g.Key.ct, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date, g.Key.stoppageTime, defectionTime };

                var query = from opr in dgQuery
                              from processReport in processReportList.Where(pr => opr.prId == pr.Id).DefaultIfEmpty()
                              group processReport by new { opr.oId, opr.oprId, operatorId = opr.oId, opr.pdId, opr.pdCode, opr.pdName, opr.snId, opr.snName, opr.aId, opr.aCode, opr.aName, opr.rId, opr.tp, opr.date, opr.productionTime, opr.stoppageTime, opr.defectionTime } into g
                              let duration = g.Sum(item => item == null ? 0 : item.DurationSeconds / item.OperatorProcessReports.Count)
                              select new { g.Key.oprId, g.Key.operatorId, g.Key.pdId, g.Key.pdCode, g.Key.pdName, g.Key.snId, g.Key.snName, g.Key.aId, g.Key.aCode, g.Key.aName, g.Key.rId, g.Key.tp, g.Key.date, g.Key.productionTime, g.Key.stoppageTime, g.Key.defectionTime, duration };

                result.Id = operatorId;
                result.Title = operatorRepository.FirstOrDefault(o => o.Id == operatorId).Name;

                // dummy data

                for (int i = 0; i < 80; i++)
                {
                    var detail = new OprDetailVM
                    {
                        Id = i,
                        Date = DateTime.Now.AddHours(2*i),
                        Product = "p" + i,
                        Station = "s" + i,
                        Activity = "a" + i,
                        TargetPoint = "100" +i,
                        DefectionTime = "5" + i,
                        ProductionTime = "80" + i,
                        StoppageTime = "1" + i
                    };
                    result.Details.Add(detail);
                }
                //

                //var newRecord = new Record();
                //foreach (var line in query)
                //{
                //    var data = new List<KeyValuePair<string, object>>
                //    {
                //        new KeyValuePair<string, object>("id", line.oprId),
                //        new KeyValuePair<string, object>("pId", line.pdId),
                //        new KeyValuePair<string, object>("pValue", line.pdCode + "-" + line.pdName),
                //        new KeyValuePair<string, object>("aId", line.aId),
                //        new KeyValuePair<string, object>("aValue", line.aCode + "-" + line.aName),
                //        new KeyValuePair<string, object>("sId", line.snId),
                //        new KeyValuePair<string, object>("sValue", line.snName),
                //        new KeyValuePair<string, object>("tp", line.tp),
                //        new KeyValuePair<string, object>("production", line.productionTime),
                //        new KeyValuePair<string, object>("defection", line.defectionTime),
                //        new KeyValuePair<string, object>("stoppage", line.stoppageTime)
                //    };
                //    newRecord.Id = line.operatorId;
                //    newRecord.StartDate = line.date;
                //    newRecord.Data = data;
                //}
                //records.Add(newRecord);
            }
            return result;
        }

        private double GetMaxOperatorByDate(OperatorBarInfo oprInfo)
        {
            using (var context = new SoheilEdmContext())
            {
                var operatorProcessReportRepository = new Repository<OperatorProcessReport>(context);
                var processReportRepository = new Repository<ProcessReport>(context);

                var oprList = operatorProcessReportRepository.GetAll();
                var processReportList = processReportRepository.GetAll();


                var query = from opr in oprList
                              from processReport in processReportList.Where(pr=> opr.ProcessReport != null && opr.ProcessReport.Id == pr.Id && pr.StartDateTime >= oprInfo.StartDate && pr.EndDateTime < oprInfo.EndDate).DefaultIfEmpty()
                              group processReport by new { opr.Id, operatorId = opr.Operator.Id, opr.OperatorProducedG1} into g
                              let duration = g.Sum(item => item == null ? 0 : item.DurationSeconds/item.OperatorProcessReports.Count)
                              select new { g.Key.Id, duration };


                var result = query.ToList();
                return result.Any() ? result.Max(item => item.duration) : 100;

            }
        }

        private DateTime AddInterval(DateTime currentDate, int value, DateTimeIntervals type)
        {
            switch (type)
            {
                case DateTimeIntervals.Hourly:
                    return currentDate.AddHours(value);
                case DateTimeIntervals.Shiftly:
                    return currentDate.AddHours((24 / SoheilConstants.ShiftPerDay) * value);
                case DateTimeIntervals.Daily:
                    return currentDate.AddDays(value);
                case DateTimeIntervals.Weekly:
                    return currentDate.AddDays(value * 7);
                case DateTimeIntervals.Monthly:
                    return currentDate.AddMonths(value);
                default:
                    return currentDate.AddMonths(value);
            }
        }
    }
}