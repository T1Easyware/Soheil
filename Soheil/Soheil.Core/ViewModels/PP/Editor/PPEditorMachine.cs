using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorMachine : DependencyObject
	{
		#region Ctor
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorMachine(Model.SelectedMachine model)
		{
			MachineId = model.StateStationActivityMachine.Machine.Id;
			SelectedMachineId = model.Id;
			StateStationActivityMachineId = model.StateStationActivityMachine.Id;
			Name = model.StateStationActivityMachine.Machine.Name;
			Code = model.StateStationActivityMachine.Machine.Code;
			IsUsed = model.StateStationActivityMachine.IsFixed;
		}
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorMachine(Model.StateStationActivityMachine ssamModel)
		{
			MachineId = ssamModel.Machine.Id;
			SelectedMachineId = ssamModel.Id;
			StateStationActivityMachineId = ssamModel.Id;
			Name = ssamModel.Machine.Name;
			Code = ssamModel.Machine.Code;
			IsUsed = ssamModel.IsFixed;
		}
		public PPEditorMachine(Fpc.StateStationActivityMachineVm ssam)
		{
			MachineId = ssam.Containment.Id;
			SelectedMachineId = -1;
			StateStationActivityMachineId = ssam.Id;
			Name = ssam.Name;
			Code = ((Fpc.MachineVm)ssam.Containment).Code;
			IsUsed = ssam.IsDefault;
		} 
		#endregion

		public int MachineId { get; set; }
		public int SelectedMachineId { get; set; }
		public int StateStationActivityMachineId { get; set; }

		#region DpProps
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorMachine), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PPEditorMachine), new UIPropertyMetadata(null));
		//IsUsed Dependency Property
		public bool IsUsed
		{
			get { return (bool)GetValue(IsUsedProperty); }
			set { SetValue(IsUsedProperty, value); }
		}
		public static readonly DependencyProperty IsUsedProperty =
			DependencyProperty.Register("IsUsed", typeof(bool), typeof(PPEditorMachine), new UIPropertyMetadata(false)); 
		#endregion
	}
}
