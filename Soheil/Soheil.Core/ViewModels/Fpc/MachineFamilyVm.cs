using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for displaying a machine family in fpc
	/// </summary>
	public class MachineFamilyVm : ViewModelBase, IToolboxData
	{
		/// <summary>
		/// Gets the model for this machine family
		/// </summary>
		public Model.MachineFamily Model { get; protected set; }
		/// <summary>
		/// Gets the Id for this machine family
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name for this machine family
		/// </summary>
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Creates an instance of this view model with the given model
		/// </summary>
		/// <param name="model">Model can be null</param>
		public MachineFamilyVm(Model.MachineFamily model)
		{
			Model = model;
		}
		/// <summary>
		/// Gets a bindable collection of machines that are part of this family
		/// </summary>
		public ObservableCollection<MachineVm> Machines { get { return _machines; } }
		private ObservableCollection<MachineVm> _machines = new ObservableCollection<MachineVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates if this item should be expanded as an expander
		/// </summary>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(MachineFamilyVm), new UIPropertyMetadata(false));

	}
}
