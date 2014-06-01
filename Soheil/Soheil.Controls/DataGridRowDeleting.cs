using System.Windows;

namespace Soheil.Controls
{
    public static class DataGridRowDeleting
    {
        // Using a DependencyProperty as the backing store for IsDeletingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDeletingValueProperty =
            DependencyProperty.RegisterAttached("IsDeletingValue", typeof(bool), typeof(DataGridRowDeleting), new UIPropertyMetadata(false));

        public static bool GetIsDeletingValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDeletingValueProperty);
        }

        public static void SetIsDeletingValue(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDeletingValueProperty, value);
        }

    }

}
