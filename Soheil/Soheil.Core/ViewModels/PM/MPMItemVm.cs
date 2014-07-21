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
		public override int Id { get { return Model.Id; } set { Model.Id = value; } }
		public MPMItemVm(Model.MachinePartMaintenance model)
		{
			Model = model;
			Name = model.Maintenance.Name;
			Code = model.Code;
			Description = model.Description;
			IsOnDemand = model.IsOnDemand;
			Period = model.PeriodDays;
			Status = model.RecordStatus;
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
			new PropertyMetadata(false, (d, e) => ((MPMItemVm)d).IsOnDemandChanged((bool)e.NewValue)));
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
			new PropertyMetadata(30, (d, e) => ((MPMItemVm)d).PeriodChanged((int)e.NewValue)));

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

	}
}
