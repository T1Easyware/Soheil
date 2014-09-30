using System;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Reports
{
    public class MaterialInventoryReportData 
    {
        public int Id { get; set; }
        public string Warehouse { get; set; }
        public string Material { get; set; }
        public string Unit { get; set; }
        public string Inventory { get; set; }

    }
}
