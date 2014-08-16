using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.Printing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace Soheil.Core.ViewModels.Reports
{
	public class DailyReportVm : ViewModelBase, ISingularList
	{
		public DailyReportVm(AccessType access)
		{
			Access = access;
			NavigateNextCommand = new Command(NavigateNext);
			NavigatePreviousCommand = new Command(NavigatePrevious);
			RefreshCommand = new Command(Refresh, () => true);
			_workProfilePlanDataService = new DataServices.WorkProfilePlanDataService();
			StartDate = DateTime.Now.Date;
			EndDate = DateTime.Now.AddDays(1).Date;
		}


		#region Properties
		public AccessType Access { get; set; }
		public Command NavigateNextCommand { get; set; }
		public Command NavigatePreviousCommand { get; set; }
		public Command PrintCommand { get; set; }
		public Command RefreshCommand { get; set; }
		DataServices.WorkProfilePlanDataService _workProfilePlanDataService;
		#endregion		

		#region DependencyProperties

		public FixedDocumentSequence Document
		{
			get { return (FixedDocumentSequence)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FixedDocumentSequence), typeof(OperationReportsVm), new PropertyMetadata(default(FixedDocumentSequence)));

		/// <summary>
		/// Gets or sets a bindable value that indicates StartDate
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(DailyReportVm), new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (DailyReportVm)d;
				var val = (DateTime)e.NewValue;
				vm.StartTime = vm._workProfilePlanDataService.GetShiftStartAt(val).TimeOfDay;
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates StartTime
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(DailyReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		public DateTime StartDateTime { get { return StartDate.Add(StartTime); } }
		/// <summary>
		/// Gets or sets a bindable value that indicates EndDate
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(DailyReportVm), new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (DailyReportVm)d;
				var val = (DateTime)e.NewValue;
				vm.EndTime = vm._workProfilePlanDataService.GetShiftStartAt(val).TimeOfDay;
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndTime
		/// </summary>
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DailyReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		public DateTime EndDateTime { get { return EndDate.Add(EndTime); } }
		/// <summary>
		/// Gets or sets a bindable value that indicates IsOneDay
		/// </summary>
		public bool IsOneDay
		{
			get { return (bool)GetValue(IsOneDayProperty); }
			set { SetValue(IsOneDayProperty, value); }
		}
		public static readonly DependencyProperty IsOneDayProperty =
			DependencyProperty.Register("IsOneDay", typeof(bool), typeof(DailyReportVm), new UIPropertyMetadata(true));
		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(DailyReportVm), new UIPropertyMetadata(null));
		#endregion

		#region Methods
		void NavigateNext(object param)
		{

		}
		void NavigatePrevious(object param)
		{

		}
		void Refresh(object param)
		{
			LoadDailyReport();
		}

		public void LoadDailyReport()
		{
			#region Init
			var dataService = new DataServices.ProcessReportDataService();
			var reports = dataService.GetDailyReport(StartDateTime, EndDateTime);

			var reportDocument = new Soheil.Core.Printing.ReportDocument();

			var reader =
				new StreamReader(new FileStream(@"Views\Reporting\DailyReportDocument.xaml", FileMode.Open, FileAccess.Read));
			reportDocument.XamlData = reader.ReadToEnd();
			reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
			reader.Close();

			var data = new ReportData();

			// set constant document values
			data.ReportDocumentValues.Add("PrintDate", DateTime.Now); 
			#endregion

			#region Title table
			var titleTabel = new DataTable("TitleTable");
			titleTabel.Columns.Add("ReportTitleDate", typeof(string));
			var date = Common.Properties.Resources.ResourceManager.GetString("txtDate") + StartDateTime.ToPersianCompactDateString();
			if (!IsOneDay) date =
				Common.Properties.Resources.ResourceManager.GetString("txtStart") + StartDateTime.ToPersianCompactDateString() +
					Common.Properties.Resources.ResourceManager.GetString("txtEnd") + EndDateTime.ToPersianCompactDateString();
			titleTabel.Rows.Add(new object[] { date });

			data.DataTables.Add(titleTabel); 
			#endregion


			#region Main table
			var mainTable = new DataTable("MainTable");

			mainTable.Columns.Add("Product", typeof(string));
			mainTable.Columns.Add("Activity", typeof(string));
			mainTable.Columns.Add("Shift", typeof(string));
			mainTable.Columns.Add("Station", typeof(string));
			mainTable.Columns.Add("TargetValue", typeof(string));
			mainTable.Columns.Add("ProductionPerHour", typeof(string));
			mainTable.Columns.Add("ProductionValue", typeof(string));
			mainTable.Columns.Add("ExecutionPercent", typeof(string));
			mainTable.Columns.Add("TotalDeviationValue", typeof(string));
			mainTable.Columns.Add("DefectionValue", typeof(string));
			mainTable.Columns.Add("MajorDefection", typeof(string));
			mainTable.Columns.Add("StoppageValue", typeof(string));
			mainTable.Columns.Add("MajorStoppage", typeof(string));

			foreach (var item in reports.Main)
			{
				mainTable.Rows.Add(new object[]
	            {
	                item.Product,
					item.Activity, 
					item.Shift, 
					item.Station, 
					item.TargetValue, 
					item.ProductionPerHour,
					item.ProductionValue,
					item.ExecutionPercent,
					item.TotalDeviationValue,
					item.DefectionValue,
					item.MajorDefection,
					item.StoppageValue,
	                item.MajorStoppage,
	            });
			}

			data.DataTables.Add(mainTable); 
			#endregion


			#region Summery table
			var summeryTable = new DataTable("SummeryTable");

			summeryTable.Columns.Add("Shift", typeof(string));
			summeryTable.Columns.Add("Supervisor", typeof(string));
			summeryTable.Columns.Add("OperatorsCount", typeof(string));
			summeryTable.Columns.Add("Description", typeof(string));

			bool flag = true;
			if(reports.Summery!=null)
				foreach (var item in reports.Summery)
				{
					summeryTable.Rows.Add(new object[]
                {
                    item.Shift, item.Supervisor, item.OperatorsCount, flag?Description.Trim():""
	            });
					flag = false;
				}

			data.DataTables.Add(summeryTable); 
			#endregion



			XpsDocument xps = reportDocument.CreateXpsDocument(data);

			Document = xps.GetFixedDocumentSequence();
		}

		#endregion
	}
}
