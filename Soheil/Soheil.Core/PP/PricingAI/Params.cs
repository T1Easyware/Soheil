using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.ViewModels.PP.PricingAI;

namespace Soheil.Core.PP.PricingAI
{
	[Serializable]
	public class Params
	{
		static int coerce0(int value)
		{
			return value < 0 ? 0 : value;
		}
		static int coerce1(int value)
		{
			return value < 1 ? 1 : value;
		}
		public Params(ParamsVm vm)
		{
			UsePricePoints = true;
			maxRuns = coerce0(vm.maxRuns);
			timeLimit = coerce0(vm.timeLimit);
			idleCount = coerce0(vm.idleCount);
			maxDfss = coerce0(vm.maxDfss);
			mmSize = coerce1(vm.mmSize);
			memorySize = coerce1(vm.memorySize);
			maxInitPop = coerce1(vm.maxInitPop);
			translationFunction = coerce0(vm.translationFunction);
		}
		public bool UsePricePoints { get; set; }
		public int maxRuns { get; set; }
		public int timeLimit { get; set; }
		public int idleCount { get; set; }
		public int maxDfss { get; set; }
		public int mmSize { get; set; }
		public int memorySize { get; set; }
		public int maxInitPop { get; set; }
		public int translationFunction { get; set; }
	}

}
