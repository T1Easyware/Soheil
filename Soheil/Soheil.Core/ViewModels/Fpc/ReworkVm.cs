using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ReworkVm : ViewModelBase, IToolboxData
	{
		/// <summary>
		/// Gets the model for this Rework
		/// </summary>
		public Model.Rework Model { get; protected set; }
		/// <summary>
		/// Gets the Id for this Rework
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name for this Rework
		/// </summary>
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Gets or sets the Code for this Rework
		/// </summary>
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}

		/// <summary>
		/// Creates an instance of this view model with the given model
		/// </summary>
		/// <param name="model">Model can be null</param>
		public ReworkVm(Model.Rework model)
		{
			Model = model;
		}
	}
}
