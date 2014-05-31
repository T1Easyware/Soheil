using System;
using System.Collections.Generic;

namespace Soheil.Core.Reports
{
    public class Record
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
        public List<KeyValuePair<string,object>> Data { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long Ticks { get; set; }
        public string Header { get; set; }
    }
}
