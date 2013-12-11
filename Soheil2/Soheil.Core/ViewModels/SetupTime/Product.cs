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
	public class Product : Cell
	{
		public Product(ProductGroup parentVm, Model.Product model, bool isRowHeader)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			CellType = isRowHeader ? CellType.ProductRowHeaderCell : CellType.ProductColumnHeaderCell;
			Color = new SolidColorBrush(model.Color);
			ProductGroup = parentVm;
			//if mainProduct is not included, add it as an 'invalid=true'
			if (!model.ProductReworks.Any(x => x.Rework == null))
			{
				Reworks.Add(Rework.InvalidMainProduct(this, isRowHeader));
			}

			foreach (var productReworkModel in model.ProductReworks)
			{
				Reworks.Add(new Rework(this, productReworkModel, isRowHeader));
			}
		}

		public int Id { get; set; }
		//ProductName Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(Product), new UIPropertyMetadata(""));
		//ProductCode Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(Product), new UIPropertyMetadata(""));
		//ProductColor Dependency Property
		public SolidColorBrush Color
		{
			get { return (SolidColorBrush)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(Product), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
		//ProductGroup Dependency Property
		//IsExpanded Dependency Property
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Product), new UIPropertyMetadata(true));
		public ProductGroup ProductGroup
		{
			get { return (ProductGroup)GetValue(ProductGroupProperty); }
			set { SetValue(ProductGroupProperty, value); }
		}
		public static readonly DependencyProperty ProductGroupProperty =
			DependencyProperty.Register("ProductGroup", typeof(ProductGroup), typeof(Product), new UIPropertyMetadata(null));
		//Reworks Observable Collection
		private ObservableCollection<Rework> _rework = new ObservableCollection<Rework>();
		public ObservableCollection<Rework> Reworks { get { return _rework; } }
	}
}
