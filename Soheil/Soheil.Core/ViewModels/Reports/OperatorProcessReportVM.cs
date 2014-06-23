using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Reports
{
    public class OperatorProcessReportVm : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }

        public double TotalTargetTime { get; set; }
        public double TotalProductionTime { get; set; }
        public double TotalExtraTime { get; set; }
        public double TotalShortageTime { get; set; }
        public double TotalDefectionTime { get; set; }
        public double TotalStoppageTime { get; set; }
        public double TotalTargetCount { get; set; }
        public double TotalProductionCount { get; set; }
        public double TotalExtraCount { get; set; }
        public double TotalShortageCount { get; set; }
        public double TotalDefectionCount { get; set; }
        public double TotalStoppageCount { get; set; }
        public double TotalWaste { get; set; }
        public double TotalSecondGrade { get; set; }
        public ObservableCollection<OprActivityDetailVM> ActivityItems { get; set; }
        public ObservableCollection<OprQualitativeDetailVM> QualitiveItems { get; set; }
        public ObservableCollection<OprTechnicalDetailVM> TechnicalItems { get; set; }

        public OperatorProcessReportVm()
        {
            ActivityItems = new ObservableCollection<OprActivityDetailVM>();
            QualitiveItems = new ObservableCollection<OprQualitativeDetailVM>();
            TechnicalItems = new ObservableCollection<OprTechnicalDetailVM>();
        }
    }
}
