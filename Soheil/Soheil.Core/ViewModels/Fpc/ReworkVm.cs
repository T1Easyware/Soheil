using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ProductReworkVm : NamedVM
	{
		public Model.ProductRework Model { get; set; }

		public ProductReworkVm(Model.ProductRework model)
		{
			Model = model;
			if (model.Rework == null)
			{
				Name = "محصول نهایی";
				ReworkName = "محصول نهایی";
				Code = "";
				IsMainProduction = true;
			}
			else
			{
				Name = model.Name;
				ReworkName = model.Rework.Name;
				Code = model.Code;
				IsMainProduction = false;
			}
			Id = model.Id;
		}
		public bool IsMainProduction { get; private set; }

		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductReworkVm), new UIPropertyMetadata(null));
		//ReworkName Dependency Property
		public string ReworkName
		{
			get { return (string)GetValue(ReworkNameProperty); }
			set { SetValue(ReworkNameProperty, value); }
		}
		public static readonly DependencyProperty ReworkNameProperty =
			DependencyProperty.Register("ReworkName", typeof(string), typeof(ProductReworkVm), new UIPropertyMetadata(null));
	}
}
