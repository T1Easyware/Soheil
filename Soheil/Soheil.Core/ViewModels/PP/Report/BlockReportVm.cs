﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Report
{
	/// <summary>
	/// ViewModel for all process reports in a Block
	/// </summary>
	public class BlockReportVm : DependencyObject
	{
		public event Action<ProcessReportVm> ProcessReportBuilderChanged;

		public Dal.SoheilEdmContext UOW { get; protected set; }
		DataServices.TaskDataService TaskDataService;
		DataServices.ProcessReportDataService ProcessReportDataService;
		Model.Block entity;
		BlockVm _parent;

		/// <summary>
		/// Creates a report for the given block, fills all process reports
		/// </summary>
		/// <param name="block"></param>
		public BlockReportVm(BlockVm vm)
		{
			_parent = vm;
			UOW = new Dal.SoheilEdmContext();
			TaskDataService = new DataServices.TaskDataService(UOW);
			ProcessReportDataService = new DataServices.ProcessReportDataService(UOW);
			entity = new DataServices.BlockDataService(UOW).GetSingle(vm.Id);

			ReloadReports();
		}

		/// <summary>
		/// Gets the bindable collection of ActivityRowVms 
		/// each of which contains a collection of SsaRowVm 
		/// each of which contains a collection of ProcessVm 
		/// each of which contains a collection of ProcessReportVm
		/// </summary>
		public ObservableCollection<ActivityRowVm> ActivityList { get { return _activityList; } }
		private ObservableCollection<ActivityRowVm> _activityList = new ObservableCollection<ActivityRowVm>();

		/// <summary>
		/// Reloads all activities, ssas and processReports for this block
		/// </summary>
		public void ReloadReports()
		{
			_parent.ReloadTasks();
			if (_parent.TaskList.Count > 0)
				_parent.TaskList[0].ReloadTaskReports();

			ActivityList.Clear();
			
			foreach (var ssaGroup in entity.StateStation.StateStationActivities.GroupBy(x => x.Activity))
			{
				var activityVm = new ActivityRowVm(ssaGroup.Key);
				ActivityList.Add(activityVm);
			}

			var ssaModels = new List<Model.StateStationActivity>();
			foreach (var task in entity.Tasks.OrderBy(x => x.StartDateTime))
			{
				//load ProcessReports
				foreach (var processGroup in task.Processes.GroupBy(x=>x.StateStationActivity.Activity))
				{
					//find activity
					var activityVm = ActivityList.FirstOrDefault(x => x.Id == processGroup.Key.Id);
					if (activityVm == null) continue;
					//load processes
					foreach (var process in processGroup)
					{
						//find ssa (row)
						var rowVm = activityVm.SsaRowList.FirstOrDefault(x => x.Id == process.StateStationActivity.Id);
						if (rowVm == null) continue;
						//create processVm
						var processVm = new ProcessVm(process, UOW);
						processVm.LayoutChanged += ReloadReports;
						rowVm.ProcessList.Add(processVm);
						//load process reports
						foreach (var processReport in process.ProcessReports.OrderBy(x => x.StartDateTime))
						{
							var processReportVm = new ProcessReportVm(processReport, UOW);
							//process report events
							processReportVm.LayoutChanged += ReloadReports;
							processReportVm.ProcessReportSelected += vm =>
							{
								if (ProcessReportBuilderChanged != null)
									ProcessReportBuilderChanged(vm);
							};

							//correct next/previous links
							var lastpr = processVm.ProcessReportList.LastOrDefault();
							if (lastpr != null)
							{
								processReportVm.Timing.PreviousReport = lastpr.Timing;
								processReportVm.Timing.PreviousReport.NextReport = processReportVm.Timing;
							}

							//add the report to its processVm
							processVm.ProcessReportList.Add(processReportVm);
						}
					}
				}

				//put processes in order
				foreach (var activityVm in ActivityList)
				{
					foreach (var rowVm in activityVm.SsaRowList)
					{
						rowVm.RearrangeRows();
					}
				}
			}
		}
	}
}
