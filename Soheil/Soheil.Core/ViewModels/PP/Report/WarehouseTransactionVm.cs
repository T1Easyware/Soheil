using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class WarehouseTransactionVm : DependencyObject
	{
	    readonly bool _isInInitializingPhase = true;
	    readonly DataServices.Storage.WarehouseTransactionDataService _dataService;
		public Model.WarehouseTransaction Model { get; private set; }

		/// <summary>
		/// Uses its own UOW to create a new transaction model and add it to database
		/// </summary>
		/// <param name="model"></param>
		public WarehouseTransactionVm(Model.TaskReport model)
		{
			_dataService = new DataServices.Storage.WarehouseTransactionDataService();
			Model = _dataService.CreateTransactionFor(model);

			//Model
			Model = new Model.WarehouseTransaction
			{
				Code = model.Code,
				ProductRework = model.Task.Block.StateStation.State.OnProductRework,
				TaskReport = model,
				Quantity = model.TaskProducedG1,
				TransactionDateTime = model.ReportEndDateTime,
				Flow = 0,
			};
			model.WarehouseTransactions.Add(Model);
			_dataService.AddModel(Model);

			//VM
			Code = Model.Code;
			Quantity = (int)Model.Quantity;
			TransactionDate = Model.TransactionDateTime.Date;
			TransactionTime = Model.TransactionDateTime.TimeOfDay;

			InitializeCommands();
			_isInInitializingPhase = false;
		}
		/// <summary>
		/// Uses its own UOW to 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="all"></param>
		public WarehouseTransactionVm(Model.WarehouseTransaction model, IEnumerable<WarehouseVm> all)
		{
			//DataService
			_dataService = new DataServices.Storage.WarehouseTransactionDataService();
			Model = model;
			
			//VM
			Code = model.Code;
			Quantity = (int)model.Quantity;
			TransactionDate = model.TransactionDateTime.Date;
			TransactionTime = model.TransactionDateTime.TimeOfDay;
			Warehouse = all.FirstOrDefault(x => x.Model.Id == model.Id);

			InitializeCommands();
			_isInInitializingPhase = false;
		}

		void InitializeCommands()
		{
			DeleteCommand = new Commands.Command(o => _dataService.DeleteModel(Model));
			SaveCommand = new Commands.Command(o =>
			{
				var msg = _dataService.Save();
				if (msg != null)
					MessageBox.Show(msg,"Error");
			});
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates Quantity
		/// </summary>
		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(int), typeof(WarehouseTransactionVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (WarehouseTransactionVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (int)e.NewValue;
				vm.Model.Quantity = val;
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
			DependencyProperty.Register("Code", typeof(string), typeof(WarehouseTransactionVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (WarehouseTransactionVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (string)e.NewValue;
				vm.Model.Code = val;
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates Warehouse
		/// </summary>
		public WarehouseVm Warehouse
		{
			get { return (WarehouseVm)GetValue(WarehouseProperty); }
			set { SetValue(WarehouseProperty, value); }
		}
		public static readonly DependencyProperty WarehouseProperty =
			DependencyProperty.Register("Warehouse", typeof(WarehouseVm), typeof(WarehouseTransactionVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (WarehouseTransactionVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (WarehouseVm)e.NewValue;
				if (val == null) return;
				vm.Model.Warehouse = vm._dataService.GetWarehouse(val.Model);
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
			DependencyProperty.Register("TransactionDate", typeof(DateTime), typeof(WarehouseTransactionVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (WarehouseTransactionVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (DateTime)e.NewValue;
				vm.Model.TransactionDateTime = val.Date.Add(vm.TransactionTime);
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
			DependencyProperty.Register("TransactionTime", typeof(TimeSpan), typeof(WarehouseTransactionVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (WarehouseTransactionVm)d;
				if (vm._isInInitializingPhase) return;
				var val = (TimeSpan)e.NewValue;
				vm.Model.TransactionDateTime = vm.TransactionDate.Add(val);
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(WarehouseTransactionVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates SaveCommand
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(WarehouseTransactionVm), new PropertyMetadata(null));

	}
}
