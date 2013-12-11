using System;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.Reports
{
    public struct CostBarInfo
    {
        public CostType Type { get; set; }
        public CostSourceType SourceType { get; set; }
        public int Id { get; set; }
        public int IntervalId { get; set; }
        public int MachineId { get; set; }
        public int StationId { get; set; }
        public int OperatorId { get; set; }
        public int Level { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Text { get; set; }
        public LinearGradientBrush Color { get; set; }
        public bool IsMenuItem { get; set; }
    }
}
