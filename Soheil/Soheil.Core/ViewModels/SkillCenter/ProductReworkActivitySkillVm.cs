using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ProductReworkActivitySkillVm : BaseSkillVm
	{
		public Model.ProductActivitySkill Model { get; protected set; }

		public ProductReworkActivitySkillVm(Model.ProductActivitySkill model)
			: base()
		{
			Data = model.Iluo;
			Model = model;
		}
	}
}
