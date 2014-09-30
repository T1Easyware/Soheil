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
		public Command NavigateNextCommand { get; set; }
		public Command NavigatePreviousCommand { get; set; }
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

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedBar
		/// </summary>
		public OperatorBarVm SelectedBar
		{
			get { return (OperatorBarVm)GetValue(SelectedBarProperty); }
			set { SetValue(SelectedBarProperty, value); }
		}
		public static readonly DependencyProperty SelectedBarProperty =
			DependencyProperty.Register("SelectedBar", typeof(OperatorBarVm), typeof(OperationReportsVm), new PropertyMetadata(null, (d, e) =>
			{
				var vm = (OperationReportsVm)d;
				if (vm._ignoreSelectedBarChanged) return;
				var val = (OperatorBarVm)e.NewValue;
				if (val == null) return;
				vm.NavigateInside(val);
			}));
		private bool _ignoreSelectedBarChanged = false;

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
                        : Format.ConvertToHM(scale));
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
            NavigateInsideCommand = new Command(NavigateInside, CanNavigateInside);
			NavigateBackCommand = new Command(NavigateBack, CanNavigateBack);
			NavigateNextCommand = new Command(NavigateNext, CanNavigateNext);
			NavigatePreviousCommand = new Command(NavigatePrevious, CanNavigatePrevious);
            InitializeProviderCommand = new Command(InitializeProviders);
            RefreshCommand = new Command(Refresh,CanRefresh);
        }


		public void InitializeProviders(object param)
		{
			_barInfo = new OperatorBarInfo { StartDate = StartDate, EndDate = EndDate };

			if (param == null)
				_history.Push(new OperatorBarInfo());
			else
				_barInfo = (OperatorBarInfo)param;

			_barInfo.CurrentType = CurrentType;
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

		int _currentOperatorIndex = 0;
		int _operatorsCount = 0;
		int _maxValue = 0;
		DateTimeIntervals _currentInterval;
		OperatorBarInfo _barInfo;
		List<OperatorBarInfo> _orderedList;
		IList<OperatorBarInfo> _barInfos;
		System.Threading.Thread _fetchingThread;

		void ReadBars()
		{
			var barProvider = new OperatorBarProvider(_currentInterval, DataService, _barInfo);
			_barInfos = barProvider.FetchAll();
			_maxValue = barProvider.GetMaxScale();

			if (_barInfo.CurrentType == OEType.TimeBased)
				_orderedList = _barInfos.OrderByDescending(x => (float)x.Data[1] / (float)x.Data[0]).ToList();
			else
				_orderedList = _barInfos.OrderByDescending(x => (int)x.Data[5] / (double)((int)x.Data[4])).ToList();

			_operatorsCount = _orderedList.Count;
			for (int i = 0; i < _operatorsCount; i++)
			{
				var x = _orderedList[i];
				x.Index = i;
				_orderedList[i] = x;
			}

			Dispatcher.Invoke(AddBars);
		}
		void AddBars()
		{
			Scales.Clear();
			ScaleLines.Clear();
			for (int i = 10; i > 0; i--)
			{
				Scales.Add(i * _maxValue / 10);
				ScaleLines.Add(i * _maxValue / 10);
			}

			Bars.Clear();
			foreach (var barInfo in _orderedList)
			{
				var bar = new OperatorBarVm
				{
					Info = barInfo,
					Header = barInfo.Text,
					//MainColor
					//MenuItems = GetMenuItems(currentInfo)
				};
				switch (_barInfo.CurrentType)
				{
					case OEType.None:
						bar.Value = Convert.ToDouble(barInfo.Data[0]) / _maxValue;
						bar.ProductionValue = Convert.ToDouble(barInfo.Data[1]) / _maxValue;
						bar.DefectionValue = Convert.ToDouble(barInfo.Data[2]) / _maxValue;
						bar.StoppageValue = Convert.ToDouble(barInfo.Data[3]) / _maxValue;
						bar.Tip = Convert.ToString(barInfo.Data[4]);
						bar.RemainingTip = Convert.ToString((int)barInfo.Data[4] - ((int)barInfo.Data[5] + (int)barInfo.Data[6] + (int)barInfo.Data[7]));
						bar.ProductionTip = Convert.ToString(barInfo.Data[5]);
						bar.DefectionTip = Convert.ToString(barInfo.Data[6]);
						bar.StoppageTip = Convert.ToString(barInfo.Data[7]);
						break;
					case OEType.TimeBased:
						bar.Value = Convert.ToDouble(barInfo.Data[0]) / _maxValue;
						bar.ProductionValue = Convert.ToDouble(barInfo.Data[1]) / _maxValue;
						bar.DefectionValue = Convert.ToDouble(barInfo.Data[2]) / _maxValue;
						bar.StoppageValue = Convert.ToDouble(barInfo.Data[3]) / _maxValue;
						bar.Tip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[0]));
						bar.RemainingTip = Format.ConvertToHM((int)((float)barInfo.Data[0] - ((float)barInfo.Data[1] + (float)barInfo.Data[2] + (float)barInfo.Data[3])));
						bar.ProductionTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[1]));
						bar.DefectionTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[2]));
						bar.StoppageTip = Format.ConvertToHM(Convert.ToInt32(barInfo.Data[3]));
						break;
					case OEType.CountBased:
						bar.Value = Convert.ToDouble(barInfo.Data[4]) / _maxValue;
						bar.ProductionValue = Convert.ToDouble(barInfo.Data[5]) / _maxValue;
						bar.DefectionValue = Convert.ToDouble(barInfo.Data[6]) / _maxValue;
						bar.StoppageValue = Convert.ToDouble(barInfo.Data[7]) / _maxValue;
						bar.Tip = Convert.ToString(barInfo.Data[4]);
						bar.RemainingTip = Convert.ToString((int)barInfo.Data[4] - ((int)barInfo.Data[5] + (int)barInfo.Data[6] + (int)barInfo.Data[7]));
						bar.ProductionTip = Convert.ToString(barInfo.Data[5]);
						bar.DefectionTip = Convert.ToString(barInfo.Data[6]);
						bar.StoppageTip = Convert.ToString(barInfo.Data[7]);
						break;
					default:
						break;
				}
				Bars.Add(bar);
			}


			OnPropertyChanged("ScaleHeaders");
		}

	    public void LoadOperatorProcessReport(OperatorBarInfo operatorInfo)
	    {
			_currentOperatorIndex = operatorInfo.Index;
	        var dataService = new OperatorReportDataService();
			Report = dataService.GetOperatorProcessReport(operatorInfo.Id, StartDate, EndDate);

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
	        titleTabel.Columns.Add("ReportTitleName", typeof(string));
			titleTabel.Columns.Add("ReportTitleCode", typeof(string));
			titleTabel.Columns.Add("ReportTitleDate", typeof(string));
			var name = Common.Properties.Resources.ResourceManager.GetString("txtName") + Report.Title;
			var code = Common.Properties.Resources.ResourceManager.GetString("txtCode") + Report.Code;
			var date = DateTime.Now.ToPersianCompactDateTimeString();
			titleTabel.Rows.Add(new object[] { name, code, date });

            data.DataTables.Add(titleTabel);

            var totalTabel = new DataTable("TotalTable");
            totalTabel.Columns.Add("TimeTotal", typeof(string));
            totalTabel.Columns.Add("CountTotal", typeof(string));

			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalTargetTime") + Format.ConvertToHMS((int)Report.TotalTargetTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalTargetCount") + Report.TotalTargetCount.ToString("##", CultureInfo.InvariantCulture) });
			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalProductionTime") + Format.ConvertToHMS((int)Report.TotalProductionTime), Common.Properties.Resources.ResourceManager.GetString("txTotalProductionCount") + Report.TotalProductionCount.ToString("##", CultureInfo.InvariantCulture) });
			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalExtraTime") + Format.ConvertToHMS((int)Report.TotalExtraTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalExtraCount") + Report.TotalExtraCount.ToString("##", CultureInfo.InvariantCulture) });
			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalShortageTime") + Format.ConvertToHMS((int)Report.TotalShortageTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalShortageCount") + Report.TotalShortageCount.ToString("##", CultureInfo.InvariantCulture) });
			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalDefectionTime") + Format.ConvertToHMS((int)Report.TotalDefectionTime), Common.Properties.Resources.ResourceManager.GetString("txtTotalDefectionCount") + Report.TotalDefectionCount.ToString("##", CultureInfo.InvariantCulture) });
			totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalWaste") + Report.TotalWaste, Common.Properties.Resources.ResourceManager.GetString("txtTotalSecondGrade") + Report.TotalSecondGrade.ToString("##", CultureInfo.InvariantCulture) });


            data.DataTables.Add(totalTabel);

	        var activitiesTable = new DataTable("ActivitiesReport");

            activitiesTable.Columns.Add("Date", typeof(string));
	        activitiesTable.Columns.Add("Product", typeof (string));
	        activitiesTable.Columns.Add("Station", typeof (string));
	        activitiesTable.Columns.Add("Activity", typeof (string));
	        activitiesTable.Columns.Add("TargetValue", typeof (string));
	        activitiesTable.Columns.Add("ProductionValue", typeof (string));
	        activitiesTable.Columns.Add("DefectionValue", typeof (string));
			activitiesTable.Columns.Add("StoppageValue", typeof(string));
			activitiesTable.Columns.Add("ShortageValue", typeof(string));
			activitiesTable.Columns.Add("ExtraValue", typeof(string));
	        activitiesTable.Columns.Add("IsRework", typeof (string));

	        foreach (var item in Report.ActivityItems)
	        {
	            activitiesTable.Rows.Add(CurrentType == OEType.TimeBased
	                ? new object[]
	                {
	                    item.Date.ToPersianCompactDateString(), item.Product, item.Station, item.Activity, item.TargetTime, item.ProductionTime,
	                    item.DefectionTime, item.StoppageTime, item.ShortageTime, item.ExtraTime, item.IsRework
	                }
					: new object[]
	                {
	                    item.Date.ToPersianCompactDateString(), item.Product, item.Station, item.Activity, item.TargetCount, item.ProductionCount,
	                    item.DefectionCount, item.StoppageCount, item.ShortageCount, item.ExtraCount, item.IsRework
	                });
	        }

	        data.DataTables.Add(activitiesTable);

            var qualitiveTable = new DataTable("QualitiveReport");

            qualitiveTable.Columns.Add("Date", typeof(string));
            qualitiveTable.Columns.Add("Product", typeof(string));
            qualitiveTable.Columns.Add("Station", typeof(string));
            qualitiveTable.Columns.Add("Activity", typeof(string));
            qualitiveTable.Columns.Add("DefectionValue", typeof(string));
			qualitiveTable.Columns.Add("DefectionType", typeof(string));

            foreach (var item in Report.QualitiveItems)
            {
                var wasteType = item.Status == QualitiveStatus.Waste ? 
					Common.Properties.Resources.ResourceManager.GetString("txtWaste"):
					Common.Properties.Resources.ResourceManager.GetString("txtSecondGrade");
				var defection = CurrentType == OEType.TimeBased ? item.DefectionTime : item.DefectionCount;
                qualitiveTable.Rows.Add(new object[]
                    {
                        item.Date.ToPersianCompactDateString(), item.Product, item.Station, item.Activity, defection, wasteType
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
                        item.Date.ToPersianCompactDateString(), item.Product, item.Station, item.Activity, stoppage
	                });
            }

            data.DataTables.Add(technicalTable);


	        XpsDocument xps = reportDocument.CreateXpsDocument(data);

	        Document = xps.GetFixedDocumentSequence();
	    }

	    public void NavigateInside(object param)
        {
			_ignoreSelectedBarChanged = true;
			OperatorBarInfo indexId;
            if (param is OperatorBarVm)
            {
                var barVm = param as OperatorBarVm;
                if(barVm.MenuItems.Count > 0)
                    return;

				SelectedBar = barVm;
				indexId = barVm.Info;
            }
            else
            {
                indexId = (OperatorBarInfo)param;
				SelectedBar = Bars.FirstOrDefault(x => x.Info.Id == indexId.Id);
			}
            indexId.Level++;
            _history.Push(indexId);
            
			//InitializeProviders(indexId);
            LoadOperatorProcessReport(indexId);
            OprVisibility = Visibility.Visible;
			_ignoreSelectedBarChanged = false;
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

		public void NavigateNext(object param)
		{
			var item = _orderedList.FirstOrDefault(x => x.Index == _currentOperatorIndex + 1);
			if (CanNavigateInside()) NavigateInside(item);
			OprVisibility = Visibility.Visible;
		}

		public bool CanNavigateNext()
		{
			return _currentOperatorIndex + 1 < _operatorsCount;
		}

		public void NavigatePrevious(object param)
		{
			var item = _orderedList.FirstOrDefault(x => x.Index == _currentOperatorIndex - 1);
			if (CanNavigateInside()) NavigateInside(item);
			OprVisibility = Visibility.Visible;
		}

		public bool CanNavigatePrevious()
		{
			return _currentOperatorIndex > 0;
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
	            LoadOperatorProcessReport(info);
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
