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
		public Model.StateStationActivity Model { get; private set; }
		public int ActivityId { get { return Model.Activity.Id; } }
		public int StateStationActivityId { get { return Model.Id; } }

		/// <summary>
		/// Creates an instance of this vm with given StateStationActivity model and PPEditorProcess parent
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parent"></param>
		public PPEditorActivityChoice(Model.StateStationActivity model, PPEditorProcess parent)
		{
			Model = model;
			Parent = parent;
			CycleTime = Model.CycleTime;
			ManHour = Model.ManHour;
		}

		//Parent Dependency Property
		public PPEditorProcess Parent
		{
			get { return (PPEditorProcess)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(PPEditorProcess), typeof(PPEditorActivityChoice), new UIPropertyMetadata(null));

		//CycleTime Dependency Property
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(PPEditorActivityChoice), new UIPropertyMetadata(0f));
		//ManHour Dependency Property
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(PPEditorActivityChoice), new UIPropertyMetadata(0f));

		/// <summary>
		/// Gets or sets a bindable value to indicate whether Manhour matches the number of operators assigned to this choice
		/// <para>This could be false when an auto planning considered this choice but not able to assign operators yet</para>
		/// </summary>
		public bool OperatorCountOk
		{
			get { return (bool)GetValue(OperatorCountOkProperty); }
			set { SetValue(OperatorCountOkProperty, value); }
		}
		public static readonly DependencyProperty OperatorCountOkProperty =
			DependencyProperty.Register("OperatorCountOk", typeof(bool), typeof(PPEditorActivityChoice), new UIPropertyMetadata(true));
	}
}
