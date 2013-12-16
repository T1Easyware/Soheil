using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Index;
using Soheil.Core.Interfaces;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.ViewModels.Index
{
	public class IndicesVm : ViewModelBase, ISingularList, IBarChartViewer
	{
        public AccessType Access { get; set; }
	    public Command InitializeProviderCommand { get; set; }
        public Command NavigateInsideCommand { get; set; }
        public Command NavigateBackCommand { get; set; }
        private readonly Stack<IndexBarInfo> _history; 

        public IList<IndexBarVm> Bars
        {
            get { return (IList<IndexBarVm>)GetValue(BarsProperty); }
            set { SetValue(BarsProperty, value); }
        }
        public static readonly DependencyProperty BarsProperty =
            DependencyProperty.Register("Bars", typeof(IList<IndexBarVm>), typeof(IndicesVm), new UIPropertyMetadata(null));

	    public static readonly DependencyProperty ScalesProperty =
            DependencyProperty.Register("Scales", typeof(ObservableCollection<int>), typeof(IndicesVm), new PropertyMetadata(default(ObservableCollection<int>)));

	    public ObservableCollection<int> Scales
	    {
            get { return (ObservableCollection<int>)GetValue(ScalesProperty); }
	        set { SetValue(ScalesProperty, value); }
	    }

	    public static readonly DependencyProperty ScaleLinesProperty =
            DependencyProperty.Register("ScaleLines", typeof(ObservableCollection<int>), typeof(IndicesVm), new PropertyMetadata(default(ObservableCollection<int>)));

        public ObservableCollection<int> ScaleLines
	    {
            get { return (ObservableCollection<int>)GetValue(ScaleLinesProperty); }
	        set { SetValue(ScaleLinesProperty, value); }
	    }

        public static readonly DependencyProperty ScaleHeightProperty =
            DependencyProperty.Register("ScaleHeight", typeof(double), typeof(IndicesVm), new PropertyMetadata(default(double)));

        public double ScaleHeight
        {
            get { return (double)GetValue(ScaleHeightProperty); }
            set { SetValue(ScaleHeightProperty, value); }
        }

	    public static readonly DependencyProperty CurrentTypeProperty =
	        DependencyProperty.Register("CurrentType", typeof (IndexType), typeof (IndicesVm), new PropertyMetadata(default(IndexType)));

	    public IndexType CurrentType
	    {
	        get { return (IndexType) GetValue(CurrentTypeProperty); }
	        set { SetValue(CurrentTypeProperty, value); }
	    }

	    public static readonly DependencyProperty HorizontalOffsetProperty =
	        DependencyProperty.Register("HorizontalOffset", typeof (double), typeof (IndicesVm), new PropertyMetadata(default(double)));

	    public double HorizontalOffset
	    {
	        get { return (double) GetValue(HorizontalOffsetProperty); }
	        set { SetValue(HorizontalOffsetProperty, value); }
	    }

	    public static readonly DependencyProperty CurrentIntervalProperty =
	        DependencyProperty.Register("CurrentInterval", typeof (DateTimeIntervals), typeof (IndicesVm), new PropertyMetadata(default(DateTimeIntervals)));

	    public DateTimeIntervals CurrentInterval
	    {
	        get { return (DateTimeIntervals) GetValue(CurrentIntervalProperty); }
	        set{ SetValue(CurrentIntervalProperty, value); }
	    }
        public IndexDataService DataService { get; set; }


        public IndicesVm(AccessType access)
        {
            CenterPoint = 0;
            StartingPoint = 0;
            Access = access;
            _history = new Stack<IndexBarInfo>();
            DataService = new IndexDataService();
            CurrentType = IndexType.OEE;
            CurrentInterval = DateTimeIntervals.Monthly;
            Scales = new ObservableCollection<int>();
            ScaleLines = new ObservableCollection<int>();
            InitializeProviders(null);
            CenterDateChanged += IndicesVmCenterDateChanged;
            NavigateInsideCommand = new Command(NavigateInside, CanNavigateInside);
            NavigateBackCommand = new Command(NavigateBack,CanNavigateBack);
            InitializeProviderCommand = new Command(InitializeProviders);
        }

        void IndicesVmCenterDateChanged(double barHOffset)
        {
            HorizontalOffset = barHOffset;
        }  
		public void InitializeProviders(object param)
		{

            var indexId = new IndexBarInfo();
		    if (param == null)
		        _history.Push(new IndexBarInfo());
		    else
                indexId = (IndexBarInfo)param;

            LittleWindowWidth = 20;
            int intervalCount = GetIntervalCount(indexId);

            BarSlides = new VirtualizingCollection<IndexBarSlideItemVm>(new IndexBarSlideProvider(intervalCount), 6);

            var barProvider = new IndexBarProvider(intervalCount, 600, CurrentType, CurrentInterval, DataService, indexId);
            Bars = new VirtualizingCollection<IndexBarVm>(barProvider, 6);

            Scales.Clear();
            ScaleLines.Clear();

            for (int i = barProvider.MaxScale + barProvider.StepScale; i >= 0; i -= barProvider.StepScale)
            {
                Scales.Add(i);
                ScaleLines.Add(0);
            }
            ScaleLines.RemoveAt(0);
		}

        private int GetIntervalCount(IndexBarInfo info)
        {
            if (info.Level == 0)
            {
                int currentYear = DateTime.Now.Year;
                var startDate = new DateTime(currentYear, 1, 1);
                var endDate = new DateTime(currentYear + 1, 1, 1).AddDays(-1);

                switch (CurrentInterval)
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
            }
            switch (info.Filter)
            {
                case IndexFilter.None:
                    break;
                case IndexFilter.ByProduct:
                    return DataService.GetProductCount();
                case IndexFilter.ByStation:
                    return DataService.GetStationCount();
                case IndexFilter.ByActivity:
                    return DataService.GetActivityCount();
                case IndexFilter.ByOperator:
                    return DataService.GetOperatorCount();
                case IndexFilter.ByMachine:
                    return DataService.GetMachineCount();
                case IndexFilter.ByCauseL1:
                    break;
                case IndexFilter.ByCauseL2:
                    break;
                case IndexFilter.ByCauseL3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return 12;
        }

        public void NavigateInside(object param)
        {
            IndexBarInfo indexId;
            if (param is IndexBarVm)
            {
                var barVm = param as IndexBarVm;
                if(barVm.MenuItems.Count > 0)
                    return;

                indexId = barVm.Info;
            }
            else
            {
                indexId = (IndexBarInfo)param;
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

        public IList<IndexBarSlideItemVm> BarSlides
		{
            get { return (IList<IndexBarSlideItemVm>)GetValue(BarSlidesProperty); }
			set { SetValue(BarSlidesProperty, value); }
		}
		public static readonly DependencyProperty BarSlidesProperty =
            DependencyProperty.Register("BarSlides", typeof(IList<IndexBarSlideItemVm>), typeof(IndicesVm), new UIPropertyMetadata(null));

		public double BarWidth
		{
			get { return (double)GetValue(BarWidthProperty); }
			set { SetValue(BarWidthProperty, value); }
		}
		public static readonly DependencyProperty BarWidthProperty =
			DependencyProperty.Register("BarWidth", typeof(double), typeof(IndicesVm), new UIPropertyMetadata(80d));

		public double LittleWindowWidth
		{
			get { return (double)GetValue(LittleWindowWidthProperty); }
			set { SetValue(LittleWindowWidthProperty, value); }
		}
		public static readonly DependencyProperty LittleWindowWidthProperty =
			DependencyProperty.Register("LittleWindowWidth", typeof(double), typeof(IndicesVm), new UIPropertyMetadata(0d));
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
