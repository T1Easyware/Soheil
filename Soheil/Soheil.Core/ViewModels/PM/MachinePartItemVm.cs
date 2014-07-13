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
		public MachinePartItemVm(Model.MachinePart model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Description = model.Description;
			Status = model.RecordStatus;
		}
	}
}
