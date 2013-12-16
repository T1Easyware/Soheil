using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Soheil.Core.ViewModels.Fpc;

namespace Soheil.Core.Fpc
{
	public class ToolboxItemTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _stationToolboxItemDataTemplate;
		private static DataTemplate StationToolboxItemDataTemplate
		{
			get
			{
				if (_stationToolboxItemDataTemplate == null)
					_stationToolboxItemDataTemplate = Application.Current.FindResource("stationToolboxItemTemplate") as DataTemplate;
				return _stationToolboxItemDataTemplate;
			}
		}
		private static DataTemplate _activityToolboxItemDataTemplate;
		private static DataTemplate ActivityToolboxItemDataTemplate
		{
			get
			{
				if (_activityToolboxItemDataTemplate == null)
					_activityToolboxItemDataTemplate = Application.Current.FindResource("activityToolboxItemTemplate") as DataTemplate;
				return _activityToolboxItemDataTemplate;
			}
		}
		private static DataTemplate _machineToolboxItemDataTemplate;
		private static DataTemplate MachineToolboxItemDataTemplate
		{
			get
			{
				if (_machineToolboxItemDataTemplate == null)
					_machineToolboxItemDataTemplate = Application.Current.FindResource("machineToolboxItemTemplate") as DataTemplate;
				return _machineToolboxItemDataTemplate;
			}
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return null;
			if (item is StationVm) return StationToolboxItemDataTemplate;
			if (item is ActivityVm) return ActivityToolboxItemDataTemplate;
			if (item is MachineVm) return MachineToolboxItemDataTemplate;
			return null;
		}
	}
}
