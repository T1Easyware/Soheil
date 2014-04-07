using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ProcessReportRowVm : DependencyObject
	{
		public ProcessReportRowVm(PPTableVm parent, StateStationActivityVm ssa, int index)
		{
			Parent = parent;
			Index = index;
			StateStationActivity = ssa;
		}

		public void RemoveProcessReport(ProcessReportCellVm vm)
		{
			ProcessReportCells.RemoveWhere(x => x.Id == vm.Id);
		}

		public PPTableVm Parent { get; private set; }
		//Index Dependency Property
		public int Index { get; private set; }

		//StateStationActivity Dependency Property
		public StateStationActivityVm StateStationActivity
		{
			get { return (StateStationActivityVm)GetValue(StateStationActivityProperty); }
			set { SetValue(StateStationActivityProperty, value); }
		}
		public static readonly DependencyProperty StateStationActivityProperty =
			DependencyProperty.Register("StateStationActivity", typeof(StateStationActivityVm), typeof(ProcessReportRowVm), new UIPropertyMetadata(null));

		//ProcessReportCells Observable Collection
		public ObservableCollection<ProcessReportCellVm> ProcessReportCells { get { return _processReportCells; } }
		private ObservableCollection<ProcessReportCellVm> _processReportCells = new ObservableCollection<ProcessReportCellVm>();
	}
}
