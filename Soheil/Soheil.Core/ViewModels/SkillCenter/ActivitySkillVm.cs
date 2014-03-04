using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ActivitySkillVm : BaseSkillVm
	{
		public Model.ActivitySkill Model { get; protected set; }

		public ActivitySkillVm(Model.ActivitySkill model)
			: base()
		{
			Data = model.Iluo;
			Model = model;
		}
	}
}
