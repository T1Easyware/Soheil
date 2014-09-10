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
		public Dal.SoheilEdmContext UOW { get; set; }
		public TransactionVm(Model.WarehouseTransaction model, IEnumerable<WarehouseVm> all, Dal.SoheilEdmContext uow)
		{
			Model = model;
			UOW = uow;
			Quantity = model.Quantity;
			UnitCode = model.UnitSet == null ? "عدد" : model.UnitSet.Code;
			TransactionDate = model.TransactionDateTime.Date;
			TransactionTime = model.TransactionDateTime.TimeOfDay;
			Code = model.Code;
			if (model.SrcWarehouse != null)
				Warehouse = all.FirstOrDefault(x => x.Model.Id == model.SrcWarehouse.Id);
			_isInitializing = false;

			SaveCommand = new Commands.Command(o =>
			{
				Model.RecordDateTime = DateTime.Now;
				UOW.Commit();
			});
			DeleteCommand = new Commands.Command(o => new Soheil.Core.DataServices.Storage.WarehouseTransactionDataService(UOW).DeleteModel(Model));
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
		/// <summary>
		/// Gets or sets a bindable value that indicates TransactionDate
		/// </summary>
		public DateTime TransactionDate
		{
			get { return (DateTime)GetValue(TransactionDateProperty); }
			set { SetValue(TransactionDateProperty, value); }
		}
		public static readonly DependencyProperty TransactionDateProperty =
			DependencyProperty.Register("TransactionDate", typeof(DateTime), typeof(TransactionVm),
			new PropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (TransactionVm)d;
				if (vm._isInitializing) return;
				var val = (DateTime)e.NewValue;
				vm.Model.TransactionDateTime = val.Add(vm.TransactionTime);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates TransactionTime
		/// </summary>
		public TimeSpan TransactionTime
		{
			get { return (TimeSpan)GetValue(TransactionTimeProperty); }
			set { SetValue(TransactionTimeProperty, value); }
		}
		public static readonly DependencyProperty TransactionTimeProperty =
			DependencyProperty.Register("TransactionTime", typeof(TimeSpan), typeof(TransactionVm),
			new PropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (TransactionVm)d;
				if (vm._isInitializing) return;
				var val = (TimeSpan)e.NewValue;
				vm.Model.TransactionDateTime = vm.TransactionDate.Add(val);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(TransactionVm),
			new PropertyMetadata("", (d, e) =>
			{
				var vm = (TransactionVm)d;
				if (vm._isInitializing) return;
				var val = (string)e.NewValue;
				vm.Model.Code = val;
			}));



		/// <summary>
		/// Gets or sets a bindable value that indicates SaveCommand
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(TransactionVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(TransactionVm), new PropertyMetadata(null));

	}
}
