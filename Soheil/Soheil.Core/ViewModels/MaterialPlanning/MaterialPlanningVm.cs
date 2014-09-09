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
		DataServices.Storage.WarehouseTransactionDataService WarehouseTransactionDataService;
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
			WarehouseTransactionDataService = new DataServices.Storage.WarehouseTransactionDataService(UOW);
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
						var reqvm = new RequestVm(station);
						reqvm.CreateTransactionCommand = new Commands.Command(o =>
						{
							var transactionModel = new Model.WarehouseTransaction
							{
								Quantity = reqvm.Quantity,
								UnitSet = station.Bom.UnitSet,
								Flow = 1,
								Code = mat.RawMaterial.Code,
								RawMaterial = mat.RawMaterial,
								TransactionDateTime = DateTime.Now,
								RecordDateTime = DateTime.Now,
								Warehouse = Warehouses.Any() ? Warehouses.FirstOrDefault().Model : null
							};
							if (WarehouseTransactionDataService.AddModel(transactionModel) > 0)
								cell.Transactions.Add(new TransactionVm(transactionModel, Warehouses));
							material.NumberOfRequests = Math.Max(material.NumberOfRequests, cell.Requests.Count + cell.Transactions.Count);
						});
						cell.Requests.Add(reqvm);
					}
					foreach (var tran in mat.Transactions)
					{
						cell.Transactions.Add(new TransactionVm(tran, Warehouses));
					}
					material.NumberOfRequests = Math.Max(material.NumberOfRequests, cell.Requests.Count + cell.Transactions.Count);
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
		/// Gets or sets a bindable value that indicates Width
		/// </summary>
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(MaterialPlanningVm), new PropertyMetadata(2400d));

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
