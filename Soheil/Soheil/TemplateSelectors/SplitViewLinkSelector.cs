using System.Windows;
using System.Windows.Controls;

using Soheil.Core.ViewModels;

namespace Soheil.TemplateSelectors
{
    public class SplitViewLinkSelector : DataTemplateSelector
    {
        public DataTemplate ProductDefectionsTemplate { get; set; }
        public DataTemplate DefectionProductsTemplate { get; set; }
        public DataTemplate ProductReworksTemplate { get; set; }
        public DataTemplate ReworkProductsTemplate { get; set; }
        public DataTemplate ActivityOperatorsTemplate { get; set; }
        public DataTemplate OperatorActivitiesTemplate { get; set; }
        public DataTemplate StationMachinesTemplate { get; set; }
        public DataTemplate MachineStationsTemplate { get; set; }
        public DataTemplate UserPositionsTemplate { get; set; }
        public DataTemplate UserAccessRulesTemplate { get; set; }
        public DataTemplate PositionUsersTemplate { get; set; }
        public DataTemplate PositionAccessRulesTemplate { get; set; }
        public DataTemplate PositionOrganizationChartsTemplate { get; set; }
        public DataTemplate AccessRuleUsersTemplate { get; set; }
        public DataTemplate AccessRulePositionsTemplate { get; set; }
        public DataTemplate OrganizationChartPositionsTemplate { get; set; }
        public DataTemplate FishboneNodeActionPlansTemplate { get; set; }
        public DataTemplate ActionPlanFishboneNodesTemplate { get; set; }
        public DataTemplate RawMaterialUnitGroupsTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProductDefectionsVM)
            {
                return ProductDefectionsTemplate;

            }
            if (item is DefectionProductsVM)
            {
                return DefectionProductsTemplate;

            }
            if (item is ProductReworksVM)
            {
                return ProductReworksTemplate;

            }
            if (item is ReworkProductsVM)
            {
                return ReworkProductsTemplate;

            }
            if (item is ActivityOperatorsVM)
            {
                return ActivityOperatorsTemplate;

            }
            if (item is OperatorActivitiesVM)
            {
                return OperatorActivitiesTemplate;
            }
            if (item is StationMachinesVM)
            {
                return StationMachinesTemplate;

            }
            if (item is MachineStationsVM)
            {
                return MachineStationsTemplate;
            }
            if (item is UserPositionsVM)
            {
                return UserPositionsTemplate;

            }
            if (item is UserAccessRulesVM)
            {
                return UserAccessRulesTemplate;

            }
            if (item is PositionUsersVM)
            {
                return PositionUsersTemplate;

            }
            if (item is PositionAccessRulesVM)
            {
                return PositionAccessRulesTemplate;

            }
            if (item is PositionOrganizationChartsVM)
            {
                return PositionOrganizationChartsTemplate;

            }
            if (item is AccessRuleUsersVM)
            {
                return AccessRuleUsersTemplate;

            }
            if (item is AccessRulePositionsVM)
            {
                return AccessRulePositionsTemplate;

            }
            if (item is OrganizationChartPositionsVM)
            {
                return OrganizationChartPositionsTemplate;

            }
            if (item is ActionPlanFishbonesVM)
            {
                return ActionPlanFishboneNodesTemplate;

            }
            if (item is FishboneNodeActionPlansVM)
            {
                return FishboneNodeActionPlansTemplate;

            }
          /*  if (item is RawMaterialUnitGroupsVM)
            {
                return RawMaterialUnitGroupsTemplate;

            }*/
            return new DataTemplate();
        }
    }
}