using System.Windows;
using System.Windows.Controls;

using Soheil.Core.ViewModels;

namespace Soheil.TemplateSelectors
{
    public class SplitViewSubContentSelector : DataTemplateSelector
    {
        public DataTemplate ProductSubContentTemplate { get; set; }

        public DataTemplate DefectionSubContentTemplate { get; set; }

        public DataTemplate OperatorSubContentTemplate { get; set; }

		public DataTemplate UserSubContentTemplate { get; set; }
		
		public DataTemplate OrganizationCalendarSubContentTemplate { get; set; }

        public DataTemplate FpcSubContentTemplate { get; set; }

        public DataTemplate CostSubContentTemplate { get; set; }


        public DataTemplate ModuleSubContentPanel { get; set; }
        public DataTemplate UnitSubContentTemplate { get; set; }

		//!@#$
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var dataContext = ((ContentControl)((ContentPresenter)container).TemplatedParent).DataContext;
            if (dataContext is UsersVM)
            {
                return UserSubContentTemplate;
            }
            if (dataContext is AccessRulesVM)
            {
                return ModuleSubContentPanel;
            }
			if (dataContext is WorkProfilesVM)
			{
				return OrganizationCalendarSubContentTemplate;
			}
            if (dataContext is ProductsVM)
            {
                return ProductSubContentTemplate;
            }
            if (dataContext is DefectionsVM)
            {
                return DefectionSubContentTemplate;
            }
			if (dataContext is FpcsVm)
			{
				return FpcSubContentTemplate;
			}
            if (dataContext is OperatorsVM)
            {
                return OperatorSubContentTemplate;
            }
            if (dataContext is CostsVM)
            {
                return CostSubContentTemplate;
            }
            if (dataContext is UnitSetsVM)
            {
                return UnitSubContentTemplate;
            }
            return new DataTemplate();
        }
    }

}

