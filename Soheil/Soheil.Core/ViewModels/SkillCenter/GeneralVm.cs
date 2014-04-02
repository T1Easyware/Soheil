using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// A <see cref="BaseTreeItemVm"/> that represents the general mode in skill center
	/// <para>When an instance of this type is selected in the skill center tree, general activity skills of operators will be in effect</para>
	/// </summary>
	public class GeneralVm : BaseTreeItemVm
	{
		/// <summary>
		/// Creates an instance of this Vm with Id=-100
		/// </summary>
		public GeneralVm()
		{
			Id = -100;
			Name = "همه";
			Code = "...";
			InitializeCommands();
		}
	}
}
