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
		}
		public string Name { get { return Model.Station.Name; } }
		public string Code { get { return Model.Station.Code; } }
	}
}
