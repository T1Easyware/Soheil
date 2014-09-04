using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class MachineVm : DependencyObject
	{
		public MachineVm(Model.Machine model)
		{
			MachineId = model.Id;
			Name = model.Name;
			Code = model.Code;
			if (model.MachineFamily != null)
				FamilyName = model.MachineFamily.Name;
		}
		public int MachineId { get; set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(MachineVm), new UIPropertyMetadata(""));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(MachineVm), new UIPropertyMetadata(""));
		//FamilyName Dependency Property
		public string FamilyName
		{
			get { return (string)GetValue(FamilyNameProperty); }
			set { SetValue(FamilyNameProperty, value); }
		}
		public static readonly DependencyProperty FamilyNameProperty =
			DependencyProperty.Register("FamilyName", typeof(string), typeof(MachineVm), new UIPropertyMetadata(""));
	}
}
