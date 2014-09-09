using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.Fpc
{
	public class UnitSetVm : DependencyObject
	{
		public Model.UnitSet Model { get; set; }
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(UnitSetVm), new PropertyMetadata(""));
		public UnitSetVm(Model.UnitSet model)
		{
			Model = model;
			if (model == null) Code = "عدد";
			else
				Code = model.Code;
		}
	}
}
