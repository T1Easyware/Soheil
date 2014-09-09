using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class DailyStationPlanVm : ViewModelBase, ISingularList
	{
		public DailyStationPlanVm(AccessType access)
		{
			Access = access;

			#region init Commands
			RefreshCommand = new Command(Refresh, () => true);

			ChangeDayCommand = new Command(offset =>
			{
				StartDate = StartDate.AddDays((int)offset);
				LoadDailyStationPlan();
			});

			InsertStationInfoCommand = new Command(o =>
			{
				if (_reportsData == null) return;
				if (!_reportsData.Any()) return;
				var reportDocument = new Soheil.Core.Printing.ReportDocument();
				var reader =
					new StreamReader(new FileStream(@"Views\Reporting\StationPlan.xaml", FileMode.Open, FileAccess.Read));
				reportDocument.XamlData = reader.ReadToEnd();
				reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
				reader.Close();

				foreach (var station in Stations.Where(x=>x.IsSelected))
				{
					var report = _reports.FirstOrDefault(x => x.StationId == station.Id);
					if (report == null) continue;
					if (!report.Activities.Any()) continue;
					object sId;
					var reportData = _reportsData.FirstOrDefault(x =>
						x.ReportDocumentValues.TryGetValue("Id", out sId) && (int)sId == report.StationId);
					if (reportData == null) continue;
					#region Summery table
					reportData.DataTables.RemoveWhere(x => x.TableName == "SummeryTable");

					var summeryTable = new DataTable("SummeryTable");
					summeryTable.Columns.Add("Description", typeof(string));
					summeryTable.Rows.Add(new object[] { station.Description, });
					summeryTable.Rows.Add(new object[] { Description, });
					reportData.DataTables.Add(summeryTable);
					#endregion
				}

				try
				{
					XpsDocument xps = reportDocument.CreateXpsDocument(_reportsData);
					Document = xps.GetFixedDocumentSequence();
				}
				catch { Document = null; }
				ShowDetails = false;
			}, () => _reportsData != null);

			CancelStationInfoCommand = new Command(o => ShowDetails = false);
			ResetStationInfoCommand = new Command(o =>
			{
				foreach (var station in Stations)
				{
					station.Description = "";
					station.IsSelected = true;
				}
				Description = "";
			});
			#endregion

			_workProfilePlanDataService = new DataServices.WorkProfilePlanDataService();
			_stationDataService = new DataServices.StationDataService();
			foreach (var station in _stationDataService.GetActives())
			{
				Stations.Add(new StationVm(station));
			}
			StartDate = DateTime.Now.AddDays(1).Date;
			LoadDailyStationPlan();
		}


		#region Properties
		public AccessType Access { get; set; }

		public Command RefreshCommand { get; set; }
		public Command ChangeDayCommand { get; set; }
		public Command InsertStationInfoCommand { get; set; }
		public Command ResetStationInfoCommand { get; set; }
		public Command CancelStationInfoCommand { get; set; }
		
		List<ReportData> _reportsData;
		IEnumerable<Core.Reports.DailyStationPlanData> _reports;

		DataServices.WorkProfilePlanDataService _workProfilePlanDataService;
		DataServices.StationDataService _stationDataService;
		#endregion		

		#region DependencyProperties

		public FixedDocumentSequence Document
		{
			get { return (FixedDocumentSequence)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FixedDocumentSequence), typeof(DailyStationPlanVm), new PropertyMetadata(default(FixedDocumentSequence)));

		/// <summary>
		/// Gets or sets a bindable value that indicates StartDate
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(DailyStationPlanVm), new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (DailyStationPlanVm)d;
				var val = (DateTime)e.NewValue;
				vm.StartTime = vm._workProfilePlanDataService.GetShiftStartOn(val).TimeOfDay;
				vm.EndDate = val.AddDays(1);
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
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(DailyStationPlanVm), new UIPropertyMetadata(TimeSpan.Zero));
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
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(DailyStationPlanVm), new UIPropertyMetadata(default(DateTime), (d, e) =>
			{
				var vm = (DailyStationPlanVm)d;
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
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DailyStationPlanVm), new UIPropertyMetadata(TimeSpan.Zero));
		public DateTime EndDateTime { get { return EndDate.Add(EndTime); } }
		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(DailyStationPlanVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable collection of Shifts with supervisors
		/// </summary>
		public ObservableCollection<ShiftVm> Shifts { get { return _shifts; } }
		private ObservableCollection<ShiftVm> _shifts = new ObservableCollection<ShiftVm>();

		/// <summary>
		/// Gets or sets a bindable collection that indicates Stations
		/// </summary>
		public ObservableCollection<StationVm> Stations { get { return _stations; } }
		private ObservableCollection<StationVm> _stations = new ObservableCollection<StationVm>();


		/// <summary>
		/// Gets or sets a bindable value that indicates ShowDetails
		/// </summary>
		public bool ShowDetails
		{
			get { return (bool)GetValue(ShowDetailsProperty); }
			set { SetValue(ShowDetailsProperty, value); }
		}
		public static readonly DependencyProperty ShowDetailsProperty =
			DependencyProperty.Register("ShowDetails", typeof(bool), typeof(DailyStationPlanVm), new UIPropertyMetadata(false));
		#endregion

		#region Methods
		void Refresh(object param)
		{
			LoadDailyStationPlan();
		}

		public void LoadDailyStationPlan()
		{
			#region Init
			var dataService = new DataServices.TaskDataService();
			_reports = dataService.GetDailyStationsPlan(StartDateTime, EndDateTime);
			_reportsData = new List<ReportData>();
			
			var reportDocument = new Soheil.Core.Printing.ReportDocument();

			var reader =
				new StreamReader(new FileStream(@"Views\Reporting\StationPlan.xaml", FileMode.Open, FileAccess.Read));
			reportDocument.XamlData = reader.ReadToEnd();
			reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
			reader.Close();


			#endregion

			foreach (var station in Stations.Where(x=>x.IsSelected))
			{
				var report = _reports.FirstOrDefault(x => x.StationId == station.Id);
				if (report == null) continue;
				if (!report.Activities.Any()) continue;

				// set constant document values
				var _reportData = new ReportData();
				_reportData.ReportDocumentValues.Add("Id", station.Id);
				_reportData.ReportDocumentValues.Add("PrintDate", DateTime.Now);
				_reportsData.Add(_reportData);


				#region Title table
				var titleTabel = new DataTable("TitleTable");
				titleTabel.Columns.Add("ReportTitleName", typeof(string));
				titleTabel.Columns.Add("ReportTitleDate", typeof(string));
				titleTabel.Columns.Add("ReportTitleShift", typeof(string));
				titleTabel.Rows.Add(new object[] 
				{  
					Common.Properties.Resources.ResourceManager.GetString("txtStation") + " : " + report.StationName, 
					Common.Properties.Resources.ResourceManager.GetString("txtDate") + " : " + StartDateTime.ToPersianCompactDateString(),
					Common.Properties.Resources.ResourceManager.GetString("txtShift") + " " + report.ShiftCode
				});

				_reportData.DataTables.Add(titleTabel);
				#endregion


				#region Main table
				var mainTable = new DataTable("MainTable");

				mainTable.Columns.Add("Product", typeof(string));
				mainTable.Columns.Add("Activity", typeof(string));
				mainTable.Columns.Add("Shift", typeof(string));
				mainTable.Columns.Add("Start", typeof(string));
				mainTable.Columns.Add("End", typeof(string));
				mainTable.Columns.Add("TargetValue", typeof(string));
				mainTable.Columns.Add("Operators", typeof(string));

				foreach (var item in report.Activities)
				{
					mainTable.Rows.Add(new object[]
					{
						item.Product,
						item.Activity, 
						item.Shift, 
						item.Start.ToString(@"hh\:mm"), 
						item.End.ToString(@"hh\:mm"),
						item.TargetValue,
						item.Operators.Aggregate((cur, next) => next + "، " + cur)
					});
				}

				_reportData.DataTables.Add(mainTable);
				#endregion


				#region Summery table
				var summeryTable = new DataTable("SummeryTable");

				summeryTable.Columns.Add("Description", typeof(string));
				summeryTable.Rows.Add(new object[] { station.Description, });
				summeryTable.Rows.Add(new object[] { Description, });
				_reportData.DataTables.Add(summeryTable);
				#endregion

			}



			try
			{
				XpsDocument xps = reportDocument.CreateXpsDocument(_reportsData);
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
