using System;
using Soheil.Common;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Reports
{
    public class OprQualitativeDetailVM : ViewModelBase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public string Station { get; set; }
        public string Activity { get; set; }
        public string DefectionTime { get; set; }
        public string DefectionCount { get; set; }
        public QualitiveStatus Status { get; set; }
    }
}