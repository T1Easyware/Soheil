using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for displaying a product rework in fpc
	/// </summary>
	public class ProductReworkVm : ViewModelBase, IToolboxData
	{
		/// <summary>
		/// Gets the model for this product rework
		/// </summary>
		public Model.ProductRework Model { get; protected set; }
		/// <summary>
		/// Gets the Id for this product rework
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name for this product rework
		/// <para>If this is a main production, returns "Final Product"</para>
		/// </summary>
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Gets or sets the Code for this product rework
		/// </summary>
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}

		/// <summary>
		/// Creates an instance of this view model with given model
		/// <para>If model is main production, updates product rework's Name to "Final Product" (not commiting)</para>
		/// </summary>
		/// <param name="model">Model can't be null</param>
		public ProductReworkVm(Model.ProductRework model)
		{
			Model = model;
			if (model.Rework == null)
			{
				Name = "محصول نهایی";
				ReworkName = "محصول نهایی";
				IsMainProduction = true;
			}
			else
			{
				ReworkName = model.Rework.Name;
				IsMainProduction = false;
			}
		}

		/// <summary>
		/// A value that indicates if this is a main production (not a rework)
		/// </summary>
		public bool IsMainProduction { get; private set; }

		/// <summary>
		/// Gets or sets a bindable value for Name of the Rework
		/// <para>If it's the main production, returns "Final Product"</para>
		/// </summary>
		public string ReworkName
		{
			get { return (string)GetValue(ReworkNameProperty); }
			set { SetValue(ReworkNameProperty, value); }
		}
		public static readonly DependencyProperty ReworkNameProperty =
			DependencyProperty.Register("ReworkName", typeof(string), typeof(ProductReworkVm), new UIPropertyMetadata(null));
	}
}
