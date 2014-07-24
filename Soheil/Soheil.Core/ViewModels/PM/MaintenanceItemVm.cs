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
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
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
        /// <param name="linkItemVm">item must be of type <see cref="MachinePartItemVm"/></param>
		public override void UpdateIsAdded(PmItemBase linkItemVm)
		{
			//if no machine is selected set to -1
            if (linkItemVm as MachinePartItemVm == null) IsAdded = false;
            else if ((linkItemVm as MachinePartItemVm).Model == null) IsAdded = false;
			else
			{
				//count the relations to machinePartMaintenance which relate to the given machine part
				int linkId = linkItemVm.Id;
				IsAdded = Model.MachinePartMaintenances.Any(x => x.MachinePart.Id == linkId);
			}
		}
	}
}
