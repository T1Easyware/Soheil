using Soheil.Dal;
using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.DbFix
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(". Soheil Database fix tool");

			try
			{
				TestFill.RunAll();
				FpcState.CorrectStates();
				//other fixes
			}
			catch(Exception exp)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(exp.Message);
				if (exp.InnerException != null)
				{
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine(exp.InnerException.Message);
				}
			}
			
			Console.ReadKey(true);
		}
	}
}
