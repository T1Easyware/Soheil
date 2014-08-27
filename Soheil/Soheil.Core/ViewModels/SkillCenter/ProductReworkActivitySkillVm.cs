using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table when in ProductRework state, representing a single <see cref="Soheil.Model.ProductActivitySkill"/>
	/// </summary>
	public class ProductReworkActivitySkillVm : BaseSkillVm
	{
		/// <summary>
		/// Gets the model for this Vm
		/// </summary>
		public Model.ProductActivitySkill Model { get; set; }

		public int ProductReworkId { get; set; }

		/// <summary>
		/// Creates an instance of ProductReworkActivitySkillVm with the given model and initializes the commands
		/// </summary>
		/// <param name="model">Model and its ILUO value are used</param>
		public ProductReworkActivitySkillVm(Model.ProductActivitySkill model, Model.ActivitySkill general, int operatorId, int activityId, int productReworkId)
			: base(operatorId, activityId)
		{
			Data = model == null ? ILUO.NA : model.Iluo;
			GeneralData = general == null ? ILUO.NA : general.Iluo;
			ProductReworkId = productReworkId;
			Model = model;
		}
		public ProductReworkActivitySkillVm(int operatorId, int activityId, int productReworkId)
			: base(operatorId, activityId)
		{
			ProductReworkId = productReworkId;
		}

		internal void Update(Soheil.Model.ProductActivitySkill skill)
		{
			if (skill != null)
			{
				Model = skill;
				Data = skill.Iluo;
			}
		}
		internal void Update(Soheil.Model.ActivitySkill skill)
		{
			if (skill != null)
			{
				GeneralData = skill.Iluo;
			}
		}
	}
}
