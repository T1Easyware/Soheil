using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ProductVm : NamedVM
	{
		public ProductVm() { }
		public ProductVm(Model.Product model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
		}
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		//Color Dependency Property
		public Color Color
		{
			get { return (System.Windows.Media.Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));
	}
}
