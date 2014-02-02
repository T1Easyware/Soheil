using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorActivityChoice : DependencyObject
	{
		public Model.StateStationActivity Model { get; private set; }
		public int ActivityId { get { return Model.Activity.Id; } }
		public int StateStationActivityId { get { return Model.Id; } }

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

	}
}
