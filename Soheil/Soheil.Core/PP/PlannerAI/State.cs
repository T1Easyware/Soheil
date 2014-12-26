using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class State
	{
		public int Id { get; set; }
		public int Index { get; set; }
		public List<StateStation> SSList { get; set; }
		public List<Tuple<int, int>> Requisites { get; set; }
	}
}
