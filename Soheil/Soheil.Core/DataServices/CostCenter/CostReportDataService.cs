
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
    public class CostReportDataService
    {
        public IList<Record> GetInRange(CostType type, DateTimeIntervals intervalType, CostBarInfo barInfo, int startIdx, int count)
        {

            switch (barInfo.Level)
            {
                case 0:
                    return GetCostsByDate(intervalType, type, startIdx, count);
                case 1:
                    return GetCostsByCostCenters(barInfo, type, startIdx, count);
                case 2:
                    return GetCostsByType(barInfo, type, startIdx, count);
            }
            return new List<Record>();
        }
        public double GetMax(CostType type, DateTimeIntervals intervalType, CostBarInfo barInfo, int count)
        {
            switch (barInfo.Level)
            {
                case 0:
                    return GetMaxCostByDate(intervalType, type, count);
                case 1:
                    return GetMaxCostByCostCenters(barInfo, type, count);
                case 2:
                    return GetMaxCostByType(barInfo, type, count);
                default:
                    return 0;
            }
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

        private IList<Record> GetCostsByDate(DateTimeIntervals intervalType, CostType type, int startIdx, int count)
        {
            IList<Record> records = new List<Record>();
            int currentYear = DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            startDate = AddInterval(startDate, startIdx, intervalType);
            var currentInterval = startDate;

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);

                var costList = costRepository.GetAll();

                var indexList = new List<KeyValuePair<int, double>>();

                for (int i = startIdx; i < count; i++)
                {
                    indexList.AddRange(from cost in costList
                                      where cost.Date >= currentInterval
                                      && cost.Date < AddInterval(currentInterval, 1, intervalType)
                                      && (type == CostType.All || cost.CostType == (decimal) type)
                                      select new KeyValuePair<int, double>(i, cost.CostValue?? 0));
                    indexList.Add(new KeyValuePair<int, double>(i, 0));
                    currentInterval = AddInterval(currentInterval, 1, intervalType);
                }

                var query = from index in indexList
                            group index by index.Key into g
                            let total = g.Sum(item => item.Value)
                            select new { interval = g.Key, value = total};

                for (int i = startIdx; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
                    {
                        if (line.interval == i)
                        {
                            newRecord.Id = line.interval;
                            newRecord.Value = line.value;
                            newRecord.StartDate = AddInterval(startDate, i - startIdx, intervalType);
                            newRecord.EndDate = AddInterval(startDate, i - startIdx + 1, intervalType);
                        }
                    }
                    records.Add(newRecord);
                }
            }
            return records;
        }
        private IList<Record> GetCostsByCostCenters(CostBarInfo indexInfo, CostType type, int startIdx, int count)
        {
            IList<Record> records = new List<Record>();

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costCenterRepository = new Repository<CostCenter>(context);

                var costList = costRepository.GetAll();
                var costCenterList = costCenterRepository.GetAll();

                var indexList = costCenterList.Skip(startIdx).Take(count).Select((cc, index) => new { interval = index, cc.Id, cc.Name });

                var ccQuery = from index in indexList
                            from cost in costList.Where(c => c.CostCenter != null && c.CostCenter.Id == index.Id
                                && c.Date >= indexInfo.StartDate && c.Date < indexInfo.EndDate
                                && (type == (decimal)CostType.All || c.CostType == (decimal)type)).DefaultIfEmpty()
                            select new {index.interval, index.Id, index.Name , Value = cost == null ? 0 : cost.CostValue?? 0 };

                var query = from costCenter in ccQuery
                            group costCenter by new {costCenter.interval, costCenter.Id, costCenter.Name } into g
                            let total = g.Sum(item => item.Value)
                            select new {g.Key.interval, g.Key.Id, g.Key.Name, value = total };

                for (int i = startIdx; i < count; i++)
                {
                    var newRecord = new Record { Header = i.ToString(CultureInfo.InvariantCulture) };
                    foreach (var line in query)
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
        private IList<Record> GetCostsByType(CostBarInfo indexInfo, CostType type, int startIdx, int count)
        {
            IList<Record> records = new List<Record>();

            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costList = costRepository.Find(c=> c.CostCenter != null && c.CostCenter.Id == indexInfo.Id);

                var indexList = costList.Skip(startIdx).Take(count).Select((c, index) => new { interval = index, c.Id, c.CostType, c.Description, c.Date, c.CostValue });

                var query = from index in indexList.Where(idx => idx.Date >= indexInfo.StartDate && idx.Date < indexInfo.EndDate
                                && (type == CostType.All || idx.CostType == (decimal)type))
                            select new {index.interval, index.Id, index.CostValue, index.Description };

                var results = query.ToList();

                for (int i = startIdx; i < count; i++)
                {
                    if (i - startIdx >= results.Count)
                    {
                        records.Add(new Record());
                    }
                    else
                    {
                        var newRecord = new Record
                            {
                                Id = results[i - startIdx].Id,
                                Header = results[i - startIdx].Description,
                                Value = results[i - startIdx].CostValue ?? 0,
                                StartDate = indexInfo.StartDate,
                                EndDate = indexInfo.EndDate
                            };
                        records.Add(newRecord);
                    }
                }
                //foreach (var result in results)
                //{
                //    var newRecord = new Record
                //    {
                //        Id = result.Id,
                //        Header = result.Description,
                //        Value = result.CostValue ?? 0,
                //        StartDate = indexInfo.StartDate,
                //        EndDate = indexInfo.EndDate
                //    };
                //    records.Add(newRecord);
                //}
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

                var costList = costRepository.GetAll();

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

                var costList = costRepository.GetAll();
                var costCenterList = costCenterRepository.GetAll();

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
        private double GetMaxCostByType(CostBarInfo indexInfo, CostType type, int count)
        {
            using (var context = new SoheilEdmContext())
            {
                var costRepository = new Repository<Cost>(context);
                var costList = costRepository.Find(c => c.CostCenter != null && c.CostCenter.Id == indexInfo.Id);

                var indexList = costList.Select((c, index) => new { interval = index, c.Id, c.CostType, c.Description, c.Date, c.CostValue });

                var query = from index in indexList.Where(idx => idx.Date >= indexInfo.StartDate && idx.Date < indexInfo.EndDate
                                && (type == CostType.All || idx.CostType == (decimal)type))
                            select new { index.interval, index.Id, index.CostValue, index.Description };

                var max = query.Max(item => item.CostValue);
                if (max != null) return (double) max;
            }
            return 0;
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