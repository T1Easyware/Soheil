using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class ProductGroup : Cell
	{
		public const double HeaderRowHeight = 25d;
		public const double HeaderColumnWidth = 25d;
		public const double RowHeight = 25d;
		public const double ColumnWidth = 75d;
		public readonly Station Station;

		public ProductGroup(Model.ProductGroup model, Station parentVm, bool isRowHeader)
		{
			Station = parentVm;
			Name = model.Name;
			CellType = isRowHeader ? CellType.ProductGroupRowHeaderCell : CellType.ProductGroupColumnHeaderCell;
			foreach (var productModel in model.Products)
			{
				Products.Add(new Product(this, productModel, isRowHeader));
			}
		}

		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductGroup), new UIPropertyMetadata(null));
		//IsExpanded Dependency Property
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ProductGroup), new UIPropertyMetadata(true));
		//Products Observable Collection
		private ObservableCollection<Product> _products = new ObservableCollection<Product>();
		public ObservableCollection<Product> Products { get { return _products; } }
	}
}
