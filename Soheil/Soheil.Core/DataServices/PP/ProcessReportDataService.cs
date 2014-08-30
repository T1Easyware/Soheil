using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Core.Interfaces;
using Soheil.Core.Base;
using Soheil.Common;
using System.Collections;

namespace Soheil.Core.DataServices
{
	public class ProcessReportDataService : DataServiceBase, IDataService<ProcessReport>
	{
		Repository<ProcessReport> _processReportRepository;
		public ProcessReportDataService()
			: this(new SoheilEdmContext())
		{
		}
		public ProcessReportDataService(SoheilEdmContext context)
		{
			this.Context = context;
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
			if (!_processReportRepository.Exists(x => x.Id == model.Id))
				_processReportRepository.Add(model);
			return 0;
		}

		public void UpdateModel(ProcessReport model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(ProcessReport model)
		{
			var processReportRepository = new Repository<ProcessReport>(Context);
			var processReportDataService = new ProcessReportDataService(Context);
			var operatorProcessReportRepository = new Repository<OperatorProcessReport>(Context);
			var defectionReportRepository = new Repository<DefectionReport>(Context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(Context);
			var stoppageReportRepository = new Repository<StoppageReport>(Context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(Context);
			processReportDataService.ClearModel(
				model,
				processReportRepository,
				operatorProcessReportRepository,
				defectionReportRepository,
				operatorDefectionReportRepository,
				stoppageReportRepository,
				operatorStoppageReportRepository,
				Context);

			model.Process.ProcessReports.Remove(model);
			_processReportRepository.Delete(model);
			Context.Commit();
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
				{
					operatorDefectionReportRepository.Delete(operatorDefectionReportModel);
				}
				defectionReportRepository.Delete(defectionReportModel);
			}
			var stoppageReports = processReportModel.StoppageReports.ToArray();
			foreach (var stoppageReportModel in stoppageReports)
			{
				var operatorStoppageReports = stoppageReportModel.OperatorStoppageReports.ToArray();
				foreach (var operatorStoppageReportModel in operatorStoppageReports)
				{
					operatorStoppageReportRepository.Delete(operatorStoppageReportModel);
				}
				foreach (var repair in stoppageReportModel.Repairs.ToArray())
				{
					stoppageReportModel.Repairs.Remove(repair);
					repair.StoppageReport = null;
					repair.RepairStatus = (byte)RepairStatus.Inactive;
					repair.Description = "StoppageReport has been Deleted.\n" + repair.Description;
				}
				stoppageReportRepository.Delete(stoppageReportModel);
			}
		}
		internal void Delete(StoppageReport Model)
		{
			var stoppageReportRepository = new Repository<StoppageReport>(Context);
			var operatorStoppageReportRepository = new Repository<OperatorStoppageReport>(Context);

			var operatorStoppageReports = Model.OperatorStoppageReports.ToArray();
			foreach (var operatorStoppageReportModel in operatorStoppageReports)
			{
				operatorStoppageReportRepository.Delete(operatorStoppageReportModel);
			}

			foreach (var repair in Model.Repairs.ToArray())
			{
				Model.Repairs.Remove(repair);
				repair.StoppageReport = null;
				repair.RepairStatus = (byte)RepairStatus.Inactive;
				repair.Description = "StoppageReport has been Deleted.\n" + repair.Description;
			}
			stoppageReportRepository.Delete(Model);
		}
		internal void Delete(DefectionReport Model)
		{
			var defectionReportRepository = new Repository<DefectionReport>(Context);
			var operatorDefectionReportRepository = new Repository<OperatorDefectionReport>(Context);

			var operatorDefectionReports = Model.OperatorDefectionReports.ToArray();
			foreach (var operatorDefectionReportModel in operatorDefectionReports)
			{
				operatorDefectionReportRepository.Delete(operatorDefectionReportModel);
			}
			defectionReportRepository.Delete(Model);
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
			var odrRepo = new Repository<OperatorDefectionReport>(Context);
			var garbage = odrRepo.Find(x => x.DefectionReport == null || x.DefectionReport.ProductDefection == null || x.Operator == null).ToArray();
			foreach (var item in garbage)
			{
				if (item.DefectionReport != null) item.DefectionReport.OperatorDefectionReports.Remove(item);
				odrRepo.Delete(item);
			}

			var drRepo = new Repository<DefectionReport>(Context);
			var garb = drRepo.Find(x => x.ProcessReport == null || x.ProductDefection == null).ToArray();
			foreach (var item in garb)
			{
				if (item.ProcessReport != null) item.ProcessReport.DefectionReports.Remove(item);
				drRepo.Delete(item);
			}

			//correct Stoppages
			var osrRepo = new Repository<OperatorStoppageReport>(Context);
			var garbage2 = osrRepo.Find(x => x.StoppageReport == null || x.StoppageReport.Cause == null || x.Operator == null).ToArray();
			foreach (var item in garbage2)
			{
				if (item.StoppageReport != null) item.StoppageReport.OperatorStoppageReports.Remove(item);
				osrRepo.Delete(item);
			}

			var srRepo = new Repository<StoppageReport>(Context);
			var garb2 = srRepo.Find(x => x.ProcessReport == null || x.Cause == null).ToArray();
			foreach (var item in garb2)
			{
				if (item.ProcessReport != null) item.ProcessReport.StoppageReports.Remove(item);
				foreach (var repair in item.Repairs.ToArray())
				{
					item.Repairs.Remove(repair);
					repair.StoppageReport = null;
					repair.RepairStatus = (byte)RepairStatus.Inactive;
					repair.Description = "StoppageReport has been Deleted.\n" + repair.Description;
				}
				srRepo.Delete(item);
			}

			//correct G1
			if (model.OperatorProcessReports.Any())
			{
				var sum = model.OperatorProcessReports.Sum(x => x.OperatorProducedG1);
				if (sum < model.ProducedG1)
					model.ProducedG1 = sum;
			}
			Context.Commit();
		}


		internal Core.Reports.DailyReportData GetDailyReport(DateTime StartDateTime, DateTime EndDateTime, bool showAllActivities)
		{
			var data = new Core.Reports.DailyReportData();

			//find shifts
			var tmp = new WorkProfilePlanDataService(Context).GetShiftsInRange(StartDateTime, EndDateTime);
			if (tmp.Any())
			{
				var shifts = tmp.Where(x => x.Item2.Date < EndDateTime.Date).Select(x => new
				{
					start = x.Item2.AddSeconds(x.Item1.StartSeconds),
					end = x.Item2.AddSeconds(x.Item1.EndSeconds),
					code = x.Item1.WorkShiftPrototype.Name
				});

				var processReports = _processReportRepository.GetAll();
				//main query (with shift)
				var prQuery = from shift in shifts
							  from processReport in processReports.Where(x =>
								  x.StartDateTime < shift.end && x.EndDateTime > shift.start
								  && (showAllActivities || x.Process.StateStationActivity.IsPrimaryOutput))
							  let start = (processReport.StartDateTime < shift.start) ? shift.start : processReport.StartDateTime
							  let end = (processReport.EndDateTime > shift.end) ? shift.end : processReport.EndDateTime
							  let durationSeconds = (end - start).TotalSeconds
							  let ratio = durationSeconds / processReport.DurationSeconds
							  where ratio > 0

							  group new
							  {
								  processReport,
								  ratio,
							  } by new
							  {
								  processReport.Process.StateStationActivity,
								  shiftCode = shift.code
							  } into ssaGroup
							  select new
							  {
								  ssaGroup.Key.shiftCode,
								  activity = ssaGroup.Key.StateStationActivity.Activity.Name,
								  station = ssaGroup.Key.StateStationActivity.StateStation.Station.Name,
								  product = ssaGroup.Key.StateStationActivity.StateStation.State.OnProductRework.Product.Name,

								  //process targetpoint
								  targetCount = ssaGroup.Sum(x => x.processReport.ProcessReportTargetPoint * x.ratio),

								  //standard production per hour
								  pph = 3600 / ssaGroup.Key.StateStationActivity.CycleTime,

								  g1count = ssaGroup.Sum(x => x.processReport.ProducedG1 * x.ratio),

								  defectionCount = ssaGroup.Sum(x => x.processReport.DefectionReports.Sum(d => d.CountEquivalence) * x.ratio),
								  stoppageCount = ssaGroup.Sum(x => x.processReport.StoppageReports.Sum(d => d.CountEquivalence) * x.ratio),

								  majorDefection = ssaGroup.Any(x => x.processReport.DefectionReports.Any()) ?
									ssaGroup
									.SelectMany(x => x.processReport.DefectionReports)
									.OrderByDescending(x => x.CountEquivalence)
									.FirstOrDefault()
									.ProductDefection.Defection.Name
									: "",
								  majorStoppage = ssaGroup.Any(x => x.processReport.StoppageReports.Any()) ?
									ssaGroup
									.SelectMany(x => x.processReport.StoppageReports)
									.OrderByDescending(x => x.CountEquivalence)
									.FirstOrDefault()
									.Cause.Name
									: "",
							  };
				data.Main = prQuery
					.OrderBy(x => x.product)
					.ThenBy(x => x.station)
					.Select(x => new Reports.DailyReportData.MainData
					{
						Product = x.product,
						Activity = x.activity,
						Shift = x.shiftCode,
						Station = x.station,
						TargetValue = x.targetCount.ToString("0.#"),
						ProductionPerHour = x.pph.ToString("0.#"),
						ProductionValue = x.g1count.ToString("0.#"),
						ExecutionPercent = (x.g1count / x.targetCount).ToString("0.# %"),
						TotalDeviationValue = (x.targetCount - x.g1count).ToString("0.#"),
						DefectionValue = x.defectionCount.ToString("0.#"),
						StoppageValue = x.stoppageCount.ToString("0.#"),
						MajorDefection = x.majorDefection,
						MajorStoppage = x.majorStoppage
					});

				data.Summery = shifts.Select(x =>
					new Reports.DailyReportData.SummeryData
					{
						Shift = x.code,
						Supervisor = "",
						OperatorsCount = processReports
							.Where(p => p.StartDateTime < x.end && p.EndDateTime > x.start)
							.SelectMany(p => p.Process.ProcessOperators)
							.Select(p => p.Operator.Id)
							.Distinct()
							.Count()
							.ToString()
					}
				);
			}
			//No shift is available
			else
			{
				var processReports = _processReportRepository.Find(x => x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime);

				//main query (without shift)
				var prQuery = from processReport in processReports
							  let start = (processReport.StartDateTime < StartDateTime) ? StartDateTime : processReport.StartDateTime
							  let end = (processReport.EndDateTime > EndDateTime) ? EndDateTime : processReport.EndDateTime
							  let durationSeconds = (end - start).TotalSeconds
							  let ratio = durationSeconds / processReport.DurationSeconds

							  group new
							  {
								  processReport,
								  ratio,
							  } by processReport.Process.StateStationActivity into ssaGroup

							  select new
							  {
								  activity = ssaGroup.Key.Activity.Name,
								  station = ssaGroup.Key.StateStation.Station.Name,
								  product = ssaGroup.Key.StateStation.State.OnProductRework.Product.Name,

								  //process targetpoint
								  targetCount = ssaGroup.Sum(x => x.processReport.ProcessReportTargetPoint * x.ratio),

								  //standard production per hour
								  pph = 3600 / ssaGroup.Key.CycleTime,

								  g1count = ssaGroup.Sum(x => x.processReport.ProducedG1 * x.ratio),

								  defectionCount = ssaGroup.Sum(x => x.processReport.DefectionReports.Sum(d => d.CountEquivalence) * x.ratio),
								  stoppageCount = ssaGroup.Sum(x => x.processReport.StoppageReports.Sum(d => d.CountEquivalence) * x.ratio),

								  majorDefection = ssaGroup.Any(x => x.processReport.DefectionReports.Any()) ?
									ssaGroup
									.SelectMany(x => x.processReport.DefectionReports)
									.OrderByDescending(x => x.CountEquivalence)
									.FirstOrDefault()
									.ProductDefection.Defection.Name
									: "",
								  majorStoppage = ssaGroup.Any(x => x.processReport.StoppageReports.Any()) ?
									ssaGroup
									.SelectMany(x => x.processReport.StoppageReports)
									.OrderByDescending(x => x.CountEquivalence)
									.FirstOrDefault()
									.Cause.Name
									: "",
							  };
				data.Main = prQuery
					.OrderBy(x => x.product)
					.ThenBy(x => x.station)
					.Select(x => new Reports.DailyReportData.MainData
					{
						Product = x.product,
						Activity = x.activity,
						Shift = "-",
						Station = x.station,
						TargetValue = x.targetCount.ToString("0.#"),
						ProductionPerHour = x.pph.ToString("0.#"),
						ProductionValue = x.g1count.ToString("0.#"),
						ExecutionPercent = (x.g1count / x.targetCount).ToString("0.# %"),
						TotalDeviationValue = (x.targetCount - x.g1count).ToString("0.#"),
						DefectionValue = x.defectionCount.ToString("0.#"),
						StoppageValue = x.stoppageCount.ToString("0.#"),
						MajorDefection = x.majorDefection,
						MajorStoppage = x.majorStoppage
					});

				data.Summery = new List<Reports.DailyReportData.SummeryData>()
				{
					new Reports.DailyReportData.SummeryData
					{
						Shift="-",
						Supervisor="", 
						OperatorsCount = processReports
							.SelectMany(x=>x.Process.ProcessOperators)
							.Select(x=>x.Operator.Id)
							.Distinct()
							.Count()
							.ToString()
					}
				};
			}

			return data;
		}

		public IEnumerable<ProcessReport> GetPendingProcessReports(DateTime date, int stationId, bool showAll, bool isSafe)
		{
			var processRepository = new Repository<Process>(Context);
			var wppDs = new Core.DataServices.WorkProfilePlanDataService(Context);
			var StartDateTime = wppDs.GetShiftStartOn(date);
			var EndDateTime = StartDateTime.AddDays(1);

			//get all processes in range
			var processes = processRepository.Find(x => 
				x.Task.Block.StateStation.Station.Id == stationId &&
				x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime);

			//fill gaps by processReports
			foreach (var process in processes)
			{
				//check for remaining
				var remDuration = process.DurationSeconds;
				var remTP = process.TargetCount;
				var reports = process.ProcessReports.OrderBy(x => x.StartDateTime).ToArray();

				//init remainings
				if (reports.Any())
				{
					remTP -= process.ProcessReports.Sum(x => x.ProcessReportTargetPoint);
					remDuration -= process.ProcessReports.Sum(x => x.DurationSeconds);
				}
				//fill spaces before each report
				var dt = process.StartDateTime;
				foreach (var processReport in reports.ToArray())
				{
					//add one before
					if (processReport.StartDateTime - dt > TimeSpan.FromSeconds(process.StateStationActivity.CycleTime))
					{
						//calculate duration and tp
						var dur = (int)(processReport.StartDateTime - dt).TotalSeconds;
						var tp = (int)(remTP * dur / remDuration);
						dur = tp * (int)process.StateStationActivity.CycleTime;

						//create processReport
						var processReportModel = new Model.ProcessReport
						{
							Process = process,
							StartDateTime = dt,
							EndDateTime = dt.AddSeconds(dur),
							DurationSeconds = dur,
							ProcessReportTargetPoint = tp,
							Code = process.Code,
							ModifiedBy = LoginInfo.Id,
						};

						//fix remainings
						remDuration -= dur;
						remTP -= tp;

						//create process operators
						foreach (var po in process.ProcessOperators)
						{
							processReportModel.OperatorProcessReports.Add(new Model.OperatorProcessReport
							{
								ProcessReport = processReportModel,
								ProcessOperator = po,
							});
						}

						//add to processReports
						process.ProcessReports.Add(processReportModel);
					}
					dt = processReport.EndDateTime;
				}

				//add one at the end
				var now = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond);
				var end = process.EndDateTime < now ? process.EndDateTime : now;
				var smallestTs = isSafe ? TimeSpan.FromMinutes(10) : TimeSpan.FromSeconds(process.StateStationActivity.CycleTime);
				if (end - dt > smallestTs)
				{
					//calculate duration and tp
					var dur = (int)(end - dt).TotalSeconds;
					var tp = (int)(remTP * dur / remDuration);
					dur = tp * (int)process.StateStationActivity.CycleTime;

					//create processReport
					var newModel = new Model.ProcessReport
					{
						Process = process,
						Code = process.Code,
						StartDateTime = dt,
						EndDateTime = dt.AddSeconds(dur),
						DurationSeconds = dur,
						ProcessReportTargetPoint = tp,
						ProducedG1 = 0,
						ModifiedBy = LoginInfo.Id,
					};

					//create process operators
					foreach (var po in process.ProcessOperators)
					{
						newModel.OperatorProcessReports.Add(new Model.OperatorProcessReport
						{
							ProcessReport = newModel,
							ProcessOperator = po,
							OperatorProducedG1 = 0,
							ModifiedBy = LoginInfo.Id,
						});
					}

					//add to processReports
					process.ProcessReports.Add(newModel);
				}

			}
			Context.Commit();
			if(showAll)
			{
				return _processReportRepository.Find(x =>
					x.Process.Task.Block.StateStation.Station.Id == stationId &&
					x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime);
			}
			else
			{
				return _processReportRepository.Find(x =>
					x.Process.Task.Block.StateStation.Station.Id == stationId &&
					x.StartDateTime < EndDateTime && x.EndDateTime > StartDateTime
					&& !x.OperatorProcessReports.Any(y => y.OperatorProducedG1 != 0));
			}
		}
	}
}