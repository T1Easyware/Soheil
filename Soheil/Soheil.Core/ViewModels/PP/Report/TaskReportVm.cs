using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class TaskReportVm : TaskReportBaseVm
	{
		public TaskReportVm(PPTaskVm parent, Model.TaskReport model)
			: base(parent)
		{
			if (model != null)
			{
				Id = model.Id;
				TargetPoint = model.TaskReportTargetPoint;
				//(int)Math.Round(Task.TaskTargetPoint * model.ReportDurationSeconds / (double)Task.DurationSeconds)
				StartDate = model.ReportStartDateTime.Date;
				StartTime = model.ReportStartDateTime.TimeOfDay;
				EndDate = model.ReportEndDateTime.Date;
				EndTime = model.ReportEndDateTime.TimeOfDay;
				DurationSeconds = model.ReportDurationSeconds;
				ProducedG1 = model.TaskProducedG1;

				SumOfDefectionCount = TaskReportDataService.GetSumOfDefectionCounts(model.Id);
				SumOfStoppageCount = TaskReportDataService.GetSumOfStoppageCounts(model.Id);
			}
			
			CanUserEditTaskTPAndG1 = true;
			_isInInitializingPhase = false;

			initializeCommands();
		}
		protected bool _isInInitializingPhase = true;

		/// <summary>
		/// TaskReport Id
		/// </summary>
		public int Id { get; set; }


		#region Count
		//ProducedG1 Dependency Property
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(TaskReportVm),
			new PropertyMetadata(0, (d, e) => { ((TaskReportVm)d).SaveG1((int)e.NewValue); }));

		private void SaveG1(int g1)
		{
			if (_isInInitializingPhase) return;
			TaskReportDataService.UpdateG1(Id, g1);
		}
		public void SaveTargetPoint(int tp)
		{
			if (_isInInitializingPhase) return;
			TaskReportDataService.UpdateTargetPoint(Id, tp);
		}

		//G1WidthToFit Dependency Property
		public int G1WidthToFit
		{
			get { return (int)GetValue(G1WidthToFitProperty); }
			set { SetValue(G1WidthToFitProperty, value); }
		}
		public static readonly DependencyProperty G1WidthToFitProperty =
			DependencyProperty.Register("G1WidthToFit", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0));
		//Excess Dependency Property
		public int Excess
		{
			get { return (int)GetValue(ExcessProperty); }
			set { SetValue(ExcessProperty, value); }
		}
		public static readonly DependencyProperty ExcessProperty =
			DependencyProperty.Register("Excess", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0)); 
		//SumOfDefection Dependency Property (This does not include those report which aren't affecting station results)
		public int SumOfDefectionCount
		{
			get { return (int)GetValue(SumOfDefectionCountProperty); }
			set { SetValue(SumOfDefectionCountProperty, value); }
		}
		public static readonly DependencyProperty SumOfDefectionCountProperty =
			DependencyProperty.Register("SumOfDefectionCount", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0));
		//SumOfStoppage Dependency Property (This does not include those report which aren't affecting station results)
		public int SumOfStoppageCount
		{
			get { return (int)GetValue(SumOfStoppageCountProperty); }
			set { SetValue(SumOfStoppageCountProperty, value); }
		}
		public static readonly DependencyProperty SumOfStoppageCountProperty =
			DependencyProperty.Register("SumOfStoppageCount", typeof(int), typeof(TaskReportVm), new PropertyMetadata(0)); 
		#endregion

		#region Commands
		void initializeCommands()
		{
			DeleteCommand = new Commands.Command(o =>
			{
				TaskReportDataService.DeleteById(Id);
				Task.ReloadTaskReports();
			});
		}
		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
		#endregion
	}
}
