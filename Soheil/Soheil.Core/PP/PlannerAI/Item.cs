using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Item
	{
		public bool IsFree { get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public Product Product { get; set; }
		public StateStation SS { get; set; }
		public Job Job { get; set; }
	}
}
