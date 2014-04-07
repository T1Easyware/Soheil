using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// A <see cref="BaseTreeItemVm"/> that represents the product group mode in skill center
	/// <para>When an instance of this type is selected in the skill center tree, product activity skills of operators will be in effect</para>
	/// </summary>
	/// <remarks>
	/// <para>The values that are shown in cells of the table will be a single ILUO value of all underlying product reworks of the product group represented by this Vm</para>
	/// <para>If the underlying values are not the same a specific symbol will be shown</para>
	/// <para>Changing the ILUO value in cells of the table will cause an overwrite to all underlying product reworks of the product group represented by this Vm</para>
	/// </remarks>
	public class ProductGroupVm : BaseTreeItemVm
	{
		/// <summary>
		/// Creates an instance of this Vm with the given model and fills it with its underlying Products recursively
		/// </summary>
		/// <param name="model">Id, Name, Code and underlying Products of the model are used</param>
		public ProductGroupVm(Model.ProductGroup model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			foreach (var product in model.Products)
			{
				AddChild(new ProductVm(product));
			}
			InitializeCommands();
		}
	}
}
