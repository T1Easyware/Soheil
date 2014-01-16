﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorActivityChoice : DependencyObject
	{
		Model.StateStationActivity _model;
		public int ActivityId { get { return _model.Activity.Id; } }
		public int StateStationActivityId { get { return _model.Id; } }

		public PPEditorActivityChoice(Model.StateStationActivity model)
		{
			_model = model;
			CycleTime = _model.CycleTime;
			ManHour = _model.ManHour;
		}

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
