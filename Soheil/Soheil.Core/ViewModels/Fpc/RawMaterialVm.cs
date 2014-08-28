using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class RawMaterialVm : ViewModelBase, IToolboxData
	{
		public Model.RawMaterial Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
		}
		
		/// <summary>
		/// Creates an instance of BomVm
		/// </summary>
		/// <param name="model"></param>
		public RawMaterialVm(Model.RawMaterial model)
		{
			Model = model;
		}

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(RawMaterialVm), new UIPropertyMetadata(true));

	}
}
