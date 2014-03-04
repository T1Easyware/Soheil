using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ProductVm : BaseTreeItemVm
	{
		public ProductVm(Model.Product model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
			foreach (var pr in model.ProductReworks)
			{
				AddChild(new ProductReworkVm(pr));
			}
		}
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));
	}
}
