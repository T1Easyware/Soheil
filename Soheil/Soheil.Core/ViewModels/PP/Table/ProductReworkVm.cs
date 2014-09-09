using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP
{
	public class ProductReworkVm : DependencyObject
	{
		public ProductReworkVm(Model.ProductRework model, ProductVm parentVm)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Product = parentVm;
			Rework = new ReworkVm(model.Rework);
		}
		/// <summary>
		/// Creates a productRework viewModel for the given model (ignores productGroup)
		/// <para>Use this constructor if you don't care for the links between the same products</para>
		/// <para>It automatically create a new ProductVm for each instance or ProductReworkVm</para>
		/// </summary>
		/// <param name="model"></param>
		public ProductReworkVm(Model.ProductRework model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Product = new ProductVm(model.Product, null);
			Rework = new ReworkVm(model.Rework);
		}

		public int Id { get; private set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductReworkVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductReworkVm), new UIPropertyMetadata(null));
		//Product Dependency Property
		public ProductVm Product
		{
			get { return (ProductVm)GetValue(ProductProperty); }
			set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(ProductVm), typeof(ProductReworkVm), new UIPropertyMetadata(null));
		//Rework Dependency Property
		public ReworkVm Rework
		{
			get { return (ReworkVm)GetValue(ReworkProperty); }
			set { SetValue(ReworkProperty, value); }
		}
		public static readonly DependencyProperty ReworkProperty =
			DependencyProperty.Register("Rework", typeof(ReworkVm), typeof(ProductReworkVm), new UIPropertyMetadata(null));
	}
}
