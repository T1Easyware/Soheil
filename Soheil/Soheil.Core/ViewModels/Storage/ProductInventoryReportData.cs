using System;
using Soheil.Common;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Reports
{
    public class ProductInventoryReportData 
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public string Production { get; set; }
        public string Inventory { get; set; }
        public string Price { get; set; }
        public string Fee { get; set; }
        public string Warehouse { get; set; }
    }
}