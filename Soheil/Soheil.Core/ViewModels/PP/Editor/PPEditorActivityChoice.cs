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
	public class PPEditorActivityChoice : DependencyObject
	{
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
		/// Creates an instance of PPEditorActivityChoice with given StateStationActivity model and PPEditorProcess parent
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parent"></param>
		public PPEditorActivityChoice(Model.StateStationActivity model, PPEditorProcess parent)
		{
			Model = model;
			SetValue(ParentProperty, parent);
			CycleTime = Model.CycleTime;
			ManHour = Model.ManHour;
		}

		/// <summary>
		/// Dependency property for parent used in xaml to compare this choice with selected choice
		/// </summary>
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(PPEditorProcess), typeof(PPEditorActivityChoice), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the bindable CycleTime of this choice
		/// </summary>
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(PPEditorActivityChoice), new UIPropertyMetadata(0f));
		/// <summary>
		/// Gets or sets the bindable ManHour of this choice
		/// </summary>
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(PPEditorActivityChoice), new UIPropertyMetadata(0f));

		/// <summary>
		/// Gets or sets a bindable value to indicate whether Manhour does not match the number of operators assigned to this choice
		/// <para>This could be true when an auto planning considered this choice but not able to assign operators yet</para>
		/// </summary>
		public bool OperatorCountError
		{
			get { return (bool)GetValue(OperatorCountErrorProperty); }
			set { SetValue(OperatorCountErrorProperty, value); }
		}
		public static readonly DependencyProperty OperatorCountErrorProperty =
			DependencyProperty.Register("OperatorCountError", typeof(bool), typeof(PPEditorActivityChoice), new UIPropertyMetadata(true));
	}
}
