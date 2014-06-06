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
	/// <summary>
	/// ViewModel for all process reports in a Block
	/// </summary>
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
			ReloadReports();
		}
		/// <summary>
		/// Gets or sets the bindable parent BlockVm
		/// </summary>
		public BlockVm Block
		{
			get { return (BlockVm)GetValue(BlockProperty); }
			set { SetValue(BlockProperty, value); }
		}
		public static readonly DependencyProperty BlockProperty =
			DependencyProperty.Register("Block", typeof(BlockVm), typeof(BlockReportVm), new UIPropertyMetadata(null));

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
			ActivityList.Clear();

			foreach (var ssaGroup in Block.Model.StateStation.StateStationActivities.GroupBy(x => x.Activity))
			{
				var activityVm = new ActivityRowVm(ssaGroup.Key);
				ActivityList.Add(activityVm);
			}
			var ssaModels = new List<Model.StateStationActivity>();
			foreach (var task in Block.TaskList.OrderBy(x => x.StartDateTime))
			{
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
						var rowVm = activityVm.SsaRowList.FirstOrDefault(x => x.Id == process.StateStationActivity.Id);
						if (rowVm == null) continue;
						//create processVm
						var processVm = new ProcessVm(process, UOW);
						processVm.LayoutChanged += ReloadReports;
						rowVm.ProcessList.Add(processVm);
						//load process reports
						foreach (var processReport in process.ProcessReports.OrderBy(x => x.StartDateTime))
						{
							var processReportVm = new ProcessReportVm(processReport);
							//process report events
							processReportVm.LayoutChanged += ReloadReports;
							processReportVm.ProcessReportSelected += vm =>
							{
								Block.Parent.PPTable.CurrentProcessReportBuilder = vm;
							};

							//correct next/previous links
							processReportVm.PreviousReport = processVm.ProcessReportList.LastOrDefault();
							if (processReportVm.PreviousReport != null)
								processReportVm.PreviousReport.NextReport = processReportVm;

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
