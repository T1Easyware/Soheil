using System;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Reports
{
    public class OprDetailVM : ViewModelBase
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
        public string TargetCount { get; set; }
        public string ProductionCount { get; set; }
        public string DefectionCount { get; set; }
        public string StoppageCount { get; set; }
        public string IsRework { get; set; }
    }
}
