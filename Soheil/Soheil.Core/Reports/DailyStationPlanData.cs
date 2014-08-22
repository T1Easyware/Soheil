using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Reports
{
	public class DailyStationPlanData
	{
		public class MainData
		{
			public string Product;
			public string Activity;
			public string Shift;
			public string TargetValue;
			public TimeSpan Start;
			public TimeSpan End;
			public IEnumerable<string> Operators;
			//public string ProductionPerHour;
		}
		public IEnumerable<MainData> Activities { get; set; }
		public int StationId;
		public string StationName;
		public string ShiftCode;
	}
}
