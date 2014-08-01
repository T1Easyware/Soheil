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
			Model = model;
			Machine = machineVm;
			if (quick) Name = model == null ? "-" : model.Name;
			else
			{
				if (model == null)
				{
					Name = "همه";
				}
				else
				{
					Name = model.Name;
					Code = model.Code;
					Description = model.Description;
					Status = model.RecordStatus;
					CanDelete = !model.IsMachine;
				}

				var bar = new PMBarVm();
				Bar = bar;
				_isInitialized = true;
				if (model != null)
					Task.Run(() =>
					{
						var mins = model.MachinePartMaintenances.Where(x => x.LastMaintenanceDate.HasValue && !x.IsOnDemand);
						var min = mins.Any() ? mins.Min(x => (x.LastMaintenanceDate.Value.AddDays(x.PeriodDays) - DateTime.Now).TotalDays) : double.NaN;
						//safely update progress bar
						Dispatcher.Invoke(new Action<double>(perc =>
						{
							bar.Update(perc);
						}),
						min);
					});
			}
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates CanDelete
		/// </summary>
		public bool CanDelete
		{
			get { return (bool)GetValue(CanDeleteProperty); }
			set { SetValue(CanDeleteProperty, value); }
		}
		public static readonly DependencyProperty CanDeleteProperty =
			DependencyProperty.Register("CanDelete", typeof(bool), typeof(MachinePartItemVm), new PropertyMetadata(false));

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


		/// <summary>
		/// Gets or sets a bindable value that indicates AddRepairCommand
		/// </summary>
		public Commands.Command AddRepairCommand
		{
			get { return (Commands.Command)GetValue(AddRepairCommandProperty); }
			set { SetValue(AddRepairCommandProperty, value); }
		}
		public static readonly DependencyProperty AddRepairCommandProperty =
			DependencyProperty.Register("AddRepairCommand", typeof(Commands.Command), typeof(MachinePartItemVm), new PropertyMetadata(null));

	}
}
