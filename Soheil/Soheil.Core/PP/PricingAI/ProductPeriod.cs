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
	public class ProductPeriod
	{
		//public ProductPeriod()
		//{
		//	Prices = new List<Price>
		//	{
		//		new Price()
		//	};
		//}

		public ProductPeriod(ProductPeriodVm pp)
		{
			PeriodIndex = pp.PeriodIndex;
			MaxProduction = pp.MaxProduction;
			Prices = new List<Price>();
			foreach (PriceVm priceVm in pp.Prices)
			{
				Prices.Add(new Price(priceVm));
			}
		}

		public ProductPeriod(Product product, Period period)
		{
			PeriodIndex = period.Index;

			Prices = new List<Price>
			{
				new Price()
			};
		}

		public int MaxProduction { get; set; }
		public List<Price> Prices { get; set; }

		public int PeriodIndex { get; set; }
	}
}
