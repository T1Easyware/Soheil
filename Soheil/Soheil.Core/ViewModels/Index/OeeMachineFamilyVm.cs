using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeMachineFamilyVm : DependencyObject
	{
		public event Action<OeeMachineVm> Selected;
		public OeeMachineFamilyVm(Model.MachineFamily model)
		{
			Name = model.Name;
			foreach (var item in model.Machines.Where(x=>x.Status == (byte)Status.Active && x.HasOEE))
			{
				var vm = new OeeMachineVm(item);
				vm.Selected += () =>
				{
					if (Selected != null) Selected(vm);
				};
				Machines.Add(vm);
			}
		}

		public ObservableCollection<OeeMachineVm> Machines { get { return _machines; } }
		private ObservableCollection<OeeMachineVm> _machines = new ObservableCollection<OeeMachineVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates Name of this MachineFamily
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(OeeMachineFamilyVm), new PropertyMetadata(""));
		
		
	}
}
