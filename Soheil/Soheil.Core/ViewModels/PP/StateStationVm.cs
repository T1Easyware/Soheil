using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StateStationVm : DependencyObject
	{
		public StateStationVm(Model.StateStation model)
		{
			StateStationId = model.Id;
			StationId = model.Station.Id;
			Name = model.Station.Name;
			Code = model.Station.Code;
		}
		public StateStationVm(Model.Station model)
		{
			StationId = model.Id;
			Name = model.Name;
			Code = model.Code;
		}

		public int StationId { get; protected set; }
		public int StateStationId { get; protected set; }
		//Name of Station
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ValidStationVm), new UIPropertyMetadata(""));
		//Code of Station
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ValidStationVm), new UIPropertyMetadata(""));
	}
}
