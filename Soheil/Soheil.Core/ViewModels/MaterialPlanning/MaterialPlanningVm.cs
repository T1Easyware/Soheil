using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Core.Interfaces;
using Soheil.Common;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class MaterialPlanningVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public Soheil.Dal.SoheilEdmContext UOW { get; set; }

		DataServices.WorkProfilePlanDataService WorkProfilePlanDataService;
		DataServices.RawMaterialDataService RawMaterialDataService;
		DataServices.WarehouseDataService WarehouseDataService;
		DataServices.TaskDataService TaskDataService;
		public AccessType Access { get; private set; }
		
		public MaterialPlanningVm(AccessType access)
		{
			Access = access;
			Refresh();
		}

		public void Refresh()
		{
			UOW = new Dal.SoheilEdmContext();
			WorkProfilePlanDataService = new DataServices.WorkProfilePlanDataService(UOW);
			RawMaterialDataService = new DataServices.RawMaterialDataService(UOW);
			WarehouseDataService = new DataServices.WarehouseDataService(UOW);
			TaskDataService = new DataServices.TaskDataService(UOW);

			#region Hours
			//hours
			Hours.Clear();
			//startDt = start date and time considering the shifts
			var startDt = WorkProfilePlanDataService.GetShiftStartOn(Date);
			//hourTs = start time of each hourVm
			var hourTs = startDt.TimeOfDay;
			var shifts = WorkProfilePlanDataService.GetShiftsInRange(Date, Date.AddDays(1));
			for (int i = 0; i < 24; i++)
			{
				//create the hour
				var hourVm = new HourVm
				{
					Index = i,
					Text = hourTs.ToString(@"hh\:mm"),
					Data = Date.Add(hourTs)
				};
				//set the color of the hour according to color of the shift
				if (shifts.Any())
				{
					//hourSec = hours passed from the start of Date for each hourVm
					var hourSec = hourTs.TotalSeconds;
					foreach (var shift in shifts)
					{
						if (shift.Item1.StartSeconds <= hourSec && shift.Item1.EndSeconds >= hourSec)
							hourVm.Color = shift.Item1.WorkShiftPrototype.Color;
					}
				}
				Hours.Add(hourVm);
				//next hour
				hourTs = hourTs.Add(TimeSpan.FromHours(1));
			} 
			#endregion

			#region RawMaterials
			int idx = 0;
			Materials.Clear();
			//get materials
			var materials = RawMaterialDataService.GetActives();
			MaterialsCount = materials.Count;
			//create materials
			foreach (var material in materials)
			{
				var materialVm = new RawMaterialVm(material) { Index = idx++ };
				Materials.Add(materialVm);
			}
			#endregion

			#region Warehouses
			Warehouses.Clear();
			//get warehouses
			var warehouses = WarehouseDataService.GetActivesForMaterials();
			//create warehouses
			foreach (var warehouse in warehouses)
			{
				var warehouseVm = new WarehouseVm(warehouse);
				Warehouses.Add(warehouseVm);
			}
			#endregion

			#region Requests
			var data = TaskDataService.GetDailyMaterialPlan(startDt, startDt.AddDays(1));
			for (int i = 0; i < 24; i++)
			{
				foreach (var mat in data[i].Materials)
				{
					var material = Materials.FirstOrDefault(x => x.Model.Id == mat.RawMaterial.Id);
					var cell = new CellVm
					{
						Hour = Hours[i],
						RawMaterial = material,
					};
					foreach (var station in mat.Stations)
					{
						cell.Requests.Add(new RequestVm
						{
							Quantity = station.Quantity,
							StationName = station.Station.Name,
							UnitCode = station.Bom.UnitSet.Code,
						});
					}
					Cells.Add(cell);
				}
			}
			#endregion
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates Date
		/// </summary>
		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(MaterialPlanningVm),
			new PropertyMetadata(DateTime.Now.Date, (d, e) =>
			{
				var vm = (MaterialPlanningVm)d;
				//var val = (DateTime)e.NewValue;
				vm.Refresh();
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates MaterialsCount
		/// </summary>
		public int MaterialsCount
		{
			get { return (int)GetValue(MaterialsCountProperty); }
			set { SetValue(MaterialsCountProperty, value); }
		}
		public static readonly DependencyProperty MaterialsCountProperty =
			DependencyProperty.Register("MaterialsCount", typeof(int), typeof(MaterialPlanningVm), new PropertyMetadata(0));

		/// <summary>
		/// Gets or sets a bindable collection that indicates Hours
		/// </summary>
		public ObservableCollection<HourVm> Hours { get { return _hours; } }
		private ObservableCollection<HourVm> _hours = new ObservableCollection<HourVm>();
		/// <summary>
		/// Gets or sets a bindable collection that indicates Materials
		/// </summary>
		public ObservableCollection<RawMaterialVm> Materials { get { return _materials; } }
		private ObservableCollection<RawMaterialVm> _materials = new ObservableCollection<RawMaterialVm>();

		/// <summary>
		/// Gets or sets a bindable collection that indicates Cells
		/// </summary>
		public ObservableCollection<CellVm> Cells { get { return _cells; } }
		private ObservableCollection<CellVm> _cells = new ObservableCollection<CellVm>();

		/// <summary>
		/// Gets or sets a bindable collection that indicates Warehouses
		/// </summary>
		public ObservableCollection<WarehouseVm> Warehouses { get { return _warehouses; } }
		private ObservableCollection<WarehouseVm> _warehouses = new ObservableCollection<WarehouseVm>();

	}
}
