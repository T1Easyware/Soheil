using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	public class PartItemVm : PmItemBase
	{
		public Model.Part Model { get; set; }
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
		public PartItemVm(Model.Part model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Description = model.Description;
			Status = model.RecordStatus;
			_isInitialized = true;
		}

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

		/// <summary>
		/// Updates Link counter automatically (should be overriden if needed)
		/// </summary>
        /// <param name="linkItemVm">item must be of type <see cref="MachineItemVm"/></param>
		public override void UpdateIsAdded(PmItemBase linkItemVm)
		{
			//if no machine is selected set to -1
            if (linkItemVm as MachineItemVm == null) IsAdded = false;
            else if ((linkItemVm as MachineItemVm).Model == null) IsAdded = false;
			else
			{
				//count the relations to machine parts which relate to the given machine
				int linkId = linkItemVm.Id;
				IsAdded = Model.MachineParts.Any(x => x.Machine.Id == linkId);
			}
		}
	}
}
