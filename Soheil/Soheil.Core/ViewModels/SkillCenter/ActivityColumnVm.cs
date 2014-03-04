using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ActivityColumnVm : BaseVm
	{
		public ActivityColumnVm(Model.Activity model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
		}
	}
}
