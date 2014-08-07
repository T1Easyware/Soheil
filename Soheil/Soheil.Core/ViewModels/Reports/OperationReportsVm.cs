using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.Printing;
using Soheil.Core.Reports;
using Soheil.Core.Virtualizing;
using System.Windows.Xps.Packaging;

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

		//public IList<OperatorBarVm> Bars
		//{
		//	get { return (IList<OperatorBarVm>)GetValue(BarsProperty); }
		//	set { SetValue(BarsProperty, value); }
		//}
		//public static readonly DependencyProperty BarsProperty =
		//	DependencyProperty.Register("Bars", typeof(IList<OperatorBarVm>), typeof(OperationReportsVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable collection that indicates Bars
		/// </summary>
		public ObservableCollection<OperatorBarVm> Bars { get { return _bars; } }
		private ObservableCollection<OperatorBarVm> _bars = new ObservableCollection<OperatorBarVm>();


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

        public static readonly DependencyProperty ReportProperty =
            DependencyProperty.Register("Report", typeof(OperatorProcessReportVm), typeof(OperationReportsVm), new PropertyMetadata(default(OperatorProcessReportVm)));

        public OperatorProcessReportVm Report
        {
            get { return (OperatorProcessReportVm)GetValue(ReportProperty); }
            set { SetValue(ReportProperty, value); }
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

	    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof(FixedDocumentSequence), typeof(OperationReportsVm), new PropertyMetadata(default(FixedDocumentSequence)));

	    public FixedDocumentSequence Document
	    {
            get { return (FixedDocumentSequence)GetValue(DocumentProperty); }
	        set { SetValue(DocumentProperty, value); }
	    }
        public OperationReportsVm(AccessType access)
        {
            CenterPoint = 0;
            StartingPoint = 0;
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
            InitializeProviderCommand = new Command(InitializeProviders);
            RefreshCommand = new Command(Refresh,CanRefresh);
        }

        void OperationReportsVmCenterDateChanged(double barHOffset)
        {
            HorizontalOffset = barHOffset;
        }

		public void InitializeProviders(object param)
		{
			_barInfo = new OperatorBarInfo { StartDate = StartDate, EndDate = EndDate };

			if (param == null)
				_history.Push(new OperatorBarInfo());
			else
				_barInfo = (OperatorBarInfo)param;

			_barInfo.IsCountBase = CurrentType == OEType.CountBased;
			_currentInterval = CurrentInterval;

			//LittleWindowWidth = 20;
			//int intervalCount = GetIntervalCount();
			//BarSlides = new VirtualizingCollection<BarSlideItemVm>(new OperatorBarSlideProvider(intervalCount), 100);
			//var barProvider = new OperatorBarProvider(CurrentInterval, DataService, barInfo);
			//Bars = new VirtualizingCollection<OperatorBarVm>(barProvider, 100);

			if(_fetchingThread!=null)
			{
				if (_fetchingThread.IsAlive)
					return;
			}

			_fetchingThread = new System.Threading.Thread(ReadBars);
			_fetchingThread.Start();
		}

		double _maxValue = 0;
		DateTimeIntervals _currentInterval;
		OperatorBarInfo _barInfo;
		IList<OperatorBarInfo> _barInfos;
		System.Threading.Thread _fetchingThread;

		void ReadBars()
		{
			var barProvider = new OperatorBarProvider(_currentInterval, DataService, _barInfo);
			_barInfos = barProvider.FetchAll();
			_maxValue = barProvider.MaxValue;

			//Scales.Clear();
			//ScaleLines.Clear();
			//ScaleHeight = barProvider.ScaleHeight;

			//for (int i = barProvider.MaxScale; i >= 0; i -= barProvider.StepScale)
			//{
			//	Scales.Add(i);
			//	ScaleLines.Add(0);
			//}
			//ScaleLines.RemoveAt(0);

			Dispatcher.Invoke(AddBars);
		}
		void AddBars()
		{
			foreach (var barInfo in _barInfos)
			{
				var bar = new OperatorBarVm
				{
					Info = barInfo,
					Header = barInfo.Text,
					//MainColor
					//MenuItems = GetMenuItems(currentInfo)
				};
				if (barInfo.IsCountBase)
				{
					bar.Value = Convert.ToDouble(barInfo.Data[4]) / _maxValue;
					bar.ProductionValue = Convert.ToDouble(barInfo.Data[5]) / _maxValue;
					bar.DefectionValue = Convert.ToDouble(barInfo.Data[6]) / _maxValue;
					bar.StoppageValue = Convert.ToDouble(barInfo.Data[7]) / _maxValue;
					bar.Tip = Convert.ToString(barInfo.Data[4]);
					bar.ProductionTip = Convert.ToString(barInfo.Data[5]);
					bar.DefectionTip = Convert.ToString(barInfo.Data[6]);
					bar.StoppageTip = Convert.ToString(barInfo.Data[7]);
				}
				else
				{
					bar.Value = Convert.ToDouble(barInfo.Data[0]) / _maxValue;
					bar.ProductionValue = Convert.ToDouble(barInfo.Data[1]) / _maxValue;
					bar.DefectionValue = Convert.ToDouble(barInfo.Data[2]) / _maxValue;
					bar.StoppageValue = Convert.ToDouble(barInfo.Data[3]) / _maxValue;
					bar.Tip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[0]));
					bar.ProductionTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[1]));
					bar.DefectionTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[2]));
					bar.StoppageTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[3]));
				}
				Bars.Add(bar);
			}


			OnPropertyChanged("ScaleHeaders");
		}

	    public void LoadOperatorProcessReport(int operatorId)
	    {
	        var dataService = new OperatorReportDataService();
	        Report = dataService.GetOperatorProcessReport(operatorId, StartDate, EndDate);

	        var reportDocument = new ReportDocument();

	        var reader =
                new StreamReader(new FileStream(@"Views\Reporting\OperatorProcessReport.xaml", FileMode.Open, FileAccess.Read));
	        reportDocument.XamlData = reader.ReadToEnd();
	        reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
	        reader.Close();

	        var data = new ReportData();

	        // set constant document values
	        data.ReportDocumentValues.Add("PrintDate", DateTime.Now);

	        var titleTabel = new DataTable("TitleTable");
	        titleTabel.Columns.Add("ReportTitle", typeof(string));
            var name = Common.Properties.Resources.ResourceManager.GetString("txtName") + Report.Title;
            var code = Common.Properties.Resources.ResourceManager.GetString("txtCode") + Report.Code;
	        var date = DateTime.Now.ToPersianCompactDateTimeString();
            titleTabel.Rows.Add(new object[] { name });
            titleTabel.Rows.Add(new object[] { code });
            titleTabel.Rows.Add(new object[] { date });

            data.DataTables.Add(titleTabel);

            var totalTabel = new DataTable("TotalTable");
            totalTabel.Columns.Add("TimeTotal", typeof(string));
            totalTabel.Columns.Add("CountTotal", typeof(string));

            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalTargetTime") + Format.ConvertToHMS((int) Report.TotalTargetTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalTargetCount") + Report.TotalTargetCount });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalProductionTime") + Format.ConvertToHMS((int)Report.TotalProductionTime), Common.Properties.Resources.ResourceManager.GetString("txTotalProductionCount") + Report.TotalProductionCount });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalExtraTime") + Format.ConvertToHMS((int)Report.TotalExtraTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalExtraCount") + Report.TotalExtraCount });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalShortageTime") + Format.ConvertToHMS((int)Report.TotalShortageTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalShortageCount") + Report.TotalShortageCount });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalDefectionTime") + Format.ConvertToHMS((int)Report.TotalDefectionTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalDefectionCount") + Report.TotalDefectionCount });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalWaste") + Report.TotalWaste, Common.Properties.Resources.ResourceManager.GetString("txtTotalSecondGrade") + Report.TotalSecondGrade });


            data.DataTables.Add(totalTabel);

	        var activitiesTable = new DataTable("ActivitiesReport");

            activitiesTable.Columns.Add("Date", typeof(string));
	        activitiesTable.Columns.Add("Product", typeof (string));
	        activitiesTable.Columns.Add("Station", typeof (string));
	        activitiesTable.Columns.Add("Activity", typeof (string));
	        activitiesTable.Columns.Add("TargetValue", typeof (string));
	        activitiesTable.Columns.Add("ProductionValue", typeof (string));
	        activitiesTable.Columns.Add("DefectionValue", typeof (string));
	        activitiesTable.Columns.Add("StoppageValue", typeof (string));
	        activitiesTable.Columns.Add("IsRework", typeof (string));

	        foreach (var item in Report.ActivityItems)
	        {
	            activitiesTable.Rows.Add(CurrentType == OEType.TimeBased
	                ? new object[]
	                {
	                    item.Date.ToShortDateString(), item.Product, item.Station, item.Activity, item.TargetTime, item.ProductionTime,
	                    item.DefectionTime, item.StoppageTime, item.IsRework
	                }
					: new object[]
	                {
	                    item.Date.ToShortDateString(), item.Product, item.Station, item.Activity, item.TargetCount, item.ProductionCount,
	                    item.DefectionCount, item.StoppageCount, item.IsRework
	                });
	        }

	        data.DataTables.Add(activitiesTable);

            var qualitiveTable = new DataTable("QualitiveReport");

            qualitiveTable.Columns.Add("Date", typeof(string));
            qualitiveTable.Columns.Add("Product", typeof(string));
            qualitiveTable.Columns.Add("Station", typeof(string));
            qualitiveTable.Columns.Add("Activity", typeof(string));
            qualitiveTable.Columns.Add("DefectionValue", typeof(string));
            qualitiveTable.Columns.Add("SecondGrade", typeof(string));
            qualitiveTable.Columns.Add("Waste", typeof(string));

            foreach (var item in Report.QualitiveItems)
            {
                var waste = item.Status == QualitiveStatus.Waste ? "*" : string.Empty;
                var secondGrade = item.Status == QualitiveStatus.SecondGrade ? "*" : string.Empty;
				var defection = CurrentType == OEType.TimeBased ? item.DefectionTime : item.DefectionCount;
                qualitiveTable.Rows.Add(new object[]
                    {
                        item.Date.ToShortDateString(), item.Product, item.Station, item.Activity, defection, secondGrade, waste
	                });
            }

            data.DataTables.Add(qualitiveTable);

            var technicalTable = new DataTable("TechnicalReport");

            technicalTable.Columns.Add("Date", typeof(string));
            technicalTable.Columns.Add("Product", typeof(string));
            technicalTable.Columns.Add("Station", typeof(string));
            technicalTable.Columns.Add("Activity", typeof(string));
            technicalTable.Columns.Add("StoppageValue", typeof(string));

            foreach (var item in Report.TechnicalItems)
            {
				var stoppage = CurrentType == OEType.TimeBased ? item.StoppageTime : item.StoppageCount;
                technicalTable.Rows.Add(new object[]
                    {
                        item.Date.ToShortDateString(), item.Product, item.Station, item.Activity, stoppage
	                });
            }

            data.DataTables.Add(technicalTable);


	        XpsDocument xps = reportDocument.CreateXpsDocument(data);

	        Document = xps.GetFixedDocumentSequence();
            
	    }

	    private int GetIntervalCount()
	    {
	        return DataService.GetOperatorsCount();
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

		//public IList<BarSlideItemVm> BarSlides
		//{
		//	get { return (IList<BarSlideItemVm>)GetValue(BarSlidesProperty); }
		//	set { SetValue(BarSlidesProperty, value); }
		//}
		//public static readonly DependencyProperty BarSlidesProperty =
		//	DependencyProperty.Register("BarSlides", typeof(IList<BarSlideItemVm>), typeof(OperationReportsVm), new UIPropertyMetadata(null));

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

	    public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
	        "StartDate", typeof (DateTime), typeof (OperationReportsVm), new PropertyMetadata(DateTime.Now.AddDays(-1)));

	    public DateTime StartDate
	    {
	        get { return (DateTime) GetValue(StartDateProperty); }
	        set { SetValue(StartDateProperty, value); }
	    }

	    public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
	        "EndDate", typeof (DateTime), typeof (OperationReportsVm), new PropertyMetadata(DateTime.Now));

	    public DateTime EndDate
	    {
	        get { return (DateTime) GetValue(EndDateProperty); }
	        set { SetValue(EndDateProperty, value); }
	    }

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
