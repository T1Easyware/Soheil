using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Common;

namespace Soheil.Core.Fpc
{
	public class TreeItemTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _dropIndicatorTemplate;
		private static DataTemplate DropIndicatorTemplate
		{
			get
			{
				if (_dropIndicatorTemplate == null)
					_dropIndicatorTemplate = Application.Current.FindResource("dropIndicatorTemplate") as DataTemplate;
				return _dropIndicatorTemplate;
			}
		}
		private static DataTemplate _treeItemTemplate;
		private static DataTemplate TreeItemTemplate
		{
			get
			{
				if (_treeItemTemplate == null)
					_treeItemTemplate = Application.Current.FindResource("treeItemTemplate") as DataTemplate;
				return _treeItemTemplate;
			}
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var data = item as TreeItemVm;
			if (data == null) return null;
			if (data.IsDropIndicator) return DropIndicatorTemplate;
			return TreeItemTemplate;
		}
	}

	public class TreeItemViewerTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _dropIndicatorTemplate;
		private static DataTemplate DropIndicatorTemplate
		{
			get
			{
				if (_dropIndicatorTemplate == null)
					_dropIndicatorTemplate = Application.Current.FindResource("dropIndicatorTemplate") as DataTemplate;
				return _dropIndicatorTemplate;
			}
		}
		private static DataTemplate _treeItemTemplate;
		private static DataTemplate TreeItemTemplate
		{
			get
			{
				if (_treeItemTemplate == null)
					_treeItemTemplate = Application.Current.FindResource("treeItemViewerTemplate") as DataTemplate;
				return _treeItemTemplate;
			}
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var data = item as TreeItemVm;
			if (data == null) return null;
			if (data.IsDropIndicator) return DropIndicatorTemplate;
			return TreeItemTemplate;
		}
	}

	public class StateTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _startStateTemplate;
		private static DataTemplate StartStateTemplate
		{
			get
			{
				if (_startStateTemplate == null)
					_startStateTemplate = Application.Current.FindResource("startStateTemplate") as DataTemplate;
				return _startStateTemplate;
			}
		}
		private static DataTemplate _tempStateTemplate;
		private static DataTemplate TempStateTemplate
		{
			get
			{
				if (_tempStateTemplate == null)
					_tempStateTemplate = Application.Current.FindResource("tempStateTemplate") as DataTemplate;
				return _tempStateTemplate;
			}
		}
		private static DataTemplate _midStateTemplate;
		private static DataTemplate MidStateTemplate
		{
			get
			{
				if (_midStateTemplate == null)
					_midStateTemplate = Application.Current.FindResource("midStateTemplate") as DataTemplate;
				return _midStateTemplate;
			}
		}
		private static DataTemplate _reworkStateTemplate;
		private static DataTemplate ReworkStateTemplate
		{
			get
			{
				if (_reworkStateTemplate == null)
					_reworkStateTemplate = Application.Current.FindResource("reworkStateTemplate") as DataTemplate;
				return _reworkStateTemplate;
			}
		}
		private static DataTemplate _finalStateTemplate;
		private static DataTemplate FinalStateTemplate
		{
			get
			{
				if (_finalStateTemplate == null)
					_finalStateTemplate = Application.Current.FindResource("finalStateTemplate") as DataTemplate;
				return _finalStateTemplate;
			}
		}
		private static DataTemplate _stateSubItemTemplate;
		private static DataTemplate StateSubItemTemplate
		{
			get
			{
				if (_stateSubItemTemplate == null)
					_stateSubItemTemplate = Application.Current.FindResource("stateSubItemTemplate") as DataTemplate;
				return _stateSubItemTemplate;
			}
		}
		private static DataTemplate _stationSubItemTemplate;
		private static DataTemplate StationSubItemTemplate
		{
			get
			{
				if (_stationSubItemTemplate == null)
					_stationSubItemTemplate = Application.Current.FindResource("stationSubItemTemplate") as DataTemplate;
				return _stationSubItemTemplate;
			}
		}
		private static DataTemplate _activitySubItemTemplate;
		private static DataTemplate ActivitySubItemTemplate
		{
			get
			{
				if (_activitySubItemTemplate == null)
					_activitySubItemTemplate = Application.Current.FindResource("activitySubItemTemplate") as DataTemplate;
				return _activitySubItemTemplate;
			}
		}
		private static DataTemplate _machineSubItemTemplate;
		private static DataTemplate MachineSubItemTemplate
		{
			get
			{
				if (_machineSubItemTemplate == null)
					_machineSubItemTemplate = Application.Current.FindResource("machineSubItemTemplate") as DataTemplate;
				return _machineSubItemTemplate;
			}
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return null;
			if (item is StateVm)
			{
				var state = item as StateVm;
				switch (state.StateType)
				{
					case StateType.Start:
						return StartStateTemplate;
					case StateType.Mid:
						return MidStateTemplate;
					case StateType.Final:
						return FinalStateTemplate;
					case StateType.Rework:
						return ReworkStateTemplate;
					case StateType.Temp:
						return TempStateTemplate;
					default:
						return null;
				}
			}
			if (item is TreeItemVm)
			{
				if (item is StateConfigVm) return StateSubItemTemplate;
				if (item is StateStationVm) return StationSubItemTemplate;
				if (item is StateStationActivityVm) return ActivitySubItemTemplate;
				if (item is StateStationActivityMachineVm) return MachineSubItemTemplate;
			}
			return null;
		}
	}
	public class StateViewerTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _startStateTemplate;
		private static DataTemplate StartStateTemplate
		{
			get
			{
				if (_startStateTemplate == null)
					_startStateTemplate = Application.Current.FindResource("startStateTemplate") as DataTemplate;
				return _startStateTemplate;
			}
		}
		private static DataTemplate _tempStateTemplate;
		private static DataTemplate TempStateTemplate
		{
			get
			{
				if (_tempStateTemplate == null)
					_tempStateTemplate = Application.Current.FindResource("tempStateTemplate") as DataTemplate;
				return _tempStateTemplate;
			}
		}
		private static DataTemplate _midStateTemplate;
		private static DataTemplate MidStateTemplate
		{
			get
			{
				if (_midStateTemplate == null)
					_midStateTemplate = Application.Current.FindResource("midStateViewerTemplate") as DataTemplate;
				return _midStateTemplate;
			}
		}
		private static DataTemplate _reworkStateTemplate;
		private static DataTemplate ReworkStateTemplate
		{
			get
			{
				if (_reworkStateTemplate == null)
					_reworkStateTemplate = Application.Current.FindResource("reworkStateTemplate") as DataTemplate;
				return _reworkStateTemplate;
			}
		}
		private static DataTemplate _finalStateTemplate;
		private static DataTemplate FinalStateTemplate
		{
			get
			{
				if (_finalStateTemplate == null)
					_finalStateTemplate = Application.Current.FindResource("finalStateTemplate") as DataTemplate;
				return _finalStateTemplate;
			}
		}
		private static DataTemplate _stateSubItemTemplate;
		private static DataTemplate StateSubItemTemplate
		{
			get
			{
				if (_stateSubItemTemplate == null)
					_stateSubItemTemplate = Application.Current.FindResource("stateSubItemTemplate") as DataTemplate;
				return _stateSubItemTemplate;
			}
		}
		private static DataTemplate _stationSubItemTemplate;
		private static DataTemplate StationSubItemTemplate
		{
			get
			{
				if (_stationSubItemTemplate == null)
					_stationSubItemTemplate = Application.Current.FindResource("stationSubItemTemplate") as DataTemplate;
				return _stationSubItemTemplate;
			}
		}
		private static DataTemplate _activitySubItemTemplate;
		private static DataTemplate ActivitySubItemTemplate
		{
			get
			{
				if (_activitySubItemTemplate == null)
					_activitySubItemTemplate = Application.Current.FindResource("activitySubItemTemplate") as DataTemplate;
				return _activitySubItemTemplate;
			}
		}
		private static DataTemplate _machineSubItemTemplate;
		private static DataTemplate MachineSubItemTemplate
		{
			get
			{
				if (_machineSubItemTemplate == null)
					_machineSubItemTemplate = Application.Current.FindResource("machineSubItemTemplate") as DataTemplate;
				return _machineSubItemTemplate;
			}
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return null;
			if (item is StateVm)
			{
				var state = item as StateVm;
				switch (state.StateType)
				{
					case StateType.Start:
						return StartStateTemplate;
					case StateType.Mid:
						return MidStateTemplate;
					case StateType.Final:
						return FinalStateTemplate;
					case StateType.Rework:
						return ReworkStateTemplate;
					case StateType.Temp:
						return TempStateTemplate;
					default:
						return null;
				}
			}
			if (item is TreeItemVm)
			{
				if (item is StateConfigVm) return StateSubItemTemplate;
				if (item is StateStationVm) return StationSubItemTemplate;
				if (item is StateStationActivityVm) return ActivitySubItemTemplate;
				if (item is StateStationActivityMachineVm) return MachineSubItemTemplate;
			}
			return null;
		}
	}
}
