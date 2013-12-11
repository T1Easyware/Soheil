using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	public class StateConfigVm : TreeItemVm
	{
		public StateConfigVm(StateVm state, FpcWindowVm parentWindowVm)
			: base(parentWindowVm)
		{
			State = state;
			TreeLevel = 0;
			IsExpanded = true;
			Id = state.Id;
		}
		//State Dependency Property
		public StateVm State
		{
			get { return (StateVm)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(StateVm), typeof(StateConfigVm), new UIPropertyMetadata(null, (d, e) =>
						d.SetValue(NameProperty, ((StateVm)e.NewValue).Name)));

		public override void Change()
		{
			State.IsChanged = true;
		}
	}
}
