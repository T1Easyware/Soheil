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
	public class ProcessReportCellVm : EmbeddedException
	{
		DataServices.ProcessReportDataService _processReportDataService;
		public ProcessReportCellVm(
			TaskReportBaseVm taskReport, 
			ProcessReportRowVm processReportRow, 
			DataServices.ProcessReportDataService processReportDataService)
		{
			_processReportDataService = processReportDataService;
			Parent = processReportRow;
			DefectionReports = new DefectionReportCollection(this);
			StoppageReports = new StoppageReportCollection(this);

			var cycleTime = processReportRow.Process.StateStationActivity.CycleTime;
			var realTaskReport = taskReport as TaskReportVm;
			if (realTaskReport == null)
			{
				Id = -1;
				ProcessReportTargetPoint =
					processReportRow.Process.TargetCount
					- processReportRow.ProcessReportCells.Where(y => y.Id != -1).Sum(x => x.ProcessReportTargetPoint);
			}
			else
			{
				var model = _processReportDataService.GetByTaskReportIdAndProcessId(realTaskReport.Id, processReportRow.ProcessId);
				if (model == null)
				{
					Id = -1;
					//DurationSeconds = 
				}
				else
				{
					Id = model.Id;
					ProducedG1 = model.ProducedG1;
					ProcessReportTargetPoint = model.ProcessReportTargetPoint;
				}
			}
			StartDateTime = taskReport.StartDateTime;
			DurationSeconds = (int)(ProcessReportTargetPoint * cycleTime);

			OpenCommand = new Commands.Command(o =>
			{
				if (ViewMode == PPProcessViewMode.Normal) IsSelected = true;
				else if(ViewMode == PPProcessViewMode.Empty)
					Parent.Parent.CurrentTaskReportBuilderInProcess = new ProcessReportCellTaskReportHolder(
						this, 
						new TaskReportHolderVm(
							Parent.Process.Task,
							Parent.ProcessReportCells.Where(x => x.ViewMode == PPProcessViewMode.Normal).Sum(x => x.DurationSeconds),
							Parent.ProcessReportCells.Where(x => x.ViewMode == PPProcessViewMode.Normal).Sum(x => x.ProcessReportTargetPoint), 
							Parent.ProcessReportCells.Count - 1));
			});
			CloseCommand = new Commands.Command(o => { IsSelected = false; });
			SaveCommand = new Commands.Command(o => { 
				Save(); 
				IsSelected = false;
				Parent.Parent.SelectedTask.ReloadProcessReportRows();
			});
			DeleteTaskReportCommand = new Commands.Command(o =>
			{
				try
				{
					if (realTaskReport != null)
					{
						taskReport.TaskReportDataService.DeleteById(realTaskReport.Id);
						Parent.Parent.SelectedTask.ReloadTaskReports();
						Parent.Parent.SelectedTask.ReloadProcessReportRows();
					}
					else
						AddEmbeddedException("این بازه هیچ گزارشی ندارد");
				}
				catch
				{
					AddEmbeddedException("قادر به حذف گزارشهای این بازه نمی باشد");
				}
			});
			DeleteProcessReportCommand = new Commands.Command(o =>
			{
				try
				{
					_processReportDataService.ResetById(Id, 
						processReportRow.Process.TargetCount
						- processReportRow.ProcessReportCells.Where(y => y.Id != -1).Sum(x => x.ProcessReportTargetPoint));
					Parent.Parent.SelectedTask.ReloadProcessReportRows();
				}
				catch
				{
					AddEmbeddedException("قادر به حذف این گزارش فعالیت نمی باشد یا این فعالیت در این بازه هیچ گزارشی ندارد");
				}
			});
		}


		/// <summary>
		/// ProcessReport Id
		/// </summary>
		public int Id { get; set; }
		//Parent Dependency Property
		public ProcessReportRowVm Parent
		{
			get { return (ProcessReportRowVm)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(ProcessReportRowVm), typeof(ProcessReportCellVm), new UIPropertyMetadata(null));
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
					vm.Parent.Parent.CurrentProcessReportBuilder = vm;
					vm.LoadInnerData();
				}
				else
					vm.Parent.Parent.CurrentProcessReportBuilder = null;
			}));
		//Offset Dependency Property
		public Point Offset
		{
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(Point), typeof(ProcessReportCellVm), new UIPropertyMetadata(new Point()));

		#region Thread/Load/Save
		Model.ProcessReport _model;
		//ViewMode Dependency Property
		public PPProcessViewMode ViewMode
		{
			get { return (PPProcessViewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public static readonly DependencyProperty ViewModeProperty =
			DependencyProperty.Register("ViewMode", typeof(PPProcessViewMode), typeof(ProcessReportCellVm), new UIPropertyMetadata(PPProcessViewMode.Acquiring));
		//Members and Props
		Timer _delayAcquisitor;
		private Thread _acqusitionThread;
		//Object threadLock;
		public static int AcquisitionStartDelay = 100;
		public static int AcquisitionPeriodicDelay = System.Threading.Timeout.Infinite;//5000???

		//Main Functions
		public void BeginAcquisition()
		{
			try
			{
				_delayAcquisitor = new Timer((s) =>
				{
					try
					{
						Dispatcher.Invoke(() =>
						{
							_acqusitionThread.ForceQuit();
							_acqusitionThread = new Thread(_acqusitionThreadStart);
							_acqusitionThread.Priority = ThreadPriority.Lowest;
							_acqusitionThread.Start();
						});
					}
					catch { }
				}, null, AcquisitionStartDelay, AcquisitionPeriodicDelay);
			}
			catch { }
		}

		//Thread Functions
		private void _acqusitionThreadStart()
		{
			//try
			{
				var model = _processReportDataService.GetSingleFull(Id);
				Dispatcher.Invoke(new Action(() =>
				{
					Dispatcher.InvokeInBackground<Model.ProcessReport>(_acqusitionThreadEnd, model);
				}));
			}
			//catch { }
		}
		private void _acqusitionThreadEnd(Model.ProcessReport model)
		{
			_model = model;
			if (model == null)
			{
				ViewMode = PPProcessViewMode.Empty;
			}
			else
			{
				ProcessReportTargetPoint = model.ProcessReportTargetPoint;
				ProducedG1 = model.ProducedG1;
				DefectionCount = (int)model.DefectionReports.Sum(x => x.CountEquivalence);
				StoppageCount = (int)model.StoppageReports.Sum(x => x.CountEquivalence);
				ViewMode = PPProcessViewMode.Normal;
			}
		}

		public void UnloadData()
		{
			_acqusitionThread.ForceQuit();
			if (_delayAcquisitor != null) _delayAcquisitor.Dispose();
		}

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
		#endregion

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

		#region Time/Duration
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
		public DateTime StartDateTime
		{
			get { return StartDate.Add(StartTime); }
			set { StartDate = value.Date; StartTime = value.TimeOfDay; }
		}
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(ProcessReportCellVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessReportCellVm)d;
				d.SetValue(DurationProperty, new TimeSpan((int)e.NewValue * TimeSpan.TicksPerSecond));
			}));
		//Duration Dependency Property
		public TimeSpan Duration
		{
			get { return (TimeSpan)GetValue(DurationProperty); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(ProcessReportCellVm), new UIPropertyMetadata(TimeSpan.Zero)); 
		#endregion

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

		#region Commands
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