﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Soheil.Common;
using Soheil.Controls.CustomControls;
using Soheil.Core.Index;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;
using Soheil.Core.ViewModels.PP;
using Soheil.Common.SoheilException;

namespace Soheil.Views
{
    /// <summary>
    /// Interaction logic for SoheilSingularView.xaml
    /// </summary>
    public partial class SoheilSingularView : INotifyPropertyChanged
    {
		#region General
        public SoheilSingularView(ISingularList viewModel, List<Tuple<string, AccessType>> accessList, Cursor openCursor, Cursor closeCursor)
        {
            InitializeComponent();
            ViewModel = viewModel;
            AccessList = accessList;
            _openCursor = openCursor;
            _closeCursor = closeCursor;
        }
		
		private ISingularList _viewMode;
        public ISingularList ViewModel
        {
            get
            {
                return _viewMode;
            }
            set
            {
                _viewMode = value;
                OnPropertyChanged("ViewModel");
            }
        }

        public List<Tuple<string, AccessType>> AccessList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
		#endregion

		#region Common
		readonly Cursor _openCursor;
		readonly Cursor _closeCursor;
		private bool _mouseIsUp = true;
		double _dragStartX = -1;//Horizontal offset of the current scrolling bar
		FrameworkElement _currentSlider;
		DateTimeScrollViewer _currentScrollViewer = null;
		#endregion

		#region BarChartViewer

		public IBarChartViewer BarChartViewer
        {
            get 
            {
                if (ViewModel is IndicesVm)
                {
                    return ViewModel as IndicesVm;
                }
                if (ViewModel is CostReportsVm)
                {
                    return ViewModel as CostReportsVm;
                }
                if (ViewModel is ActualCostReportsVm)
                {
                    return ViewModel as ActualCostReportsVm;
                }
                return null;
            }
        }
		#endregion

        #region Indices


        private void AreaMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseIsUp) return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                AreaMouseUp(sender, new MouseButtonEventArgs(e.MouseDevice, 0, MouseButton.Left));
                return;
            }
            if (_currentSlider == null) return;
            if (_currentScrollViewer == null) return;
            if (Math.Abs(_dragStartX - -1) < 0.01) return;
            double dx = _dragStartX - e.GetPosition(_currentSlider).X;
            BarChartViewer.MoveCenterBy(dx / (BarChartViewer.BarWidth), _currentScrollViewer.ScrollableWidth);
        }
        private void AreaMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentSlider != null && _currentScrollViewer != null)
            {
                double dx = _dragStartX - e.GetPosition(_currentSlider).X;
                BarChartViewer.CenterPoint += dx / (BarChartViewer.BarWidth);

            }

            Cursor = Cursors.Arrow;
            if (_currentSlider != null)
                _currentSlider.Cursor = _openCursor;
            _currentSlider = null;
            _currentScrollViewer = null;
            _dragStartX = -1;

            _mouseIsUp = true;
        }

        private void RepeatButtonLClick(object sender, RoutedEventArgs e)
        {
            _dragStartX = -1;
            _mouseIsUp = true;
            const double offset = -0.2;

            _currentSlider = (FrameworkElement)sender;
            _currentScrollViewer = (DateTimeScrollViewer)((FrameworkElement)_currentSlider.TemplatedParent).TemplatedParent;

            BarChartViewer.MoveCenterBy(5 * offset, _currentScrollViewer.ScrollableWidth);
                //BarChartViewer.CenterPoint += (5 * offset);

        }
        private void RepeatButtonRClick(object sender, RoutedEventArgs e)
        {
            _dragStartX = -1;
            _mouseIsUp = true;
            const double offset = 0.2;

            _currentSlider = (FrameworkElement)sender;
            _currentScrollViewer = (DateTimeScrollViewer)((FrameworkElement)_currentSlider.TemplatedParent).TemplatedParent;

            BarChartViewer.MoveCenterBy(5 * offset, _currentScrollViewer.ScrollableWidth);
                //BarChartViewer.CenterPoint += (5 * offset);

        }
        private void RectangleMouseDown(object sender, MouseButtonEventArgs e)
        {
            _currentSlider = (FrameworkElement)sender;
            _currentSlider.Cursor = _closeCursor;
            Cursor = _closeCursor;
            _currentScrollViewer = (DateTimeScrollViewer)((FrameworkElement)_currentSlider.TemplatedParent).TemplatedParent;
            _dragStartX = e.GetPosition(_currentSlider).X;
            _mouseIsUp = false;
        }
        #endregion

		#region PPTable

		private enum SliderType
		{
			none,
			monthsBar,
			daysBar,
			hoursBar,
			monthsLittleWindow,
			daysLittleWindow,
		}
	
		#region Members
		SliderType _sliderType = SliderType.none;
		private FrameworkElement ic_months;
		private ScrollViewer _stationsScrollbar;
		private ScrollViewer _activitiesScrollbar;

		public PPTableVm PPTableVm { get { return ViewModel as PPTableVm; } }
		#endregion

		#region Load/Unload PPTable,Task,Job,NPT,PRC
		private void ppTable_Loaded_1(object sender, RoutedEventArgs e)
		{
			ic_months = (ItemsControl)(sender as FrameworkElement).FindChild("ic_months");

			ItemsControl ic_stations = (ItemsControl)(sender as FrameworkElement).FindChild("ic_stations");
			_stationsScrollbar = (ScrollViewer)ic_stations.Template.FindName("sv_stations", ic_stations);
			ItemsControl ic_activities = (ItemsControl)(sender as FrameworkElement).FindChild("ic_activities");
			_activitiesScrollbar = (ScrollViewer)ic_activities.Template.FindName("sv_activities", ic_activities);

			PPTableVm.InitializeViewModel();
			PPTableVm.UpdateWidths();
			PPTableVm.ResetTimeLine();
		}

		//TaskReports Load/Unload
		private void TaskReports_Loaded(object sender, RoutedEventArgs e)
		{
			var task = (sender as FrameworkElement).DataContext as PPTaskVm;
			if (task != null)
			{
				task.ReloadTaskReports();
			}
		}
		private void TaskReports_Unloaded(object sender, RoutedEventArgs e)
		{
			var task = (sender as FrameworkElement).DataContext as PPTaskVm;
			if (task != null)
			{
				task.ClearTaskReports();
			}
		}
		#endregion

		#region Scroll and Zoom

		#region Mouse Down
		private void monthBarItemClick(object sender, MouseButtonEventArgs e)
		{

		}
		private void daysBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			_sliderType = SliderType.daysBar;
			pptableCommonMouseDown(sender, e);
		}
		private void hoursBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			_sliderType = SliderType.hoursBar;
			pptableCommonMouseDown(sender, e);
		}
		private void monthLittleWindowMouseDown(object sender, MouseButtonEventArgs e)
		{
			_sliderType = SliderType.monthsLittleWindow;
			pptableCommonMouseDown(sender, e);
		}
		private void dayLittleWindowMouseDown(object sender, MouseButtonEventArgs e)
		{
			_sliderType = SliderType.daysLittleWindow;
			pptableCommonMouseDown(sender, e);
		}
		private void pptableCommonMouseDown(object sender, MouseButtonEventArgs e)
		{
			_currentSlider = (FrameworkElement)sender;
			_currentSlider.Cursor = _closeCursor;
			this.Cursor = _closeCursor;
			_dragStartX = e.GetPosition(_currentSlider).X;
			_prevX = e.GetPosition(ic_months).X;
			_mouseIsUp = false;
		}
		Point _dragStartPoint;
		#endregion

		#region Mouse Move/Up
		double _prevX;
		[System.Diagnostics.DebuggerStepThrough]
		private void Area_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			#region Checking...
			if (_mouseIsUp) return;
			else if (e.LeftButton == System.Windows.Input.MouseButtonState.Released)
			{
				Area_MouseUp(sender, new System.Windows.Input.MouseButtonEventArgs(e.MouseDevice, 0, System.Windows.Input.MouseButton.Left));
				return;
			}
			if (_currentSlider == null) return; 
			#endregion

			//find out how much is scrolled
			double newX = 0;
			double dx = 0;
			switch (_sliderType)
			{
				case SliderType.none:
					break;
				case SliderType.monthsBar:
					break;
				case SliderType.daysBar:
					break;
				case SliderType.hoursBar:
					newX = e.GetPosition(ic_months).X;
					dx = _prevX - newX;
					_prevX = newX;
					PPTableVm.HoursPassed += (dx / PPTableVm.HourZoom);
					break;
				case SliderType.monthsLittleWindow:
					break;
				case SliderType.daysLittleWindow:
					newX = e.GetPosition(ic_months).X;
					dx = newX - _prevX;
					_prevX = newX;
					PPTableVm.HoursPassed += (dx * 24 / PPTableVm.DayZoom);
					break;
				default:
					break;
			}
			PPTableVm.UpdateRange(false);
		}
		[System.Diagnostics.DebuggerStepThrough]
		private void Area_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (_currentSlider != null)
			{
				//find out how much is scrolled
				double dx = 0;
				switch (_sliderType)
				{
					case SliderType.none:
						break;
					case SliderType.monthsBar:
						break;
					case SliderType.daysBar:
						break;
					case SliderType.hoursBar:
						dx = _dragStartX - e.GetPosition(ic_months).X;
						PPTableVm.HoursPassed = (dx / PPTableVm.HourZoom - PPTableVm.SelectedMonth.DaysFromStartOfYear * 24);
						if (PPTableVm.HoursPassed > 24 * PPTableVm.SelectedMonth.NumOfDays)
						{
							if (PPTableVm.SelectedMonth.ColumnIndex < 11)
							{
								PPTableVm.HoursPassed -= (24 * PPTableVm.SelectedMonth.NumOfDays);
								PPTableVm.SelectedMonth = PPTableVm.Months[PPTableVm.SelectedMonth.ColumnIndex + 1];
							}
							else
								PPTableVm.HoursPassed = PPTableVm.SelectedMonth.NumOfDays * 24 - PPTableVm.GridWidth / PPTableVm.HourZoom;
						}
						else if (PPTableVm.HoursPassed < -PPTableVm.GridWidth / PPTableVm.HourZoom)
						{
							if (PPTableVm.SelectedMonth.ColumnIndex > 0)
							{
								PPTableVm.HoursPassed += (24 * PPTableVm.SelectedMonth.NumOfDays);
								PPTableVm.SelectedMonth = PPTableVm.Months[PPTableVm.SelectedMonth.ColumnIndex - 1];
							}
							else
								PPTableVm.HoursPassed = 0;
						}
						_currentSlider.Cursor = _openCursor;
						break;
					case SliderType.monthsLittleWindow:
						break;
					case SliderType.daysLittleWindow:
						dx = e.GetPosition(ic_months).X - _dragStartX;
						PPTableVm.HoursPassed = (dx * 24 / PPTableVm.DayZoom);
						if (PPTableVm.HoursPassed > 24 * PPTableVm.SelectedMonth.NumOfDays)
						{
							if (PPTableVm.SelectedMonth.ColumnIndex < 11)
							{
								PPTableVm.HoursPassed -= (24 * PPTableVm.SelectedMonth.NumOfDays);
								PPTableVm.SelectedMonth = PPTableVm.Months[PPTableVm.SelectedMonth.ColumnIndex + 1];
							}
							else
								PPTableVm.HoursPassed = PPTableVm.SelectedMonth.NumOfDays * 24 - PPTableVm.GridWidth / PPTableVm.HourZoom;
						}
						else if (PPTableVm.HoursPassed < -PPTableVm.GridWidth / PPTableVm.HourZoom)
						{
							if (PPTableVm.SelectedMonth.ColumnIndex > 0)
							{
								PPTableVm.HoursPassed += (24 * PPTableVm.SelectedMonth.NumOfDays);
								PPTableVm.SelectedMonth = PPTableVm.Months[PPTableVm.SelectedMonth.ColumnIndex - 1];
							}
							else
								PPTableVm.HoursPassed = 0;
						}
						_currentSlider.Cursor = _openCursor;
						break;
					default:
						break;
				}
				PPTableVm.UpdateRange(true);
			}

			#region Terminating...
			this.Cursor = System.Windows.Input.Cursors.Arrow;
			_currentSlider = null;
			_dragStartX = -1;

			_mouseIsUp = true;
			#endregion
		}
		#endregion

		#region Zoom
		private void UndoZoomClicked(object sender, RoutedEventArgs e)
		{
			PPTableVm.RestoreZoom();
		}
		private void ZoomStarted(object sender, MouseButtonEventArgs e)
		{
			if (PPTableVm.SelectedBlock == null)//no block is in report state
				PPTableVm.BackupZoom();
		}
		#endregion

		#region Misc
		private void tasksScrolled(object sender, ScrollChangedEventArgs e)
		{
			if (_stationsScrollbar != null)
				_stationsScrollbar.ScrollToVerticalOffset(e.VerticalOffset);
			PPTableVm.VerticalScreenOffset = e.VerticalOffset;
		}
		private void detailedReportsScrolled(object sender, ScrollChangedEventArgs e)
		{
			if (_activitiesScrollbar != null)
				_activitiesScrollbar.ScrollToVerticalOffset(e.VerticalOffset);
			PPTableVm.VerticalScreenOffset = e.VerticalOffset;
		}
		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			PPTableVm.GridWidth = ((FrameworkElement)sender).ActualWidth - 20;
			PPTableVm.UpdateWidths();
		} 
		#endregion

        #endregion





		#endregion
	}
}
