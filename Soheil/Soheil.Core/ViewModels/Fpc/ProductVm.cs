using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for displaying a product in fpc
	/// </summary>
	public class ProductVm : ViewModelBase, IToolboxData
	{
		/// <summary>
		/// Gets the model for this product
		/// </summary>
		public Model.Product Model { get; protected set; }
		/// <summary>
		/// Gets the Id for this product
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name for this product
		/// </summary>
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Gets or sets the Code for this product
		/// </summary>
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}
		/// <summary>
		/// Gets or sets the Color for this product
		/// </summary>
		public Color Color
		{
			get { return Model == null ? Colors.White : Model.Color; }
			set { Model.Color = value; OnPropertyChanged("Color"); }
		}

		/// <summary>
		/// Creates an instance of this view model with the given model
		/// </summary>
		/// <param name="model">Model can be null</param>
		public ProductVm(Model.Product model)
		{
			Model = model;
		}
	}
}
