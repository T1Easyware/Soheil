using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Reports
{
	public class DailyReportData
	{
		public class MainData
		{
			public string Product;
			public string Activity;
			public string Shift;
			public string Station;
			public string TargetValue;
			public string ProductionPerHour;
			public string ProductionValue;
			public string ExecutionPercent;
			public string TotalDeviationValue;
			public string DefectionValue;
			public string MajorDefection;
			public string StoppageValue;
			public string MajorStoppage;
		}
		public class SummeryData
		{
			public string Shift;
			public string Supervisor;
			public string OperatorsCount;
			//public string Description;
		}
		public IEnumerable<MainData> Main { get; set; }
		public IEnumerable<SummeryData> Summery { get; set; }
	}
}
