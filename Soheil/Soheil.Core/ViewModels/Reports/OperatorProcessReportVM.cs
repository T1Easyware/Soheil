using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Reports
{
    public class OperatorProcessReportVm : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<OprDetailVM> Details { get; set; }

        public OperatorProcessReportVm()
        {
            Details = new ObservableCollection<OprDetailVM>();
        }
    }
}
