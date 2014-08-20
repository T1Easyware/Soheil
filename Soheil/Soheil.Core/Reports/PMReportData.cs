using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Reports
{
	public class PMReportData
	{
		public abstract class Base
		{
			public string Machine;
			public string Part;
			public string Description;
		}
		public class PM : Base
		{
			public string Maintenance;
			public DateTime MaintenanceDate;
			public DateTime? PerformedDate;
			public string Period;
			public int Delay;
			public bool IsPerformed;
			public DateTime LastMaintenanceDate;
		}
		public class Repair : Base
		{
			public int RepairStatus;
			public DateTime CreatedDate;
			public DateTime AcquiredDate;
			public DateTime DeliveredDate;
		}
		public IEnumerable<PM> PMList { get; set; }
		public IEnumerable<Repair> RepairList { get; set; }
	}
}
