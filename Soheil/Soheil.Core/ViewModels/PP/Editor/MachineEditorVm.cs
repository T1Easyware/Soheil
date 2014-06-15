using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class MachineEditorVm : DependencyObject
	{
		public event Action<bool> SelectedChanged;
		public int MachineId { get; protected set; }

		IGrouping<Model.Machine, Model.StateStationActivityMachine> _group;

		#region Ctor
		/// <summary>
		/// Creates a selected machine
		/// </summary>
		/// <param name="smModel"></param>
		public MachineEditorVm(Model.SelectedMachine smModel)
		{
			Name = smModel.StateStationActivityMachine.Machine.Name;
			Code = smModel.StateStationActivityMachine.Machine.Code;
			MachineId = smModel.StateStationActivityMachine.Machine.Id;
			IsUsed = true;
			CanBeUsed = true;
		}
		/// <summary>
		/// Create a machine with the given SSAM Group
		/// </summary>
		/// <param name="machine"></param>
		public MachineEditorVm(IGrouping<Model.Machine, Model.StateStationActivityMachine> machine)
		{
			_group = machine;
			MachineId = machine.Key.Id;
			Name = machine.Key.Name;
			Code = machine.Key.Code;
		}

		public void Revalidate(Model.Process process)
		{
			CanBeUsed = _group.Any(x => x.StateStationActivity.Id == process.StateStationActivity.Id);
		}
		#endregion


		#region DpProps
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(MachineEditorVm), new UIPropertyMetadata(null));
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(MachineEditorVm), new UIPropertyMetadata(null));
		public bool IsUsed
		{
			get { return (bool)GetValue(IsUsedProperty); }
			set { SetValue(IsUsedProperty, value); }
		}
		public static readonly DependencyProperty IsUsedProperty =
			DependencyProperty.Register("IsUsed", typeof(bool), typeof(MachineEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if (((MachineEditorVm)d).SelectedChanged != null)
					((MachineEditorVm)d).SelectedChanged((bool)e.NewValue);
			}));
		public bool CanBeUsed
		{
			get { return (bool)GetValue(CanBeUsedProperty); }
			set { SetValue(CanBeUsedProperty, value); }
		}
		public static readonly DependencyProperty CanBeUsedProperty =
			DependencyProperty.Register("CanBeUsed", typeof(bool), typeof(MachineEditorVm), new UIPropertyMetadata(false));
		#endregion
	}
}
