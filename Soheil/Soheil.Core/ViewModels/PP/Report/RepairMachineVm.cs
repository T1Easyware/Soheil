using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class RepairMachineVm : DependencyObject
	{
		public Model.Machine Model { get; set; }
		public RepairMachineVm(Model.Machine model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
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
			DependencyProperty.Register("Name", typeof(string), typeof(RepairMachineVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(RepairMachineVm), new PropertyMetadata(null));
	}
}
