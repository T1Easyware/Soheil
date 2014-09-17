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
				var oprRepository = new Repository<OperatorProcessReport>(context);
                var osrRepository = new Repository<OperatorStoppageReport>(context);
                var odrRepository = new Repository<OperatorDefectionReport>(context);

				var workProfilePlanDs = new DataServices.WorkProfilePlanDataService(context);
				var start = workProfilePlanDs.GetShiftStartOn(startDate);
				var end = workProfilePlanDs.GetShiftStartOn(endDate);

				var oprList = oprRepository.Find(x =>
					x.ProcessReport.StartDateTime >= start && 
					x.ProcessReport.EndDateTime <= end &&
					x.ProcessOperator.Operator.Id == operatorId);

				var mainQuery = from opr in oprList

								let pr = opr.ProcessReport
								let ssa = pr.Process.StateStationActivity
								let product = ssa.StateStation.State.FPC.Product
								let productRework = ssa.StateStation.State.OnProductRework

								let prId = pr.Id
								let pdId = product.Id
								let pdCode = product.Code
								let pdName = product.Name
								let snId = ssa.StateStation.Station.Id
								let snName = ssa.StateStation.Station.Name
								let aId = ssa.Activity.Id
								let aCode = ssa.Activity.Code
								let aName = ssa.Activity.Name
								let rId = productRework == null ? -1 : (productRework.Rework == null ? -1 : productRework.Rework.Id)

								let ct = ssa.CycleTime
								let tp = pr.Process.TargetCount
								let g1 = opr.OperatorProducedG1
								let oc = pr.OperatorProcessReports.Count
								let stp = pr.StoppageReports.Sum(x => x.CountEquivalence)
								let def = pr.DefectionReports.Sum(x => x.CountEquivalence)
								let date = pr.StartDateTime
								select new
								{
									oprId = opr.Id,
									prId,
									pdId,
									pdCode,
									pdName,
									snId,
									snName,
									aId,
									aCode,
									aName,
									rId,

									ct,
									tp = oc == 0 ? 0 : tp / oc,
									g1,
									stp = oc == 0 ? stp : stp / oc,
									def = oc == 0 ? def : def / oc,
									date
								};

                result.Id = operatorId;
                result.Code = operatorRepository.FirstOrDefault(o => o.Id == operatorId).Code;
                result.Title = operatorRepository.FirstOrDefault(o => o.Id == operatorId).Name;

                var mainList = mainQuery.ToArray();

                if (mainList.Any())
                {
					//tp&g1
					result.TotalTargetCount = mainList.Sum(record => record.tp);
                    result.TotalTargetTime = mainList.Sum(record => record.tp * record.ct);
					result.TotalProductionCount = mainList.Sum(record => record.g1);
                    result.TotalProductionTime = mainList.Sum(record => record.g1 * record.ct);

					//def&stp
					result.TotalDefectionCount = mainList.Sum(record => record.def);
					result.TotalDefectionTime = mainList.Sum(record => record.def * record.ct);
					result.TotalStoppageCount = mainList.Sum(record => record.stp);
					result.TotalStoppageTime = mainList.Sum(record => record.stp * record.ct);

					//deviant
					var deltaCount = result.TotalProductionCount + result.TotalDefectionCount + result.TotalStoppageCount - result.TotalTargetCount;
					var deltaTime = result.TotalProductionTime + result.TotalDefectionTime + result.TotalStoppageTime - result.TotalTargetTime;
					result.TotalExtraCount = deltaCount > 0 ? deltaCount : 0;
					result.TotalExtraTime = deltaTime > 0 ? deltaTime : 0;
					result.TotalShortageCount = deltaCount < 0 ? -deltaCount : 0;
					result.TotalShortageTime = deltaTime < 0 ? -deltaTime : 0;

					//lines
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

                            TargetTime = Format.ConvertToHMS((int)(line.tp * line.ct)),
							ProductionTime = Format.ConvertToHMS((int)(line.g1 * line.ct)),
							DefectionTime = Format.ConvertToHMS((int)(line.def * line.ct)),
							StoppageTime = Format.ConvertToHMS((int)(line.stp * line.ct)),
							DeltaTime = (line.g1 + line.def + line.stp - line.tp) * line.ct,

							TargetCount = line.tp.ToString("##", CultureInfo.InvariantCulture),
							ProductionCount = line.g1.ToString("##", CultureInfo.InvariantCulture),
							DefectionCount = line.def.ToString("##", CultureInfo.InvariantCulture),
							StoppageCount = line.stp.ToString("##", CultureInfo.InvariantCulture),
							DeltaCount = (line.g1 + line.def + line.stp - line.tp),
                        };
                        result.ActivityItems.Add(mainDetail);
                    }
                }


				var odrList = odrRepository.Find(x =>
					x.DefectionReport.ProcessReport.StartDateTime >= start && 
					x.DefectionReport.ProcessReport.EndDateTime <= end &&
					x.Operator.Id == operatorId);

				var qualitiveQuery = from odr in odrList
									 let pr = odr.DefectionReport.ProcessReport
									 let ssa = pr.Process.StateStationActivity
									 let ss = ssa.StateStation
									 let product = ss.State.FPC.Product
									 select new {
										 odr.Id,
										 date = pr.StartDateTime,
										 pdCode = product.Code,
										 pdName = product.Name,
										 snName = ss.Station.Name,
										 aCode = ssa.Activity.Code,
										 aName = ssa.Activity.Name,
										 ct = ssa.CycleTime,
										 isG2 = odr.DefectionReport.IsG2,
										 lost = odr.DefectionReport.CountEquivalence,/*can be divided by number of guilty operators*/
									 };

                var qualitiveList = qualitiveQuery.ToList();
				result.TotalWaste = qualitiveList.Any()
					? qualitiveList.Where(x => !x.isG2).Sum(record => record.lost)
					: 0;
                result.TotalSecondGrade = qualitiveList.Any()
					? qualitiveList.Where(x => x.isG2).Sum(record => record.lost)
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
                        Status = line.isG2 ? QualitiveStatus.SecondGrade : QualitiveStatus.Waste,

                        DefectionTime = Format.ConvertToHMS((int)(line.lost * line.ct)),
                        DefectionCount = line.lost.ToString(CultureInfo.InvariantCulture),
                    };
                    result.QualitiveItems.Add(qualitiveDetail);
                }


				var osrList = osrRepository.Find(x =>
					x.StoppageReport.ProcessReport.StartDateTime >= start &&
					x.StoppageReport.ProcessReport.EndDateTime <= end &&
					x.Operator.Id == operatorId);

				var technicalQuery = from osr in osrList
									 let pr = osr.StoppageReport.ProcessReport
									 let ssa = pr.Process.StateStationActivity
									 let ss = ssa.StateStation
									 let product = ss.State.FPC.Product
									 select new
									 {
										 osr.Id,
										 date = pr.StartDateTime,
										 pdCode = product.Code,
										 pdName = product.Name,
										 snName = ss.Station.Name,
										 aCode = ssa.Activity.Code,
										 aName = ssa.Activity.Name,
										 ct = ssa.CycleTime,
										 lost = osr.StoppageReport.CountEquivalence,/*can be divided by number of guilty operators*/
									 };

                foreach (var line in technicalQuery)
                {
                    var technicalDetail = new OprTechnicalDetailVM
                    {
                        Id = line.Id,
                        Date = line.date,
                        Product = line.pdCode + "-" + line.pdName,
                        Station = line.snName,
                        Activity = line.aCode + "-" + line.aName,

                        StoppageTime = Format.ConvertToHMS((int)(line.lost * line.ct)),
                        StoppageCount = line.lost.ToString(CultureInfo.InvariantCulture),
                    };
                    result.TechnicalItems.Add(technicalDetail);
                }
            }
            return result;
        }

      
    }
}