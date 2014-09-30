using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Reports
{
    public class WarehouseReportData 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }

        public double TotalStorage { get; set; }
        public double TotalDischarge { get; set; }
        public double TotalProduction { get; set; }
        public double TotalSale { get; set; }

        public ObservableCollection<MaterialInventoryReportData> MaterialItems { get; set; }
        public ObservableCollection<ProductInventoryReportData> ProductItems { get; set; }

        public WarehouseReportData()
        {
            MaterialItems = new ObservableCollection<MaterialInventoryReportData>();
            ProductItems = new ObservableCollection<ProductInventoryReportData>();
        }
    }
}
