using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class TransactionVm : DependencyObject
	{
		public Model.WarehouseTransaction Model { get; set; }
		bool _isInitializing = true;
		public TransactionVm(Model.WarehouseTransaction model, IEnumerable<WarehouseVm> all)
		{
			Model = model;
			Quantity = model.Quantity;
			UnitCode = model.UnitSet == null ? "عدد" : model.UnitSet.Code;
			if (model.SrcWarehouse != null)
				Warehouse = all.FirstOrDefault(x => x.Model.Id == model.SrcWarehouse.Id);
			_isInitializing = false;
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates Quantity
		/// </summary>
		public double Quantity
		{
			get { return (double)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(double), typeof(TransactionVm),
			new PropertyMetadata(0d, (d, e) =>
			{
				var vm = (TransactionVm)d;
				if (vm._isInitializing) return;
				var val = (double)e.NewValue;
				vm.Model.Quantity = val;
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates UnitCode
		/// </summary>
		public string UnitCode
		{
			get { return (string)GetValue(UnitCodeProperty); }
			set { SetValue(UnitCodeProperty, value); }
		}
		public static readonly DependencyProperty UnitCodeProperty =
			DependencyProperty.Register("UnitCode", typeof(string), typeof(TransactionVm), new PropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable value that indicates Warehouse
		/// </summary>
		public WarehouseVm Warehouse
		{
			get { return (WarehouseVm)GetValue(WarehouseProperty); }
			set { SetValue(WarehouseProperty, value); }
		}
		public static readonly DependencyProperty WarehouseProperty =
			DependencyProperty.Register("Warehouse", typeof(WarehouseVm), typeof(TransactionVm),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (TransactionVm)d;
				if (vm._isInitializing) return;
				var val = (WarehouseVm)e.NewValue;
				vm.Model.SrcWarehouse = val.Model;
			}));

	}
}
