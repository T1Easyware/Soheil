using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StateStationVm : DependencyObject
	{
		public Model.StateStation Model { get; protected set; }
		public int StationId { get { return Model.Station.Id; } }
		public int StateStationId { get { return Model.Id; } }
		public StateStationVm(Model.StateStation model)
		{
			Model = model;
			Name = model.Station.Name;
			Code = model.Station.Code;
		}
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(StateStationVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(StateStationVm), new UIPropertyMetadata(null));
	}
}
