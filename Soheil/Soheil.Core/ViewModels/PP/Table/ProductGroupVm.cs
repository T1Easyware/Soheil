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
	public class ProductGroupVm : DependencyObject
	{
		protected ProductGroupVm()
		{

		}
		public ProductGroupVm(Model.ProductGroup model)
		{
			if (model == null) return;
			_id = model.Id;
			Name = model.Name;
			Code = model.Code;

			foreach (var p_model in model.Products.Where(x => x.RecordStatus == Common.Status.Active))
			{
				Products.Add(new ProductVm(p_model, this));
			}
		}


		protected int _id = 0;
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductGroupVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductGroupVm), new UIPropertyMetadata(null));
		//Products Observable Collection
		public ObservableCollection<ProductVm> Products { get { return _products; } }
		private ObservableCollection<ProductVm> _products = new ObservableCollection<ProductVm>();
	}
}
