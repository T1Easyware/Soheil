using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One column in SkillCenter table, representing a single Activity
	/// </summary>
	public class ActivityColumnVm : BaseVm
	{
		/// <summary>
		/// Creates an instance of ActivityColumnVm with the given model
		/// </summary>
		/// <param name="model">Id, Code and Name of this model are used</param>
		public ActivityColumnVm(Model.Activity model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
		}
	}
}
