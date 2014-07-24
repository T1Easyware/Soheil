using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	public class MachinePartItemVm : PmItemBase
	{
		public Model.MachinePart Model { get; set; }
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
		public MachinePartItemVm(Model.MachinePart model, MachineItemVm machineVm, bool quick = false)
		{
			if (quick) Name = model == null ? "-" : model.Name;
			else
			{
				if (model == null)
				{
					Name = "همه";
				}
				else
				{
					Model = model;
					Name = model.Name;
					Code = model.Code;
					Description = model.Description;
					Status = model.RecordStatus;
				}
				Bar = new PMBarVm();
				_isInitialized = true;
			}
			Machine = machineVm;
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates Machine
		/// </summary>
		public MachineItemVm Machine
		{
			get { return (MachineItemVm)GetValue(MachineProperty); }
			set { SetValue(MachineProperty, value); }
		}
		public static readonly DependencyProperty MachineProperty =
			DependencyProperty.Register("Machine", typeof(MachineItemVm), typeof(MachinePartItemVm), new PropertyMetadata(null));


		#region Callbacks
		protected override void NameChanged(string val)
		{
			Model.Name = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
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
			Model.RecordStatus = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
		}
		#endregion


		public void CalculateTimings()
		{
			var q = Model.MachinePartMaintenances.Where(x => !x.IsOnDemand);
			if (!q.Any())
			{
				Bar.SafeUpdateTimings(0);
			}
			else
			{
				var rem = q.Min(x => x.PeriodDays - (DateTime.Now - x.LastMaintenanceDate).TotalHours);
				Bar.SafeUpdateTimings(rem);
			}
		}
	}
}
