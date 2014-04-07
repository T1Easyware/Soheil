using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class BlockReportVm : DependencyObject
	{
		public DataServices.ProcessReportDataService ProcessReportDataService { get { return Block.Parent.PPTable.ProcessReportDataService; } }
		public DataServices.TaskDataService TaskDataService { get { return Block.Parent.PPTable.TaskDataService; } }
		public BlockReportVm(BlockVm block)
		{
			Block = block;
			ReloadProcessReportRows();
		}
		//Block Dependency Property
		public BlockVm Block
		{
			get { return (BlockVm)GetValue(BlockProperty); }
			set { SetValue(BlockProperty, value); }
		}
		public static readonly DependencyProperty BlockProperty =
			DependencyProperty.Register("Block", typeof(BlockVm), typeof(BlockReportVm), new UIPropertyMetadata(null));

		//SSAList Observable Collection
		public ObservableCollection<StateStationActivityVm> SSAList { get { return _ssaList; } }
		private ObservableCollection<StateStationActivityVm> _ssaList = new ObservableCollection<StateStationActivityVm>();

		//ProcessReportRows Observable Collection
		public ObservableCollection<ProcessReportRowVm> ProcessReportRows { get { return _processReportRows; } }
		private ObservableCollection<ProcessReportRowVm> _processReportRows = new ObservableCollection<ProcessReportRowVm>();

		//TaskReports Observable Collection
		public ObservableCollection<TaskReportBaseVm> TaskReports { get { return _taskReport; } }
		private ObservableCollection<TaskReportBaseVm> _taskReport = new ObservableCollection<TaskReportBaseVm>();

		public void ReloadProcessReportRows()
		{
			TaskReports.Clear();
			SSAList.Clear();
			ProcessReportRows.Clear();
			int index = 0;

			var ssaModels = new List<Model.StateStationActivity>();
			foreach (var task in Block.TaskList.OrderBy(x => x.StartDateTime))
			{
				//SSAList (add ssas from processes of each task)
				ssaModels.AddRange(TaskDataService.GetProcesses(task.Id).Select(x => x.StateStationActivity));

				//TaskReports
				foreach (var taskReport in task.TaskReports.OrderBy(x => x.StartDateTime))
				{
					TaskReports.Add(taskReport);
				}
			}

			//removes duplicate SSAs from ssaModels
			ssaModels = ssaModels.DistinctBy(x => x.Id).OrderBy(x => x.Activity.Id).ThenBy(x => x.ManHour).ToList();

			foreach (var ssa in ssaModels)
			{
				//SSAList (finally add ssaVm to SSAList)
				var ssaVm = new StateStationActivityVm(ssa);
				SSAList.Add(ssaVm);
				//ProcessReportRows
				var processReportRow = new ProcessReportRowVm(Block.Parent.PPTable,  ssaVm, index);
				index++;
				ProcessReportRows.Add(processReportRow);
			}

			//ProcessReportRows
			foreach (var taskReport in TaskReports)
			{
				if (taskReport is TaskReportVm)
				{
					var processReportModels = ProcessReportDataService.GetProcessReports((taskReport as TaskReportVm).Task.Id);
                    foreach (var processReportModel in processReportModels)
                    {
                        var row = ProcessReportRows.First(x => x.StateStationActivity.Id == processReportModel.Process.StateStationActivity.Id);
                        row.ProcessReportCells.Add(new ProcessReportCellVm(processReportModel, taskReport, row));
                    }
				}
				else//is holder
				{
					var processModels = TaskDataService.GetProcesses(taskReport.Task.Id);
                    foreach (var processModel in processModels)
                    {
                        var row = ProcessReportRows.First(x => x.StateStationActivity.Id == processModel.StateStationActivity.Id);
                        row.ProcessReportCells.Add(new ProcessReportCellVm(null, taskReport, row));
                    }
				}
			}
		}
	}
}
