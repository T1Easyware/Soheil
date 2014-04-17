using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table when in ProductGroup state, representing accumulated ILUO values of <see cref="Soheil.Model.ProductActivitySkill"/>s
	/// </summary>
	public class ProductGroupActivitySkillVm : BaseSkillVm
	{
		//public Model.ProductActivitySkill Model { get; protected set; }

		/// <summary>
		/// Creates an instance of ProductGroupActivitySkillVm and initializes the commands
		/// </summary>
		public ProductGroupActivitySkillVm()
			: base()
		{
			//Data = model.Iluo;
			//Model = model;
		}
	}
}
