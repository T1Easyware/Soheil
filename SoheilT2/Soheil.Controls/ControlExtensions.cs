using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Soheil.Controls
{
    public static class ControlExtensions
    {
        #region DataGrid

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            var row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            var row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.SelectedItem);
                row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
            }
            return row;
        }

        #endregion
    }
}