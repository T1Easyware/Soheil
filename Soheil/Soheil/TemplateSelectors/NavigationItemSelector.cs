using System.Windows;
using System.Windows.Controls;
using Soheil.Common;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;

namespace Soheil.TemplateSelectors
{
    public class NavigationItemSelector : DataTemplateSelector
    {
        public DataTemplate ProductDefectionTemplate { get; set; }
        public DataTemplate DefectionProductTemplate { get; set; }
        public DataTemplate ProductReworkTemplate { get; set; }
        public DataTemplate ReworkProductTemplate { get; set; }
        public DataTemplate ActivityOperatorTemplate { get; set; }
        public DataTemplate GeneralActivitySkillTemplate { get; set; }
        public DataTemplate UserPositionTemplate { get; set; }
        public DataTemplate PositionUserTemplate { get; set; }
        public DataTemplate UserAccessRuleTemplate { get; set; }
        public DataTemplate PositionAccessRuleTemplate { get; set; }
        public DataTemplate StationMachineTemplate { get; set; }
        public DataTemplate MachineStationTemplate { get; set; }
        public DataTemplate ActionPlanFishboneNodeTemplate { get; set; }
        public DataTemplate FishboneNodeActionPlanTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = (ISplitDetail)item;
            if (item is ProductDefectionVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return ProductDefectionTemplate;
                }
                return DefectionProductTemplate;
            }
            if (item is ProductReworkVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return ProductReworkTemplate;
                }
                return ReworkProductTemplate;
            }
            if (item is ActivityOperatorVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return ActivityOperatorTemplate;
                }
                return GeneralActivitySkillTemplate;
            }
            if (item is UserPositionVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return UserPositionTemplate;
                }
                return PositionUserTemplate;
            }
            if (item is StationMachineVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return StationMachineTemplate;
                }
                return MachineStationTemplate;
            }
            if (item is UserAccessRuleVM)
            {
                return UserAccessRuleTemplate;
            }
            if (item is PositionAccessRuleVM)
            {
                return PositionAccessRuleTemplate;
            }
            if (item is ActionPlanFishboneVM)
            {
                if (viewModel.PresentationType == RelationDirection.Straight)
                {
                    return ActionPlanFishboneNodeTemplate;
                }
                return FishboneNodeActionPlanTemplate;
            }
           /* if (item is RawMaterialUnitGroupVM)
            {
                return RawMaterialUnitGroupTemplate;
            }*/
            return new DataTemplate();
        }

    }
}