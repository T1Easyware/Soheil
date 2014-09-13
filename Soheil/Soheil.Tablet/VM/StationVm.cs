using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Tablet.VM
{
	public class StationVm : DependencyObject
	{
		#region Properties and Events
		public event Action<StationVm> Selected;
		public event Action<ReportVm, ReportVm> ReportChanged;
		public Dal.SoheilEdmContext UOW { get; set; }
		int stationId;
		Core.DataServices.ProcessReportDataService ProcessReportDataService;

		/// <summary>
		/// Gets or sets a bindable collection of Reports
		/// </summary>
		public ObservableCollection<ReportVm> Reports { get { return _reports; } }
		private ObservableCollection<ReportVm> _reports = new ObservableCollection<ReportVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedReport
		/// </summary>
		public ReportVm SelectedReport
		{
			get { return (ReportVm)GetValue(SelectedReportProperty); }
			set { SetValue(SelectedReportProperty, value); }
		}
		public static readonly DependencyProperty SelectedReportProperty =
			DependencyProperty.Register("SelectedReport", typeof(ReportVm), typeof(StationVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (StationVm)d;
				var oldval = (ReportVm)e.OldValue;
				if (oldval != null) 
					oldval.IsSelected = false;
				var val = (ReportVm)e.NewValue;
				if (val != null)
					val.Load();

				if (vm.ReportChanged != null)
					vm.ReportChanged(oldval, val);
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(StationVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable value that indicates IsSelected
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(StationVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (StationVm)d;
				if ((bool)e.NewValue)
					if (vm.Selected != null)
						vm.Selected(vm);
			}));
		#endregion

		#region Ctor and Init
		public StationVm(Model.Station model)
		{
			Name = model.Name;
			stationId = model.Id;
		}
		#endregion

		#region Methods
		internal void Reload(DateTime date, bool showAll, bool isSafe)
		{
			UOW = new Dal.SoheilEdmContext();
			ProcessReportDataService = new Core.DataServices.ProcessReportDataService(UOW);
			
			SelectedReport = null;

			Reports.Clear();
			var data = ProcessReportDataService.GetPendingProcessReports(date, stationId, showAll, isSafe);
			foreach (var item in data)
			{
				var reportVm = new ReportVm(item);
				reportVm.Selected += r => SelectedReport = r;
				Reports.Add(reportVm);
			}
			if (!data.Any())
				MessageBox.Show(string.Format("{0} برای این روز وجود ندارد", showAll ? "هیچ برنامه ای" : "هیچ گزارش وارد نشده ای"));
		}
		#endregion

		#region Commands
		#endregion

	}
}
