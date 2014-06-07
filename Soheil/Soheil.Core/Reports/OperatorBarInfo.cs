using System;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.Reports
{
    public struct OperatorBarInfo
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Text { get; set; }
        public LinearGradientBrush Color { get; set; }
        public bool IsMenuItem { get; set; }
        public bool IsCountBase { get; set; }
    }
}
