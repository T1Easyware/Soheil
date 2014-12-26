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
	public class Product
	{
		public Product()
		{
			Id = -1;
			IsActive = true;
			Periods = new List<ProductPeriod>();
		}
		public Product(ProductVm vm)
		{
			Id = vm.Id;
			IsActive = vm.IsActive;
			Name = vm.Name;
			GroupName = vm.ProductGroupName;
			FinishedCost = vm.FinishedCost;
			Inventory = vm.Inventory;
			InventoryCost = vm.InventoryCost;
			LostSaleCost = vm.LostSaleCost;
			SpaceCoef = vm.SpaceCoef;
			Periods = vm.Periods.OrderBy(x => x.PeriodIndex).Select(x => new ProductPeriod(x)).ToList();
		}

		public int Id { get; set; }
		public bool IsActive { get; set; }
		//[NonSerialized]
		public string Name;
		//[NonSerialized]
		public string GroupName;
		public int FinishedCost { get; set; }
		public int InventoryCost { get; set; }
		public int LostSaleCost { get; set; }
		public int Inventory { get; set; }
		public double SpaceCoef { get; set; }
		public List<ProductPeriod> Periods { get; set; }
	}
}
