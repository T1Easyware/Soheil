using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Job
	{
		public int Index { get; set; }
		public int GroupId { get; set; }
		public Product Product { get; set; }
		public int Quantity { get; set; }
		public int ReleaseTime { get; set; }
		public int Deadline { get; set; }
		public float Weight { get; set; }
		public int DelayCount { get; set; }
		public int DelayTime { get; set; }
	}
}
