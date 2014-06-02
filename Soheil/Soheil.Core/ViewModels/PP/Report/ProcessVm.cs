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
		public event Action LayoutChanged;
		public Model.Process Model { get; protected set; }
		public override int Id
		{
			get { return Model.Id; }
		}
		/// <summary>
		/// Creates an instance of <see cref="ProcessVm"/> for the given model
		/// </summary>
		/// <param name="model"></param>
		public ProcessVm(Model.Process model)
		{
			Model = model;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
			TargetPoint = model.TargetCount;

			initializeCommands();
		}
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(ProcessVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets a bindable collection of process reports for this process
		/// </summary>
		public ObservableCollection<ProcessReportVm> ProcessReportList { get { return _processReportList; } }
		private ObservableCollection<ProcessReportVm> _processReportList = new ObservableCollection<ProcessReportVm>();

		void initializeCommands()
		{
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
							processReportModel.OperatorProcessReports.Add(new Model.OperatorProcessReport
							{
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
						newModel.OperatorProcessReports.Add(new Model.OperatorProcessReport
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

				if (LayoutChanged != null)
					LayoutChanged();
			});
		}

		/// <summary>
		/// Gets or sets the bindable command to fill empty (unreported) spaces in this process with process reports
		/// </summary>
		public Commands.Command FillEmptySpacesCommand
		{
			get { return (Commands.Command)GetValue(FillEmptySpacesCommandProperty); }
			set { SetValue(FillEmptySpacesCommandProperty, value); }
		}
		public static readonly DependencyProperty FillEmptySpacesCommandProperty =
			DependencyProperty.Register("FillEmptySpacesCommand", typeof(Commands.Command), typeof(ProcessVm), new UIPropertyMetadata(null));

	}
}
