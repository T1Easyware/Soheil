using System.Windows;
using System.Windows.Controls;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.ViewModels.Reports;

namespace Soheil.TemplateSelectors
{
    public class SingularViewListSelector : DataTemplateSelector
    {
		public DataTemplate FpcManagerVmTemplate { get; set; }
        public DataTemplate IndicesVmTemplate { get; set; }
        public DataTemplate CostReportsVmTemplate { get; set; }
        public DataTemplate ActualCostReportsVmTemplate { get; set; }
        public DataTemplate OperationReportsVmTemplate { get; set; }
		public DataTemplate PPTableVmTemplate { get; set; }
		public DataTemplate PmVmTemplate { get; set; }
		public DataTemplate SetupTimesVmTemplate { get; set; }
		public DataTemplate SkillCenterVmTemplate { get; set; }
		public DataTemplate DailyReportVmTemplate { get; set; }
		public DataTemplate DailyStationPlanVmTemplate { get; set; }
		public DataTemplate PMReportVmTemplate { get; set; }
		public DataTemplate MaterialPlanningVmTemplate { get; set; }
		
		//!@#$
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
			if (item is Soheil.Core.ViewModels.Fpc.FpcManagerVm)
				return FpcManagerVmTemplate;
			if (item is IndicesVm)
				return IndicesVmTemplate;
			if (item is CostReportsVm)
                return CostReportsVmTemplate;
            if (item is ActualCostReportsVm)
                return ActualCostReportsVmTemplate;
            if (item is OperationReportsVm)
                return OperationReportsVmTemplate;
			if (item is Core.ViewModels.PP.PPTableVm)
				return PPTableVmTemplate;
			if (item is Core.ViewModels.MaterialPlanning.MaterialPlanningVm)
				return MaterialPlanningVmTemplate;
			if (item is Core.ViewModels.PM.PmVm)
				return PmVmTemplate;
			if (item is Core.ViewModels.SetupTime.SetupTimeTableVm)
                return SetupTimesVmTemplate;
			if (item is Core.ViewModels.SkillCenter.SkillCenterVm)
				return SkillCenterVmTemplate;
			if (item is Core.ViewModels.Reports.DailyReportVm)
				return DailyReportVmTemplate;
			if (item is Core.ViewModels.Reports.DailyStationPlanVm)
				return DailyStationPlanVmTemplate;
			if (item is Core.ViewModels.Reports.PMReportVm)
				return PMReportVmTemplate;
            return null;
        }
    }
}