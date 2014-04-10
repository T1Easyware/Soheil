using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class MachineVm : ViewModelBase, IToolboxData
	{
		public Model.Machine Model { get; protected set; }
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

		public MachineVm(Model.Machine model, MachineFamilyVm familyVm)
		{
			Model = model;
			Family = familyVm;
		}
		//Family Dependency Property
		public MachineFamilyVm Family
		{
			get { return (MachineFamilyVm)GetValue(FamilyProperty); }
			set { SetValue(FamilyProperty, value); }
		}
		public static readonly DependencyProperty FamilyProperty =
			DependencyProperty.Register("Family", typeof(MachineFamilyVm), typeof(MachineVm), new UIPropertyMetadata(null));

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(MachineVm), new UIPropertyMetadata(true));
	}
}
