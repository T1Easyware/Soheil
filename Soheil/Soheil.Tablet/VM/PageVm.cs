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
			Reload();
			RefreshCommand = new Core.Commands.Command(o => Reload());
		}
		#endregion

		#region Methods
		public void Reload()
		{
			UOW = new Dal.SoheilEdmContext();
			StationDataService = new Core.DataServices.StationDataService(UOW);

			if (SelectedStation != null) SelectedStation.SelectedReport = null;
			SelectedStation = null;

			Stations.Clear();
			var stations = StationDataService.GetActives();
			foreach (var station in stations)
			{
				var stationVm = new StationVm(station);
				stationVm.Selected += s => SelectedStation = s;
				Stations.Add(stationVm);
			}
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
		#endregion
	}
}
