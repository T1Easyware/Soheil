using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class MachineFamilyVm : ViewModelBase, IToolboxData
	{
		public Model.MachineFamily Model { get; protected set; }
		public int Id { get { return Model == null ? -1 : Model.Id; } }
		public string Name
		{
			get { return Model == null ? "" : Model.Name; }
			set { Model.Name = value; OnPropertyChanged("Name"); }
		}

		public MachineFamilyVm(Model.MachineFamily model)
		{
			Model = model;
			/*foreach (var machine in model.Machines)
			{
				Machines.Add(new MachineVm(machine, this));
			}*/
		}
		//Machines Observable Collection
		public ObservableCollection<MachineVm> Machines { get { return _machines; } }
		private ObservableCollection<MachineVm> _machines = new ObservableCollection<MachineVm>();

		//IsExpanded Dependency Property
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(MachineFamilyVm), new UIPropertyMetadata(false));

	}
}
