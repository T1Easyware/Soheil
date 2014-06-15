using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class MachineFamilyEditorVm : DependencyObject
	{
		public event Action<MachineEditorVm, bool> SelectionChanged;
		#region Ctor
		public MachineFamilyEditorVm(IGrouping<Model.MachineFamily, IGrouping<Model.Machine, Model.StateStationActivityMachine>> machineFamily)
		{
			Name = machineFamily.Key.Name;
			Code = machineFamily.Key.Code;
			foreach (var machine in machineFamily)
			{
				var machineVm = new MachineEditorVm(machine);
				machineVm.SelectedChanged += val =>
				{
					if (SelectionChanged != null)
						SelectionChanged(machineVm, val);
				};
				MachineList.Add(machineVm);
			}
		}

		#endregion

		//MachineList Observable Collection
		public ObservableCollection<MachineEditorVm> MachineList { get { return _machineList; } }
		private ObservableCollection<MachineEditorVm> _machineList = new ObservableCollection<MachineEditorVm>();

		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(MachineFamilyEditorVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(MachineFamilyEditorVm), new UIPropertyMetadata(null));
	}
}
