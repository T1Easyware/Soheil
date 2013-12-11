using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
    ///     <MyNamespace:ChromiumTabPanel/>
    ///
    /// </summary>
    [ToolboxItem(false)]
    public class ChromeTabPanel : Panel
    {
        private readonly Button _addButton;
        private readonly double _defaultMeasureHeight;
        private readonly Brush _hoverBrush;
        private readonly double _leftMargin;
        private readonly double _maxTabWidth;
        private readonly double _minTabWidth;
        private readonly double _overlap;
        private readonly Brush _pressedBrush;
        private readonly double _rightMargin;
        private Rect _addButtonRect;
        private Size _addButtonSize;
        private int _captureGuard;
        private double _currentTabWidth;
        private Point _downPoint;
        private ChromeTabItem _draggedTab;
        private bool _draggingWindow;
        private Size _finalSize;
        private int _originalIndex;
        private int _slideIndex;
        private List<double> _slideIntervals;
        private ChromeTabControl _tabControlParent;

        static ChromeTabPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ChromeTabPanel),
                                                     new FrameworkPropertyMetadata(typeof (ChromeTabPanel)));
        }

        public ChromeTabPanel()
        {
            _maxTabWidth = 125.0;
            _minTabWidth = 40.0;
            _leftMargin = 50.0;
            _rightMargin = 30.0;
            _overlap = 10.0;
            _defaultMeasureHeight = 30.0;

            var key = new ComponentResourceKey(typeof (ChromeTabPanel), "addButtonStyle");
            var addButtonStyle = (Style) FindResource(key);
            //_addButton = new Button { Style = addButtonStyle };
            _addButton = new Button();
            _addButton.Width = 0;
            _addButton.Height = 0;
            _addButtonSize = new Size(20, 12);

            _hoverBrush = (Brush) FindResource("HoverSelectionBrush");
            _pressedBrush = (Brush) FindResource("PressedSelectionBrush");
        }

		protected override int VisualChildrenCount
        {
			[System.Diagnostics.DebuggerStepThrough]
			get { return base.VisualChildrenCount + 1; }
        }

		private ChromeTabControl ParentTabControl
        {
			[System.Diagnostics.DebuggerStepThrough]
			get
            {
                if (_tabControlParent == null)
                {
                    DependencyObject getParent = this;
                    while (getParent != null && !(getParent is ChromeTabControl))
                    {
                        getParent = VisualTreeHelper.GetParent(getParent);
                    }
                    _tabControlParent = getParent as ChromeTabControl;
                }
                return _tabControlParent;
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override Visual GetVisualChild(int index)
        {
            if (index == VisualChildrenCount - 1)
            {
                return _addButton;
            }
            if (index < VisualChildrenCount - 1)
            {
                return base.GetVisualChild(index);
            }
            throw new IndexOutOfRangeException("Not enough visual children in the ChromeTabPanel.");
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var start = new Point(0, Math.Round(_finalSize.Height));
            var end = new Point(_finalSize.Width, Math.Round(_finalSize.Height));
            object convertFromString = ColorConverter.ConvertFromString("#FF999999");
            if (convertFromString != null)
            {
                var penColor = (Color) convertFromString;
                Brush brush = new SolidColorBrush(penColor);
                var pen = new Pen(brush, .5);
                dc.DrawLine(pen, start, end);
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override Size ArrangeOverride(Size size)
        {
            double activeWidth = size.Width - _leftMargin - _rightMargin;
            _currentTabWidth =
                Math.Min(Math.Max((activeWidth + (Children.Count - 1)*_overlap)/Children.Count, _minTabWidth),
                         _maxTabWidth);
            ParentTabControl.SetCanAddTab(_currentTabWidth > _minTabWidth);
            _addButton.Visibility = _currentTabWidth > _minTabWidth ? Visibility.Visible : Visibility.Collapsed;
            _finalSize = size;
            double offset = _leftMargin;
            foreach (UIElement element in Children)
            {
                var item = ItemsControl.ContainerFromElement(ParentTabControl, element) as ChromeTabItem;
                if (item != null)
                {
                    double thickness = item.Margin.Bottom;
                    element.Arrange(new Rect(offset, 0, _currentTabWidth, size.Height - thickness));
                }
                offset += _currentTabWidth - _overlap;
            }
            _addButtonRect = new Rect(new Point(offset + _overlap, (size.Height - _addButtonSize.Height)/2),
                                      _addButtonSize);
            _addButton.Arrange(_addButtonRect);
            return size;
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override Size MeasureOverride(Size availableSize)
        {
            double activeWidth = double.IsPositiveInfinity(availableSize.Width)
                                     ? 500
                                     : availableSize.Width - _leftMargin - _rightMargin;
            _currentTabWidth =
                Math.Min(Math.Max((activeWidth + (Children.Count - 1)*_overlap)/Children.Count, _minTabWidth),
                         _maxTabWidth);
            ParentTabControl.SetCanAddTab(_currentTabWidth > _minTabWidth);
            _addButton.Visibility = _currentTabWidth > _minTabWidth ? Visibility.Visible : Visibility.Collapsed;
            double height = double.IsPositiveInfinity(availableSize.Height)
                                ? _defaultMeasureHeight
                                : availableSize.Height;
            var resultSize = new Size(0, availableSize.Height);
            foreach (UIElement child in Children)
            {
                var item = ItemsControl.ContainerFromElement(ParentTabControl, child) as ChromeTabItem;
                if (item != null)
                {
                    var tabSize = new Size(_currentTabWidth, height - item.Margin.Bottom);
                    child.Measure(tabSize);
                }
                resultSize.Width += child.DesiredSize.Width - _overlap;
            }
            _addButton.Measure(_addButtonSize);
            resultSize.Width += _addButtonSize.Width;
            return resultSize;
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            SetTabItemsOnTabs();
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            SetTabItemsOnTabs();
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            _slideIntervals = null;
            if (_addButtonRect.Contains(e.GetPosition(this)))
            {
                _addButton.Background = _hoverBrush;
                InvalidateVisual();
                return;
            }

            _downPoint = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, _downPoint);
            if (result == null)
            {
                return;
            }
            DependencyObject source = result.VisualHit;
            while (source != null && !Children.Contains(source as UIElement))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            if (source == null)
            {
                return;
            }
            _draggedTab = source as ChromeTabItem;
            if (_draggedTab != null && Children.Count > 1)
            {
                SetZIndex(_draggedTab, 1000);
            }
            else if (_draggedTab != null && Children.Count == 1)
            {
                _draggingWindow = true;
                Window window = Window.GetWindow(this);
                if (window != null) window.DragMove();
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (_addButtonRect.Contains(e.GetPosition(this)) && _addButton.Background != _pressedBrush &&
                _addButton.Background != _hoverBrush)
            {
                _addButton.Background = _hoverBrush;
                InvalidateVisual();
            }
            else if (!_addButtonRect.Contains(e.GetPosition(this)) && _addButton.Background != null)
            {
                _addButton.Background = null;
                InvalidateVisual();
            }
            if (_draggedTab == null || _draggingWindow)
            {
                return;
            }
            Point nowPoint = e.GetPosition(this);
            var margin = new Thickness(nowPoint.X - _downPoint.X, 0, _downPoint.X - nowPoint.X, 0);
            _draggedTab.Margin = margin;
            if (Math.Abs(margin.Left - 0) > 0.001)
            {
                int guardValue = Interlocked.Increment(ref _captureGuard);
                if (guardValue == 1)
                {
                    _originalIndex = _draggedTab.Index;
                    _slideIndex = _originalIndex + 1;
                    _slideIntervals = new List<double> {double.NegativeInfinity};
                    for (int i = 1; i <= Children.Count; i += 1)
                    {
                        int diff = i - _slideIndex;
                        int sign = diff == 0 ? 0 : diff/Math.Abs(diff);
                        double bound = Math.Min(1, Math.Abs(diff))*
                                       ((sign*_currentTabWidth/3) +
                                        ((Math.Abs(diff) < 2) ? 0 : (diff - sign)*(_currentTabWidth - _overlap)));
                        _slideIntervals.Add(bound);
                    }
                    _slideIntervals.Add(double.PositiveInfinity);
                    CaptureMouse();
                }
                else
                {
                    int changed = 0;
					if (_slideIntervals == null) return;//???
                    if (margin.Left < _slideIntervals[_slideIndex - 1])
                    {
                        SwapSlideInterval(_slideIndex - 1);
                        _slideIndex -= 1;
                        changed = 1;
                    }
                    else if (margin.Left > _slideIntervals[_slideIndex + 1])
                    {
                        SwapSlideInterval(_slideIndex + 1);
                        _slideIndex += 1;
                        changed = -1;
                    }
                    if (changed != 0)
                    {
                        int rightedOriginalIndex = _originalIndex + 1;
                        int diff = 1;
                        if (changed > 0 && _slideIndex >= rightedOriginalIndex)
                        {
                            changed = 0;
                            diff = 0;
                        }
                        else if (changed < 0 && _slideIndex <= rightedOriginalIndex)
                        {
                            changed = 0;
                            diff = 2;
                        }
                        var shiftedTab = Children[_slideIndex - diff] as ChromeTabItem;
                        if (shiftedTab != _draggedTab)
                        {
                            StickyReanimate(shiftedTab, changed*(_currentTabWidth - _overlap), .25);
                        }
                    }
                }
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _draggingWindow = false;
            if (_addButtonRect.Contains(e.GetPosition(this)) && _addButton.Background == _hoverBrush)
            {
                _addButton.Background = null;
                InvalidateVisual();
                if (_addButton.Visibility == Visibility.Visible)
                {
                    //ParentTabControl.AddTab(new Label(), true); // HACK: Do something with default templates, here.
                }
                return;
            }
            if (IsMouseCaptured)
            {
                Mouse.Capture(null);

                double offset = 0;
                if (_slideIndex < _originalIndex + 1)
                {
                    offset = _slideIntervals[_slideIndex + 1] - 2*_currentTabWidth/3 + _overlap;
                }
                else if (_slideIndex > _originalIndex + 1)
                {
                    offset = _slideIntervals[_slideIndex - 1] + 2*_currentTabWidth/3 - _overlap;
                }
                Console.WriteLine(offset);
                Action completed = () =>
                                       {
                                           if (_draggedTab != null)
                                           {
                                               ParentTabControl.ChangeSelectedItem(_draggedTab);
                                               _draggedTab.Margin = new Thickness(offset, 0, -offset, 0);
                                               _draggedTab = null;
                                               _captureGuard = 0;
                                               ParentTabControl.MoveTab(_originalIndex, _slideIndex - 1);
                                           }
                                       };
                Reanimate(_draggedTab, offset, .1, completed);
            }
            else
            {
                if (_draggedTab != null)
                {
                    ParentTabControl.ChangeSelectedItem(_draggedTab);
                    _draggedTab.Margin = new Thickness(0);
                }
                _draggedTab = null;
                _captureGuard = 0;
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            _tabControlParent = null;
        }

		[System.Diagnostics.DebuggerStepThrough]
		private static void StickyReanimate(ChromeTabItem tab, double left, double duration)
        {
            Action completed = () => { tab.Margin = new Thickness(left, 0, -left, 0); };
            Reanimate(tab, left, duration, completed);
        }

		[System.Diagnostics.DebuggerStepThrough]
		private static void Reanimate(ChromeTabItem tab, double left, double duration, Action completed)
        {
            if (tab == null)
            {
                return;
            }
            var offset = new Thickness(left, 0, -left, 0);
            var moveBackAnimation = new ThicknessAnimation(tab.Margin, offset,
                                                           new Duration(TimeSpan.FromSeconds(duration)));
            Storyboard.SetTarget(moveBackAnimation, tab);
            Storyboard.SetTargetProperty(moveBackAnimation, new PropertyPath(MarginProperty));
            var sb = new Storyboard();
            sb.Children.Add(moveBackAnimation);
            sb.Completed += (o, ea) =>
                                {
                                    sb.Remove();
                                    if (completed != null)
                                    {
                                        completed();
                                    }
                                };
            sb.Begin();
        }

		[System.Diagnostics.DebuggerStepThrough]
		private void SetTabItemsOnTabs()
        {
            for (int i = 0; i < Children.Count; i += 1)
            {
                var depObj = Children[i] as DependencyObject;
                if (depObj == null)
                {
                    continue;
                }
                var item = ItemsControl.ContainerFromElement(ParentTabControl, depObj) as ChromeTabItem;
                if (item != null)
                {
                    KeyboardNavigation.SetTabIndex(item, i);
                }
            }
        }

		[System.Diagnostics.DebuggerStepThrough]
		private void SwapSlideInterval(int index)
        {
            _slideIntervals[_slideIndex] = _slideIntervals[index];
            _slideIntervals[index] = 0;
        }
    }
}