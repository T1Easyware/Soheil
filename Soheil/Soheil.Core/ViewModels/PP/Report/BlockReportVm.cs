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
		public Dal.SoheilEdmContext UOW { get; protected set; }
		DataServices.TaskDataService TaskDataService;
		DataServices.ProcessReportDataService ProcessReportDataService;

		/// <summary>
		/// Creates a report for the given block, fills all process reports
		/// </summary>
		/// <param name="block"></param>
		public BlockReportVm(BlockVm block)
		{
			Block = block;
			UOW = block.UOW;
			TaskDataService = new DataServices.TaskDataService(UOW);
			ProcessReportDataService = new DataServices.ProcessReportDataService(UOW);
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

		//ProcessReportRows Observable Collection
		public ObservableCollection<ActivityRowVm> ActivityList { get { return _activityList; } }
		private ObservableCollection<ActivityRowVm> _activityList = new ObservableCollection<ActivityRowVm>();

		//TaskReports Observable Collection
		public ObservableCollection<TaskReportVm> TaskReports { get { return _taskReport; } }
		private ObservableCollection<TaskReportVm> _taskReport = new ObservableCollection<TaskReportVm>();

		/// <summary>
		/// Reloads all activities, ssas and processReports for this block
		/// </summary>
		public void ReloadProcessReportRows()
		{
			TaskReports.Clear();
			ActivityList.Clear();

			foreach (var ssaGroup in Block.Model.StateStation.StateStationActivities.GroupBy(x => x.Activity))
			{
				var activityVm = new ActivityRowVm(ssaGroup.Key);
				ActivityList.Add(activityVm);
			}
			var ssaModels = new List<Model.StateStationActivity>();
			foreach (var task in Block.TaskList.OrderBy(x => x.StartDateTime))
			{
				//load TaskReports
				foreach (var taskReport in task.TaskReports.OrderBy(x => x.StartDateTime))
				{
					TaskReports.Add(taskReport);
				}

				//load ProcessReports
				foreach (var processGroup in task.Model.Processes.GroupBy(x=>x.StateStationActivity.Activity))
				{
					//find activity
					var activityVm = ActivityList.FirstOrDefault(x => x.Id == processGroup.Key.Id);
					if (activityVm == null) continue;
					//load processes
					foreach (var process in processGroup)
					{
						//find ssa (row)
						var rowVm = activityVm.ProcessRowList.FirstOrDefault(x => x.Id == process.StateStationActivity.Id);
						if (rowVm == null) continue;
						//create processVm
						var processVm = new ProcessVm(process);
						rowVm.ProcessList.Add(processVm);
						//load process reports
						foreach (var processReport in process.ProcessReports.OrderBy(x => x.StartDateTime))
						{
							var processReportVm = new ProcessReportVm(processReport, rowVm);
							//process report events
							processReportVm.Refresh += ReloadProcessReportRows;
							processReportVm.ProcessReportSelected += vm =>
							{
								Block.Parent.PPTable.CurrentProcessReportBuilder = vm;
							};

							processVm.ProcessReportList.Add(processReportVm);
						}

						//put processes in order
						rowVm.RearrangeRows();
					}
				}
			}
		}
	}
}
