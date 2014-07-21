using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PM
{
	public class ReportItemVm : PmItemBase
	{
		public Model.MaintenanceReport Model { get; set; }
		public override int Id { get { return Model.Id; } set { Model.Id = value; } }
		public ReportItemVm(Model.MaintenanceReport model)
		{
			Model = model;
			Name = model.MachinePartMaintenance.Maintenance.Name;
			Code = model.Code;
			PerformedDate = model.PerformedDate;
			MaintenanceDate = model.MaintenanceDate;
			Model.UpdateStatus();
			MaintenanceStatus = model.PerformanceStatus;
			_isInitialized = true;
		}

		#region Callbacks
		protected override void NameChanged(string val)
		{
		}

		protected override void CodeChanged(string val)
		{
			Model.Code = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
		}

		protected override void DescriptionChanged(string val)
		{
			Model.Description = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
		}

		protected override void StatusChanged(Common.Status val)
		{
		} 
		#endregion

		/// <summary>
		/// Gets or sets a bindable value that indicates Machine
		/// </summary>
		public MachineItemVm Machine
		{
			get { return (MachineItemVm)GetValue(MachineProperty); }
			set { SetValue(MachineProperty, value); }
		}
		public static readonly DependencyProperty MachineProperty =
			DependencyProperty.Register("Machine", typeof(MachineItemVm), typeof(PartItemVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates MachinePart
		/// </summary>
		public MachinePartItemVm MachinePart
		{
			get { return (MachinePartItemVm)GetValue(MachinePartProperty); }
			set { SetValue(MachinePartProperty, value); }
		}
		public static readonly DependencyProperty MachinePartProperty =
			DependencyProperty.Register("MachinePart", typeof(MachinePartItemVm), typeof(PartItemVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Part
		/// </summary>
		public PartItemVm Part
		{
			get { return (PartItemVm)GetValue(PartProperty); }
			set { SetValue(PartProperty, value); }
		}
		public static readonly DependencyProperty PartProperty =
			DependencyProperty.Register("Part", typeof(PartItemVm), typeof(PartItemVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Maintenance
		/// </summary>
		public MaintenanceItemVm Maintenance
		{
			get { return (MaintenanceItemVm)GetValue(MaintenanceProperty); }
			set { SetValue(MaintenanceProperty, value); }
		}
		public static readonly DependencyProperty MaintenanceProperty =
			DependencyProperty.Register("Maintenance", typeof(MaintenanceItemVm), typeof(PartItemVm), new PropertyMetadata(null));


		/// <summary>
		/// Gets or sets a bindable value that indicates MaintenanceStatus
		/// </summary>
		public MaintenanceStatus MaintenanceStatus
		{
			get { return (MaintenanceStatus)GetValue(MaintenanceStatusProperty); }
			set { SetValue(MaintenanceStatusProperty, value); }
		}
		public static readonly DependencyProperty MaintenanceStatusProperty =
			DependencyProperty.Register("MaintenanceStatus", typeof(MaintenanceStatus), typeof(ReportItemVm),
			new PropertyMetadata(MaintenanceStatus.Early, (d, e) =>
			{
				var vm = (ReportItemVm)d;
				var val = (MaintenanceStatus)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.PerformanceStatus = val;
					vm.Model.UpdateStatus();
					if (vm.Model.PerformanceStatus != val)
						vm.MaintenanceStatus = val;
					vm.Model.ModifiedDate = DateTime.Now;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates MaintenanceDate
		/// </summary>
		public DateTime MaintenanceDate
		{
			get { return (DateTime)GetValue(MaintenanceDateProperty); }
			set { SetValue(MaintenanceDateProperty, value); }
		}
		public static readonly DependencyProperty MaintenanceDateProperty =
			DependencyProperty.Register("MaintenanceDate", typeof(DateTime), typeof(ReportItemVm),
			new PropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (ReportItemVm)d;
				var val = (DateTime)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.MaintenanceDate = val;
					vm.Model.UpdateStatus();
					vm.MaintenanceStatus = vm.Model.PerformanceStatus;
					vm.Model.ModifiedDate = DateTime.Now;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates PerformedDate
		/// </summary>
		public DateTime PerformedDate
		{
			get { return (DateTime)GetValue(PerformedDateProperty); }
			set { SetValue(PerformedDateProperty, value); }
		}
		public static readonly DependencyProperty PerformedDateProperty =
			DependencyProperty.Register("PerformedDate", typeof(DateTime), typeof(ReportItemVm),
			new PropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (ReportItemVm)d;
				var val = (DateTime)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.PerformedDate = val;
					vm.Model.UpdateStatus();
					vm.MaintenanceStatus = vm.Model.PerformanceStatus;
					vm.Model.ModifiedDate = DateTime.Now;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}));

	}
}
