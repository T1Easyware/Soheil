using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Row
	{
		public Row(int id)
		{
			Id = id;
		}
		public readonly int Id;
		public List<Item> Items { get; set; }
	}
}
