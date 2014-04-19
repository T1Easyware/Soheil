using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ConnectorVm : DragTarget
	{
		public Model.Connector Model { get; protected set; }
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		public event Action ConnectorRemoved;

		public ConnectorVm(Model.Connector model, StateVm start, StateVm end, DataServices.ConnectorDataService connectorDataService, bool isLoose = false)
		{
			Model = model;
			Start = start;
			End = end;
			IsLoose = isLoose;
			DeleteCommand = new Commands.Command(o =>
			{
				if(Model != null)
					connectorDataService.DeleteModel(Model);
				if (ConnectorRemoved != null) 
					ConnectorRemoved();
			});
		}
		//Start Dependency Property
		public StateVm Start
		{
			get { return (StateVm)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}
		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(StateVm), typeof(ConnectorVm), new UIPropertyMetadata(null));
		//End Dependency Property
		public StateVm End
		{
			get { return (StateVm)GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}
		public static readonly DependencyProperty EndProperty =
			DependencyProperty.Register("End", typeof(StateVm), typeof(ConnectorVm), new UIPropertyMetadata(null));
		//IsLoose Dependency Property
		public bool IsLoose
		{
			get { return (bool)GetValue(IsLooseProperty); }
			set { SetValue(IsLooseProperty, value); }
		}
		public static readonly DependencyProperty IsLooseProperty =
			DependencyProperty.Register("IsLoose", typeof(bool), typeof(ConnectorVm), new UIPropertyMetadata(false));

		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(ConnectorVm), new UIPropertyMetadata(null));
	}
}
