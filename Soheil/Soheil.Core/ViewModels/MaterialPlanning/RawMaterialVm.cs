using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class RawMaterialVm : DependencyObject
	{
		public Model.RawMaterial Model { get; set; }
		public static double ItemHeight = 34;
		public RawMaterialVm(Model.RawMaterial model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			SafetyStock = model.SafetyStock;
			UpdateInventory();
		}

		public void UpdateInventory()
		{
			Inventory = Model.Inventory;
		}


		/// <summary>
		/// Gets or sets a bindable value that indicates Index
		/// </summary>
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(RawMaterialVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates NumberOfRequests and by setting its value, Height is updated
		/// </summary>
		public int NumberOfRequests
		{
			get { return (int)((double)GetValue(HeightProperty) / ItemHeight); }
			set { SetValue(HeightProperty, value * ItemHeight); }
		}
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(RawMaterialVm), new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(RawMaterialVm), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(RawMaterialVm), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Inventory
		/// </summary>
		public double Inventory
		{
			get { return (double)GetValue(InventoryProperty); }
			set { SetValue(InventoryProperty, value); }
		}
		public static readonly DependencyProperty InventoryProperty =
			DependencyProperty.Register("Inventory", typeof(double), typeof(RawMaterialVm), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates SafetyStock
		/// </summary>
		public int SafetyStock
		{
			get { return (int)GetValue(SafetyStockProperty); }
			set { SetValue(SafetyStockProperty, value); }
		}
		public static readonly DependencyProperty SafetyStockProperty =
			DependencyProperty.Register("SafetyStock", typeof(int), typeof(RawMaterialVm), new PropertyMetadata(0));

	}
}
