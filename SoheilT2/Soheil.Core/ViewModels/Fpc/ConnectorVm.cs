using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ConnectorVm : DragTarget
	{
		public ConnectorVm()
		{
		}
		public ConnectorVm(StateVm start, StateVm end, FpcVm fpc)
		{
			Start = start;
			End = end;
			FPC = fpc;
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
		//FPC Dependency Property
		public FpcVm FPC
		{
			get { return (FpcVm)GetValue(FPCProperty); }
			set { SetValue(FPCProperty, value); }
		}
		public static readonly DependencyProperty FPCProperty =
			DependencyProperty.Register("FPC", typeof(FpcVm), typeof(ConnectorVm), new UIPropertyMetadata(null));
		//IsLoose Dependency Property
		public bool IsLoose
		{
			get { return (bool)GetValue(IsLooseProperty); }
			set { SetValue(IsLooseProperty, value); }
		}
		public static readonly DependencyProperty IsLooseProperty =
			DependencyProperty.Register("IsLoose", typeof(bool), typeof(ConnectorVm), new UIPropertyMetadata(false));
	}
}
