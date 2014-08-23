using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class WarehouseVm : DependencyObject
	{
		public Model.Warehouse Model { get; private set; }

		public WarehouseVm(Model.Warehouse model)
		{
			Model = model;
			Name = model.Name;
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(WarehousesVM), new UIPropertyMetadata(null));
	}
}
