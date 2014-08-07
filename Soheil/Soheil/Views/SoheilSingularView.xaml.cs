using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Soheil.Common;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;
using Soheil.Core.ViewModels.PP;

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
                if (ViewModel is OperationReportsVm)
                {
                    return ViewModel as OperationReportsVm;
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

		private void RepeatButtonClick(object sender, RoutedEventArgs e)
		{
			_dragStartX = -1;
			_mouseIsUp = true;

			_currentSlider = (FrameworkElement)sender;
			_currentScrollViewer = (DateTimeScrollViewer)((FrameworkElement)_currentSlider.TemplatedParent).TemplatedParent;

			double offset = (_currentSlider.Tag == "L") ? -0.2 : 0.2;
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
		public PPTableVm PPTableVm { get { return ViewModel as PPTableVm; } }
	
		#region Members
		SliderType _sliderType = SliderType.none;
		private FrameworkElement ic_months;
		#endregion

		#region ppTable_Loaded, Grid_SizeChanged
		private void ppTable_Loaded(object sender, RoutedEventArgs e)
		{
			ic_months = (ItemsControl)(sender as FrameworkElement).FindChild("ic_months");

			PPTableVm.UpdateWidths();
			PPTableVm.ResetTimeLine();
		}
		private void ppTable_Unloaded(object sender, RoutedEventArgs e)
		{
			var pptable = sender.GetDataContext<PPTableVm>();
			if(pptable!=null) pptable.Dispose();
		}
		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.WidthChanged)
			{
				PPTableVm.GridWidth = ((FrameworkElement)sender).ActualWidth - 20;
				PPTableVm.UpdateWidths();
			}
		} 
		#endregion

		#region Scroll and Zoom

		#region Mouse Down
		private void hoursBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			_sliderType = SliderType.hoursBar;
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

        #endregion
		#endregion

        #region OperatorsReport

        private List<string> _columnHeaders;
        private void SetOperatorReportHeaders()
        {
           
        }

        //private void OnEndDateChanged(object sender, RoutedEventArgs e)
        //{
        //    ((OperationReportsVm)ViewModel).InitializeProviders(null);
        //}

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            SetOperatorReportHeaders();
            if (_columnHeaders.Contains(e.Column.Header.ToString()))
            {
                e.Column.Header = Common.Properties.Resources.ResourceManager.GetString("txt" + e.Column.Header);
            }
            else
            {
                e.Cancel = true;
            }
        }


        #endregion


		private void oeeClicked(object sender, RoutedEventArgs e)
		{
			var control = (sender as FrameworkElement).Tag as Control;
			if (control != null)
				control.Tag = "oee";
		}

		private void generalClicked(object sender, RoutedEventArgs e)
		{
			var control = (sender as FrameworkElement).Tag as Control;
			if (control != null)
				control.Tag = "general";
		}

		private void PMPageLoaded(object sender, EventArgs e)
		{
			var control = (sender as FrameworkElement);
			if (control != null)
			{
				var dc = control.DataContext as Soheil.Core.ViewModels.PM.PmPageBase;
			}
		}

		//private void PmTabsMouseDown(object sender, EventArgs e)
		//{
		//	var arr = (sender as FrameworkElement).Tag as FrameworkElement[];
		//	(arr[0].Tag as Expander).IsExpanded = false;
		//	(arr[1].Tag as Expander).IsExpanded = false;
		//}
	}
}
