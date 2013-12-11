﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StateStationVm : DependencyObject
	{
		public StateStationVm() { }
		public StateStationVm(Model.Station model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
		}

		public int Id { get; set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ValidStationVm), new UIPropertyMetadata(""));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ValidStationVm), new UIPropertyMetadata(""));
	}
}
