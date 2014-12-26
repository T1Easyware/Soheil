using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Level
	{
		public int Index { get; set; }
		public List<State> States { get; set; }
	}
}
