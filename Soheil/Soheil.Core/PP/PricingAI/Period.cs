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
	public class Period
	{
		static string[] _periodNames = new string[]{
			Soheil.Common.Properties.Resources.txtPeriod1,
			Soheil.Common.Properties.Resources.txtPeriod2,
			Soheil.Common.Properties.Resources.txtPeriod3,
			Soheil.Common.Properties.Resources.txtPeriod4,
		};
		public Period(int index)
		{
			Duration = 90;
			Index = index;
			Name = _periodNames[index];
			TotalBudget = int.MaxValue;
			TotalCapacity = int.MaxValue;
			StartDate = DateTime.Now.Date.AddDays(90 * index++);
			EndDate = DateTime.Now.Date.AddDays(90 * index);
		}
		public Period(PeriodVm vm)
		{
			Name = vm.Name;
			Duration = vm.Duration;
			TotalBudget = vm.TotalBudget.HasValue ? vm.TotalBudget.Value : -1;
			TotalCapacity = vm.TotalCapacity.HasValue ? vm.TotalCapacity.Value : -1;
			StartDate = vm.StartDate;
			EndDate = vm.EndDate;
			Index = vm.Index;
		}
		public string Name { get; set; }
		public int Duration { get; set; }
		public int TotalCapacity { get; set; }
		public int TotalBudget { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Index { get; set; }
	}
}
