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
		public event Action Deleted;
		public Model.WarehouseTransaction Model { get; private set; }
		public Dal.SoheilEdmContext UOW { get; set; }
		/// <summary>
		/// Creates a new WarehouseTransaction model and adds it to database
		/// <para>returns null if no Warehouse model is provided</para>
		/// </summary>
		/// <param name="all">all warehouses as itemsSource</param>
		internal static WarehouseTransactionVm CreateNew(
			Soheil.Model.TaskReport taskReportModel,
			IEnumerable<WarehouseVm> all, 
			Dal.SoheilEdmContext uow)
		{
			if(!all.Any()) return null;

			var model = new Soheil.Model.WarehouseTransaction
			{
				Code = taskReportModel.Code,
				ProductRework = taskReportModel.Task.Block.StateStation.State.OnProductRework,
				TaskReport = taskReportModel,
				Warehouse = all.FirstOrDefault().Model,
				Quantity = taskReportModel.TaskProducedG1,
				TransactionDateTime = taskReportModel.ReportEndDateTime,
				Flow = 0,
				//WarehouseReceipt = new WarehouseReceipt { RecordDateTime = DateTime.Now, ModifiedDate = DateTime.Now, ModifiedBy = 0, CreatedDate = DateTime.Now }
			};
			var dataService = new DataServices.Storage.WarehouseTransactionDataService(uow);
			dataService.AddModel(model);
			
			var vm = new WarehouseTransactionVm(model, all, uow, dataService);
			return vm;
		}
		/// <summary>
		/// Creates a new VM for an existing WarehouseTransaction
		/// <para>returns null if no WarehouseTransaction model is available</para>
		/// </summary>
		/// <param name="all">all warehouses as itemsSource</param>
		internal static WarehouseTransactionVm CreateExisting(
			Soheil.Model.TaskReport taskReportModel, 
			IEnumerable<WarehouseVm> all,
			Dal.SoheilEdmContext uow)
		{
			var model = taskReportModel.WarehouseTransactions.FirstOrDefault();
			if (model == null) return null;

			var dataService = new DataServices.Storage.WarehouseTransactionDataService(uow);
			var vm = new WarehouseTransactionVm(model, all, uow, dataService);
			return vm;
		}
		private WarehouseTransactionVm(
			Model.WarehouseTransaction model, 
			IEnumerable<WarehouseVm> all,
			Dal.SoheilEdmContext uow,
			DataServices.Storage.WarehouseTransactionDataService dataService)
		{
			//DataService
			UOW = uow;
			_dataService = dataService;
			Model = model;

			//VM
			Code = model.Code;
			Quantity = (int)model.Quantity;
			TransactionDate = model.TransactionDateTime.Date;
			TransactionTime = model.TransactionDateTime.TimeOfDay;
			Warehouse = all.FirstOrDefault(x => x.Model.Id == model.Warehouse.Id);

			initializeCommands();
			_isInInitializingPhase = false;
		}

		void initializeCommands()
		{
			DeleteCommand = new Commands.Command(o =>
			{
				if(Model != null)
					_dataService.DeleteModel(Model);
				if (Deleted != null)
					Deleted();
			});
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
