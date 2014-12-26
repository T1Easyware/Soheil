using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Product
	{
		public int Id { get; set; }
		public int MainProductReworkId { get; set; }
		public int Index { get; set; }
		public Level[] Levels { get; set; }

		public Product(Model.Product model)
		{
			Id = model.Id;
			MainProductReworkId = model.MainProductRework.Id;
		}

		public Tuple<int,int> FindLevelState(int stateId)
		{
			int level = Levels.FirstOrDefault(y => y.States.Any(z => z.Id == stateId)).Index;
			int state = Levels[level].States.FirstOrDefault(x => x.Id ==stateId).Index;
			return new Tuple<int, int>(level, state);
		}
	}
}
