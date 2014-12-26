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
	/// <summary>
	/// ViewModel for product
	/// <para><remarks>cannot be used in JobEditor (in jobEditor use JobProductVm)</remarks></para>
	/// </summary>
	public class ProductVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of ProductVm with the given model and parent
		/// </summary>
		/// <param name="model">product reworks are also in use</param>
		/// <param name="parentVm">ProductGroup parent</param>
		public ProductVm(Model.Product model, ProductGroupVm parentVm)
		{
			if (model == null) return;
			Model = model;
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
		public Model.Product Model { get; private set; }
		/// <summary>
		/// Gets the model Id
		/// </summary>
		public int Id { get; protected set; }
		/// <summary>
		/// Gets a bindable value for name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			protected set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable value for code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			protected set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable value for color
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			protected set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));

		/// <summary>
		/// Gets a bindable value for product group
		/// </summary>
		public ProductGroupVm Group
		{
			get { return (ProductGroupVm)GetValue(GroupProperty); }
			protected set { SetValue(GroupProperty, value); }
		}
		public static readonly DependencyProperty GroupProperty =
			DependencyProperty.Register("Group", typeof(ProductGroupVm), typeof(ProductVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets a bindable collection of product reworks
		/// </summary>
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();

		/// <summary>
		/// Gets or sets a bindable collection that indicates Connectors
		/// </summary>
		//public ObservableCollection<PPConnectorVm> Connectors { get { return _connectors; } }
		//private ObservableCollection<PPConnectorVm> _connectors = new ObservableCollection<PPConnectorVm>();
	}
}
