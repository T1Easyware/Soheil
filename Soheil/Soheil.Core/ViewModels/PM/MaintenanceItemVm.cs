using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	public class MaintenanceItemVm : PmItemBase
	{
		public Model.Maintenance Model { get; set; }
		public override int Id { get { return Model.Id; } set { Model.Id = value; } }
		public MaintenanceItemVm(Model.Maintenance model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Description = model.Description;
			Status = model.RecordStatus;
			_isInitialized = true;
		}

		#region Callbacks
		protected override void NameChanged(string val)
		{
			Model.Name = val;
			Model.ModifiedBy = LoginInfo.Id;
			Model.ModifiedDate = DateTime.Now;
		}

		protected override void CodeChanged(string val)
		{
			Model.Code = val;
			Model.ModifiedBy = LoginInfo.Id;
			Model.ModifiedDate = DateTime.Now;
		}

		protected override void DescriptionChanged(string val)
		{
			Model.Description = val;
			Model.ModifiedBy = LoginInfo.Id;
			Model.ModifiedDate = DateTime.Now;
		}

		protected override void StatusChanged(Common.Status val)
		{
			Model.RecordStatus = val;
			Model.ModifiedBy = LoginInfo.Id;
			Model.ModifiedDate = DateTime.Now;
		} 
		#endregion

		/// <summary>
		/// Updates Link counter automatically (should be overriden if needed)
		/// </summary>
		/// <param name="linkItemVm">item must be of type machine part item vm</param>
		public override void UpdateLinkCounter(PmItemBase linkItemVm)
		{
			//if no machine is selected set to -1
			if (linkItemVm as MachinePartItemVm == null) LinkCounter = -1;
			else
			{
				//count the relations to machinePartMaintenance which relate to the given machine part
				int linkId = linkItemVm.Id;
				LinkCounter = Model.MachinePartMaintenances.Count(x => x.MachinePart.Id == linkId);
			}
		}
	}
}
