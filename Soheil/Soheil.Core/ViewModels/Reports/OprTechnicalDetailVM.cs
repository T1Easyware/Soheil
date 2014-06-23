using System;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Reports
{
    public class OprTechnicalDetailVM : ViewModelBase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public string Station { get; set; }
        public string Activity { get; set; }
        public string StoppageTime { get; set; }
        public string StoppageCount { get; set; }
    }
}