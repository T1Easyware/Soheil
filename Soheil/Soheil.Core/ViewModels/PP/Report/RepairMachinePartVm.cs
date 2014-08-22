using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class RepairMachinePartVm : DependencyObject
	{
		public Model.MachinePart Model { get; set; }
		public RepairMachinePartVm(Model.MachinePart model)
		{
			Model = model;

			if (model.IsMachine)
			{
				Name = "خود ماشین";
				Code = "";
			}
			else
			{
				if (string.IsNullOrWhiteSpace(model.Name))
					Name = model.Part.Name;
				else
					Name = model.Name;

				if (string.IsNullOrWhiteSpace(model.Code))
					Code = model.Part.Code;
				else
					Code = model.Code;
			}
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
			DependencyProperty.Register("Name", typeof(string), typeof(RepairMachinePartVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(RepairMachinePartVm), new PropertyMetadata(null));


	}
}
