using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public class ProcessReportRowVm : DependencyObject
	{
		DataServices.ProcessReportDataService _processReportDataService;
		public ProcessReportRowVm(PPTableVm parent, ProcessVm processVm, DataServices.ProcessReportDataService processReportDataService, int index)
		{
			Parent = parent;
			Process = processVm;
			_processReportDataService = processReportDataService;
			Index = index;
		}

		public int ProcessId { get { return Process.ProcessId; } }
		public void RemoveProcessReport(ProcessReportCellVm vm)
		{
			ProcessReportCells.RemoveWhere(x => x.Id == vm.Id);
		}

		public PPTableVm Parent { get; set; }
		//Index Dependency Property
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(ProcessReportRowVm), new UIPropertyMetadata(0));
		//Process Dependency Property
		public ProcessVm Process
		{
			get { return (ProcessVm)GetValue(ProcessProperty); }
			set { SetValue(ProcessProperty, value); }
		}
		public static readonly DependencyProperty ProcessProperty =
			DependencyProperty.Register("Process", typeof(ProcessVm), typeof(ProcessReportRowVm), new UIPropertyMetadata(null));

		//ProcessReportCells Observable Collection
		private ObservableCollection<ProcessReportCellVm> _processReportCells = new ObservableCollection<ProcessReportCellVm>();
		public ObservableCollection<ProcessReportCellVm> ProcessReportCells { get { return _processReportCells; } }
	}
}
