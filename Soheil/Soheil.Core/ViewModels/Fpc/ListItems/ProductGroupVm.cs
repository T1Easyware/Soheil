using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc.ListItems
{
	public class ProductGroupVm : DependencyObject
	{
		#region Properties and Events
		public event Action<FpcVm> SelectionChanged;
		public event Action<ProductVm> FpcAddedToProduct;

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductGroupVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductGroupVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable collection of Products
		/// </summary>
		public ObservableCollection<ProductVm> Products { get { return _products; } }
		private ObservableCollection<ProductVm> _products = new ObservableCollection<ProductVm>();
		/// <summary>
		/// Gets or sets a bindable value that indicates IsExpanded
		/// </summary>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ProductGroupVm), new UIPropertyMetadata(false));
		#endregion

		#region Ctor and Init
		public ProductGroupVm(Model.ProductGroup model)
		{
			Name = model.Name;
			Code = model.Code;
			foreach (var product in model.Products)
			{
				var productVm = new ProductVm(product);
				productVm.SelectionChanged += fpc =>
				{
					if (SelectionChanged != null)
						SelectionChanged(fpc);
				};
				productVm.AddCommand = new Commands.Command(o =>
				{
					if (FpcAddedToProduct != null)
						FpcAddedToProduct(productVm);
				});
				Products.Add(productVm);
			}
		}
		#endregion

		#region Methods
		#endregion

		#region Commands
		#endregion
	}
}
