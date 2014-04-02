using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table when in General state, representing a single <see cref="Soheil.Model.ActivitySkill"/> 
	/// </summary>
	public class ActivitySkillVm : BaseSkillVm
	{
		/// <summary>
		/// Gets the model for this Vm
		/// </summary>
		public Model.ActivitySkill Model { get; protected set; }

		/// <summary>
		/// Creates an instance of this Vm with the given model and initializes the commands
		/// </summary>
		/// <param name="model">Model and its ILUO value are used</param>
		public ActivitySkillVm(Model.ActivitySkill model)
			: base()
		{
			Data = model.Iluo;
			Model = model;
		}
	}
}
