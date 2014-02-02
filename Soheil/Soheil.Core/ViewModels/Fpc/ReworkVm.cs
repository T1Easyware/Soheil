using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ProductReworkVm : ViewModelBase, IToolboxData
	{
		public Model.ProductRework Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}

		public ProductReworkVm(Model.ProductRework model)
		{
			Model = model;
			if (model.Rework == null)
			{
				Name = "محصول نهایی";
				ReworkName = "محصول نهایی";
				IsMainProduction = true;
			}
			else
			{
				ReworkName = model.Rework.Name;
				IsMainProduction = false;
			}
		}
		public bool IsMainProduction { get; private set; }

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
