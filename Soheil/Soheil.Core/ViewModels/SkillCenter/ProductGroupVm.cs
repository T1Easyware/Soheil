using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ProductGroupVm : BaseTreeItemVm
	{
		public ProductGroupVm(Model.ProductGroup model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			foreach (var product in model.Products)
			{
				AddChild(new ProductVm(product));
			}
		}
	}
}
