using Soheil.Dal;
using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.DbFix
{
	static class FpcState
	{
		internal static void CorrectStates()
		{
			int c = 0;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("This module fixes all missing mainProduct refs in all states in db");
			Console.ForegroundColor = ConsoleColor.DarkGray;

			//actual db shit
			using (var ctx = new SoheilEdmContext())
			{
				var repo = new Repository<FPC>(ctx);
				var all = repo.GetAll();
				foreach (var fpc in all)
				{
					foreach (var state in fpc.States.Where(x => x.StateType == Soheil.Common.StateType.Mid))
					{
						if (state.OnProductRework == null)
						{
							state.OnProductRework = fpc.Product.MainProductRework;
							c++;
							Console.WriteLine(string.Format("FPC with ID {0} : State with ID {1} corrected.", fpc.Id, state.Id));
						}
					}
				}
				ctx.Commit();
			}

			//result
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(string.Format("{0} States corrected successfully.", c));
		}
	}
}
