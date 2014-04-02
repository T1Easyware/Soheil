using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table when in Product state, representing accumulated ILUO values of <see cref="Soheil.Model.ProductActivitySkill"/>s
	/// </summary>
	public class ProductActivitySkillVm : BaseSkillVm
	{
		//public Model.ProductActivitySkill Model { get; protected set; }

		/// <summary>
		/// Creates an instance of this Vm and initializes the commands
		/// </summary>
		public ProductActivitySkillVm()
			: base()
		{
			//Data = model.Iluo;
			//Model = model;
		}
	}
}
