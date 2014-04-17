using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for displaying a machine in fpc
	/// </summary>
	public class MachineVm : ViewModelBase, IToolboxData
	{
		/// <summary>
		/// Gets the model for this machine
		/// </summary>
		public Model.Machine Model { get; protected set; }
		/// <summary>
		/// Gets the Id for this machine
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name for this machine
		/// </summary>
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Gets or sets the Code for this machine
		/// </summary>
		public string Code
		{
			get { return Model == null ? "" : Model.Code; }
			set { Model.Code = value; OnPropertyChanged("Code"); }
		}

		/// <summary>
		/// Creates an instance of MachineVm with the given model and parent
		/// </summary>
		/// <param name="model">Model can be null</param>
		/// <param name="familyVm">Parent can be null</param>
		public MachineVm(Model.Machine model, MachineFamilyVm familyVm)
		{
			Model = model;
			Family = familyVm;
		}
		/// <summary>
		/// Gets or sets a bindable value for MachineFamily of this machine
		/// </summary>
		public MachineFamilyVm Family
		{
			get { return (MachineFamilyVm)GetValue(FamilyProperty); }
			set { SetValue(FamilyProperty, value); }
		}
		public static readonly DependencyProperty FamilyProperty =
			DependencyProperty.Register("Family", typeof(MachineFamilyVm), typeof(MachineVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates if this item should be visible
		/// </summary>
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(MachineVm), new UIPropertyMetadata(true));
	}
}
