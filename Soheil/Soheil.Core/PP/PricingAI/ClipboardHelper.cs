using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PricingAI
{
	public static class ClipboardHelper
	{
		public const string Product = "aiProduct";
		public const string ProductPeriod = "aiProductPeriod";
		public const string Period = "aiPeriod";
		public const string Price = "aiPrice";

		public static Product GetProduct()
		{
			return Clipboard.GetData(Product) as Product;
		}
		public static List<Price> GetPrices()
		{
			var prices = new List<Price>();
			var pp = Clipboard.GetData(ProductPeriod) as ProductPeriod;
			if (pp == null)
			{
				var p = Clipboard.GetData(Price) as Price;
				if (p == null)
					return null;
				else
					prices.Add(p);
			}
			else
				prices.AddRange(pp.Prices);
			return prices;
		}
		public static bool HasPrice()
		{
			return
				Clipboard.ContainsData(ClipboardHelper.ProductPeriod)
				|| Clipboard.ContainsData(ClipboardHelper.Price);
		}
		internal static void SetData(object data)
		{
			if (data is Product) Clipboard.SetData(Product, data);
			else if (data is ProductPeriod) Clipboard.SetData(ProductPeriod, data);
			else if (data is Period) Clipboard.SetData(Period, data);
			else if (data is Price) Clipboard.SetData(Price, data);
		}
	}
}
