
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Index;
using Soheil.Core.Reports;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class IndexDataService
    {
        public IList<Record> GetInRange(IndexType type, DateTimeIntervals intervalType, IndexBarInfo barInfo, int startIndex, int count)
        {
            switch (type)
            {
                case IndexType.None:
                    break;
                case IndexType.OEE:
                    switch (barInfo.Level)
                    {
                        case 0:
                            return GetOEEByDateTime(intervalType, startIndex, count);
                        case 1:
                            return GetOEEByMachines(barInfo, startIndex, count);
                    }
                    break;
                case IndexType.Performance:
                    if (barInfo.Level == 0)
                    {
                        return GetPerformanceByDateTime(intervalType, startIndex, count);
                    }
                    switch (barInfo.Filter)
                    {
                        case IndexFilter.ByProduct:
                            return GetPerformanceByProducts(barInfo, startIndex, count);
                        case IndexFilter.ByStation:
                            return GetPerformanceByStations(barInfo, startIndex, count);
                        case IndexFilter.ByActivity:
                            return GetPerformanceByActivities(barInfo, startIndex, count);
                        case IndexFilter.ByOperator:
                            return GetPerformanceByOperators(barInfo, startIndex, count);
                        default:
                            return new List<Record>();
                    }
                case IndexType.InternalPPM:
                    switch (barInfo.Level)
                    {
                        case 0:
                            return GetPPMByDateTime(intervalType, startIndex, count);
                    }
                    break;
                case IndexType.RemainingCapacity:
                    switch (barInfo.Level)
                    {
                        case 0:
                            return GetCapacityByDateTime(intervalType, startIndex, count);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            return new List<Record>();
        }
        public int GetMachineCount()
        {
            int machineCount;
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                machineCount = machineRepository.GetAll().Count();
            }
            return machineCount;
        }
        public int GetProductCount()
        {
            int productCount;
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                productCount = productRepository.GetAll().Count();
            }
            return productCount;
        }
        public int GetStationCount()
        {
            int stationCount;
            using (var context = new SoheilEdmContext())
            {
                var stationRepository = new Repository<Station>(context);
                stationCount = stationRepository.GetAll().Count();
            }
            return stationCount;
        }
        public int GetActivityCount()
        {
            int activityCount;
            using (var context = new SoheilEdmContext())
            {
                var activityRepository = new Repository<Activity>(context);
                activityCount = activityRepository.GetAll().Count();
            }
            return activityCount;
        }
        public int GetOperatorCount()
        {
            int operatorCount;
            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<Operator>(context);
                operatorCount = operatorRepository.GetAll().Count();
            }
            return operatorCount;
        }

        private IList<Record> GetOEEByDateTime(DateTimeIntervals intervalType, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIndex, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);

                var ssaList = ssaRepository.GetAll();
                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();

                var indexList = new List<KeyValuePair<int, int>>();

                for (int i = startIndex; i < count; i++)
                {
                    indexList.AddRange(from taskReport in taskReportList
                                      where taskReport.ReportStartDateTime >= currentInterval
                                      && taskReport.ReportStartDateTime < AddInterval(currentInterval, 1, intervalType)
                                      select new KeyValuePair<int, int>(i, taskReport.Id));
                    indexList.Add(new KeyValuePair<int, int>(i, -1));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var processReportQuery = from ssActivity in ssaList
                              from process in processList.Where(pr => pr.StateStationActivity != null && ssActivity != null && pr.StateStationActivity.Id == ssActivity.Id).DefaultIfEmpty()
                              from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                              let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                              let cycleTime = ssActivity == null ? 0 : ssActivity.CycleTime
                              let targetCount = process == null ? 0 : process.TargetCount
                              let prId = processReport == null? -1 : processReport.Id
                              let trId = processReport == null ? -1 : (0)
                              select new { prId, trId, fValue = cycleTime * producedG1, bValue = cycleTime * targetCount };

                var query = from index in indexList
                            from processReport in processReportQuery.Where(pr => pr.trId == index.Value).DefaultIfEmpty()
                            group processReport by  index.Key into g
                            let sumf = g.Sum(item => item == null? 0 : item.fValue)
                            let sumb = g.Sum(item => item == null ? 0 : item.bValue)
                            select new { interval = g.Key, value = sumb == 0? 0 : sumf/sumb };

                var results = query.ToList();
                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in results)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = AddInterval(startDate, i - startIndex, intervalType);
                            newRecord.EndDate = AddInterval(startDate, i - startIndex + 1, intervalType);
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetOEEByMachines(IndexBarInfo indexInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();

            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                var selectedMachineRepository = new Repository<SelectedMachine>(context);
                var ssamRepository = new Repository<StateStationActivityMachine>(context);
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);

                var machineList = machineRepository.GetAll();
                var selectedMachineList = selectedMachineRepository.GetAll();
                var ssamList = ssamRepository.GetAll();
                var ssaList = ssaRepository.GetAll();
                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();

                var indexList = machineList.Skip(startIndex).Take(count).Select((m, index) => new { interval = index, m.Id, m.Code, m.Name });

                var pQuery = from ssamachine in ssamList
                            from selmachine in selectedMachineList.Where(sm => ssamachine != null && sm.StateStationActivityMachine != null && sm.StateStationActivityMachine.Id == ssamachine.Id).DefaultIfEmpty()
                            from process in processList.Where(p => selmachine != null && selmachine.Process != null && p.Id == selmachine.Process.Id).DefaultIfEmpty()
                            from ssActivity in ssaList.Where(ssa=> process!=null && process.StateStationActivity!=null && process.StateStationActivity.Id == ssa.Id).DefaultIfEmpty()
                            let mId = ssamachine == null ? -1 : (ssamachine.Machine == null? -1 : ssamachine.Machine.Id)
                            let pId = process == null ? -1 : process.Id
                            let cycleTime = ssActivity == null ? 0 : ssActivity.CycleTime
                            let targetCount = process == null ? 0 : process.TargetCount
                            select new { pId, mId, targetCount, cycleTime };

                var prQuery = from process in pQuery
                              from processReport in processReportList.Where(pr => process != null && pr.Process != null && pr.Process.Id == process.pId).DefaultIfEmpty()
                              from taskReport in taskReportList.Where(tr=>  tr.ReportStartDateTime >= indexInfo.StartDate && tr.ReportEndDateTime < indexInfo.EndDate).DefaultIfEmpty()
                              let mId = process == null ? -1 : process.mId
                              let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                              let cycleTime = process == null ? 0 : process.cycleTime
                              let targetCount = process == null ? 0 : process.targetCount
                              select new { mId, fValue = cycleTime * producedG1, bValue = cycleTime * targetCount };

                var query = from machine in indexList
                             from pr in prQuery.Where(p => p.mId == machine.Id).DefaultIfEmpty()
                             group pr by new { machine.interval, machine.Id, machine.Code, machine.Name } into g
                             let sumf = g.Sum(item => item == null ? 0 : item.fValue)
                             let sumb = g.Sum(item => item == null ? 0 : item.bValue)
                             select new {g.Key.interval, mId = g.Key.Id, mCode = g.Key.Code, mName = g.Key.Name, value = sumb == 0 ? 0 : sumf / sumb };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record();
                    foreach (var line in query)
                    {
                        if (line.interval + startIndex == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.mName;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }

        private IList<Record> GetPPMByDateTime(DateTimeIntervals intervalType, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIndex, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var defectionReportRepository = new Repository<DefectionReport>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);

                var ssaList = ssaRepository.GetAll();
                var processList = processRepository.GetAll();
                var defectionReportList = defectionReportRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();

                var indexList = new List<KeyValuePair<int, int>>();

                for (int i = startIndex; i < count; i++)
                {
                    indexList.AddRange(from taskReport in taskReportList
                                       where taskReport.ReportStartDateTime >= currentInterval
                                       && taskReport.ReportStartDateTime < AddInterval(currentInterval, 1, intervalType)
                                       select new KeyValuePair<int, int>(i, taskReport.Id));
                    indexList.Add(new KeyValuePair<int, int>(i, -1));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var drQuery = from ssActivity in ssaList
                              from process in processList.Where(pr => pr.StateStationActivity != null && ssActivity != null && pr.StateStationActivity.Id == ssActivity.Id).DefaultIfEmpty()
                              from processReport in processReportList
                              from defectionReport in defectionReportList.Where(dr => dr.ProcessReport != null && processReport != null && dr.ProcessReport.Id == processReport.Id).DefaultIfEmpty()
                              let trId = processReport == null ? -1 : 0
                              let prId = processReport == null ? -1 : processReport.Id
                              let cycleTime = ssActivity == null ? 0 : ssActivity.CycleTime
                              let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                              let lostCount = defectionReport == null ? 0 : defectionReport.LostCount
                              let lostTime = defectionReport == null ? 0 : defectionReport.LostTime
                              let lostTotal = cycleTime == 0? lostCount : lostCount + lostTime/cycleTime
                              select new { trId, prId, producedG1, lostTotal };

                var query = from index in indexList
                            from pr in drQuery.Where(dr => dr.trId == index.Value).DefaultIfEmpty()
                            group pr by index.Key into g
                            let sumPg1 = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumLc = g.Sum(item => item == null ? 0 : item.lostTotal)
                            select new { interval = g.Key, value = sumLc + sumPg1 == 0 ? 0 : sumLc / (sumLc + sumPg1) };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = AddInterval(startDate, i - startIndex, intervalType);
                            newRecord.EndDate = AddInterval(startDate, i - startIndex + 1, intervalType);
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }

        private IList<Record> GetCapacityByDateTime(DateTimeIntervals intervalType, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIndex, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var ssaRepository = new Repository<StateStationActivity>(context);
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);

                var ssaList = ssaRepository.GetAll();
                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();

                var indexList = new List<KeyValuePair<int, int>>();

                for (int i = startIndex; i < count; i++)
                {
                    indexList.AddRange(from taskReport in taskReportList
                                       where taskReport.ReportStartDateTime >= currentInterval
                                       && taskReport.ReportStartDateTime < AddInterval(currentInterval, 1, intervalType)
                                       select new KeyValuePair<int, int>(i, taskReport.Id));
                    indexList.Add(new KeyValuePair<int, int>(i, -1));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var processReportQuery = from ssActivity in ssaList
                                         from process in processList.Where(pr => pr.StateStationActivity != null && ssActivity != null && pr.StateStationActivity.Id == ssActivity.Id).DefaultIfEmpty()
                                         from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                         let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                         let cycleTime = ssActivity == null ? 0 : ssActivity.CycleTime
                                         let prId = processReport == null ? -1 : processReport.Id
                                         let trId = processReport == null ? -1 : (0)
                                         select new { prId, trId, producedG1, cycleTime};

                var query = from index in indexList
                            from processReport in processReportQuery.Where(pr => pr.trId == index.Value).DefaultIfEmpty()
                            group processReport by index.Key into g
                            let sumf = g.Sum(item => item == null ? 0 : item.producedG1 * item.cycleTime)
                            select new { interval = g.Key, value = sumf / 3600 };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = AddInterval(startDate, i - startIndex, intervalType);
                            newRecord.EndDate = AddInterval(startDate, i - startIndex + 1, intervalType);
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }

        private IList<Record> GetPerformanceByDateTime(DateTimeIntervals intervalType, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIndex, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);

                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();

                var indexList = new List<KeyValuePair<int, int>>();

                for (int i = startIndex; i < count; i++)
                {
                    indexList.AddRange(from taskReport in taskReportList
                                       where taskReport.ReportStartDateTime >= currentInterval
                                       && taskReport.ReportStartDateTime < AddInterval(currentInterval, 1, intervalType)
                                       select new KeyValuePair<int, int>(i, taskReport.Id));
                    indexList.Add(new KeyValuePair<int, int>(i, -1));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var processReportQuery = from process in processList
                                         from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                         let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                         let targetCount = process == null ? 0 : process.TargetCount
                                         let prId = processReport == null ? -1 : processReport.Id
                                         let trId = processReport == null ? -1 : (0)
                                         select new { prId, trId, producedG1, targetCount };

                var query = from index in indexList
                            from processReport in processReportQuery.Where(pr => pr.trId == index.Value).DefaultIfEmpty()
                            group processReport by index.Key into g
                            let sumPg = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumTc = g.Sum(item => item == null ? 0 : item.targetCount)
                            select new { interval = g.Key, value = sumTc == 0 ? 0 : sumPg / sumTc };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = AddInterval(startDate, i - startIndex, intervalType);
                            newRecord.EndDate = AddInterval(startDate, i - startIndex + 1, intervalType);
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetPerformanceByProducts(IndexBarInfo indexInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            using (var context = new SoheilEdmContext())
            {
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var operatorRepository = new Repository<Operator>(context);
                var ssaRepositoryRepository = new Repository<StateStationActivity>(context);
                var activityRepository = new Repository<Activity>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();
                var processOperatortList = processOperatortRepository.GetAll();
                var operatorList = indexInfo.OperatorId > 0 ? operatorRepository.Find(item=> item.Id == indexInfo.OperatorId) : operatorRepository.GetAll();
                var ssaList = ssaRepositoryRepository.GetAll();
                var activityList = indexInfo.ActivityId > 0 ? activityRepository.Find(item=>item.Id == indexInfo.ActivityId) : activityRepository.GetAll();
                var stateStationList = stateStationRepository.GetAll();
                var stationList = indexInfo.StationId > 0 ? stationRepository.Find(item=>item.Id == indexInfo.StationId) : stationRepository.GetAll();
                var stateList = stateRepository.GetAll();
                var fpcList = fpcRepository.GetAll();
                var productList = productRepository.GetAll();

                var indexList = productList.Skip(startIndex).Take(count).Select((p, index) => new { interval = index, p.Id, p.Code, p.Name });

                var productQuery = from process in processList
                                         from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                         from taskReport in taskReportList.Where(tr=>  tr.ReportStartDateTime >= indexInfo.StartDate && tr.ReportEndDateTime < indexInfo.EndDate).DefaultIfEmpty()
                                         from ssActivity in ssaList.Where(ssa => process!= null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                         from activity in activityList.Where(a=> ssActivity != null && ssActivity.Activity != null && ssActivity.Activity.Id == a.Id).DefaultIfEmpty()
                                         from pOpr in processOperatortList.Where(po=> po.Process != null && process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                         from opr in operatorList.Where(o=> pOpr != null && pOpr.Operator != null && pOpr.Operator.Id == o.Id).DefaultIfEmpty()
                                         from stateStation in stateStationList.Where(ss=> ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                         from station in stationList.Where(s=> stateStation != null && stateStation.Station != null && s.Id == stateStation.Station.Id).DefaultIfEmpty()
                                         from state in stateList.Where(s=> stateStation != null && stateStation.State !=null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                         from fpc in fpcList.Where(f=> state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                                         let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                         let targetCount = process == null ? 0 : process.TargetCount
                                         let pId = fpc == null ? -1 : (fpc.Product == null ? -1 : fpc.Product.Id)
                                         let prId = processReport == null ? -1 : processReport.Id
                                         let trId = taskReport == null ? -1 : taskReport.Id
                                         select new { pId, prId, trId, producedG1, targetCount };

                var query = from index in indexList
                            from product in productQuery.Where(p => p.pId == index.Id).DefaultIfEmpty()
                            group product by new {index.Id , index.interval, index.Code, index.Name} into g
                            let sumPg = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumTc = g.Sum(item => item == null ? 0 : item.targetCount)
                            select new { g.Key, value = sumTc == 0 ? 0 : sumPg / sumTc };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.Key.interval + startIndex == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.Key.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetPerformanceByStations(IndexBarInfo indexInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            using (var context = new SoheilEdmContext())
            {
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var operatorRepository = new Repository<Operator>(context);
                var ssaRepositoryRepository = new Repository<StateStationActivity>(context);
                var activityRepository = new Repository<Activity>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();
                var processOperatortList = processOperatortRepository.GetAll();
                var operatorList = indexInfo.OperatorId > 0 ? operatorRepository.Find(item => item.Id == indexInfo.OperatorId) : operatorRepository.GetAll();
                var ssaList = ssaRepositoryRepository.GetAll();
                var activityList = indexInfo.ActivityId > 0 ? activityRepository.Find(item => item.Id == indexInfo.ActivityId) : activityRepository.GetAll();
                var stateStationList = stateStationRepository.GetAll();
                var stationList = stationRepository.GetAll();
                var stateList = stateRepository.GetAll();
                var fpcList = fpcRepository.GetAll();
                var productList = indexInfo.ProductId > 0 ? productRepository.Find(item => item.Id == indexInfo.ProductId) : productRepository.GetAll();

                var indexList = stationList.Skip(startIndex).Take(count).Select((p, index) => new { interval = index, p.Id, p.Code, p.Name });

                var stationQuery = from process in processList
                                   from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                   from taskReport in taskReportList.Where(tr =>  tr.ReportStartDateTime >= indexInfo.StartDate && tr.ReportEndDateTime < indexInfo.EndDate).DefaultIfEmpty()
                                   from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                   from activity in activityList.Where(a => ssActivity != null && ssActivity.Activity != null && ssActivity.Activity.Id == a.Id).DefaultIfEmpty()
                                   from pOpr in processOperatortList.Where(po => po.Process != null && process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                   from opr in operatorList.Where(o => pOpr != null && pOpr.Operator != null && pOpr.Operator.Id == o.Id).DefaultIfEmpty()
                                   from stateStation in stateStationList.Where(ss => ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                   from state in stateList.Where(s => stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                   from fpc in fpcList.Where(f => state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                                   from product in productList.Where(p=> fpc != null && fpc.Product != null && p.Id == fpc.Product.Id).DefaultIfEmpty()
                                   let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                   let targetCount = process == null ? 0 : process.TargetCount
                                   let sId = stateStation == null ? -1 : (stateStation.Station == null ? -1 : stateStation.Station.Id)
                                   let prId = processReport == null ? -1 : processReport.Id
                                   let trId = taskReport == null ? -1 : taskReport.Id
                                   select new { pId = sId, prId, trId, producedG1, targetCount };

                var query = from index in indexList
                            from station in stationQuery.Where(s => s.pId == index.Id).DefaultIfEmpty()
                            group station by new { index.Id, index.interval, index.Code, index.Name } into g
                            let sumPg = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumTc = g.Sum(item => item == null ? 0 : item.targetCount)
                            select new { g.Key, value = sumTc == 0 ? 0 : sumPg / sumTc };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.Key.interval + startIndex == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.Key.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetPerformanceByActivities(IndexBarInfo indexInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            using (var context = new SoheilEdmContext())
            {
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var operatorRepository = new Repository<Operator>(context);
                var ssaRepositoryRepository = new Repository<StateStationActivity>(context);
                var activityRepository = new Repository<Activity>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();
                var processOperatortList = processOperatortRepository.GetAll();
                var operatorList = indexInfo.OperatorId > 0 ? operatorRepository.Find(item => item.Id == indexInfo.OperatorId) : operatorRepository.GetAll();
                var ssaList = ssaRepositoryRepository.GetAll();
                var activityList = activityRepository.GetAll();
                var stateStationList = stateStationRepository.GetAll();
                var stationList = indexInfo.StationId > 0 ? stationRepository.Find(item => item.Id == indexInfo.StationId) : stationRepository.GetAll();
                var stateList = stateRepository.GetAll();
                var fpcList = fpcRepository.GetAll();
                var productList = indexInfo.ProductId > 0 ? productRepository.Find(item => item.Id == indexInfo.ProductId) : productRepository.GetAll();

                var indexList = activityList.Skip(startIndex).Take(count).Select((p, index) => new { interval = index, p.Id, p.Code, p.Name });

                var activityQuery = from process in processList
                                   from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                   from taskReport in taskReportList.Where(tr =>  tr.ReportStartDateTime >= indexInfo.StartDate && tr.ReportEndDateTime < indexInfo.EndDate).DefaultIfEmpty()
                                   from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                   from pOpr in processOperatortList.Where(po => po.Process != null && process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                   from opr in operatorList.Where(o => pOpr != null && pOpr.Operator != null && pOpr.Operator.Id == o.Id).DefaultIfEmpty()
                                   from stateStation in stateStationList.Where(ss => ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                   from station in stationList.Where(s => stateStation != null && stateStation.Station != null && s.Id == stateStation.Station.Id).DefaultIfEmpty()
                                   from state in stateList.Where(s => stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                   from fpc in fpcList.Where(f => state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                                   from product in productList.Where(p => fpc != null && fpc.Product != null && p.Id == fpc.Product.Id).DefaultIfEmpty()
                                   let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                   let targetCount = process == null ? 0 : process.TargetCount
                                   let aId = ssActivity == null ? -1 : (ssActivity.Activity == null ? -1 : ssActivity.Activity.Id)
                                   let prId = processReport == null ? -1 : processReport.Id
                                   let trId = taskReport == null ? -1 : taskReport.Id
                                   select new { pId = aId, prId, trId, producedG1, targetCount };

                var query = from index in indexList
                            from activity in activityQuery.Where(a => a.pId == index.Id).DefaultIfEmpty()
                            group activity by new { index.Id, index.interval, index.Code, index.Name } into g
                            let sumPg = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumTc = g.Sum(item => item == null ? 0 : item.targetCount)
                            select new { g.Key, value = sumTc == 0 ? 0 : sumPg / sumTc };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.Key.interval + startIndex == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.Key.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetPerformanceByOperators(IndexBarInfo indexInfo, int startIndex, int count)
        {
            IList<Record> records = new List<Record>();
            using (var context = new SoheilEdmContext())
            {
                var processRepository = new Repository<Process>(context);
                var processReportRepository = new Repository<ProcessReport>(context);
                var taskReportRepository = new Repository<TaskReport>(context);
                var processOperatortRepository = new Repository<ProcessOperator>(context);
                var operatorRepository = new Repository<Operator>(context);
                var ssaRepositoryRepository = new Repository<StateStationActivity>(context);
                var activityRepository = new Repository<Activity>(context);
                var stateStationRepository = new Repository<StateStation>(context);
                var stationRepository = new Repository<Station>(context);
                var stateRepository = new Repository<State>(context);
                var fpcRepository = new Repository<FPC>(context);
                var productRepository = new Repository<Product>(context);

                var processList = processRepository.GetAll();
                var processReportList = processReportRepository.GetAll();
                var taskReportList = taskReportRepository.GetAll();
                var processOperatortList = processOperatortRepository.GetAll();
                var operatorList = operatorRepository.GetAll();
                var ssaList = ssaRepositoryRepository.GetAll();
                var activityList = indexInfo.ActivityId > 0 ? activityRepository.Find(item => item.Id == indexInfo.ActivityId) : activityRepository.GetAll();
                var stateStationList = stateStationRepository.GetAll();
                var stationList = indexInfo.StationId > 0 ? stationRepository.Find(item => item.Id == indexInfo.StationId) : stationRepository.GetAll();
                var stateList = stateRepository.GetAll();
                var fpcList = fpcRepository.GetAll();
                var productList = indexInfo.ProductId > 0 ? productRepository.Find(item => item.Id == indexInfo.ProductId) : productRepository.GetAll();

                var indexList = operatorList.Skip(startIndex).Take(count).Select((p, index) => new { interval = index, p.Id, p.Code, p.Name });

                var operatorQuery = from process in processList
                                   from processReport in processReportList.Where(ssapr => ssapr.Process != null && process != null && ssapr.Process.Id == process.Id).DefaultIfEmpty()
                                   from taskReport in taskReportList.Where(tr =>  tr.ReportStartDateTime >= indexInfo.StartDate && tr.ReportEndDateTime < indexInfo.EndDate).DefaultIfEmpty()
                                   from ssActivity in ssaList.Where(ssa => process != null && process.StateStationActivity != null && ssa.Id == process.StateStationActivity.Id).DefaultIfEmpty()
                                   from activity in activityList.Where(a => ssActivity != null && ssActivity.Activity != null && ssActivity.Activity.Id == a.Id).DefaultIfEmpty()
                                   from pOpr in processOperatortList.Where(po => po.Process != null && process != null && po.Process.Id == process.Id).DefaultIfEmpty()
                                   from stateStation in stateStationList.Where(ss => ssActivity != null && ssActivity.StateStation != null && ss.Id == ssActivity.StateStation.Id).DefaultIfEmpty()
                                   from station in stationList.Where(s => stateStation != null && stateStation.Station != null && s.Id == stateStation.Station.Id).DefaultIfEmpty()
                                   from state in stateList.Where(s => stateStation != null && stateStation.State != null && s.Id == stateStation.State.Id).DefaultIfEmpty()
                                   from fpc in fpcList.Where(f => state != null && state.FPC != null && f.Id == state.FPC.Id).DefaultIfEmpty()
                                   from product in productList.Where(p => fpc != null && fpc.Product != null && p.Id == fpc.Product.Id).DefaultIfEmpty()
                                   let producedG1 = processReport == null ? 0 : processReport.ProducedG1
                                   let targetCount = process == null ? 0 : process.TargetCount
                                   let oId = pOpr == null ? -1 : (pOpr.Operator == null ? -1 : pOpr.Operator.Id)
                                   let prId = processReport == null ? -1 : processReport.Id
                                   let trId = taskReport == null ? -1 : taskReport.Id
                                   select new { pId = oId, prId, trId, producedG1, targetCount };

                var query = from index in indexList
                            from opr in operatorQuery.Where(o => o.pId == index.Id).DefaultIfEmpty()
                            group opr by new { index.Id, index.interval, index.Code, index.Name } into g
                            let sumPg = g.Sum(item => item == null ? 0 : item.producedG1)
                            let sumTc = g.Sum(item => item == null ? 0 : item.targetCount)
                            select new { g.Key, value = sumTc == 0 ? 0 : sumPg / sumTc };

                for (int i = startIndex; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.Key.interval + startIndex == i)
                        {
                            newRecord.Value = line.value;
                            newRecord.Value = 50; //???
                            newRecord.StartDate = indexInfo.StartDate;
                            newRecord.EndDate = indexInfo.EndDate;
                            newRecord.Header = line.Key.Name;
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
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