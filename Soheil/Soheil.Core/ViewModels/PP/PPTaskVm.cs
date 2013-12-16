using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	public class PPTaskVm : PPItemVm
	{
		public DataServices.TaskDataService TaskDataService { get { return Parent.TaskDataService; } }
		public DataServices.JobDataService JobDataService { get { return Parent.JobDataService; } }
		public DataServices.TaskReportDataService TaskReportDataService { get { return Parent.Parent.TaskReportDataService; } }
		DataServices.ProcessReportDataService _processReportDataService;

		#region Ctor
		public PPTaskVm(Model.Task taskModel, Core.PP.TaskCollection parent, int stationIndex)
		{
			_threadLock = new Object();
			
			_processReportDataService = new DataServices.ProcessReportDataService();
			
			RowIndex = stationIndex;
			Parent = parent;
			Id = taskModel.Id;
			ProductId = taskModel.StateStation.State.FPC.Product.Id;
			StartDateTime = taskModel.StartDateTime;
			DurationSeconds = taskModel.DurationSeconds;
			TaskTargetPoint = taskModel.TaskTargetPoint;

			AddTaskToEditorCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.AppendToTaskEditor(this);
				}
				catch (Exception exp) { AddEmbeddedException(exp.Message); }
			});
			EditItemCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.TaskEditor.IsVisible = true;
					Parent.Parent.JobEditor.IsVisible = false;
					Parent.Parent.ResetTaskEditor(this);
				}
				catch (Exception exp) { AddEmbeddedException(exp.Message); }
			});
			AddJobToEditorCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.AppendToJobEditor(Job);
				}
				catch (Exception exp) { AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false; 
				if (Job.Id == -1) return false; 
				return true;
			});
			EditJobCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.TaskEditor.IsVisible = false;
					Parent.Parent.JobEditor.IsVisible = true;
					Parent.Parent.ResetJobEditor(Job);
				}
				catch (Exception exp) { AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false; 
				if (Job.Id == -1) return false;
				return true;
			});
			EditReportCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.SelectedTask = this;
					Parent.Parent.ViewMode = PPTableViewMode.ProcessReport;

					ReloadProcessReportRows();
				}
				catch (Exception exp)
				{
					AddEmbeddedException(exp.Message);
				}
			});
			DeleteItemCommand = new Commands.Command(o =>
			{
				try
				{
					TaskDataService.DeleteModel(Id);
					Parent.RemoveTask(this);
				}
				catch (Exception exp)
				{
					AddEmbeddedException(exp.Message);
				}
			});
			DeleteJobCommand = new Commands.Command(o =>
			{
				try
				{
					JobDataService.DeleteModel(Job.Id);
					Parent.Parent.RemoveTasks(Job);
				}
				catch (RoutedException exp)
				{
					var task = Parent.FindTaskById(((Model.Task)exp.Target).Id);
					if(task == null)
						AddEmbeddedException(exp.Message);
					else
						task.AddEmbeddedException(exp.Message);
				}
				catch (Exception exp)
				{
					AddEmbeddedException(exp.Message);
				}
			}, () => { return Job != null; });
			InsertSetupBefore = new Commands.Command(async o =>
			{
				var tmp = TaskDataService;
				var result = await Task.Run(() => tmp.InsertSetupBeforeTask(Id));
				//var result = tmp.InsertSetupBeforeTask(Id);
				if (result.IsSaved)
				{
					parent.Reload();
				}
				else
				{
					if (result.IsSaved) return;
					AddEmbeddedException("قادر به افزودن آماده سازی نمی باشد.\nبرخی از Taskهای بعدی در این ایستگاه قابل تغییر نیستند.");
					foreach (var error in result.Errors)
					{
						switch (error.Value1)
						{
							case Soheil.Core.DataServices.TaskDataService.InsertSetupBeforeTaskErrors.ErrorSource.Task:
								var task = parent[this.RowIndex].Tasks.FirstOrDefault(x => x.Id == error.Value3);
								if (task != null) task.AddEmbeddedException(error.Value2);
								else AddEmbeddedException(error.Value2);
								break;
							case Soheil.Core.DataServices.TaskDataService.InsertSetupBeforeTaskErrors.ErrorSource.NPT:
								var npt = parent[this.RowIndex].NPTs.FirstOrDefault(x => x.Id == error.Value3);
								if (npt != null) npt.AddEmbeddedException(error.Value2);
								else AddEmbeddedException(error.Value2);
								break;
							case Soheil.Core.DataServices.TaskDataService.InsertSetupBeforeTaskErrors.ErrorSource.This:
								AddEmbeddedException(error.Value2);
								break;
							default:
								break;
						}
					}
				}
			});
		}
		//Thread Functions
		protected override void acqusitionThreadStart()
		{
			try
			{
				Dispatcher.Invoke(new Action(() =>
				{
					if (!TaskDataService.UpdateViewModel(this))
					{
						Parent.RemoveTask(this);
					}
					else
					{
						Dispatcher.Invoke(acqusitionThreadEnd);
					}
				}));
			}
			catch { }
		}
		protected override void acqusitionThreadEnd()
		{
			ViewMode = Parent.ViewMode;
		}
		#endregion

		#region Members
		/// <summary>
		/// Product Id
		/// </summary>
		public int ProductId { get; set; }
		//Parent Dependency Property
		public Core.PP.TaskCollection Parent
		{
			get { return (Core.PP.TaskCollection)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(Core.PP.TaskCollection), typeof(PPTaskVm), new UIPropertyMetadata(null));
		#endregion

		#region Full State Data
		//Job Dependency Property
		public PPJobVm Job
		{
			get { return (PPJobVm)GetValue(JobProperty); }
			set { SetValue(JobProperty, value); }
		}
		public static readonly DependencyProperty JobProperty =
			DependencyProperty.Register("Job", typeof(PPJobVm), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//ProductCode Dependency Property
		public string ProductCode
		{
			get { return (string)GetValue(ProductCodeProperty); }
			set { SetValue(ProductCodeProperty, value); }
		}
		public static readonly DependencyProperty ProductCodeProperty =
			DependencyProperty.Register("ProductCode", typeof(string), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//ProductName Dependency Property
		public string ProductName
		{
			get { return (string)GetValue(ProductNameProperty); }
			set { SetValue(ProductNameProperty, value); }
		}
		public static readonly DependencyProperty ProductNameProperty =
			DependencyProperty.Register("ProductName", typeof(string), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//ProductColor Dependency Property
		public Color ProductColor
		{
			get { return (Color)GetValue(ProductColorProperty); }
			set { SetValue(ProductColorProperty, value); }
		}
		public static readonly DependencyProperty ProductColorProperty =
			DependencyProperty.Register("ProductColor", typeof(Color), typeof(PPTaskVm), new UIPropertyMetadata(Colors.White, (d, e) =>
			{
				((PPTaskVm)d).ForeColor = ((Color)e.NewValue).IsDark() ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
			}));
		//ForeColor Dependency Property
		public SolidColorBrush ForeColor
		{
			get { return (SolidColorBrush)GetValue(ForeColorProperty); }
			set { SetValue(ForeColorProperty, value); }
		}
		public static readonly DependencyProperty ForeColorProperty =
			DependencyProperty.Register("ForeColor", typeof(SolidColorBrush), typeof(PPTaskVm), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
		//StateCode Dependency Property
		public string StateCode
		{
			get { return (string)GetValue(StateCodeProperty); }
			set { SetValue(StateCodeProperty, value); }
		}
		public static readonly DependencyProperty StateCodeProperty =
			DependencyProperty.Register("StateCode", typeof(string), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//IsRework Dependency Property
		public bool IsRework
		{
			get { return (bool)GetValue(IsReworkProperty); }
			set { SetValue(IsReworkProperty, value); }
		}
		public static readonly DependencyProperty IsReworkProperty =
			DependencyProperty.Register("IsRework", typeof(bool), typeof(PPTaskVm), new UIPropertyMetadata(false));
		//TaskTargetPoint Dependency Property
		public int TaskTargetPoint
		{
			get { return (int)GetValue(TaskTargetPointProperty); }
			set { SetValue(TaskTargetPointProperty, value); }
		}
		public static readonly DependencyProperty TaskTargetPointProperty =
			DependencyProperty.Register("TaskTargetPoint", typeof(int), typeof(PPTaskVm), new PropertyMetadata(0));

		//CanAddSetupBefore Dependency Property
		public bool CanAddSetupBefore
		{
			get { return (bool)GetValue(CanAddSetupBeforeProperty); }
			set { SetValue(CanAddSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty CanAddSetupBeforeProperty =
			DependencyProperty.Register("CanAddSetupBefore", typeof(bool), typeof(PPTaskVm), new UIPropertyMetadata(false));
		#endregion

		#region TaskReports
		/// <summary>
		/// Partitions a Task into TaskReports from PPTable's Simple Mode
		/// </summary>
		public void ReloadTaskReports()
		{
			TaskReports.Clear();
			var models = TaskReportDataService.GetAllForTask(Id);
			int i = 0;
			int sumOfTP = 0;
			foreach (var model in models)
			{
				var vm = new TaskReportVm(this, model, i);
				TaskReports.Add(vm);
				sumOfTP += vm.TargetPoint;
				i++;
			}
			int sumOfDurations = models.Sum(x => x.ReportDurationSeconds);
			if (sumOfDurations < this.DurationSeconds)
				TaskReports.Add(new TaskReportHolderVm(this, sumOfDurations, sumOfTP, i));
			SumOfReportedHours = sumOfDurations / 3600d;
		}
		public void ClearTaskReports()
		{
			TaskReports.Clear();
		}
		//TaskReports Observable Collection
		private ObservableCollection<TaskReportBaseVm> _taskReports = new ObservableCollection<TaskReportBaseVm>();
		public ObservableCollection<TaskReportBaseVm> TaskReports { get { return _taskReports; } }

		public void ReloadProcessReportRows()
		{
			ProcessReportRows.Clear();
			Processes.Clear();
			var processes = TaskDataService.GetProcesses(Id);
			int index = 0;
			foreach (var process in processes)
			{
				var processVm = new ProcessVm(process, this);
				Processes.Add(processVm);

				var processReportRow = new ProcessReportRowVm(Parent.Parent, processVm, _processReportDataService, index);
				index++;
				ProcessReportRows.Add(processReportRow);

				//Load cells from the already-built-TaskReports in the ReportSimpleView
				foreach (var taskReport in TaskReports)
				{
					processReportRow.ProcessReportCells.Add(new ProcessReportCellVm(taskReport, processReportRow, _processReportDataService));
				}
			}
		}
		//Processes Observable Collection
		private ObservableCollection<ProcessVm> _processes = new ObservableCollection<ProcessVm>();
		public ObservableCollection<ProcessVm> Processes { get { return _processes; } }
		//ProcessReportRows Observable Collection
		private ObservableCollection<ProcessReportRowVm> _processReportRows = new ObservableCollection<ProcessReportRowVm>();
		public ObservableCollection<ProcessReportRowVm> ProcessReportRows { get { return _processReportRows; } }
		//SumOfReportedHours Dependency Property
		public double SumOfReportedHours
		{
			get { return (double)GetValue(SumOfReportedHoursProperty); }
			set { SetValue(SumOfReportedHoursProperty, value); }
		}
		public static readonly DependencyProperty SumOfReportedHoursProperty =
			DependencyProperty.Register("SumOfReportedHours", typeof(double), typeof(PPTaskVm), new UIPropertyMetadata(0d));
		#endregion

		//AddTaskToEditorCommand Dependency Property
		public Commands.Command AddTaskToEditorCommand
		{
			get { return (Commands.Command)GetValue(AddTaskToEditorCommandProperty); }
			set { SetValue(AddTaskToEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty AddTaskToEditorCommandProperty =
			DependencyProperty.Register("AddTaskToEditorCommand", typeof(Commands.Command), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//AddJobToEditorCommand Dependency Property
		public Commands.Command AddJobToEditorCommand
		{
			get { return (Commands.Command)GetValue(AddJobToEditorCommandProperty); }
			set { SetValue(AddJobToEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty AddJobToEditorCommandProperty =
			DependencyProperty.Register("AddJobToEditorCommand", typeof(Commands.Command), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//EditJobCommand Dependency Property
		public Commands.Command EditJobCommand
		{
			get { return (Commands.Command)GetValue(EditJobCommandProperty); }
			set { SetValue(EditJobCommandProperty, value); }
		}
		public static readonly DependencyProperty EditJobCommandProperty =
			DependencyProperty.Register("EditJobCommand", typeof(Commands.Command), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//DeleteJobCommand Dependency Property
		public Commands.Command DeleteJobCommand
		{
			get { return (Commands.Command)GetValue(DeleteJobCommandProperty); }
			set { SetValue(DeleteJobCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteJobCommandProperty =
			DependencyProperty.Register("DeleteJobCommand", typeof(Commands.Command), typeof(PPTaskVm), new UIPropertyMetadata(null));
		//InsertSetupBefore Dependency Property
		public Commands.Command InsertSetupBefore
		{
			get { return (Commands.Command)GetValue(InsertSetupBeforeProperty); }
			set { SetValue(InsertSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty InsertSetupBeforeProperty =
			DependencyProperty.Register("InsertSetupBefore", typeof(Commands.Command), typeof(PPTaskVm), new UIPropertyMetadata(null));
	}
}
