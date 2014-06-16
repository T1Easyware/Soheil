using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Soheil.Controls.CustomControls
{
    public class ChromeTabControl : Selector
    {
        internal static readonly DependencyPropertyKey CanAddTabPropertyKey =
            DependencyProperty.RegisterReadOnly("CanAddTab", typeof (bool), typeof (ChromeTabControl),
                                                new PropertyMetadata(true));

        public static readonly DependencyProperty CanAddTabProperty = CanAddTabPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register("SelectedContent", typeof (object), typeof (ChromeTabControl),
                                        new FrameworkPropertyMetadata(null,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender));

        private Dictionary<object, DependencyObject> _objectToContainerMap;

        static ChromeTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ChromeTabControl),
                                                     new FrameworkPropertyMetadata(typeof (ChromeTabControl)));
        }

        public bool CanAddTab
        {
            get { return (bool) GetValue(CanAddTabProperty); }
        }

        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        private Dictionary<object, DependencyObject> ObjectToContainer
        {
            get { return _objectToContainerMap ?? (_objectToContainerMap = new Dictionary<object, DependencyObject>()); }
        }

        public void AddTab(object tab, bool select)
        {
            if (!CanAddTab)
            {
                return;
            }
            Items.Add(tab);
            if (select || Items.Count == 1)
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        public void RemoveTab(object tab)
        {
            int selectedIndex = SelectedIndex;
            bool removedSelectedTab = false;
            ChromeTabItem removeItem = AsTabItem(tab);
            foreach (object item in Items)
            {
                ChromeTabItem tabItem = AsTabItem(item);
                if (tabItem != null && tabItem == removeItem)
                {
                    if (tabItem.Content == SelectedContent)
                    {
                        removedSelectedTab = true;
                    }
                    if (ObjectToContainer.ContainsKey(tab))
                    {
                        ObjectToContainer.Remove(tab);
                    }
                    Items.Remove(item);
                    break;
                }
            }
            if (removedSelectedTab && Items.Count > 0)
            {
                SelectedItem = Items[Math.Min(selectedIndex, Items.Count - 1)];
            }
            else if (removedSelectedTab)
            {
                SelectedItem = null;
                SelectedContent = null;
            }
        }

        internal int GetTabIndex(ChromeTabItem item)
        {
            for (int i = 0; i < Items.Count; i += 1)
            {
                ChromeTabItem tabItem = AsTabItem(Items[i]);
                if (tabItem == item)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ChangeSelectedItem(ChromeTabItem item)
        {
            int index = GetTabIndex(item);
            if (index > -1)
            {
                if (SelectedItem != null)
                {
                    Panel.SetZIndex(AsTabItem(SelectedItem), 0);
                }
                SelectedIndex = index;
                Panel.SetZIndex(item, 1001);
            }
        }

        internal void MoveTab(int fromIndex, int toIndex)
        {
            if (Items.Count == 0)
            {
                return;
            }
            object tab = Items[fromIndex];
            Items.RemoveAt(fromIndex);
            Items.Insert(toIndex, tab);
            for (int i = 0; i < Items.Count; i += 1)
            {
                AsTabItem(Items[i]).Margin = new Thickness(0);
            }
            SelectedItem = tab;
        }

        internal void SetCanAddTab(bool value)
        {
            SetValue(CanAddTabPropertyKey, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ChromeTabItem {Header = "New Tab"};
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ChromeTabItem);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            bool somethingSelected = Items.Cast<UIElement>().Aggregate(false,
                                                                       (current, element) =>
                                                                       current | ChromeTabItem.GetIsSelected(element));
            if (!somethingSelected)
            {
                SelectedIndex = 0;
            }
            KeyboardNavigation.SetIsTabStop(this, false);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    if (ObjectToContainer.ContainsKey(item))
                    {
                        ObjectToContainer.Remove(item);
                    }
                }
            }
            SetChildrenZ();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SetChildrenZ();
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            foreach (UIElement element in Items)
            {
                if (element == e.AddedItems[0])
                {
                    continue;
                }
                ChromeTabItem.SetIsSelected(element, false);
            }
            ChromeTabItem item = AsTabItem(SelectedItem);
            SelectedContent = item != null ? item.Content : null;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element != item)
            {
                ObjectToContainer[item] = element;
                SetChildrenZ();
            }
        }

        private ChromeTabItem AsTabItem(object item)
        {
            var tabItem = item as ChromeTabItem;
            if (tabItem == null && item != null && ObjectToContainer.ContainsKey(item))
            {
                tabItem = ObjectToContainer[item] as ChromeTabItem;
            }
            return tabItem;
        }

        private void SetChildrenZ()
        {
            int zindex = Items.Count - 1;
            foreach (object element in Items)
            {
                ChromeTabItem tabItem = AsTabItem(element);
                if (tabItem == null)
                {
                    continue;
                }
                Panel.SetZIndex(tabItem, ChromeTabItem.GetIsSelected(tabItem) ? Items.Count : zindex);
                zindex -= 1;
            }
        }
    }
}