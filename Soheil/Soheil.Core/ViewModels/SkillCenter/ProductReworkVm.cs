using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ProductReworkVm : BaseTreeItemVm
	{
		public ProductReworkVm(Model.ProductRework model)
		{
			Id = model.Id;
			if(model.Rework == null)
			{
				IsMainProduct = true;
				Name = "تولید اصلی";
				Code = model.Code;
			}
			else
			{
				IsMainProduct = false;
				Name = model.Rework.Name;
				Code = model.Rework.Code;
			}
		}
		//IsMainProduct Dependency Property
		public bool IsMainProduct
		{
			get { return (bool)GetValue(IsMainProductProperty); }
			set { SetValue(IsMainProductProperty, value); }
		}
		public static readonly DependencyProperty IsMainProductProperty =
			DependencyProperty.Register("IsMainProduct", typeof(bool), typeof(ProductReworkVm), new UIPropertyMetadata(true));
	}
}
