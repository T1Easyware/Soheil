using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PM
{
	public class MachineItemVm : PmItemBase
	{
		public Model.Machine Model { get; set; }
        public override int Id { get { return Model == null ? -1 : Model.Id; } }
        public MachineItemVm(Model.Machine model, bool quick = false)
        {
			if (quick) Name = model == null ? "-" : model.Name;

            else if (model == null)
            {
                Name = "همه";
            }
            else
            {
                Model = model;
                Name = model.Name;
                Code = model.Code;
                Status = model.RecordStatus;
            }
            Bar = new PMBarVm();
        }

		#region Callbacks
		protected override void NameChanged(string val)
		{
		}

		protected override void CodeChanged(string val)
		{
		}

		protected override void DescriptionChanged(string val)
		{
		}

		protected override void StatusChanged(Common.Status val)
		{
		} 
		#endregion
	}
}
