﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Soheil.Controls.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs;assembly=ChromiumTabs"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ChromiumTabItem/>
    ///
    /// </summary>
    public class ChromeTabItem : HeaderedContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof (ChromeTabItem),
                                                 new FrameworkPropertyMetadata(false,
                                                                               FrameworkPropertyMetadataOptions.
                                                                                   AffectsRender |
                                                                               FrameworkPropertyMetadataOptions.
                                                                                   AffectsParentMeasure |
                                                                               FrameworkPropertyMetadataOptions.
                                                                                   AffectsParentArrange));

        private static readonly RoutedUICommand CloseTabCmd = new RoutedUICommand("Close tab", "CloseTab",
                                                                                  typeof (ChromeTabItem));

        static ChromeTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ChromeTabItem),
                                                     new FrameworkPropertyMetadata(typeof (ChromeTabItem)));
            CommandManager.RegisterClassCommandBinding(typeof (ChromeTabItem),
                                                       new CommandBinding(CloseTabCmd, HandleCloseTabCommand));
        }

        public static RoutedUICommand CloseTabCommand
        {
            get { return CloseTabCmd; }
        }

        public int Index
        {
            get { return ParentTabControl == null ? -1 : ParentTabControl.GetTabIndex(this); }
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private ChromeTabControl ParentTabControl
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as ChromeTabControl; }
        }

        public static void SetIsSelected(DependencyObject item, bool value)
        {
            item.SetValue(IsSelectedProperty, value);
        }

        public static bool GetIsSelected(DependencyObject item)
        {
            return (bool) item.GetValue(IsSelectedProperty);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Return)
            {
                ParentTabControl.ChangeSelectedItem(this);
            }
        }

        private void Close()
        {
            ParentTabControl.RemoveTab(this);
        }

        private static void HandleCloseTabCommand(object sender, ExecutedRoutedEventArgs args)
        {
            var item = sender as ChromeTabItem;
            if (item == null)
            {
                return;
            }
            item.Close();
        }
    }
}