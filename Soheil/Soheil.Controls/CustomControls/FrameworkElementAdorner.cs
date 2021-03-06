﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Soheil.Controls.CustomControls
{
    //
    // This class is an adorner that allows a FrameworkElement derived class to adorn another FrameworkElement.
    //
    public class FrameworkElementAdorner : Adorner
    {
        //
        // The framework element that is the adorner. 
        //
        private readonly FrameworkElement _child;

        //
        // Placement of the child.
        //
        private readonly AdornerPlacement _horizontalAdornerPlacement = AdornerPlacement.Inside;

        //
        // Offset of the child.
        //
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly AdornerPlacement _verticalAdornerPlacement = AdornerPlacement.Inside;

        //
        // Position of the child (when not set to NaN).
        //
        private double _positionX = Double.NaN;
        private double _positionY = Double.NaN;

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _child = adornerChildElement;

            AddLogicalChild(adornerChildElement);
            AddVisualChild(adornerChildElement);
        }

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement,
                                       AdornerPlacement horizontalAdornerPlacement,
                                       AdornerPlacement verticalAdornerPlacement,
                                       double offsetX, double offsetY)
            : base(adornedElement)
        {
            _child = adornerChildElement;
            _horizontalAdornerPlacement = horizontalAdornerPlacement;
            _verticalAdornerPlacement = verticalAdornerPlacement;
            _offsetX = offsetX;
            _offsetY = offsetY;

            adornedElement.SizeChanged += AdornedElementSizeChanged;

            AddLogicalChild(adornerChildElement);
            AddVisualChild(adornerChildElement);
        }

        //
        // Position of the child (when not set to NaN).
        //
        public double PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }

        public double PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }

        protected override Int32 VisualChildrenCount
        {
            get { return 1; }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                var list = new ArrayList {_child};
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Override AdornedElement from base class for less type-checking.
        /// </summary>
        public new FrameworkElement AdornedElement
        {
            get { return (FrameworkElement) base.AdornedElement; }
        }

        /// <summary>
        /// Event raised when the adorned control's size has changed.
        /// </summary>
        private void AdornedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        /// <summary>
        /// Determine the X coordinate of the child.
        /// </summary>
        private double DetermineX()
        {
            switch (_child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        if (_horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -_child.DesiredSize.Width + _offsetX;
                        }
                        return _offsetX;
                    }
                case HorizontalAlignment.Right:
                    {
                        if (_horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            double adornedWidth = AdornedElement.ActualWidth;
                            return adornedWidth + _offsetX;
                        }
                        else
                        {
                            double adornerWidth = _child.DesiredSize.Width;
                            double adornedWidth = AdornedElement.ActualWidth;
                            double x = adornedWidth - adornerWidth;
                            return x + _offsetX;
                        }
                    }
                case HorizontalAlignment.Center:
                    {
                        double adornerWidth = _child.DesiredSize.Width;
                        double adornedWidth = AdornedElement.ActualWidth;
                        double x = (adornedWidth/2) - (adornerWidth/2);
                        return x + _offsetX;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the Y coordinate of the child.
        /// </summary>
        private double DetermineY()
        {
            switch (_child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        if (_verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -_child.DesiredSize.Height + _offsetY;
                        }
                        return _offsetY;
                    }
                case VerticalAlignment.Bottom:
                    {
                        if (_verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            double adornedHeight = AdornedElement.ActualHeight;
                            return adornedHeight + _offsetY;
                        }
                        else
                        {
                            double adornerHeight = _child.DesiredSize.Height;
                            double adornedHeight = AdornedElement.ActualHeight;
                            double x = adornedHeight - adornerHeight;
                            return x + _offsetY;
                        }
                    }
                case VerticalAlignment.Center:
                    {
                        double adornerHeight = _child.DesiredSize.Height;
                        double adornedHeight = AdornedElement.ActualHeight;
                        double x = (adornedHeight/2) - (adornerHeight/2);
                        return x + _offsetY;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the width of the child.
        /// </summary>
        private double DetermineWidth()
        {
            if (!Double.IsNaN(PositionX))
            {
                return _child.DesiredSize.Width;
            }

            switch (_child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        return _child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Right:
                    {
                        return _child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Center:
                    {
                        return _child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return AdornedElement.ActualWidth;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the height of the child.
        /// </summary>
        private double DetermineHeight()
        {
            if (!Double.IsNaN(PositionY))
            {
                return _child.DesiredSize.Height;
            }

            switch (_child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        return _child.DesiredSize.Height;
                    }
                case VerticalAlignment.Bottom:
                    {
                        return _child.DesiredSize.Height;
                    }
                case VerticalAlignment.Center:
                    {
                        return _child.DesiredSize.Height;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return AdornedElement.ActualHeight;
                    }
            }

            return 0.0;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = PositionX;
            if (Double.IsNaN(x))
            {
                x = DetermineX();
            }
            double y = PositionY;
            if (Double.IsNaN(y))
            {
                y = DetermineY();
            }
            double adornerWidth = DetermineWidth();
            double adornerHeight = DetermineHeight();
            _child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            return finalSize;
        }

        protected override Visual GetVisualChild(Int32 index)
        {
            return _child;
        }

        /// <summary>
        /// Disconnect the child element from the visual tree so that it may be reused later.
        /// </summary>
        public void DisconnectChild()
        {
            RemoveLogicalChild(_child);
            RemoveVisualChild(_child);
        }
    }
}