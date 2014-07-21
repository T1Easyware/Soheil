﻿using System;
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
		public override int Id { get { return Model.Id; } set { Model.Id = value; } }
		public MachinePartItemVm(Model.MachinePart model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Description = model.Description;
			Status = model.RecordStatus;
			Bar = new PMBarVm();
			_isInitialized = true;
		}



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
		/// <summary>
		/// Gets or sets a bindable value that indicates Bar
		/// </summary>
		public PMBarVm Bar
		{
			get { return (PMBarVm)GetValue(BarProperty); }
			set { SetValue(BarProperty, value); }
		}
		public static readonly DependencyProperty BarProperty =
			DependencyProperty.Register("Bar", typeof(PMBarVm), typeof(MachinePartItemVm), new PropertyMetadata(null));

	}
}
