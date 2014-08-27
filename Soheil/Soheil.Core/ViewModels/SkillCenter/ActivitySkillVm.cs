﻿using Soheil.Common;
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
		public Model.ActivitySkill Model { get; set; }

		/// <summary>
		/// Creates an instance of ActivitySkillVm with the given model and initializes the commands
		/// </summary>
		/// <param name="model">Model and its ILUO value are used</param>
		public ActivitySkillVm(Model.ActivitySkill model, int operatorId, int activityId)
			: base(operatorId, activityId)
		{
			Data = model == null ? ILUO.NA : model.Iluo;
			Model = model;
		}
		public ActivitySkillVm(int operatorId, int activityId)
			: base(operatorId, activityId)
		{
		}

		internal void Update(Soheil.Model.ActivitySkill skill)
		{
			if (skill != null)
			{
				Model = skill;
				Data = skill.Iluo;
			}
		}
	}
}
