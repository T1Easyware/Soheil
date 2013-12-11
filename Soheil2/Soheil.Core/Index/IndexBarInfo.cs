using System;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.Index
{
    public struct IndexBarInfo
    {
        public IndexType Type { get; set; }
        public IndexFilter Filter { get; set; }
        public int IntervalId { get; set; }
        public int MachineId { get; set; }
        public int CauseL1Id { get; set; }
        public int CauseL2Id { get; set; }
        public int CauseL3Id { get; set; }
        public int ProductId { get; set; }
        public int StationId { get; set; }
        public int ActivityId { get; set; }
        public int OperatorId { get; set; }
        public int Level { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Text { get; set; }
        public LinearGradientBrush Color { get; set; }
        public bool IsMenuItem { get; set; }
    }
}
