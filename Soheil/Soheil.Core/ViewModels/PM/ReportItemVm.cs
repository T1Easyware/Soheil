﻿using System;
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
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
		public ReportItemVm(Model.MaintenanceReport model, MPMItemVm mpmVm)
		{
			Model = model;
			Name = model.MachinePartMaintenance.Maintenance.Name;
			Code = model.Code;
			if (model.PerformedDate.HasValue)
				PerformedDate = model.PerformedDate.Value;
			MaintenanceDate = model.MaintenanceDate;
			DiffDate = (int)Model.UpdateStatus();
			MaintenanceStatus = model.PerformanceStatus;
			MachinePartMaintenance = mpmVm;
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
		/// Gets or sets a bindable value that indicates MachinePartMaintenanceProperty
		/// </summary>
		public MPMItemVm MachinePartMaintenance
		{
			get { return (MPMItemVm)GetValue(MachinePartMaintenanceProperty); }
			set { SetValue(MachinePartMaintenanceProperty, value); }
		}
		public static readonly DependencyProperty MachinePartMaintenanceProperty =
			DependencyProperty.Register("MachinePartMaintenance", typeof(MPMItemVm), typeof(ReportItemVm), new PropertyMetadata(null));


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
					vm.DiffDate = (int)vm.Model.UpdateStatus();
					if (vm.Model.PerformanceStatus != val)
					{
						vm.MaintenanceStatus = val;
						return;
					}
					vm.Model.ModifiedDate = DateTime.Now;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
				vm.OperationwiseStatus = val & (MaintenanceStatus.Done | MaintenanceStatus.NotDone);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates OperationwiseStatus
		/// </summary>
		public MaintenanceStatus OperationwiseStatus
		{
			get { return (MaintenanceStatus)GetValue(OperationwiseStatusProperty); }
			set { SetValue(OperationwiseStatusProperty, value); }
		}
		public static readonly DependencyProperty OperationwiseStatusProperty =
			DependencyProperty.Register("OperationwiseStatus", typeof(MaintenanceStatus), typeof(ReportItemVm),
			new UIPropertyMetadata(MaintenanceStatus.Inactive, (d, e) =>
			{
				var vm = (ReportItemVm)d;
				var val = (MaintenanceStatus)e.NewValue;
				if (vm._isInitialized)
				{
					vm.MaintenanceStatus =
						(vm.MaintenanceStatus
						& (MaintenanceStatus.Early | MaintenanceStatus.Late | MaintenanceStatus.OnTime))
						| val;
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
				var val = ((DateTime)e.NewValue).Date;
				if (vm._isInitialized)
				{
					vm.Model.MaintenanceDate = val;
					vm.DiffDate = (int)vm.Model.UpdateStatus();
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
				var val = ((DateTime)e.NewValue).Date;
				if (vm._isInitialized)
				{
					vm.Model.PerformedDate = val;
					vm.DiffDate = (int)vm.Model.UpdateStatus();
					vm.MaintenanceStatus = vm.Model.PerformanceStatus;
					vm.Model.ModifiedDate = DateTime.Now;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}, (d, v) =>
			{
				var val = ((DateTime)v).Date;
				if (val > DateTime.Now.Date)
				{
					MessageBox.Show("زمان اجرا نمی تواند در آینده باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return DateTime.Now.Date;
				}
				return val;
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates DiffDate
		/// </summary>
		public int DiffDate
		{
			get { return (int)GetValue(DiffDateProperty); }
			set { SetValue(DiffDateProperty, value); }
		}
		public static readonly DependencyProperty DiffDateProperty =
			DependencyProperty.Register("DiffDate", typeof(int), typeof(ReportItemVm), new PropertyMetadata(0));

	}
}
