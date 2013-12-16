using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.ViewModels.Reports
{
	public class CostReportsVm : ViewModelBase, ISingularList, IBarChartViewer
	{
        public AccessType Access { get; set; }
	    public Command InitializeProviderCommand { get; set; }
        public Command NavigateInsideCommand { get; set; }
        public Command NavigateBackCommand { get; set; }
        private readonly Stack<CostBarInfo> _history; 

        public IList<CostBarVm> Bars
        {
            get { return (IList<CostBarVm>)GetValue(BarsProperty); }
            set { SetValue(BarsProperty, value); }
        }
        public static readonly DependencyProperty BarsProperty =
            DependencyProperty.Register("Bars", typeof(IList<CostBarVm>), typeof(CostReportsVm), new UIPropertyMetadata(null));

	    public static readonly DependencyProperty ScalesProperty =
            DependencyProperty.Register("Scales", typeof(ObservableCollection<int>), typeof(CostReportsVm), new PropertyMetadata(default(ObservableCollection<int>)));

	    public ObservableCollection<int> Scales
	    {
            get { return (ObservableCollection<int>)GetValue(ScalesProperty); }
	        set { SetValue(ScalesProperty, value); }
	    }

	    public static readonly DependencyProperty ScaleLinesProperty =
            DependencyProperty.Register("ScaleLines", typeof(ObservableCollection<int>), typeof(CostReportsVm), new PropertyMetadata(default(ObservableCollection<int>)));

        public ObservableCollection<int> ScaleLines
	    {
            get { return (ObservableCollection<int>)GetValue(ScaleLinesProperty); }
	        set { SetValue(ScaleLinesProperty, value); }
	    }

	    public static readonly DependencyProperty ScaleHeightProperty =
	        DependencyProperty.Register("ScaleHeight", typeof (double), typeof (CostReportsVm), new PropertyMetadata(default(double)));

	    public double ScaleHeight
	    {
	        get { return (double) GetValue(ScaleHeightProperty); }
	        set { SetValue(ScaleHeightProperty, value); }
	    }

	    public static readonly DependencyProperty CurrentTypeProperty =
	        DependencyProperty.Register("CurrentType", typeof (CostType), typeof (CostReportsVm), new PropertyMetadata(default(CostType)));

	    public CostType CurrentType
	    {
	        get { return (CostType) GetValue(CurrentTypeProperty); }
	        set { SetValue(CurrentTypeProperty, value); }
	    }

	    public static readonly DependencyProperty HorizontalOffsetProperty =
	        DependencyProperty.Register("HorizontalOffset", typeof (double), typeof (CostReportsVm), new PropertyMetadata(default(double)));

	    public double HorizontalOffset
	    {
	        get { return (double) GetValue(HorizontalOffsetProperty); }
	        set { SetValue(HorizontalOffsetProperty, value); }
	    }

	    public static readonly DependencyProperty CurrentIntervalProperty =
	        DependencyProperty.Register("CurrentInterval", typeof (DateTimeIntervals), typeof (CostReportsVm), new PropertyMetadata(default(DateTimeIntervals)));

	    public DateTimeIntervals CurrentInterval
	    {
	        get { return (DateTimeIntervals) GetValue(CurrentIntervalProperty); }
	        set{ SetValue(CurrentIntervalProperty, value); }
	    }
        public CostReportDataService DataService { get; set; }


        public CostReportsVm(AccessType access)
        {
            CenterPoint = 0;
            StartingPoint = 0;
            Access = access;
            _history = new Stack<CostBarInfo>();
            DataService = new CostReportDataService();
            CurrentType = CostType.All;
            CurrentInterval = DateTimeIntervals.Monthly;
            Scales = new ObservableCollection<int>();
            ScaleLines = new ObservableCollection<int>();
            InitializeProviders(null);
            CenterDateChanged += CostReportsVmCenterDateChanged;
            NavigateInsideCommand = new Command(NavigateInside, CanNavigateInside);
            NavigateBackCommand = new Command(NavigateBack,CanNavigateBack);
            InitializeProviderCommand = new Command(InitializeProviders);
        }

        void CostReportsVmCenterDateChanged(double barHOffset)
        {
            HorizontalOffset = barHOffset;
        }  
		public void InitializeProviders(object param)
		{

            var barInfo = new CostBarInfo();
		    if (param == null)
		        _history.Push(new CostBarInfo());
		    else
                barInfo = (CostBarInfo)param;

            LittleWindowWidth = 20;
            int intervalCount = GetIntervalCount(CurrentInterval, barInfo);

            BarSlides = new VirtualizingCollection<CostBarSlideItemVm>(new CostBarSlideProvider(intervalCount), 6);

            var barProvider = new CostBarProvider(intervalCount, 600, CurrentType, CurrentInterval, DataService, barInfo);
            Bars = new VirtualizingCollection<CostBarVm>(barProvider, 6);

            Scales.Clear();
            ScaleLines.Clear();
            ScaleHeight = barProvider.ScaleHeight;

            for (int i = barProvider.MaxScale; i >= 0; i -= barProvider.StepScale)
            {
                Scales.Add(i);
                ScaleLines.Add(0);
            }
            ScaleLines.RemoveAt(0);
		}

        private int GetIntervalCount(DateTimeIntervals interval, CostBarInfo barInfo)
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
                    return DataService.GetCostCentersCount();

                case 2:
                    return DataService.GetCostsCount(barInfo.Id);

                default:
                    return 0;
            }
        }

        public void NavigateInside(object param)
        {
            CostBarInfo indexId;
            if (param is CostBarVm)
            {
                var barVm = param as CostBarVm;
                if(barVm.MenuItems.Count > 0)
                    return;

                indexId = barVm.Info;
            }
            else
            {
                indexId = (CostBarInfo)param;
            }
            indexId.Level++;
            _history.Push(indexId);
            InitializeProviders(indexId);
        }

        public bool CanNavigateInside()
        {
            return true;
        }

        public void NavigateBack(object param)
        {
            _history.Pop();
            var barInfo = _history.Pop(); 
            _history.Push(barInfo);
            InitializeProviders(barInfo);
        }

        public bool CanNavigateBack()
        {
            return _history.Count > 1;
        }

		#region Slide

        public IList<CostBarSlideItemVm> BarSlides
		{
            get { return (IList<CostBarSlideItemVm>)GetValue(BarSlidesProperty); }
			set { SetValue(BarSlidesProperty, value); }
		}
		public static readonly DependencyProperty BarSlidesProperty =
            DependencyProperty.Register("BarSlides", typeof(IList<CostBarSlideItemVm>), typeof(CostReportsVm), new UIPropertyMetadata(null));

		public double BarWidth
		{
			get { return (double)GetValue(BarWidthProperty); }
			set { SetValue(BarWidthProperty, value); }
		}
		public static readonly DependencyProperty BarWidthProperty =
			DependencyProperty.Register("BarWidth", typeof(double), typeof(CostReportsVm), new UIPropertyMetadata(80d));

		public double LittleWindowWidth
		{
			get { return (double)GetValue(LittleWindowWidthProperty); }
			set { SetValue(LittleWindowWidthProperty, value); }
		}
		public static readonly DependencyProperty LittleWindowWidthProperty =
			DependencyProperty.Register("LittleWindowWidth", typeof(double), typeof(CostReportsVm), new UIPropertyMetadata(0d));
		#endregion

		#region Center
		public double CenterPoint { get; set; }
		public static int StartingPoint { get; private set; }
        private const double Step = 5;

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
