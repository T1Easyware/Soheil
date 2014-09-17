using System;
using Soheil.Core.Base;
using System.Globalization;

namespace Soheil.Core.ViewModels.Reports
{
    public class OprActivityDetailVM : ViewModelBase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public string Station { get; set; }
        public string Activity { get; set; }

        public string TargetTime { get; set; }
        public string ProductionTime { get; set; }
        public string DefectionTime { get; set; }
		public string StoppageTime { get; set; }
		public double DeltaTime
		{
			set
			{
				if (value > 0) ExtraTime = value.ToString("##", CultureInfo.InvariantCulture);
				else if (value < 0) ShortageTime = (-value).ToString("##", CultureInfo.InvariantCulture);
			}
		}
		public string ExtraTime { get; set; }
		public string ShortageTime { get; set; }
		
        public string TargetCount { get; set; }
        public string ProductionCount { get; set; }
        public string DefectionCount { get; set; }
		public string StoppageCount { get; set; }
		public double DeltaCount
		{
			set
			{
				if (value > 0) ExtraCount = value.ToString("##", CultureInfo.InvariantCulture);
				else if (value < 0) ShortageCount = (-value).ToString("##", CultureInfo.InvariantCulture);
			}
		}

		public string ExtraCount { get; set; }
		public string ShortageCount { get; set; }
        public string IsRework { get; set; }
    }
}
