using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class MachineVm : NamedVM
	{
		public MachineVm()
		{

		}
		public MachineVm(Model.Machine model, MachineFamilyVm familyVm)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Family = familyVm;
		}
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(MachineVm), new UIPropertyMetadata(null));
		//Family Dependency Property
		public MachineFamilyVm Family
		{
			get { return (MachineFamilyVm)GetValue(FamilyProperty); }
			set { SetValue(FamilyProperty, value); }
		}
		public static readonly DependencyProperty FamilyProperty =
			DependencyProperty.Register("Family", typeof(MachineFamilyVm), typeof(MachineVm), new UIPropertyMetadata(null));
	}
}
