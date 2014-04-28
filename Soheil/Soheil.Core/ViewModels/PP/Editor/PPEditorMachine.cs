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
		public int MachineId { get; protected set; }

		#region Ctor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public PPEditorMachine(Model.StateStationActivityMachine ssamModel)
		{
			Name = ssamModel.Machine.Name;
			Code = ssamModel.Machine.Code;
			IsUsed = ssamModel.IsFixed;
		}
		public PPEditorMachine(Model.Machine machineModel)
		{
			MachineId = machineModel.Id;
			Name = machineModel.Name;
			Code = machineModel.Code;
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
		//CanBeUsed Dependency Property
		public bool CanBeUsed
		{
			get { return (bool)GetValue(CanBeUsedProperty); }
			set { SetValue(CanBeUsedProperty, value); }
		}
		public static readonly DependencyProperty CanBeUsedProperty =
			DependencyProperty.Register("CanBeUsed", typeof(bool), typeof(PPEditorMachine), new UIPropertyMetadata(false));
		#endregion
	}
}
