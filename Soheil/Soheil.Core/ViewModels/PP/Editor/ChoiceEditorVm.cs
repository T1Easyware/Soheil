using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// ViewModel for a specific StateStationActivity that can be a candidate for a process with same activity, but different StateStationActivity
	/// <para>Before considering the number of operators (ManHour), multiple choices can represent a single process</para>
	/// <para>After that operators are assigned, one of these choices (which manhour is equal to number of operators) are selected</para>
	/// </summary>
	public class ChoiceEditorVm : DependencyObject
	{
		public event Action<ChoiceEditorVm> Selected;
		/// <summary>
		/// Gets the model for StateStationActivity
		/// </summary>
		public Model.StateStationActivity Model { get; private set; }
		/// <summary>
		/// Gets the Activity Id
		/// </summary>
		public int ActivityId { get { return Model.Activity.Id; } }
		/// <summary>
		/// Gets the StateStationActivity Id
		/// </summary>
		public int StateStationActivityId { get { return Model.Id; } }

		/// <summary>
		/// Creates an instance of ChoiceEditorVm with given StateStationActivity model and PPEditorProcess parent
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parent"></param>
		public ChoiceEditorVm(Model.StateStationActivity model)
		{
			Model = model;
			CycleTime = Model.CycleTime;
			ManHour = Model.ManHour;
			IsMany = model.IsMany;
			SelectCommand = new Commands.Command(o => { if (Selected != null)Selected(this); });
		}

		/// <summary>
		/// Gets or sets the bindable CycleTime of this choice
		/// </summary>
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(ChoiceEditorVm), new UIPropertyMetadata(0f));
		/// <summary>
		/// Gets or sets the bindable ManHour of this choice
		/// </summary>
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(ChoiceEditorVm), new UIPropertyMetadata(0f));

		/// <summary>
		/// Gets or sets a bindable value that indicate whether this choice can be used multiple times
		/// </summary>
		public bool IsMany
		{
			get { return (bool)GetValue(IsManyProperty); }
			set { SetValue(IsManyProperty, value); }
		}
		public static readonly DependencyProperty IsManyProperty =
			DependencyProperty.Register("IsMany", typeof(bool), typeof(ChoiceEditorVm), new UIPropertyMetadata(false));

		/// <summary>
		/// For use in ProcessEditorVm to select a choice
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(ChoiceEditorVm), new UIPropertyMetadata(null));

	}
}
