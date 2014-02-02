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
	public class ProductVm : ViewModelBase, IToolboxData
	{
		public Model.Product Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}
		public Color Color
		{
			get { return Model == null ? Colors.White : Model.Color; }
			set { Model.Color = value; OnPropertyChanged("Color"); }
		}

		public ProductVm(Model.Product model)
		{
			Model = model;
		}
	}
}
