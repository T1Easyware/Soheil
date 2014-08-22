using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class RepairVm : DependencyObject
	{
		private bool _isInitialized = false;
		public Model.Repair Model { get; set; }
		public RepairVm(Model.Repair model, Dal.SoheilEdmContext uow)
		{
			//PM
			var list = new Soheil.Core.DataServices.MachineDataService(uow).GetActives();
			foreach (var machine in list)
			{
				Machines.Add(new RepairMachineVm(machine));
			} 
			
			if (model == null)
			{
				model = new Model.Repair
				{
					CreatedDate = DateTime.Now,
					AcquiredDate = DateTime.Now,
					DeliveredDate = DateTime.Now,
					RepairStatus = (byte)Common.RepairStatus.Reported,
					ModifiedBy = LoginInfo.Id,
				};
			}

			Model = model;
			CreatedDate = model.CreatedDate.Date;
			CreatedTime = model.CreatedDate.TimeOfDay;
			SetValue(CreatedDateTimeProperty, model.CreatedDate);
			if (model.MachinePart != null)
				Machine = Machines.FirstOrDefault(x => x.Model.Id == model.MachinePart.Machine.Id);

			_isInitialized = true;
		}

		#region Combos

		//used in PM repairs
		public ObservableCollection<RepairMachineVm> Machines { get { return _machines; } }
		private ObservableCollection<RepairMachineVm> _machines = new ObservableCollection<RepairMachineVm>();

		public ObservableCollection<RepairMachinePartVm> Parts { get { return _parts; } }
		private ObservableCollection<RepairMachinePartVm> _parts = new ObservableCollection<RepairMachinePartVm>();


		/// <summary>
		/// Gets or sets a bindable value that indicates Machine
		/// </summary>
		public RepairMachineVm Machine
		{
			get { return (RepairMachineVm)GetValue(MachineProperty); }
			set { SetValue(MachineProperty, value); }
		}
		public static readonly DependencyProperty MachineProperty =
			DependencyProperty.Register("Machine", typeof(RepairMachineVm), typeof(RepairVm),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (RepairVm)d;
				var val = (RepairMachineVm)e.NewValue;
				if (val == null) return;

				//Reload Parts
				vm.Parts.Clear();
				foreach (var part in val.Model.MachineParts)
				{
					vm.Parts.Add(new RepairMachinePartVm(part));
				}
				if (vm._isInitialized)
					vm.MachinePart = vm.Parts.FirstOrDefault(x => x.Model.IsMachine);
				else if (vm.Model.MachinePart != null)
					vm.MachinePart = vm.Parts.FirstOrDefault(x => x.Model.Id == vm.Model.MachinePart.Id);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates MachinePart
		/// </summary>
		public RepairMachinePartVm MachinePart
		{
			get { return (RepairMachinePartVm)GetValue(MachinePartProperty); }
			set { SetValue(MachinePartProperty, value); }
		}
		public static readonly DependencyProperty MachinePartProperty =
			DependencyProperty.Register("MachinePart", typeof(RepairMachinePartVm), typeof(RepairVm),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (RepairVm)d;
				var val = (RepairMachinePartVm)e.NewValue;
				if (vm._isInitialized && val != null)
				{
					vm.Model.MachinePart = val.Model;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			})); 
		#endregion

		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(RepairVm),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (RepairVm)d;
				var val = (string)e.NewValue;
				if(vm._isInitialized)
				{
					vm.Model.Description = val;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}));


		#region Date
		/// <summary>
		/// Gets or sets a bindable value that indicates CreatedDate
		/// </summary>
		public DateTime CreatedDate
		{
			get { return (DateTime)GetValue(CreatedDateProperty); }
			set { SetValue(CreatedDateProperty, value); }
		}
		public static readonly DependencyProperty CreatedDateProperty =
			DependencyProperty.Register("CreatedDate", typeof(DateTime), typeof(RepairVm),
			new PropertyMetadata(DateTime.Now.Date, (d, e) =>
			{
				var vm = (RepairVm)d;
				var val = (DateTime)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.CreatedDate = val.Add(vm.CreatedTime);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(CreatedDateTimeProperty, vm.Model.CreatedDate);
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates CreatedTime
		/// </summary>
		public TimeSpan CreatedTime
		{
			get { return (TimeSpan)GetValue(CreatedTimeProperty); }
			set { SetValue(CreatedTimeProperty, value); }
		}
		public static readonly DependencyProperty CreatedTimeProperty =
			DependencyProperty.Register("CreatedTime", typeof(TimeSpan), typeof(RepairVm),
			new PropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (RepairVm)d;
				var val = (TimeSpan)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.CreatedDate = vm.CreatedDate.Add(val);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(CreatedDateTimeProperty, vm.Model.CreatedDate);
				}
			}));
		public static readonly DependencyProperty CreatedDateTimeProperty =
			DependencyProperty.Register("CreatedDateTime", typeof(DateTime), typeof(RepairVm), new PropertyMetadata(DateTime.Now));
		#endregion

		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(RepairVm), new PropertyMetadata(null));

	}
}
