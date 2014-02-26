using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	public class ProcessReportCellVm : PPItemVm
	{
		DataServices.ProcessReportDataService _processReportDataService;
		DataServices.TaskReportDataService _taskReportDataService;
		Model.ProcessReport _model;
		public override int Id { get { return _model == null ? 0 : _model.Id; } }
		public int ProcessId { get { return _model.Process.Id; } }
		public ProcessReportRowVm ParentRow { get; private set; }
		public TaskReportBaseVm ParentColumn { get; private set; }


		#region Ctor
		/// <summary>
		/// Creates a ViewModel for the given ProcessReport with given row and column
		/// </summary>
		/// <param name="model">if null, it automatically assign unreported process space</param>
		/// <param name="taskReport">column of the viewModel cell within the report grid</param>
		/// <param name="processReportRow">row of the viewModel cell within the report grid</param>
		public ProcessReportCellVm(Model.ProcessReport model, TaskReportBaseVm taskReport, ProcessReportRowVm processReportRow)
		{
			ParentRow = processReportRow;
			ParentColumn = taskReport;
			_model = model;
			_processReportDataService = processReportRow.Parent.ProcessReportDataService;
			_taskReportDataService = processReportRow.Parent.TaskReportDataService;

			if (model == null)
			{
                ProcessReportTargetPoint = 0;
                    //- processReportRow.ProcessReportCells.Where(y => y.Id > 0).Sum(x => x.ProcessReportTargetPoint);
			}
			else
			{
				ProducedG1 = model.ProducedG1;
				ProcessReportTargetPoint = model.ProcessReportTargetPoint;
			}

			StartDateTime = taskReport.StartDateTime;

			var tmp = (int)(ProcessReportTargetPoint * processReportRow.StateStationActivity.CycleTime);
			if (tmp == 0 || tmp > taskReport.DurationSeconds)
				DurationSeconds = taskReport.DurationSeconds;
			else
				DurationSeconds = tmp;
	
			DefectionReports = new DefectionReportCollection(this);
			StoppageReports = new StoppageReportCollection(this);

			initializeCommands();
		}

		#endregion

		public void LoadInnerData()
		{
			_model = _processReportDataService.GetSingleFull(Id);
			DefectionReports.Reset();
			foreach (var def in _model.DefectionReports)
			{
				DefectionReports.List.Add(new DefectionReportVm(DefectionReports, def));
			}
			StoppageReports.Reset();
			foreach (var stp in _model.StoppageReports)
			{
				StoppageReports.List.Add(new StoppageReportVm(StoppageReports, stp));
			}
		}
		public void Save()
		{
			_processReportDataService.Save(this);
		}


		#region Count
		//ProcessTargetPoint Dependency Property
		public int ProcessReportTargetPoint
		{
			get { return (int)GetValue(ProcessReportTargetPointProperty); }
			set { SetValue(ProcessReportTargetPointProperty, value); }
		}
		public static readonly DependencyProperty ProcessReportTargetPointProperty =
			DependencyProperty.Register("ProcessReportTargetPoint", typeof(int), typeof(ProcessReportCellVm), new UIPropertyMetadata(0));
		//ProducedG1 Dependency Property
		public int ProducedG1
		{
			get { return (int)GetValue(ProducedG1Property); }
			set { SetValue(ProducedG1Property, value); }
		}
		public static readonly DependencyProperty ProducedG1Property =
			DependencyProperty.Register("ProducedG1", typeof(int), typeof(ProcessReportCellVm), new UIPropertyMetadata(0));
		//the following 2 props are different than DefectionReports & StoppageReports
		//they show the overall equivalent for the progressbar view, but the 2 lists show every detail
		//DefectionCount Dependency Property
		public int DefectionCount
		{
			get { return (int)GetValue(DefectionCountProperty); }
			set { SetValue(DefectionCountProperty, value); }
		}
		public static readonly DependencyProperty DefectionCountProperty =
			DependencyProperty.Register("DefectionCount", typeof(int), typeof(ProcessReportCellVm), new UIPropertyMetadata(0));
		//StoppageCount Dependency Property
		public int StoppageCount
		{
			get { return (int)GetValue(StoppageCountProperty); }
			set { SetValue(StoppageCountProperty, value); }
		}
		public static readonly DependencyProperty StoppageCountProperty =
			DependencyProperty.Register("StoppageCount", typeof(int), typeof(ProcessReportCellVm), new UIPropertyMetadata(0));
		#endregion

		#region Startdt
		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(ProcessReportCellVm), new UIPropertyMetadata(DateTime.Now));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(ProcessReportCellVm), new UIPropertyMetadata(TimeSpan.Zero));
		public override DateTime StartDateTime
		{
			get { return StartDate.Add(StartTime); }
			set { StartDate = value.Date; StartTime = value.TimeOfDay; SetValue(StartDateTimeProperty, value); }
		}
		#endregion

		#region Other Members
		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProcessReportCellVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (ProcessReportCellVm)d;
				if ((bool)e.NewValue)
				{
					vm.ParentRow.Parent.CurrentProcessReportBuilder = vm;
					vm.LoadInnerData();
				}
				else
					vm.ParentRow.Parent.CurrentProcessReportBuilder = null;
			}));
		//DefectionReports Dependency Property
		public DefectionReportCollection DefectionReports
		{
			get { return (DefectionReportCollection)GetValue(DefectionReportsProperty); }
			set { SetValue(DefectionReportsProperty, value); }
		}
		public static readonly DependencyProperty DefectionReportsProperty =
			DependencyProperty.Register("DefectionReports", typeof(DefectionReportCollection), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		//StoppageReports Dependency Property
		public StoppageReportCollection StoppageReports
		{
			get { return (StoppageReportCollection)GetValue(StoppageReportsProperty); }
			set { SetValue(StoppageReportsProperty, value); }
		}
		public static readonly DependencyProperty StoppageReportsProperty =
			DependencyProperty.Register("StoppageReports", typeof(StoppageReportCollection), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));

		#endregion

		#region Commands
		void initializeCommands()
		{
			OpenCommand = new Commands.Command(o =>
			{
				if (ViewMode == PPViewMode.Simple) IsSelected = true;
				else if(ViewMode == PPViewMode.Empty)
				{
					//create task report column???
				}
			});
			CloseCommand = new Commands.Command(o => { IsSelected = false; });
			SaveCommand = new Commands.Command(o =>
			{
				Save();
				IsSelected = false;
				ParentRow.Parent.SelectedBlock.BlockReport.ReloadProcessReportRows();
			});
			DeleteTaskReportCommand = new Commands.Command(o =>
			{
				try
				{
					if (_model != null)
					{
						var realTaskReport = ParentColumn as TaskReportVm;
						if (realTaskReport != null)
						{
							_taskReportDataService.DeleteById(realTaskReport.Id);
							realTaskReport.Task.ReloadTaskReports();
							ParentRow.Parent.SelectedBlock.BlockReport.ReloadProcessReportRows();
						}
					}
					else
						Message.AddEmbeddedException("این بازه هیچ گزارشی ندارد");
				}
				catch
				{
					Message.AddEmbeddedException("قادر به حذف گزارشهای این بازه نمی باشد");
				}
			});
			DeleteProcessReportCommand = new Commands.Command(o =>
			{
				try
				{
					_processReportDataService.ResetById(Id,
						_model.Process.TargetCount
						- ParentRow.ProcessReportCells.Where(y => y.Id != -1).Sum(x => x.ProcessReportTargetPoint));
					ParentColumn.Task.Block.BlockReport.ReloadProcessReportRows();
				}
				catch
				{
					Message.AddEmbeddedException("قادر به حذف این گزارش فعالیت نمی باشد یا این فعالیت در این بازه هیچ گزارشی ندارد");
				}
			});
		}

		//OpenCommand Dependency Property
		public Commands.Command OpenCommand
		{
			get { return (Commands.Command)GetValue(OpenCommandProperty); }
			set { SetValue(OpenCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenCommandProperty =
			DependencyProperty.Register("OpenCommand", typeof(Commands.Command), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		//SaveCommand Dependency Property
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		//CloseCommand Dependency Property
		public Commands.Command CloseCommand
		{
			get { return (Commands.Command)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}
		public static readonly DependencyProperty CloseCommandProperty =
			DependencyProperty.Register("CloseCommand", typeof(Commands.Command), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		//DeleteProcessReportCommand Dependency Property
		public Commands.Command DeleteProcessReportCommand
		{
			get { return (Commands.Command)GetValue(DeleteProcessReportCommandProperty); }
			set { SetValue(DeleteProcessReportCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteProcessReportCommandProperty =
			DependencyProperty.Register("DeleteProcessReportCommand", typeof(Commands.Command), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		//DeleteTaskReportCommand Dependency Property
		public Commands.Command DeleteTaskReportCommand
		{
			get { return (Commands.Command)GetValue(DeleteTaskReportCommandProperty); }
			set { SetValue(DeleteTaskReportCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteTaskReportCommandProperty =
			DependencyProperty.Register("DeleteTaskReportCommand", typeof(Commands.Command), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
		#endregion
	}
}