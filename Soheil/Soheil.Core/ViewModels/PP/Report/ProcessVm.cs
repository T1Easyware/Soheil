using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ProcessVm : PPItemVm
	{
		public Model.Process Model { get; protected set; }
		public override int Id
		{
			get { return Model.Id; }
		}
		public ProcessVm(Model.Process model)
		{
			Model = model;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;

			FillEmptySpacesCommand = new Commands.Command(o =>
			{
				//check for remaining
				var remDuration = Model.DurationSeconds;
				var remTP = Model.TargetCount;

				//init remainings
				if (Model.ProcessReports.Any())
				{
					remTP -= Model.ProcessReports.Sum(x => x.ProcessReportTargetPoint);
					remDuration -= Model.ProcessReports.Sum(x => x.DurationSeconds);
				}

				//fill spaces between reports
				var dt = Model.StartDateTime;
				foreach (var processReport in Model.ProcessReports.ToArray())
				{
					//add one before
					if (processReport.StartDateTime - dt > TimeSpan.FromSeconds(Model.StateStationActivity.CycleTime))
					{
						//calculate duration and tp
						var dur = (int)(processReport.StartDateTime - dt).TotalSeconds;
						var tp = (int)(remTP * dur / remDuration);
						dur = tp * (int)Model.StateStationActivity.CycleTime;

						//create processReport
						var processReportModel = new Model.ProcessReport
						{
							Process = Model,
							StartDateTime = dt,
							EndDateTime = dt.AddSeconds(dur),
							DurationSeconds = dur,
							ProcessReportTargetPoint = tp,
							Code = Model.Code,
							ModifiedBy = LoginInfo.Id,
						};

						//fix remainings
						remDuration -= dur;
						remTP -= tp;

						//create process operators
						foreach (var po in Model.ProcessOperators)
						{
							processReportModel.ProcessOperatorReports.Add(new Model.ProcessOperatorReport{
								ProcessReport = processReportModel,
								ProcessOperator = po,
								OperatorProducedG1 = tp / Model.ProcessOperators.Count,
							});
						}

						//add to processReports
						Model.ProcessReports.Add(processReportModel);
					}
					dt = processReport.EndDateTime;
				}

				//add one at the end
				if (Model.EndDateTime - dt > TimeSpan.FromSeconds(Model.StateStationActivity.CycleTime))
				{
					//calculate duration and tp
					var dur = (int)(Model.EndDateTime - dt).TotalSeconds;
					var tp = (int)(remTP * dur / remDuration);
					dur = tp * (int)Model.StateStationActivity.CycleTime;

					//create processReport
					var newModel = new Model.ProcessReport
					{
						Process = Model,
						Code = Model.Code,
						StartDateTime = dt,
						EndDateTime = dt.AddSeconds(dur),
						DurationSeconds = dur,
						ProcessReportTargetPoint = tp,
						ProducedG1 = 0,
						ModifiedBy = LoginInfo.Id,
					};

					//create process operators
					foreach (var po in Model.ProcessOperators)
					{
						newModel.ProcessOperatorReports.Add(new Model.ProcessOperatorReport
						{
							ProcessReport = newModel,
							ProcessOperator = po,
							OperatorProducedG1 = tp / Model.ProcessOperators.Count,
							ModifiedBy = LoginInfo.Id,
						});
					}

					//add to processReports
					Model.ProcessReports.Add(newModel);
				}
			});
		}

		//ProcessReportList Observable Collection
		public ObservableCollection<ProcessReportVm> ProcessReportList { get { return _processReportList; } }
		private ObservableCollection<ProcessReportVm> _processReportList = new ObservableCollection<ProcessReportVm>();

		//FillEmptySpacesCommand Dependency Property
		public Commands.Command FillEmptySpacesCommand
		{
			get { return (Commands.Command)GetValue(FillEmptySpacesCommandProperty); }
			set { SetValue(FillEmptySpacesCommandProperty, value); }
		}
		public static readonly DependencyProperty FillEmptySpacesCommandProperty =
			DependencyProperty.Register("FillEmptySpacesCommand", typeof(Commands.Command), typeof(ProcessVm), new UIPropertyMetadata(null));
	}
}
