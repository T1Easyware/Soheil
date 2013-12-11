using System.Windows;
using System.Windows.Controls;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.ViewModels.Reports;

namespace Soheil.TemplateSelectors
{
    public class SingularViewListSelector : DataTemplateSelector
    {
        public DataTemplate IndicesVmTemplate { get; set; }
        public DataTemplate CostReportsVmTemplate { get; set; }
        public DataTemplate ActualCostReportsVmTemplate { get; set; }
		public DataTemplate PPTableVmTemplate { get; set; }
		public DataTemplate SetupTimesVmTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IndicesVm)
                return IndicesVmTemplate;
            if (item is CostReportsVm)
                return CostReportsVmTemplate;
            if (item is ActualCostReportsVm)
                return ActualCostReportsVmTemplate;
			if (item is Core.ViewModels.PP.PPTableVm)
				return PPTableVmTemplate;
            if (item is Core.ViewModels.SetupTime.SetupTimeTableVm)
                return SetupTimesVmTemplate;
            return null;
        }
    }
}