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
		public MachineItemVm(Model.Machine model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Status = model.RecordStatus;
		}
	}
}
