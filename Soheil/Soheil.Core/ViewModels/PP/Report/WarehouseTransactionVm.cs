using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class WarehouseTransactionVm : DependencyObject
	{
		bool _isInInitializingPhase = true;
		DataServices.Storage.WarehouseTransactionDataService _dataService;
		public Model.WarehouseTransaction Model { get; private set; }
		public Dal.SoheilEdmContext UOW { get; protected set; }

		public WarehouseTransactionVm(Model.TaskReport model, Dal.SoheilEdmContext uow)
		{
			//DataService
			UOW = uow;
			_dataService = new DataServices.Storage.WarehouseTransactionDataService(uow);

			//Model
			Model = new Soheil.Model.WarehouseTransaction
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

			initializeCommands();
			_isInInitializingPhase = false;
		}
		public WarehouseTransactionVm(Model.WarehouseTransaction model, IEnumerable<WarehouseVm> all, Dal.SoheilEdmContext uow)
		{
			//DataService
			UOW = uow;
			_dataService = new DataServices.Storage.WarehouseTransactionDataService(uow);

			//Model
			Model = model;
			
			//VM
			Code = model.Code;
			Quantity = (int)model.Quantity;
			TransactionDate = model.TransactionDateTime.Date;
			TransactionTime = model.TransactionDateTime.TimeOfDay;
			Warehouse = all.FirstOrDefault(x => x.Model.Id == model.Id);

			initializeCommands();
			_isInInitializingPhase = false;
		}

		void initializeCommands()
		{
			DeleteCommand = new Commands.Command(o =>
			{
				_dataService.DeleteModel(Model);
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
				vm.Model.Quantity = (double)val;
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
				vm.Model.Warehouse = val.Model;
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
	}
}
