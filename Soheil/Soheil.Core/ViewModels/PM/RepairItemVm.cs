using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PM
{
	public class RepairItemVm : PmItemBase
	{
		public Model.Repair Model { get; set; }
		public override int Id { get { return Model == null ? -1 : Model.Id; } }
		public RepairItemVm(Model.Repair model)
		{
			Model = model;
			Description = model.Description;

			//times
			AcquiredDate = model.AcquiredDate.Date;
			AcquiredTime = model.AcquiredDate.TimeOfDay;
			SetValue(AcquiredDateTimeProperty, model.AcquiredDate);
			DeliveredDate = model.DeliveredDate.Date;
			DeliveredTime = model.DeliveredDate.TimeOfDay;
			SetValue(DeliveredDateTimeProperty, model.DeliveredDate);
			CreatedDate = model.CreatedDate.Date;
			CreatedTime = model.CreatedDate.TimeOfDay;
			SetValue(CreatedDateTimeProperty, model.CreatedDate);

			MachinePart = new MachinePartItemVm(model.MachinePart, new MachineItemVm(model.MachinePart.Machine, true), true);
			if (model.StoppageReport != null)
				CauseText = string.Format("{0} - {1} - {2}",
					model.StoppageReport.Cause.Parent.Parent.Name,
					model.StoppageReport.Cause.Parent.Name,
					model.StoppageReport.Cause.Name);
			RepairStatus = (RepairStatus)model.RepairStatus;
			_isInitialized = true;
		}

		#region PropDps
		/// <summary>
		/// Gets or sets a bindable value that indicates AcquiredDate
		/// </summary>
		public DateTime AcquiredDate
		{
			get { return (DateTime)GetValue(AcquiredDateProperty); }
			set { SetValue(AcquiredDateProperty, value); }
		}
		public static readonly DependencyProperty AcquiredDateProperty =
			DependencyProperty.Register("AcquiredDate", typeof(DateTime), typeof(RepairItemVm),
			new PropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (DateTime)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.AcquiredDate = val.Add(vm.AcquiredTime);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(AcquiredDateTimeProperty, vm.Model.AcquiredDate);
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates AcquiredTime
		/// </summary>
		public TimeSpan AcquiredTime
		{
			get { return (TimeSpan)GetValue(AcquiredTimeProperty); }
			set { SetValue(AcquiredTimeProperty, value); }
		}
		public static readonly DependencyProperty AcquiredTimeProperty =
			DependencyProperty.Register("AcquiredTime", typeof(TimeSpan), typeof(RepairItemVm),
			new PropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (TimeSpan)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.AcquiredDate = vm.AcquiredDate.Add(val);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(AcquiredDateTimeProperty, vm.Model.AcquiredDate);
				}
			}));
		public static readonly DependencyProperty AcquiredDateTimeProperty =
			DependencyProperty.Register("AcquiredDateTime", typeof(DateTime), typeof(RepairItemVm), new PropertyMetadata(DateTime.Now));


		/// <summary>
		/// Gets or sets a bindable value that indicates DeliveredDate
		/// </summary>
		public DateTime DeliveredDate
		{
			get { return (DateTime)GetValue(DeliveredDateProperty); }
			set { SetValue(DeliveredDateProperty, value); }
		}
		public static readonly DependencyProperty DeliveredDateProperty =
			DependencyProperty.Register("DeliveredDate", typeof(DateTime), typeof(RepairItemVm),
			new PropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (DateTime)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.DeliveredDate = val.Add(vm.DeliveredTime);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(DeliveredDateTimeProperty, vm.Model.DeliveredDate);
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeliveredTime
		/// </summary>
		public TimeSpan DeliveredTime
		{
			get { return (TimeSpan)GetValue(DeliveredTimeProperty); }
			set { SetValue(DeliveredTimeProperty, value); }
		}
		public static readonly DependencyProperty DeliveredTimeProperty =
			DependencyProperty.Register("DeliveredTime", typeof(TimeSpan), typeof(RepairItemVm),
			new PropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (TimeSpan)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.DeliveredDate = vm.DeliveredDate.Add(val);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(DeliveredDateTimeProperty, vm.Model.DeliveredDate);
				}
			}));
		public static readonly DependencyProperty DeliveredDateTimeProperty =
			DependencyProperty.Register("DeliveredDateTime", typeof(DateTime), typeof(RepairItemVm), new PropertyMetadata(DateTime.Now));


		/// <summary>
		/// Gets or sets a bindable value that indicates CreatedDate
		/// </summary>
		public DateTime CreatedDate
		{
			get { return (DateTime)GetValue(CreatedDateProperty); }
			set { SetValue(CreatedDateProperty, value); }
		}
		public static readonly DependencyProperty CreatedDateProperty =
			DependencyProperty.Register("CreatedDate", typeof(DateTime), typeof(RepairItemVm),
			new PropertyMetadata(DateTime.Now.Date, (d, e) =>
			{
				var vm = (RepairItemVm)d;
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
			DependencyProperty.Register("CreatedTime", typeof(TimeSpan), typeof(RepairItemVm),
			new PropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (TimeSpan)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.CreatedDate = vm.CreatedDate.Add(val);
					vm.Model.ModifiedBy = LoginInfo.Id;
					d.SetValue(CreatedDateTimeProperty, vm.Model.CreatedDate);
				}
			}));
		public static readonly DependencyProperty CreatedDateTimeProperty =
			DependencyProperty.Register("CreatedDateTime", typeof(DateTime), typeof(RepairItemVm), new PropertyMetadata(DateTime.Now));


		/// <summary>
		/// Gets or sets a bindable value that indicates CauseText
		/// </summary>
		public string CauseText
		{
			get { return (string)GetValue(CauseTextProperty); }
			set { SetValue(CauseTextProperty, value); }
		}
		public static readonly DependencyProperty CauseTextProperty =
			DependencyProperty.Register("CauseText", typeof(string), typeof(RepairItemVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates RepairStatus
		/// </summary>
		public RepairStatus RepairStatus
		{
			get { return (RepairStatus)GetValue(RepairStatusProperty); }
			set { SetValue(RepairStatusProperty, value); }
		}
		public static readonly DependencyProperty RepairStatusProperty =
			DependencyProperty.Register("RepairStatus", typeof(RepairStatus), typeof(RepairItemVm),
			new PropertyMetadata(RepairStatus.Inactive, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (RepairStatus)e.NewValue;
				if (vm._isInitialized)
				{
					vm.Model.RepairStatus = (byte)val;
					vm.Model.ModifiedBy = LoginInfo.Id;
				}
			}));


		/// <summary>
		/// Gets or sets a bindable value that indicates MachinePart
		/// </summary>
		public MachinePartItemVm MachinePart
		{
			get { return (MachinePartItemVm)GetValue(MachinePartProperty); }
			set { SetValue(MachinePartProperty, value); }
		}
		public static readonly DependencyProperty MachinePartProperty =
			DependencyProperty.Register("MachinePart", typeof(MachinePartItemVm), typeof(RepairItemVm),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (RepairItemVm)d;
				var val = (MachinePartItemVm)e.NewValue;
				if (vm._isInitialized)
				{
				}
			})); 
		#endregion


		#region Callbacks
		protected override void NameChanged(string val)
		{
		}

		protected override void CodeChanged(string val)
		{
		}

		protected override void DescriptionChanged(string val)
		{
			Model.ModifiedBy = LoginInfo.Id;
			Model.Description = val;
		}

		protected override void StatusChanged(Common.Status val)
		{
		} 
		#endregion
	}
}
