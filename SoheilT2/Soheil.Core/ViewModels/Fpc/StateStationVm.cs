using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateStationVm : TreeItemVm
	{
		public StateStationVm(FpcWindowVm parentWindowVm)
			: base(parentWindowVm)
		{
			TreeLevel = 1;
		}
		public StateConfigVm ContainerS { get { return (StateConfigVm)base.Container; } set { base.Container = value; } }
		public StationVm ContainmentStation { get { return (StationVm)base.Containment; } set { base.Containment = value; } }

		public override void Change()
		{
			ContainerS.State.IsChanged = true;
		}
	}
}
