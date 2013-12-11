using System.Windows;
using System.Windows.Controls;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;

namespace Soheil.TemplateSelectors
{
    public class SplitViewListSelector : DataTemplateSelector
    {
        public DataTemplate DataGridListTemplate { get; set; }

        public DataTemplate TreeViewListTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var dataContext = ((ContentControl)((ContentPresenter)container).TemplatedParent).DataContext;
            if (dataContext is IEntityNode)
            {
                return TreeViewListTemplate;
            }
            return DataGridListTemplate;
        }
    }
}