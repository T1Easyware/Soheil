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
	public class Price
	{
		public Price()
		{

		}
		public Price(PriceVm priceVm)
		{
			Fee = priceVm.Fee;
			MinDemand = priceVm.MinDemand;
			MaxDemand = priceVm.MaxDemand;
		}

		public int Fee { get; set; }
		public int MinDemand { get; set; }
		public int MaxDemand { get; set; }
	}
}
