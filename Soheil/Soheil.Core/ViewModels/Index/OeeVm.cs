using Soheil.Common;
using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }

		public OeeVm(AccessType access)
		{
			Access = access;
			var ds = new DataServices.MachineFamilyDataService();
			var list = ds.GetActives();
			foreach (var item in list)
			{
				var vm = new OeeMachineFamilyVm(item);
				vm.Selected += machine =>
				{
					CurrentMachine = machine;
					machine.Load();
				};
				MachineFamilies.Add(vm);
			}
		}


		public ObservableCollection<OeeMachineFamilyVm> MachineFamilies { get { return _machineFamilies; } }
		private ObservableCollection<OeeMachineFamilyVm> _machineFamilies = new ObservableCollection<OeeMachineFamilyVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates the Current selected Machine in the list
		/// </summary>
		public OeeMachineVm CurrentMachine
		{
			get { return (OeeMachineVm)GetValue(CurrentMachineProperty); }
			set { SetValue(CurrentMachineProperty, value); }
		}
		public static readonly DependencyProperty CurrentMachineProperty =
			DependencyProperty.Register("CurrentMachine", typeof(OeeMachineVm), typeof(OeeVm), new PropertyMetadata(null));
	}
}
