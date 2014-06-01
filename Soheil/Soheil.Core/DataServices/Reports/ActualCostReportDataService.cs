using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Reports;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ActualCostReportDataService
    {
        public IList<Record> GetInRange(CostType type, DateTimeIntervals intervalType, CostBarInfo barInfo, int startIdx, int count)
        {
            switch (intervalType)
            {
                case DateTimeIntervals.None:
                    return GetCostsByProducts(barInfo, startIdx, count);
                default:
                    return GetCostsByDate(intervalType, barInfo, startIdx, count);

            }
        }
        public double GetMax(CostType type, DateTimeIntervals intervalType, CostBarInfo barInfo, int count)
        {
            switch (barInfo.Level)
            {
                case 0:
                    return GetMaxCostByDate(intervalType, type, count);
                case 1:
                    return GetMaxCostByCostCenters(barInfo, type, count);
                default:
                    return 0;
            }
        }

        public int GetProductCount()
        {
            int count;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                count = repository.GetAll().Count();
            }
            return count;
        }
        public int GetCostCentersCount()
        {
            int count;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<CostCenter>(context);
                count = repository.GetAll().Count();
            }
            return count;
        }
        public int GetCostsCount(int costCenterId)
        {
            int count;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cost>(context);
                count = repository.Find(item=>item.CostCenter != null && item.CostCenter.Id == costCenterId).Count();
            }
            return count;
        }

        private IList<Record> GetCostsByDate(DateTimeIntervals intervalType, CostBarInfo barInfo, int startIdx, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIdx, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);
                var ssamRepository = new Repository<StateStationActivityMachine>(context);
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var costList = costRepository.GetAll().ToList();
                var costCenterList = costCenterRepository.GetAll().ToList();
                var ssamList = ssamRepository.GetAll().ToList();
                var processList = processRepository.GetAll().ToList();
                var processReportList = processReportRepository.GetAll().ToList();
                var taskReportList = taskReportRepository.GetAll().ToList();
                var processOperatortList = processOperatortRepository.GetAll().ToList();
                var ssaList = ssaRepository.GetAll().ToList();
                var stateStationList = stateStationRepository.GetAll().ToList();
                var stateList = stateRepository.GetAll().ToList();
                var fpcList = fpcRepository.GetAll().ToList();
                var productList = productRepository.GetAll().ToList();

                // List of all products in range
                var indexList = new List<KeyValuePair<int, int>>();

                for (int i = startIdx; i < count; i++)
                {
                    indexList.AddRange(from taskReport in taskReportList
                                       where taskReport.ReportStartDateTime >= currentInterval
                                       && taskReport.ReportStartDateTime < AddInterval(currentInterval, 1, intervalType)
                                       select new KeyValuePair<int, int>(i, taskReport.Id));
                    indexList.Add(new KeyValuePair<int, int>(i, -1));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                // Products duration
                var productDurationQuery = from index in indexList
                                           from taskReport in taskReportList.Where(tr => tr.Id == index.Value).DefaultIfEmpty()
                                           from processReport in processReportList.Where(pr =>  taskReport != null ).DefaultIfEmpty()
                                           from process in processList.Where(p => processReport != null && processReport.Process != null && p.Id == processReport.Process.Id).DefaultIfEmpty()
                                           from processOpr in processOperatortList.Where(po => process != null && po.Process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                           from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                           from ssaMachine in ssamList.Where(ssam => ssActivity != null && ssam.StateStationActivity != null && ssActivity.Id == ssam.StateStationActivity.Id).DefaultIfEmpty()
                                           from stateStation in stateStationList.Where(ss => ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                           from state in stateList.Where(s => stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                           from fpc in fpcList.Where(f => state != null && state.FPC != null && state.FPC.Id == f.Id).DefaultIfEmpty()
                                           from product in productList.Where(p => fpc != null && fpc.Product != null && p.Id == fpc.Product.Id).DefaultIfEmpty()
                                           let productId = product == null ? -1 : product.Id
                                           let stationId = stateStation == null ? -1 : (stateStation.Station == null ? -1 : stateStation.Station.Id)
                                           let machineId = ssaMachine == null ? -1 : (ssaMachine.Machine == null ? -1 : ssaMachine.Machine.Id)
                                           let activityId = ssActivity == null ? -1 : (ssActivity.Activity == null ? -1 : ssActivity.Activity.Id)
                                           let operatorId = processOpr == null ? -1 : (processOpr.Operator == null ? -1 : processOpr.Operator.Id)
                                           let start = taskReport == null ? DateTime.MinValue : taskReport.ReportStartDateTime
                                           let end = taskReport == null ? DateTime.MinValue : taskReport.ReportEndDateTime
                                           let duration = taskReport == null ? 0 : taskReport.ReportDurationSeconds
                                           select new { interval = index.Key, productId, stationId, machineId, activityId, operatorId, start, end, duration };

                // Cost centers totals
                var productDurationList = productDurationQuery.ToList();
                var productCostByStationQuery = from product in productDurationList
                                                from cost in costList.Where(c => c.Station != null && c.Station.Id == product.stationId
                                                    && c.Date >= product.start && c.Date < product.end).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let stationId = product == null ? -1 : product.stationId
                                                let productDuration = product == null ? 0 : product.duration
                                                let stationCost = cost == null ? 0 : cost.CostValue
                                                select new { product.interval, productId, stationId, productDuration, stationCost };

                var costByStationList = productCostByStationQuery.ToList();
                var sTotalByProductQuery = from station in costByStationList
                                           group station by station.stationId into g
                                           select new { g.Key, stationTotal = g.Sum(item => item.stationCost) };

                var stationCostByProductQuery = from product in costByStationList
                                                from station in sTotalByProductQuery.Where(s => s.Key == product.stationId).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let stationId = product == null ? -1 : product.stationId
                                                let productDuration = product == null ? 0 : product.productDuration
                                                let stationCost = station == null ? 0 : station.stationTotal
                                                select new { product.interval, productId, stationId, productDuration, stationCost };

                var productCostByMachineQuery = from product in productDurationList
                                                from cost in costList.Where(c => c.Machine != null && c.Machine.Id == product.machineId
                                                                            && c.Date >= product.start && c.Date < product.end).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let machineId = product == null ? -1 : product.machineId
                                                let productDuration = product == null ? 0 : product.duration
                                                let machineCost = cost == null ? 0 : cost.CostValue
                                                select new { product.interval, productId, machineId, productDuration, machineCost };

                var costByMachineList = productCostByMachineQuery.ToList();
                var mTotalByProductQuery = from machine in costByMachineList
                                           group machine by machine.machineId into g
                                           select new { g.Key, machineTotal = g.Sum(item => item.machineCost) };

                var machineCostByProductQuery = from product in costByMachineList
                                                from machine in mTotalByProductQuery.Where(s => s.Key == product.machineId).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let machineId = product == null ? -1 : product.machineId
                                                let productDuration = product == null ? 0 : product.productDuration
                                                let machineCost = machine == null ? 0 : machine.machineTotal
                                                select new { product.interval, productId, machineId, productDuration, machineCost };

                var productCostByActivityQuery = from product in productDurationList
                                                 from cost in costList.Where(c => c.Activity != null && c.Activity.Id == product.activityId
                                                                            && c.Date >= product.start && c.Date < product.end).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let activityId = product == null ? -1 : product.activityId
                                                 let productDuration = product == null ? 0 : product.duration
                                                 let activityCost = cost == null ? 0 : cost.CostValue
                                                 select new { product.interval, productId, activityId, productDuration, activityCost };

                var costByActivityList = productCostByActivityQuery.ToList();
                var aTotalByProductQuery = from activity in costByActivityList
                                           group activity by activity.activityId into g
                                           select new { g.Key, activityTotal = g.Sum(item => item.activityCost) };

                var activityCostByProductQuery = from product in costByActivityList
                                                 from activity in aTotalByProductQuery.Where(s => s.Key == product.activityId).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let activityId = product == null ? -1 : product.activityId
                                                 let productDuration = product == null ? 0 : product.productDuration
                                                 let activityCost = activity == null ? 0 : activity.activityTotal
                                                 select new { product.interval, productId, activityId, productDuration, activityCost };

                var productCostByOperatorQuery = from product in productDurationList
                                                 from cost in costList.Where(c => c.Operator != null && c.Operator.Id == product.operatorId
                                                                            && c.Date >= product.start && c.Date < product.end).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let operatorId = product == null ? -1 : product.operatorId
                                                 let productDuration = product == null ? 0 : product.duration
                                                 let operatorCost = cost == null ? 0 : cost.CostValue
                                                 select new { product.interval, productId, operatorId, productDuration, operatorCost };

                var costByOperatorList = productCostByOperatorQuery.ToList();
                var oTotalByProductQuery = from opr in costByOperatorList
                                           group opr by opr.operatorId into g
                                           select new { g.Key, operatorTotal = g.Sum(item => item.operatorCost) };

                var operatorCostByProductQuery = from product in costByOperatorList
                                                 from opr in oTotalByProductQuery.Where(s => s.Key == product.operatorId).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let operatorId = product == null ? -1 : product.operatorId
                                                 let productDuration = product == null ? 0 : product.productDuration
                                                 let operatorCost = opr == null ? 0 : opr.operatorTotal
                                                 select new { product.interval, productId, operatorId, productDuration, operatorCost };

                var productCostByMiscQuery = from product in productDurationList
                                             from costCenter in costCenterList.Where(cc=> cc.SourceType == (decimal) CostSourceType.Other).DefaultIfEmpty()
                                                 from cost in costList.Where(c=> c.CostCenter.Id == costCenter.Id && c.CostCenter != null && costCenter != null
                                                     && c.Date >= product.start && c.Date < product.end).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let productDuration = product == null ? 0 : product.duration
                                                 let miscCost = cost == null ? 0 : cost.CostValue
                                             select new { product.interval, productId, productDuration, miscCost };

                // Cost centers durations
                var stationCostByProductList = stationCostByProductQuery.ToList();
                var stationDurationQuery = from product in stationCostByProductList
                                           group product by new { product.interval, product.stationId } into g
                                           let stationDuration = g.Sum(item => item.productDuration)
                                           select new { g.Key.interval, g.Key.stationId, stationDuration };

                var machineCostByProductList = machineCostByProductQuery.ToList();
                var machineDurationQuery = from product in machineCostByProductList
                                           group product by new { product.interval, product.machineId } into g
                                           let machineDuration = g.Sum(item => item.productDuration)
                                           select new { g.Key.interval, g.Key.machineId, machineDuration };

                var activityCostByProductList = activityCostByProductQuery.ToList();
                var activityDurationQuery = from product in activityCostByProductList
                                            group product by new { product.interval, product.activityId } into g
                                            let activityDuration = g.Sum(item => item.productDuration)
                                            select new { g.Key.interval, g.Key.activityId, activityDuration };

                var operatorCostByProductList = operatorCostByProductQuery.ToList();
                var operatorDurationQuery = from product in operatorCostByProductList
                                            group product by new { product.interval, product.operatorId } into g
                                            let operatorDuration = g.Sum(item => item.productDuration)
                                            select new { g.Key.interval, g.Key.operatorId, operatorDuration };

                var productCostByMiscList = productCostByMiscQuery.ToList();
                var productSumDurationQuery = from product in productCostByMiscList
                                            group product by new { product.interval, product.productId } into g
                                            let productDuration = g.Sum(item => item.productDuration)
                                            select new { g.Key.interval, g.Key.productId, productDuration };

                // Products costs
                var productStationCostQuery = from station in stationDurationQuery
                                              from product in stationCostByProductList.Where(p => station != null && p.stationId == station.stationId && p.interval == station.interval).DefaultIfEmpty()
                                              let stationDuration = station == null ? 0 : station.stationDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.stationCost
                                              let reletiveCost = stationDuration == 0 ? 0 : totalCost * (productDuration / stationDuration)
                                              select new { product.interval, product.productId, reletiveCost };

                var productMachineCostQuery = from machine in machineDurationQuery
                                              from product in machineCostByProductList.Where(p => machine != null && p.machineId == machine.machineId && p.interval == machine.interval).DefaultIfEmpty()
                                              let machineDuration = machine == null ? 0 : machine.machineDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.machineCost
                                              let reletiveCost = machineDuration == 0 ? 0 : totalCost * (productDuration / machineDuration)
                                              select new { product.interval, product.productId, reletiveCost };

                var productActivityCostQuery = from activity in activityDurationQuery
                                               from product in activityCostByProductList.Where(p => activity != null && p.activityId == activity.activityId && p.interval == activity.interval).DefaultIfEmpty()
                                               let activityDuration = activity == null ? 0 : activity.activityDuration
                                               let productDuration = product == null ? 0 : product.productDuration
                                               let totalCost = product == null ? 0 : product.activityCost
                                               let reletiveCost = activityDuration == 0 ? 0 : totalCost * (productDuration / activityDuration)
                                               select new { product.interval, product.productId, reletiveCost };

                var productOperatorCostQuery = from opr in operatorDurationQuery
                                               from product in operatorCostByProductList.Where(p => opr != null && p.operatorId == opr.operatorId && p.interval == opr.interval).DefaultIfEmpty()
                                               let operatorDuration = opr == null ? 0 : opr.operatorDuration
                                               let productDuration = product == null ? 0 : product.productDuration
                                               let totalCost = product == null ? 0 : product.operatorCost
                                               let reletiveCost = operatorDuration == 0 ? 0 : totalCost * (productDuration / operatorDuration)
                                               select new { product.interval, product.productId, reletiveCost };

                var intervalSumDurationQuery = from product in productCostByMiscList
                                               group product by product.interval into g
                                               let intervalDuration = g.Sum(item => item.productDuration)
                                               let intervalCost = g.Sum(item => item.miscCost)
                                               select new { interval = g.Key, intervalDuration, intervalCost };

                var productMiscCostQuery = from product in productSumDurationQuery
                         from interval in intervalSumDurationQuery.Where(i => i.interval == product.interval).DefaultIfEmpty()
                         let miscCost = interval.intervalDuration == 0 ? 0 : interval.intervalCost * (product.productDuration / interval.intervalDuration)
                         select new { interval.interval, product.productId, miscCost };

                // Interval total cost
                var intervalTotalCost = from index in indexList
                                        from sCost in productStationCostQuery.Where(s => s.productId == barInfo.Id && s.interval == index.Key).DefaultIfEmpty()
                                        from mCost in productMachineCostQuery.Where(m => m.productId == barInfo.Id && m.interval == index.Key).DefaultIfEmpty()
                                        from aCost in productActivityCostQuery.Where(a => a.productId == barInfo.Id && a.interval == index.Key).DefaultIfEmpty()
                                        from oCost in productOperatorCostQuery.Where(o => o.productId == barInfo.Id && o.interval == index.Key).DefaultIfEmpty()
                                        from iCost in productMiscCostQuery.Where(i=> i.productId == barInfo.Id && i.interval == index.Key).DefaultIfEmpty()
                                        let psCost = sCost == null ? 0 : sCost.reletiveCost
                                        let pmCost = mCost == null ? 0 : sCost.reletiveCost
                                        let paCost = aCost == null ? 0 : sCost.reletiveCost
                                        let poCost = oCost == null ? 0 : sCost.reletiveCost
                                        let inCost = iCost == null ? 0 : iCost.miscCost
                                        select new { index.Key, psCost, pmCost, paCost, poCost, inCost };

                var query = from interval in intervalTotalCost
                            group interval by interval.Key into g
                            let productCost = g.Sum(item => item == null ? 0 : item.psCost + item.pmCost + item.poCost + item.paCost + item.inCost)
                            select new { interval = g.Key, value = productCost??0 };

                var results = query.ToList();

                for (int i = startIdx; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in results)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Id = barInfo.Id;
                            newRecord.Value = line.value;
                            newRecord.StartDate = barInfo.StartDate;
                            newRecord.EndDate = barInfo.EndDate;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetCostsByProducts(CostBarInfo indexInfo, int startIdx, int count)
        {
            IList<Record> records = new List<Record>();

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);
                var machineRepository = new Repository<Machine>(context);
                var ssamRepository = new Repository<StateStationActivityMachine>(context);
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var operatorRepository = new Repository<Operator>(context);
                var activityRepository = new Repository<Activity>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var costList = costRepository.GetAll().ToList();
                var costCenterList = costCenterRepository.GetAll().ToList();
                var machineList = machineRepository.GetAll().ToList();
                var ssamList = ssamRepository.GetAll().ToList();
                var processList = processRepository.GetAll().ToList();
                var processReportList = processReportRepository.GetAll().ToList();
                var taskReportList = taskReportRepository.GetAll().ToList();
                var processOperatortList = processOperatortRepository.GetAll().ToList();
                var operatorList = operatorRepository.GetAll().ToList();
                var ssaList = ssaRepository.GetAll().ToList();
                var activityList = activityRepository.GetAll().ToList();
                var stateStationList = stateStationRepository.GetAll().ToList();
                var stationList = stationRepository.GetAll().ToList();
                var stateList = stateRepository.GetAll().ToList();
                var fpcList = fpcRepository.GetAll().ToList();
                var productList = productRepository.GetAll().ToList();

                // List of all products in range
                var indexList = productList.Skip(startIdx).Take(count).Select((p, index) => new { interval = index, p.Code, p.Id, p.Name });

                // Cost lists
                var stationQuery = from station in stationList
                                   from cost in costList.Where(c => c.Station != null && station != null && c.Station.Id == station.Id).DefaultIfEmpty()
                                   select new { station.Id, cost.CostValue };

                var machineQuery = from machine in machineList
                                   from cost in costList.Where(c => c.Machine != null && machine != null && c.Machine.Id == machine.Id).DefaultIfEmpty()
                                   select new { machine.Id, cost.CostValue };

                var activityQuery = from activity in activityList
                                    from cost in costList.Where(c => c.Activity != null && activity != null && c.Activity.Id == activity.Id).DefaultIfEmpty()
                                    select new { activity.Id, cost.CostValue };

                var operatorQuery = from opr in operatorList
                                    from cost in costList.Where(c => c.Operator != null && opr != null && c.Operator.Id == opr.Id).DefaultIfEmpty()
                                    select new { opr.Id, cost.CostValue };

                
                var miscQuery = from costCenter in costCenterList.Where(cc=> cc.SourceType == (decimal) CostSourceType.Other)
                                    from cost in costList.Where(c=> c.CostCenter != null && costCenter != null 
                                        && c.CostCenter.Id == costCenter.Id).DefaultIfEmpty()
                                select new { cost.Id, cost.CostValue };

                // Total cost lists
                var stationCostQuery = from station in stationQuery
                                       group station by station.Id into g
                                       select new { sId = g.Key, totalCost = g.Sum(item => item.CostValue ?? 0) };

                var machineCostQuery = from machine in machineQuery
                                       group machine by machine.Id into g
                                       select new { mId = g.Key, totalCost = g.Sum(item => item.CostValue ?? 0) };

                var activityCostQuery = from activity in activityQuery
                                        group activity by activity.Id into g
                                        select new { aId = g.Key, totalCost = g.Sum(item => item.CostValue ?? 0) };

                var operatorCostQuery = from opr in operatorQuery
                                        group opr by opr.Id into g
                                        select new { oId = g.Key, totalCost = g.Sum(item => item.CostValue ?? 0) };

                // Products duration
                var productDurationQuery = from taskReport in taskReportList
                                           from processReport in processReportList.Where(pr =>  taskReport != null ).DefaultIfEmpty()
                                           from process in processList.Where(p => processReport != null && processReport.Process != null && p.Id == processReport.Process.Id).DefaultIfEmpty()
                                           from processOpr in processOperatortList.Where(po => process != null && po.Process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                           from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                           from ssaMachine in ssamList.Where(ssam => ssActivity != null && ssam.StateStationActivity != null && ssActivity.Id == ssam.StateStationActivity.Id).DefaultIfEmpty()
                                           from stateStation in stateStationList.Where(ss => ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                           from state in stateList.Where(s => stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                           from fpc in fpcList.Where(f => state != null && state.FPC != null && state.FPC.Id == f.Id).DefaultIfEmpty()
                                           from product in productList.Where(p => fpc != null && fpc.Product != null && p.Id == fpc.Product.Id).DefaultIfEmpty()
                                           let productId = product == null ? -1 : product.Id
                                           let stationId = stateStation == null ? -1 : (stateStation.Station == null ? -1 : stateStation.Station.Id)
                                           let machineId = ssaMachine == null ? -1 : (ssaMachine.Machine == null ? -1 : ssaMachine.Machine.Id)
                                           let activityId = ssActivity == null ? -1 : (ssActivity.Activity == null ? -1 : ssActivity.Activity.Id)
                                           let operatorId = processOpr == null ? -1 : (processOpr.Operator == null ? -1 : processOpr.Operator.Id)
                                           let start = taskReport == null ? DateTime.MinValue : taskReport.ReportStartDateTime
                                           let end = taskReport == null ? DateTime.MinValue : taskReport.ReportEndDateTime
                                           let duration = taskReport == null ? 0 : taskReport.ReportDurationSeconds
                                           select new { productId, stationId, machineId, activityId, operatorId, start, end, duration };

                // Cost centers totals
                var stationCostByProductQuery = from product in productDurationQuery
                                                from cost in stationCostQuery.Where(c => c.sId == product.stationId).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let stationId = product == null ? -1 : product.stationId
                                                let productDuration = product == null ? 0 : product.duration
                                                let stationCost = cost == null ? 0 : cost.totalCost
                                                select new { productId, stationId, productDuration, stationCost };

                var machineCostByProductQuery = from product in productDurationQuery
                                                from cost in machineCostQuery.Where(c => c.mId == product.machineId).DefaultIfEmpty()
                                                let productId = product == null ? -1 : product.productId
                                                let machineId = product == null ? -1 : product.machineId
                                                let productDuration = product == null ? 0 : product.duration
                                                let machineCost = cost == null ? 0 : cost.totalCost
                                                select new { productId, machineId, productDuration, machineCost };

                var activityCostByProductQuery = from product in productDurationQuery
                                                 from cost in activityCostQuery.Where(c => c.aId == product.activityId).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let activityId = product == null ? -1 : product.activityId
                                                 let productDuration = product == null ? 0 : product.duration
                                                 let activityCost = cost == null ? 0 : cost.totalCost
                                                 select new { productId, activityId, productDuration, activityCost };

                var operatorCostByProductQuery = from product in productDurationQuery
                                                 from cost in operatorCostQuery.Where(c => c.oId == product.operatorId).DefaultIfEmpty()
                                                 let productId = product == null ? -1 : product.productId
                                                 let operatorId = product == null ? -1 : product.operatorId
                                                 let productDuration = product == null ? 0 : product.duration
                                                 let operatorCost = cost == null ? 0 : cost.totalCost
                                                 select new { productId, operatorId, productDuration, operatorCost };


                // Cost centers durations
                var stationDurationQuery = from product in stationCostByProductQuery
                                           group product by product.stationId into g
                                           let stationDuration = g.Sum(item => item.productDuration)
                                           select new { stationId = g.Key, stationDuration };

                var machineDurationQuery = from product in machineCostByProductQuery
                                           group product by product.machineId into g
                                           let machineDuration = g.Sum(item => item.productDuration)
                                           select new { machineId = g.Key, machineDuration };

                var activityDurationQuery = from product in activityCostByProductQuery
                                            group product by product.activityId into g
                                            let activityDuration = g.Sum(item => item.productDuration)
                                            select new { activityId = g.Key, activityDuration };

                var operatorDurationQuery = from product in operatorCostByProductQuery
                                            group product by product.operatorId into g
                                            let operatorDuration = g.Sum(item => item.productDuration)
                                            select new { operatorId = g.Key, operatorDuration };

                var productSumDurationQuery = from product in productDurationQuery
                                        group product by product.productId into g
                                        select new { g.Key, productDuration = g.Sum(item=> item.duration) };


                // Products costs
                var productStationCostQuery = from station in stationDurationQuery
                                              from product in stationCostByProductQuery.Where(p => station != null && p.stationId == station.stationId).DefaultIfEmpty()
                                              let stationDuration = station == null ? 0 : station.stationDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.stationCost
                                              let reletiveCost = stationDuration == 0 ? 0 : totalCost * (productDuration / stationDuration)
                                              select new { product.productId, reletiveCost };

                var productMachineCostQuery = from machine in machineDurationQuery
                                              from product in machineCostByProductQuery.Where(p => machine != null && p.machineId == machine.machineId).DefaultIfEmpty()
                                              let machineDuration = machine == null ? 0 : machine.machineDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.machineCost
                                              let reletiveCost = machineDuration == 0 ? 0 : totalCost * (productDuration / machineDuration)
                                              select new { product.productId, reletiveCost };

                var productActivityCostQuery = from activity in activityDurationQuery
                                              from product in activityCostByProductQuery.Where(p => activity != null && p.activityId == activity.activityId).DefaultIfEmpty()
                                              let activityDuration = activity == null ? 0 : activity.activityDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.activityCost
                                              let reletiveCost = activityDuration == 0 ? 0 : totalCost * (productDuration / activityDuration)
                                              select new { product.productId, reletiveCost };

                var productOperatorCostQuery = from opr in operatorDurationQuery
                                              from product in operatorCostByProductQuery.Where(p => opr != null && p.operatorId == opr.operatorId).DefaultIfEmpty()
                                              let operatorDuration = opr == null ? 0 : opr.operatorDuration
                                              let productDuration = product == null ? 0 : product.productDuration
                                              let totalCost = product == null ? 0 : product.operatorCost
                                              let reletiveCost = operatorDuration == 0 ? 0 : totalCost * (productDuration / operatorDuration)
                                              select new { product.productId, reletiveCost };

                var totalDuration = productSumDurationQuery.Sum(item => item == null ? 0 : item.productDuration);
                var avgMiscCost = miscQuery.Sum(item => item == null ? 0 : item.CostValue ?? 0);
                var productMiscCostQuery = from product in productSumDurationQuery
                                           let miscCost = totalDuration == 0 || product == null ? 0 : avgMiscCost * (product.productDuration / totalDuration)
                                           select new { productId = product.Key, miscCost };

                // Product total cost
                var productTotalCost = from index in indexList
                                       from sCost in productStationCostQuery.Where(s => s.productId == index.Id).DefaultIfEmpty()
                                       from mCost in productMachineCostQuery.Where(m => m.productId == index.Id).DefaultIfEmpty()
                                       from aCost in productActivityCostQuery.Where(a => a.productId == index.Id).DefaultIfEmpty()
                                       from oCost in productOperatorCostQuery.Where(o => o.productId == index.Id).DefaultIfEmpty()
                                       from cCost in productMiscCostQuery.Where(c => c.productId == index.Id).DefaultIfEmpty()
                                       let psCost = sCost == null ? 0 : sCost.reletiveCost
                                       let pmCost = mCost == null ? 0 : sCost.reletiveCost
                                       let paCost = aCost == null ? 0 : sCost.reletiveCost
                                       let poCost = oCost == null ? 0 : sCost.reletiveCost
                                       let ccCost = cCost == null ? 0 : cCost.miscCost
                                       select new { index.Id, index.Code, index.Name, index.interval, psCost, pmCost, paCost, poCost, ccCost};

                var query = from product in productTotalCost
                            group product by new { product.interval, product.Id, product.Name } into g
                            let productCost = g.Sum(item => item == null ? 0 : item.psCost + item.pmCost + item.poCost + item.paCost + item.ccCost)
                            select new { g.Key.interval, g.Key.Id, g.Key.Name, value = productCost };

                var results = query.ToList();

                for (int i = startIdx; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in results)
                    {
                        if (line.interval + startIdx == i)
                        {
                            newRecord.Id = line.Id;
                            newRecord.Value = line.value;
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }

        private double GetMaxCostByDate(DateTimeIntervals intervalType, CostType type, int count)
        {
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, 0, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);

                var costList = costRepository.GetAll().ToList();

                var indexList = new List<KeyValuePair<int, double>>();

                for (int i = 0; i < count; i++)
                {
                    indexList.AddRange(from cost in costList
                                       where cost.Date >= currentInterval
                                       && cost.Date < AddInterval(currentInterval, 1, intervalType)
                                       && (type == CostType.All || cost.CostType == (decimal)type)
                                       select new KeyValuePair<int, double>(i, cost.CostValue ?? 0));
                    indexList.Add(new KeyValuePair<int, double>(i, 0));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var query = from index in indexList
                            group index by index.Key into g
                            let total = g.Sum(item => item.Value)
                            select new { interval = g.Key, value = total };

                return query.Max(item => item.value);
            }
        }
        private double GetMaxCostByCostCenters(CostBarInfo indexInfo, CostType type, int count)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);

                var costList = costRepository.GetAll().ToList();
                var costCenterList = costCenterRepository.GetAll().ToList();

                var indexList = costCenterList.Select((cc, index) => new { interval = index, cc.Id, cc.Name });

                var ccQuery = from index in indexList
                              from cost in costList.Where(c => c.CostCenter != null && c.CostCenter.Id == index.Id
                                  && c.Date >= indexInfo.StartDate && c.Date < indexInfo.EndDate
                                  && (type == (decimal)CostType.All || c.CostType == (decimal)type)).DefaultIfEmpty()
                              select new { index.interval, index.Id, index.Name, Value = cost == null ? 0 : cost.CostValue ?? 0 };

                var query = from costCenter in ccQuery
                            group costCenter by new { costCenter.interval, costCenter.Id, costCenter.Name } into g
                            let total = g.Sum(item => item.Value)
                            select new { g.Key.interval, g.Key.Id, g.Key.Name, value = total };

                return query.Max(item => item.value);
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
        private long GetIntervalTicks(DateTime currentDate, int value, DateTimeIntervals type)
        {
            switch (type)
            {
                case DateTimeIntervals.Hourly:
                    return (currentDate - currentDate.AddHours(value)).Ticks;
                case DateTimeIntervals.Shiftly:
                    return (currentDate - currentDate.AddHours((24 / SoheilConstants.ShiftPerDay) * value)).Ticks;
                case DateTimeIntervals.Daily:
                    return (currentDate - currentDate.AddDays(value)).Ticks;
                case DateTimeIntervals.Weekly:
                    return (currentDate - currentDate.AddDays(value * 7)).Ticks;
                case DateTimeIntervals.Monthly:
                    return (currentDate - currentDate.AddMonths(value)).Ticks;
                default:
                    return (currentDate - currentDate.AddMonths(value)).Ticks;
            }
        }


    }
}