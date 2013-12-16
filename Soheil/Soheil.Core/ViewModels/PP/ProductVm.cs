using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP
{
	public class ProductVm : DependencyObject
	{
		public ProductVm()
		{
			Id = -1;
		}
		/// <summary>
		/// Must be called from within a Context (or have its productReworks filled beforehand)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parentVm"></param>
		public ProductVm(Model.Product model, ProductGroupVm parentVm)
		{
			if (model == null) return;
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
			Group = parentVm;
			foreach (var pr_model in model.ProductReworks)
			{
				ProductReworks.Add(new ProductReworkVm(pr_model, this));
			}
		}

		public ProductVm(Fpc.ProductVm productVm)
		{
			Id = productVm.Id;
			Name = productVm.Name;
			Code = productVm.Code;
			Color = productVm.Color;
			//???
		}

		public int Id { get; protected set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
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
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));
		//Group Dependency Property
		public ProductGroupVm Group
		{
			get { return (ProductGroupVm)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public static readonly DependencyProperty GroupProperty =
			DependencyProperty.Register("Group", typeof(ProductGroupVm), typeof(ProductVm), new UIPropertyMetadata(null));
		//ProductReworks Observable Collection
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
	}
}
