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
		public override int Id { get { return Model.Id; } set { Model.Id = value; } }
		public MachineItemVm(Model.Machine model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			Status = model.RecordStatus;
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
