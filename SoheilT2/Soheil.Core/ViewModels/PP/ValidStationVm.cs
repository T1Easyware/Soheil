using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class ValidStationVm : StateStationVm
	{
		public ValidStationVm(Model.Station model)
			: base(model)
		{
			IsValid = false;
		}
		//use base.Id instead of StateStationId, to refer to StationId
		//public int StateStationId { get; set; }

		//StateStationId is not used anymore, because StateDataService Update method does not provide any update
		//to the stateStation Id of any StateConfig. so stateStations in fpc states are not provided with StateStationId
		//therefore we need to search through every Station each time we want to find a stateStation

		//IsValid Dependency Property
		public bool IsValid
		{
			get { return (bool)GetValue(IsValidProperty); }
			set { SetValue(IsValidProperty, value); }
		}
		public static readonly DependencyProperty IsValidProperty =
			DependencyProperty.Register("IsValid", typeof(bool), typeof(ValidStationVm), new UIPropertyMetadata(false));
	}
}
