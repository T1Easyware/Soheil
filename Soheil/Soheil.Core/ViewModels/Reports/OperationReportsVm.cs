﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.ViewModels.Reports
{
	public class OperationReportsVm : ViewModelBase, ISingularList, IBarChartViewer
	{
        public AccessType Access { get; set; }
	    public Command InitializeProviderCommand { get; set; }
        public Command NavigateInsideCommand { get; set; }
        public Command NavigateBackCommand { get; set; }
        public Command PrintCommand { get; set; }
	    public Command RefreshCommand { get; set; }

        private readonly Stack<OperatorBarInfo> _history; 

        public IList<OperatorBarVm> Bars
        {
            get { return (IList<OperatorBarVm>)GetValue(BarsProperty); }
            set { SetValue(BarsProperty, value); }
        }
        public static readonly DependencyProperty BarsProperty =
            DependencyProperty.Register("Bars", typeof(IList<OperatorBarVm>), typeof(OperationReportsVm), new UIPropertyMetadata(null));

	    public static readonly DependencyProperty ScalesProperty =
            DependencyProperty.Register("Scales", typeof(ObservableCollection<int>), typeof(OperationReportsVm), new PropertyMetadata(default(ObservableCollection<int>)));

	    public ObservableCollection<int> Scales
	    {
            get { return (ObservableCollection<int>)GetValue(ScalesProperty); }
	        set { SetValue(ScalesProperty, value); }
	    }

	    public ObservableCollection<string> ScaleHeaders
	    {
	        get
	        {
                var scaleHeaders = new ObservableCollection<string>();
                foreach (var scale in Scales)
                {
                    scaleHeaders.Add(CurrentType == OEType.CountBased
                        ? scale.ToString(CultureInfo.InvariantCulture)
                        : Format.ConvertToHours(scale));
                }
                return scaleHeaders;
	        }   
	    }

        public static readonly DependencyProperty OperatorProcessReportProperty =
            DependencyProperty.Register("OperatorProcessReport", typeof(OperatorProcessReportVm), typeof(OperationReportsVm), new PropertyMetadata(default(OperatorProcessReportVm)));

        public OperatorProcessReportVm OperatorProcessReport
        {
            get { return (OperatorProcessReportVm)GetValue(OperatorProcessReportProperty); }
            set { SetValue(OperatorProcessReportProperty, value); }
        }

        public static readonly DependencyProperty OprVisibilityProperty =
            DependencyProperty.Register("OprVisibility", typeof(Visibility), typeof(OperationReportsVm), new PropertyMetadata(Visibility.Collapsed));

        public Visibility OprVisibility
        {
            get { return (Visibility)GetValue(OprVisibilityProperty); }
            set { SetValue(OprVisibilityProperty, value); }
        }

	    public static readonly DependencyProperty ScaleLinesProperty =
            DependencyProperty.Register("ScaleLines", typeof(ObservableCollection<int>), typeof(OperationReportsVm), new PropertyMetadata(default(ObservableCollection<int>)));

        public ObservableCollection<int> ScaleLines
	    {
            get { return (ObservableCollection<int>)GetValue(ScaleLinesProperty); }
	        set { SetValue(ScaleLinesProperty, value); }
	    }

	    public static readonly DependencyProperty ScaleHeightProperty =
	        DependencyProperty.Register("ScaleHeight", typeof (double), typeof (OperationReportsVm), new PropertyMetadata(default(double)));

	    public double ScaleHeight
	    {
	        get { return (double) GetValue(ScaleHeightProperty); }
	        set { SetValue(ScaleHeightProperty, value); }
	    }


	    public static readonly DependencyProperty HorizontalOffsetProperty =
	        DependencyProperty.Register("HorizontalOffset", typeof (double), typeof (OperationReportsVm), new PropertyMetadata(default(double)));

	    public double HorizontalOffset
	    {
	        get { return (double) GetValue(HorizontalOffsetProperty); }
	        set { SetValue(HorizontalOffsetProperty, value); }
	    }

	    public static readonly DependencyProperty CurrentIntervalProperty =
	        DependencyProperty.Register("CurrentInterval", typeof (DateTimeIntervals), typeof (OperationReportsVm), new PropertyMetadata(default(DateTimeIntervals)));

	    public DateTimeIntervals CurrentInterval
	    {
	        get { return (DateTimeIntervals) GetValue(CurrentIntervalProperty); }
	        set{ SetValue(CurrentIntervalProperty, value); }
	    }

	    public static readonly DependencyProperty CurrentTypeProperty = DependencyProperty.Register(
	        "CurrentType", typeof (OEType), typeof (OperationReportsVm), new PropertyMetadata(default(OEType)));

	    public OEType CurrentType
	    {
	        get { return (OEType) GetValue(CurrentTypeProperty); }
	        set { SetValue(CurrentTypeProperty, value); }
	    }
        public OperatorReportDataService DataService { get; set; }


        public OperationReportsVm(AccessType access)
        {
            CenterPoint = 0;
            StartingPoint = 0;
            StartDate = DateTime.Now.AddDays(-1);
            EndDate = DateTime.Now;
            Access = access;
            _history = new Stack<OperatorBarInfo>();
            DataService = new OperatorReportDataService();
            CurrentInterval = DateTimeIntervals.Monthly;
            Scales = new ObservableCollection<int>();
            ScaleLines = new ObservableCollection<int>();
            InitializeProviders(null);
            CenterDateChanged += OperationReportsVmCenterDateChanged;
            NavigateInsideCommand = new Command(NavigateInside, CanNavigateInside);
            NavigateBackCommand = new Command(NavigateBack,CanNavigateBack);
            PrintCommand = new Command(Print, CanPrint);
            InitializeProviderCommand = new Command(InitializeProviders);
            RefreshCommand = new Command(Refresh,CanRefresh);
        }

        void OperationReportsVmCenterDateChanged(double barHOffset)
        {
            HorizontalOffset = barHOffset;
        }  
		public void InitializeProviders(object param)
		{
            var barInfo = new OperatorBarInfo {StartDate = StartDate, EndDate = EndDate};

		    if (param == null)
		        _history.Push(new OperatorBarInfo());
		    else
                barInfo = (OperatorBarInfo)param;

		    barInfo.IsCountBase = CurrentType == OEType.CountBased;

            LittleWindowWidth = 20;
            int intervalCount = GetIntervalCount(CurrentInterval, barInfo);

            BarSlides = new VirtualizingCollection<BarSlideItemVm>(new OperatorBarSlideProvider(intervalCount), 6);

            var barProvider = new OperatorBarProvider(intervalCount, 600, CurrentInterval, DataService, barInfo);
            Bars = new VirtualizingCollection<OperatorBarVm>(barProvider, 6);

            Scales.Clear();
            ScaleLines.Clear();
            ScaleHeight = barProvider.ScaleHeight;

            for (int i = barProvider.MaxScale; i >= 0; i -= barProvider.StepScale)
            {
                Scales.Add(i);
                ScaleLines.Add(0);
            }
            ScaleLines.RemoveAt(0);
            OnPropertyChanged("ScaleHeaders");
		}

	    public void LoadOperatorProcessReport(int operatorId)
	    {
	        var dataService = new OperatorReportDataService();
	        OperatorProcessReport = dataService.GetOperatorProcessReport(operatorId, StartDate, EndDate);
	    }

	    private int GetIntervalCount(DateTimeIntervals interval, OperatorBarInfo barInfo)
        {
            switch (barInfo.Level)
            {
                case 0:
                    int currentYear = DateTime.Now.Year;
                    var startDate = new DateTime(currentYear, 1, 1);
                    var endDate = new DateTime(currentYear + 1, 1, 1).AddDays(-1);

                    switch (interval)
                    {
                        case DateTimeIntervals.Hourly:
                            return (int)Math.Ceiling((endDate - startDate).TotalHours);
                        case DateTimeIntervals.Shiftly:
                            return (int)Math.Ceiling((endDate - startDate).TotalDays * SoheilConstants.ShiftPerDay);
                        case DateTimeIntervals.Daily:
                            return (int)Math.Ceiling((endDate - startDate).TotalDays);
                        case DateTimeIntervals.Weekly:
                            return (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
                        case DateTimeIntervals.Monthly:
                            return 12;
                        default:
                            return 12;
                    }

                case 1:
                    return 0;

                case 2:
                    return DataService.GetOperatorsCount();

                default:
                    return 0;
            }
        }

        public void NavigateInside(object param)
        {
            OperatorBarInfo indexId;
            if (param is OperatorBarVm)
            {
                var barVm = param as OperatorBarVm;
                if(barVm.MenuItems.Count > 0)
                    return;

                indexId = barVm.Info;
            }
            else
            {
                indexId = (OperatorBarInfo)param;
            }
            indexId.Level++;
            _history.Push(indexId);
            //InitializeProviders(indexId);
            LoadOperatorProcessReport(indexId.Id);
            OprVisibility = Visibility.Visible;
        }

        public bool CanNavigateInside()
        {
            return true;
        }

        public void NavigateBack(object param)
        {
            OprVisibility = Visibility.Collapsed;
        }

        public bool CanNavigateBack()
        {
            return _history.Count > 1;
        }

	    public void Print(object param)
	    {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == false)
                return;
            var barInfo = _history.Pop();
            string documentTitle = barInfo.Text;
            var pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            var paginator = new CustomDataGridDocumentPaginator(param as DataGrid, documentTitle, pageSize, new Thickness(30, 20, 30, 20));
            printDialog.PrintDocument(paginator, "Grid");


	    }

	    public bool CanPrint()
	    {
	        return true;
	    }

	    public void Refresh(object param)
	    {
	        if (OprVisibility == Visibility.Visible)
	        {
	            var info = _history.Pop();
	            _history.Push(info);
	            LoadOperatorProcessReport(info.Id);
	        }
	        else
	            InitializeProviders(null);
	    }
	    public bool CanRefresh()
	    {
	        return true;
	    }

	    #region Slide

        public IList<BarSlideItemVm> BarSlides
		{
            get { return (IList<BarSlideItemVm>)GetValue(BarSlidesProperty); }
			set { SetValue(BarSlidesProperty, value); }
		}
		public static readonly DependencyProperty BarSlidesProperty =
            DependencyProperty.Register("BarSlides", typeof(IList<BarSlideItemVm>), typeof(OperationReportsVm), new UIPropertyMetadata(null));

		public double BarWidth
		{
			get { return (double)GetValue(BarWidthProperty); }
			set { SetValue(BarWidthProperty, value); }
		}
		public static readonly DependencyProperty BarWidthProperty =
			DependencyProperty.Register("BarWidth", typeof(double), typeof(OperationReportsVm), new UIPropertyMetadata(80d));

		public double LittleWindowWidth
		{
			get { return (double)GetValue(LittleWindowWidthProperty); }
			set { SetValue(LittleWindowWidthProperty, value); }
		}
		public static readonly DependencyProperty LittleWindowWidthProperty =
			DependencyProperty.Register("LittleWindowWidth", typeof(double), typeof(OperationReportsVm), new UIPropertyMetadata(0d));
		#endregion

		#region Center
		public double CenterPoint { get; set; }
		public static int StartingPoint { get; private set; }
        private const double Step = 5;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

		public void MoveCenterBy(double offset, double scrollableRange)
		{
            if (CenterDateChanged != null)
            {
                double value = CenterPoint + (offset * Step);
                if (value < 0 || value > scrollableRange)
                {
                    return;
                }
                CenterDateChanged(value);
                CenterPoint += (offset * Step);
            }
		}
		#endregion

		public event CenterDateChangedEventHandler CenterDateChanged;
		public delegate void CenterDateChangedEventHandler(double barHOffset); 
	}
}
