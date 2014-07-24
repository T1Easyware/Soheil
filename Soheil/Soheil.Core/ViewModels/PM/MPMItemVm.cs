using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	/// <summary>
	/// MachinePartMaintenance item vm
	/// </summary>
	public class MPMItemVm : PmItemBase
	{
		public Model.MachinePartMaintenance Model { get; set; }
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
		public MPMItemVm(Model.MachinePartMaintenance model, MachinePartItemVm machinePartVm, bool quick = false)
		{
			if (quick) Name = model == null ? "-" : model.Maintenance.Name;
			else
			{
				Model = model;
				Name = model.Maintenance.Name;
				Code = model.Code;
				Description = model.Description;
				IsOnDemand = model.IsOnDemand;
				Period = model.PeriodDays;
				Status = model.RecordStatus;

				Bar = new PMBarVm();
				Bar.SafeUpdateTimings(0);
				_isInitialized = true;
			}
			MachinePart = machinePartVm;
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates IsOnDemand
		/// </summary>
		public bool IsOnDemand
		{
			get { return (bool)GetValue(IsOnDemandProperty); }
			set { SetValue(IsOnDemandProperty, value); }
		}
        public static readonly DependencyProperty IsOnDemandProperty =
            DependencyProperty.Register("IsOnDemand", typeof(bool), typeof(MPMItemVm),
            new PropertyMetadata(false, (d, e) => { if (((MPMItemVm)d)._isInitialized) ((MPMItemVm)d).IsOnDemandChanged((bool)e.NewValue); }));
        /// <summary>
		/// Gets or sets a bindable value that indicates Period
		/// </summary>
		public int Period
		{
			get { return (int)GetValue(PeriodProperty); }
			set { SetValue(PeriodProperty, value); }
		}
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(MPMItemVm),
            new PropertyMetadata(1, (d, e) => { if (((MPMItemVm)d)._isInitialized) ((MPMItemVm)d).PeriodChanged((int)e.NewValue); },
                (d, v) => { if ((int)v < 1) return 1; return v; }));

		/// <summary>
		/// Gets or sets a bindable value that indicates MachinePart
		/// </summary>
		public MachinePartItemVm MachinePart
		{
			get { return (MachinePartItemVm)GetValue(MachinePartProperty); }
			set { SetValue(MachinePartProperty, value); }
		}
		public static readonly DependencyProperty MachinePartProperty =
			DependencyProperty.Register("MachinePart", typeof(MachinePartItemVm), typeof(MPMItemVm), new PropertyMetadata(null));



		#region Callbacks
		protected override void NameChanged(string val) { }
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
		protected void IsOnDemandChanged(bool val)
		{
			Model.IsOnDemand = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
		}
		protected void PeriodChanged(int val)
		{
			Model.PeriodDays = val;
			Model.ModifiedDate = DateTime.Now;
			Model.ModifiedBy = LoginInfo.Id;
		}
		#endregion

        /// <summary>
        /// Gets or sets a bindable value that indicates AddReportCommand
        /// </summary>
        public Commands.Command AddReportCommand
        {
            get { return (Commands.Command)GetValue(AddReportCommandProperty); }
            set { SetValue(AddReportCommandProperty, value); }
        }
        public static readonly DependencyProperty AddReportCommandProperty =
            DependencyProperty.Register("AddReportCommand", typeof(Commands.Command), typeof(PmItemBase), new PropertyMetadata(null));

	}
}
