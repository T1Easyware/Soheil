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
	public class PMReportVm : ViewModelBase, ISingularList
	{
		public PMReportVm(AccessType access)
		{
			Access = access;
			_workProfilePlanDataService = new DataServices.WorkProfilePlanDataService();
			StartDate = DateTime.Now.Date;

			#region init Commands
			RefreshCommand = new Command(Refresh, () => true);

			ChangeDayCommand = new Command(offset =>
			{
				StartDate = StartDate.AddDays((int)offset);
				LoadReports();
			});
			#endregion
			LoadReports();
		}

		#region Properties
		public AccessType Access { get; set; }

		ReportData _reportData;
		Core.Reports.PMReportData _report;
		DataServices.WorkProfilePlanDataService _workProfilePlanDataService;
		public Command RefreshCommand { get; set; }
		public Command ChangeDayCommand { get; set; }
		public DateTime StartDateTime { get { return StartDate.Add(StartTime); } }
		public DateTime EndDateTime { get { return EndDate.Add(EndTime); } }
		#endregion

		#region Dependency Properties
		public FixedDocumentSequence Document
		{
			get { return (FixedDocumentSequence)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FixedDocumentSequence), typeof(PMReportVm), new PropertyMetadata(default(FixedDocumentSequence)));



		/// <summary>
		/// Gets or sets a bindable value that indicates StartDate
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(PMReportVm),
			new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (PMReportVm)d;
				var val = (DateTime)e.NewValue;
				vm.StartTime = vm._workProfilePlanDataService.GetShiftStartOn(val).TimeOfDay;
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
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(PMReportVm), new UIPropertyMetadata(TimeSpan.Zero));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndDate
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(PMReportVm), new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (PMReportVm)d;
				var val = (DateTime)e.NewValue;
				vm.EndTime = vm._workProfilePlanDataService.GetShiftStartOn(val).TimeOfDay;
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
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(PMReportVm), new UIPropertyMetadata(TimeSpan.Zero));

		/// <summary>
		/// Gets or sets a bindable value that indicates IsOneDay
		/// </summary>
		public bool IsOneDay
		{
			get { return (bool)GetValue(IsOneDayProperty); }
			set { SetValue(IsOneDayProperty, value); }
		}
		public static readonly DependencyProperty IsOneDayProperty =
			DependencyProperty.Register("IsOneDay", typeof(bool), typeof(PMReportVm),
			new UIPropertyMetadata(false, (d, e) => {
				if ((bool)e.NewValue)
				{
					d.SetValue(IsOneWeekProperty, false);
					d.SetValue(IsManualEndProperty, false);
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsOneWeek
		/// </summary>
		public bool IsOneWeek
		{
			get { return (bool)GetValue(IsOneWeekProperty); }
			set { SetValue(IsOneWeekProperty, value); }
		}
		public static readonly DependencyProperty IsOneWeekProperty =
			DependencyProperty.Register("IsOneWeek", typeof(bool), typeof(PMReportVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				if ((bool)e.NewValue)
				{
					d.SetValue(IsOneDayProperty, false);
					d.SetValue(IsManualEndProperty, false);
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsManualEnd
		/// </summary>
		public bool IsManualEnd
		{
			get { return (bool)GetValue(IsManualEndProperty); }
			set { SetValue(IsManualEndProperty, value); }
		}
		public static readonly DependencyProperty IsManualEndProperty =
			DependencyProperty.Register("IsManualEnd", typeof(bool), typeof(PMReportVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
				{
					d.SetValue(IsOneWeekProperty, false);
					d.SetValue(IsOneDayProperty, false);
				}
			}));
		#endregion


		#region Methods
		void Refresh(object param)
		{
			LoadReports();
		}
		public void LoadReports()
		{
			#region Init
			var dataService = new DataServices.PM.ReportDataService();
			if (IsOneDay)
				EndDate = StartDateTime.AddDays(1);
			else if (IsOneWeek)
				EndDate = StartDateTime.AddDays(7);
			_report = dataService.GetAllReportsInRange(StartDateTime, EndDateTime);
			_reportData = new ReportData();

			var reportDocument = new Soheil.Core.Printing.ReportDocument();

			var reader =
				new StreamReader(new FileStream(@"Views\Reporting\PMReport.xaml", FileMode.Open, FileAccess.Read));
			reportDocument.XamlData = reader.ReadToEnd();
			reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
			reader.Close();


			#endregion

			// set constant document values
			_reportData.ReportDocumentValues.Add("PrintDate", DateTime.Now);


			#region Title table
			var titleTabel = new DataTable("TitleTable");
			titleTabel.Columns.Add("ReportTitleStart", typeof(string));
			titleTabel.Columns.Add("ReportTitleEnd", typeof(string));
			titleTabel.Rows.Add(new object[] 
				{  
					Common.Properties.Resources.ResourceManager.GetString("txtStart") + " : " + StartDateTime.ToPersianCompactDateString(),
					Common.Properties.Resources.ResourceManager.GetString("txtEnd") + " " + EndDateTime.ToPersianCompactDateString(),
				});

			_reportData.DataTables.Add(titleTabel);
			#endregion


			#region Main table
			var mainTable = new DataTable("PMs");
			mainTable.Columns.Add("Machine", typeof(string));
			mainTable.Columns.Add("Part", typeof(string));
			mainTable.Columns.Add("Maintenance", typeof(string));
			mainTable.Columns.Add("MaintenanceDate", typeof(string));
			mainTable.Columns.Add("PerformedDate", typeof(string));
			mainTable.Columns.Add("LastMaintenanceDate", typeof(string));
			mainTable.Columns.Add("PeriodDays", typeof(string));
			mainTable.Columns.Add("Status", typeof(string));
			mainTable.Columns.Add("Delay", typeof(string));
			mainTable.Columns.Add("Description", typeof(string));

			foreach (var item in _report.PMList)
			{
				mainTable.Rows.Add(new object[]
					{
						item.Machine,
						item.Part,
						item.Maintenance,
						item.MaintenanceDate.ToPersianMinimalDateTimeString(),
						item.PerformedDate.HasValue ? item.PerformedDate.Value.ToPersianMinimalDateTimeString() : "---", 
						item.LastMaintenanceDate.ToPersianMinimalDateTimeString(), 
						item.Period, 
						item.IsPerformed ? "#" : "",
						item.Delay.ToString(),
						item.Description
					});
			}

			_reportData.DataTables.Add(mainTable);
			#endregion


			#region repairs table
			var repairsTable = new DataTable("Repairs");
			repairsTable.Columns.Add("Machine", typeof(string));
			repairsTable.Columns.Add("Part", typeof(string));
			repairsTable.Columns.Add("RepairStatus", typeof(string));
			repairsTable.Columns.Add("CreatedDate", typeof(string));
			repairsTable.Columns.Add("AcquiredDate", typeof(string));
			repairsTable.Columns.Add("DeliveredDate", typeof(string));
			repairsTable.Columns.Add("Description", typeof(string));

			//#enum#
			var tmp = new object[]
			{
				Common.Properties.Resources.ResourceManager.GetString("txtInactive"),
				Common.Properties.Resources.ResourceManager.GetString("txtReported"),
				Common.Properties.Resources.ResourceManager.GetString("txtNotDone"),
				Common.Properties.Resources.ResourceManager.GetString("txtDone"),
			};
			foreach (var item in _report.RepairList)
			{
				repairsTable.Rows.Add(new object[]
					{
						item.Machine,
						item.Part,
						tmp[item.RepairStatus],
						item.CreatedDate.ToPersianMinimalDateTimeString(),
						item.AcquiredDate.ToPersianMinimalDateTimeString(), 
						item.DeliveredDate.ToPersianMinimalDateTimeString(), 
						item.Description
					});
			}
			_reportData.DataTables.Add(repairsTable);
			#endregion


			try
			{
				XpsDocument xps = reportDocument.CreateXpsDocument(_reportData);
				Document = xps.GetFixedDocumentSequence();
			}
			catch
			{
				Document = null;
			}
		}
		#endregion
	}
}