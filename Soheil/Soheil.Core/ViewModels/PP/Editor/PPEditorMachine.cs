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
		public Model.StateStationActivityMachine StateStationActivityMachineModel { get; private set; }
		public int MachineId { get { return StateStationActivityMachineModel.Machine.Id; } }
		public int SelectedMachineId { get; set; }
		public int StateStationActivityMachineId { get { return StateStationActivityMachineModel.Id; } }

		#region Ctor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public PPEditorMachine(Model.SelectedMachine model)
		{
			StateStationActivityMachineModel = model.StateStationActivityMachine;
			SelectedMachineId = model.Id;
			Name = model.StateStationActivityMachine.Machine.Name;
			Code = model.StateStationActivityMachine.Machine.Code;
			IsUsed = model.StateStationActivityMachine.IsFixed;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public PPEditorMachine(Model.StateStationActivityMachine ssamModel)
		{
			StateStationActivityMachineModel = ssamModel;
			SelectedMachineId = ssamModel.Id;
			Name = ssamModel.Machine.Name;
			Code = ssamModel.Machine.Code;
			IsUsed = ssamModel.IsFixed;
		}
		public PPEditorMachine(Fpc.StateStationActivityMachineVm ssam)
		{
			StateStationActivityMachineModel = ssam.Model;
			SelectedMachineId = -1;
			Name = ssam.Name;
			Code = ((Fpc.MachineVm)ssam.Containment).Code;
			IsUsed = ssam.IsDefault;
		} 
		#endregion


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
