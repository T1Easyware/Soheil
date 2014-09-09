using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Tablet.VM
{
	public class PageVm : DependencyObject
	{
		#region Properties and Events
		public Dal.SoheilEdmContext UOW { get; set; }
		Core.DataServices.StationDataService StationDataService;


		/// <summary>
		/// Gets or sets a bindable value that indicates Date
		/// </summary>
		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(PageVm), new UIPropertyMetadata(DateTime.Now.Date));

		/// <summary>
		/// Gets or sets a bindable collection of Stations
		/// </summary>
		public ObservableCollection<StationVm> Stations { get { return _stations; } }
		private ObservableCollection<StationVm> _stations = new ObservableCollection<StationVm>();
		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedStation
		/// </summary>
		public StationVm SelectedStation
		{
			get { return (StationVm)GetValue(SelectedStationProperty); }
			set { SetValue(SelectedStationProperty, value); }
		}
		public static readonly DependencyProperty SelectedStationProperty =
			DependencyProperty.Register("SelectedStation", typeof(StationVm), typeof(PageVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PageVm)d;
				var oldval = (StationVm)e.OldValue;
				if (oldval != null) oldval.IsSelected = false;
				var val = (StationVm)e.NewValue;
				if (val == null) return;
				val.Reload(vm.Date, vm.ShowAll, vm.IsSafe);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowAll
		/// </summary>
		public bool ShowAll
		{
			get { return (bool)GetValue(ShowAllProperty); }
			set { SetValue(ShowAllProperty, value); }
		}
		public static readonly DependencyProperty ShowAllProperty =
			DependencyProperty.Register("ShowAll", typeof(bool), typeof(PageVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (PageVm)d;
				var val = (bool)e.NewValue;
				if (vm.SelectedStation != null)
					vm.SelectedStation.Reload(vm.Date, vm.ShowAll, vm.IsSafe);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsSafe
		/// </summary>
		public bool IsSafe
		{
			get { return (bool)GetValue(IsSafeProperty); }
			set { SetValue(IsSafeProperty, value); }
		}
		public static readonly DependencyProperty IsSafeProperty =
			DependencyProperty.Register("IsSafe", typeof(bool), typeof(PageVm), new UIPropertyMetadata(true));
		#endregion

		#region Ctor and Init
		public PageVm()
		{
			RefreshCommand = new Core.Commands.Command(o => Reload());
			ExitCommand = new Core.Commands.Command(o =>
			{
				HasMessage = true;
				MessageText = "آیا مایلید از برنامه خارج شوید؟";
				YesCommand = new Core.Commands.Command(oo =>
				{
					if (AutoSave)
						if (SelectedStation != null)
							if (SelectedStation.SelectedReport != null)
								SaveReport(SelectedStation.SelectedReport);
					Application.Current.MainWindow.Close();
				});
				NoCommand = new Core.Commands.Command(oo =>
				{
					HasMessage = false;
				});
			});
		}
		#endregion

		#region Methods
		public void Reload()
		{
			IsLoading = true;
			UOW = new Dal.SoheilEdmContext();
			StationDataService = new Core.DataServices.StationDataService(UOW);

			if (SelectedStation != null) SelectedStation.SelectedReport = null;
			SelectedStation = null;

			Stations.Clear();
			IEnumerable<Model.Station> stations = null;
			Task.Factory.StartNew(() => stations = StationDataService.GetActives()).Wait();
			if (stations == null) return;
			foreach (var station in stations)
			{
				var stationVm = new StationVm(station);
				stationVm.Selected += s => SelectedStation = s;
				stationVm.ReportChanged += (oldval, newval) =>
				{
					if (oldval != null)
					{
						if (AutoSave)
							SaveReport(oldval);
						else
						{
							HasMessage = true;
							MessageText = string.Format("آیا تغییرات گزارش {0} ذخیره شود؟", oldval.OperatorsText);
							YesCommand = new Core.Commands.Command(o =>
							{
								SaveReport(oldval);
								HasMessage = false;
							});
							NoCommand = new Core.Commands.Command(o =>
							{
								HasMessage = false;
							});
						}
					}
				};
				Stations.Add(stationVm);
			}
			IsLoading = false;
		}
		void SaveReport(ReportVm val)
		{
			if (val.StoppageReports.List.Any(x => x.StoppageLevels.FilterBoxes.Last().SelectedItem == null))
				MessageBox.Show("علت توقف سطح سوم انتخاب نشده است");
			else if (val.DefectionReports.List.Any(x => x.ProductDefection.SelectedItem == null))
				MessageBox.Show("نوع عیب انتخاب نشده است");
			else if (val.StoppageReports.List.Any(x => x.GuiltyOperators.FilterBoxes.Any(f => f.SelectedItem == null)))
				MessageBox.Show("اپراتور مقصر در توقفات انتخاب نشده است");
			else if (val.DefectionReports.List.Any(x => x.GuiltyOperators.FilterBoxes.Any(f => f.SelectedItem == null)))
				MessageBox.Show("اپراتور مقصر در ضایعات انتخاب نشده است");
			//else if (oldval.StoppageReports.List.Any(x => x.Repairs.Any(r => r.Machine == null || r.MachinePart == null)))
			//	MessageBox.Show("ماشین و قطعه در تعمیرات انتخاب نشده است");
			else
				new Core.DataServices.ProcessReportDataService(val.UOW).Save(val.Model);

		}
		#endregion

		#region Commands
		/// <summary>
		/// Gets or sets a bindable value that indicates RefreshCommand
		/// </summary>
		public Core.Commands.Command RefreshCommand
		{
			get { return (Core.Commands.Command)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshCommandProperty =
			DependencyProperty.Register("RefreshCommand", typeof(Core.Commands.Command), typeof(PageVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates ExitCommand
		/// </summary>
		public Core.Commands.Command ExitCommand
		{
			get { return (Core.Commands.Command)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public static readonly DependencyProperty ExitCommandProperty =
			DependencyProperty.Register("ExitCommand", typeof(Core.Commands.Command), typeof(PageVm), new UIPropertyMetadata(null));


		/// <summary>
		/// Gets or sets a bindable value that indicates HasMessage
		/// </summary>
		public bool HasMessage
		{
			get { return (bool)GetValue(HasMessageProperty); }
			set { SetValue(HasMessageProperty, value); }
		}
		public static readonly DependencyProperty HasMessageProperty =
			DependencyProperty.Register("HasMessage", typeof(bool), typeof(PageVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable value that indicates MessageText
		/// </summary>
		public string MessageText
		{
			get { return (string)GetValue(MessageTextProperty); }
			set { SetValue(MessageTextProperty, value); }
		}
		public static readonly DependencyProperty MessageTextProperty =
			DependencyProperty.Register("MessageText", typeof(string), typeof(PageVm), new UIPropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates YesCommand
		/// </summary>
		public Core.Commands.Command YesCommand
		{
			get { return (Core.Commands.Command)GetValue(YesCommandProperty); }
			set { SetValue(YesCommandProperty, value); }
		}
		public static readonly DependencyProperty YesCommandProperty =
			DependencyProperty.Register("YesCommand", typeof(Core.Commands.Command), typeof(PageVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates NoCommand
		/// </summary>
		public Core.Commands.Command NoCommand
		{
			get { return (Core.Commands.Command)GetValue(NoCommandProperty); }
			set { SetValue(NoCommandProperty, value); }
		}
		public static readonly DependencyProperty NoCommandProperty =
			DependencyProperty.Register("NoCommand", typeof(Core.Commands.Command), typeof(PageVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsLoading
		/// </summary>
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			set { SetValue(IsLoadingProperty, value); }
		}
		public static readonly DependencyProperty IsLoadingProperty =
			DependencyProperty.Register("IsLoading", typeof(bool), typeof(PageVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable value that indicates AutoSave
		/// </summary>
		public bool AutoSave
		{
			get { return (bool)GetValue(AutoSaveProperty); }
			set { SetValue(AutoSaveProperty, value); }
		}
		public static readonly DependencyProperty AutoSaveProperty =
			DependencyProperty.Register("AutoSave", typeof(bool), typeof(PageVm), new UIPropertyMetadata(true));
		#endregion
	}
}
