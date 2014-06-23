using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Index;

namespace Soheil.Core.Index
{
	public class OeeRecord : DependencyObject
	{
		public int MachineId { get; private set; }
		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }
		public OeeRecord(int machineId, DateTime start, DateTime end)
		{
			MachineId = machineId;
			Start = start;
			End = end;

			TotalHours = (end - start).TotalHours;
			TimeRange = string.Format("بازه زمانی: از {0} تا {1}", start, end);

			var ds = new DataServices.IndexDataService();
			ds.FillOEEByMachine(this);
		}
		private double caliber(double input)
		{
			return input > 0 ? input : 0;
		}

		public string TimeRange { get; private set; }

		public double TotalHours { get; private set; }
		public double ScheduledTime { get { return MainScheduledTime + ReworkScheduledTime; } }
		public double MainScheduledTime { get; set; }
		public double ReworkScheduledTime { get; set; }
		public double UnscheduledTime { get { return caliber(TotalHours - ScheduledTime); } }

		public double AvailableTime { get { return caliber(ScheduledTime - StoppageTime); } }
		public double StoppageTime { get; set; }

		public double WorkingTime { get { return ProductionTime + DefectionTime; } }
		public double ReportedTime { get; set; }
		public double UnreportedTime { get { return caliber(ScheduledTime - ReportedTime - StoppageTime); } }
		public double IdleTime { get { return caliber(AvailableTime - WorkingTime - UnreportedTime); } }

		public double ProductionTime { get; set; }
		public double DefectionTime { get; set; }


		public double OEE { get { return TotalHours > 0 ? 100 * ProductionTime / TotalHours : 0; } }
		public double SchedulingRate { get { return TotalHours > 0 ? 100 * ScheduledTime / TotalHours : 0; } }
		public double AvailabilityRate { get { return ScheduledTime > 0 ? 100 * AvailableTime / ScheduledTime : 0; } }
		public double EfficiencyRate { get { return AvailableTime > 0 ? 100 * WorkingTime / AvailableTime : 0; } }
		public double QualityRate { get { return WorkingTime > 0 ? 100 * ProductionTime / WorkingTime : 0; } }


	}
}
