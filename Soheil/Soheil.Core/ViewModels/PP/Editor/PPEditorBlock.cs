using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// Has its own UnitOfWork
	/// </summary>
	public class PPEditorBlock : DependencyObject
	{
		public Model.Block Model { get; protected set; }
		public int StateId { get { return State.Id; } }
		public int StationId { get { return StateStation == null ? 0 : StateStation.StationId; } }
		public int StateStationId { get { return StateStation == null ? 0 : StateStation.StateStationId; } }

		public event Action<Model.Block> BlockAdded;

		public DataServices.TaskDataService TaskDataService { get; private set; }
		public DataServices.BlockDataService BlockDataService { get; private set; }

		Dal.SoheilEdmContext _uow;

		#region Ctor and methods
		/// <summary>
		/// Creates an instance of PPEditorState viewModel for an existing block model
		/// </summary>
		/// <param name="blockModel">block containing some (or no) tasks to edit</param>
		public PPEditorBlock(Model.Block blockModel)
		{
			_uow = new Dal.SoheilEdmContext();

			initMembers();
			initializeCommands();
			
			//change context graph
			Model = BlockDataService.GetSingle(blockModel.Id);
			State = new StateVm(Model.StateStation.State);
			StateStation = State.StateStationList.First(x => x.StateStationId == Model.StateStation.Id);
            SelectedStateStation = StateStation;
			StartDate = Model.StartDateTime.Date;
			StartTime = Model.StartDateTime.TimeOfDay;

			//Tasks
			foreach (var task in Model.Tasks)
				TaskList.Add(new PPEditorTask(task, this, _uow));
			TaskList.Add(new PPEditorTaskHolder(this));
		}
		/// <summary>
		/// Creates an instance of PPEditorState viewModel for an existing fpcState model
		/// </summary>
		/// <param name="stateModel">state model to create a block</param>
		public PPEditorBlock(Model.State stateModel)
		{
			_uow = new Dal.SoheilEdmContext();

			initMembers();
			initializeCommands();

			var stateEntity = new Soheil.Core.DataServices.StateDataService(_uow).GetSingle(stateModel.Id);
			State = new StateVm(stateEntity);
			SelectedStateStation = State.StateStationList.FirstOrDefault();
			Model = new Model.Block
			{
				StateStation = SelectedStateStation.Model,
				StartDateTime = DateTime.Now,
			};
			StartDate = DateTime.Now.Date;
			StartTime = DateTime.Now.TimeOfDay;

			//Tasks
			//InsertTask();
			TaskList.Add(new PPEditorTaskHolder(this));
		}
		void initMembers()
		{
			//DS
			TaskDataService = new DataServices.TaskDataService(_uow);
			BlockDataService = new DataServices.BlockDataService(_uow);
			//TaskList
			TaskList.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null) foreach (var item in e.NewItems.OfType<PPEditorTask>())
				{
					item.TaskDurationChanged += (d1, d2) => Duration += (d2 - d1);
					item.TaskTargetPointChanged += (t1, t2) => BlockTargetPoint += (t2 - t1);
				}
				if (e.OldItems != null) foreach (var item in e.OldItems.OfType<PPEditorTask>())
				{
					item.ForceCalculateDuration();
					Duration -= new TimeSpan(item.DurationSeconds);
					BlockTargetPoint -= item.TaskTargetPoint;
				}
			};
			//Errors
			Message = new Common.SoheilException.EmbeddedException();	
		}
		internal void InsertTask()
		{
			var last = TaskList.OfType<PPEditorTask>().LastOrDefault();
			var startDt = (last == null) ? Model.StartDateTime : last.EndDateTime;

			//task
			var taskModel = new Model.Task
			{
				Block = Model,
				Code = string.Format("{0}{1:D2}", Model.Code, TaskList.Count),
				DurationSeconds = 0,
				StartDateTime = startDt,
				EndDateTime = startDt,
				TaskTargetPoint = 0,
			};
			Model.Tasks.Add(taskModel);

			//ss
			if (SelectedStateStation == null)
				throw new Soheil.Common.SoheilException.SoheilExceptionBase(
					"ایستگاه انتخاب نشده است", 
					Common.SoheilException.ExceptionLevel.Warning);

			//processes
			//taskModel.CreateBasicProcesses();

			//add the VM and select it
			TaskList.Insert(TaskList.Any()? TaskList.Count - 1 : 0, new PPEditorTask(taskModel, this, _uow));
			last = TaskList.OfType<PPEditorTask>().LastOrDefault();
			if(last!=null) last.IsSelected = true;
		}

		/// <summary>
		/// Resets all tasks within this block (tries not to delete anything)
		/// </summary>
		internal void Reset()
		{
			foreach (var task in TaskList.OfType<PPEditorTask>())
			{
				task.Reset();
			}
			if (!TaskList.OfType<PPEditorTask>().Any())
				InsertTask();
		}
		/*public bool ValidateTimeRange(Model.Task taskModel)
		{
			var tasks = _model.Tasks.ToList();
			int idx = tasks.IndexOf(taskModel);
			if (idx == -1) 
				throw new Soheil.Common.SoheilException.SoheilExceptionBase(
					"Task cannot be found. Please reload the data.", 
					Common.SoheilException.ExceptionLevel.Warning);
			if(idx > 0)
			{
				return taskModel.StartDateTime == tasks[idx - 1].EndDateTime;
			}
			if(idx < tasks.Count- 1)
			{
				return taskModel.EndDateTime == tasks[idx + 1].StartDateTime;
			}
			return true;
		}*/
		#endregion



		#region Top members
		public StateVm State
		{
			get { return (StateVm)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(StateVm), typeof(PPEditorBlock), new PropertyMetadata(null));
		public StateStationVm StateStation
		{
			get { return (StateStationVm)GetValue(StateStationProperty); }
			set { SetValue(StateStationProperty, value); }
		}
		public static readonly DependencyProperty StateStationProperty =
			DependencyProperty.Register("StateStation", typeof(StateStationVm), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//SelectedStateStation Dependency Property (only for combobox usage)
		public StateStationVm SelectedStateStation
		{
			get { return (StateStationVm)GetValue(SelectedStateStationProperty); }
			set { SetValue(SelectedStateStationProperty, value); }
		}
		public static readonly DependencyProperty SelectedStateStationProperty =
			DependencyProperty.Register("SelectedStateStation", typeof(StateStationVm), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(PPEditorBlock),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as PPEditorBlock;
				var val = (DateTime)e.NewValue;
				DateTime start = val.Add(vm.StartTime);
				vm.Model.StartDateTime = start;
				DateTime end = start.AddSeconds(vm.TaskList.OfType<PPEditorTask>().Sum(x => x.DurationSeconds));
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(PPEditorBlock),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as PPEditorBlock;
				var val = (TimeSpan)e.NewValue;
				DateTime start = vm.StartDate.Add(val);
				vm.Model.StartDateTime = start;
				DateTime end = start.AddSeconds(vm.TaskList.OfType<PPEditorTask>().Sum(x => x.DurationSeconds));
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));
		//IsAutoStart Dependency Property
		public bool IsAutoStart
		{
			get { return (bool)GetValue(IsAutoStartProperty); }
			set { SetValue(IsAutoStartProperty, value); }
		}
		public static readonly DependencyProperty IsAutoStartProperty =
			DependencyProperty.Register("IsAutoStart", typeof(bool), typeof(PPEditorBlock), new PropertyMetadata(false));
		#endregion

		#region Additional Readonly info
		//EndDate Dependency Property
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(PPEditorBlock), new UIPropertyMetadata(DateTime.Now));
		//EndTime Dependency Property
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(PPEditorBlock), new UIPropertyMetadata(TimeSpan.Zero));
		//Duration Dependency Property
		public TimeSpan Duration
		{
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPEditorBlock),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (PPEditorBlock)d;
				var val = (TimeSpan)e.NewValue;
				var end = vm.StartDate.Add(vm.StartTime).Add(val);
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));
		//BlockTargetPoint Dependency Property
		public int BlockTargetPoint
		{
			get { return (int)GetValue(BlockTargetPointProperty); }
			set { SetValue(BlockTargetPointProperty, value); }
		}
		public static readonly DependencyProperty BlockTargetPointProperty =
			DependencyProperty.Register("BlockTargetPoint", typeof(int), typeof(PPEditorBlock),
			new UIPropertyMetadata(0, (d, e) => ((PPEditorBlock)d).Model.BlockTargetPoint = (int)(e.NewValue)));

		//Message Dependency Property
		public Soheil.Common.SoheilException.EmbeddedException Message
		{
			get { return (Soheil.Common.SoheilException.EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(Soheil.Common.SoheilException.EmbeddedException), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		#endregion

		//TaskList Observable Collection
		public ObservableCollection<DependencyObject> TaskList { get { return _taskList; } }
		private ObservableCollection<DependencyObject> _taskList = new ObservableCollection<DependencyObject>();

		#region Commands
		void initializeCommands()
		{
			ChangeStationCommand = new Commands.Command(o =>
			{
				StateStation = SelectedStateStation;
				Model.StateStation = SelectedStateStation.Model;
				Reset();
			});
			DontChangeStationCommand = new Commands.Command(o =>
			{
				SelectedStateStation = StateStation;
				Model.StateStation = StateStation.Model;
			});
			DeleteBlockFromList = new Commands.Command(vm =>
			{
				((PPTaskEditorVm)vm).FpcViewer_RemoveBlock(this);
			});
			SelectTodayCommand = new Commands.Command(o => StartDate = DateTime.Now.Date);
			SelectTomorrowCommand = new Commands.Command(o => StartDate = DateTime.Now.AddDays(1).Date);
			SelectNextHourCommand = new Commands.Command(o =>
			{
				SelectTodayCommand.Execute(o);
				StartTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
			});
			AddOneHourCommand = new Commands.Command(o =>
			{
				StartTime = StartTime.Add(TimeSpan.FromHours(1));
				if (StartTime.CompareTo(TimeSpan.FromDays(1)) >= 1)
				{
					StartTime = StartTime.Add(TimeSpan.FromHours(-24));
					StartDate = StartDate.AddDays(1);
				}
			});
		}

		//ChangeStationCommand Dependency Property
		public Commands.Command ChangeStationCommand
		{
			get { return (Commands.Command)GetValue(ChangeStationCommandProperty); }
			set { SetValue(ChangeStationCommandProperty, value); }
		}
		public static readonly DependencyProperty ChangeStationCommandProperty =
			DependencyProperty.Register("ChangeStationCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//DontChangeStationCommand Dependency Property
		public Commands.Command DontChangeStationCommand
		{
			get { return (Commands.Command)GetValue(DontChangeStationCommandProperty); }
			set { SetValue(DontChangeStationCommandProperty, value); }
		}
		public static readonly DependencyProperty DontChangeStationCommandProperty =
			DependencyProperty.Register("DontChangeStationCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//DeleteBlockFromList Dependency Property
		public Commands.Command DeleteBlockFromList
		{
			get { return (Commands.Command)GetValue(DeleteBlockFromListProperty); }
			set { SetValue(DeleteBlockFromListProperty, value); }
		}
		public static readonly DependencyProperty DeleteBlockFromListProperty =
			DependencyProperty.Register("DeleteBlockFromList", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//SelectTodayCommand Dependency Property
		public Commands.Command SelectTodayCommand
		{
			get { return (Commands.Command)GetValue(SelectTodayCommandProperty); }
			set { SetValue(SelectTodayCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectTodayCommandProperty =
			DependencyProperty.Register("SelectTodayCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//SelectTomorrowCommand Dependency Property
		public Commands.Command SelectTomorrowCommand
		{
			get { return (Commands.Command)GetValue(SelectTomorrowCommandProperty); }
			set { SetValue(SelectTomorrowCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectTomorrowCommandProperty =
			DependencyProperty.Register("SelectTomorrowCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//SelectNextHourCommand Dependency Property
		public Commands.Command SelectNextHourCommand
		{
			get { return (Commands.Command)GetValue(SelectNextHourCommandProperty); }
			set { SetValue(SelectNextHourCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectNextHourCommandProperty =
			DependencyProperty.Register("SelectNextHourCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		//AddOneHourCommand Dependency Property
		public Commands.Command AddOneHourCommand
		{
			get { return (Commands.Command)GetValue(AddOneHourCommandProperty); }
			set { SetValue(AddOneHourCommandProperty, value); }
		}
		public static readonly DependencyProperty AddOneHourCommandProperty =
			DependencyProperty.Register("AddOneHourCommand", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null));
		#endregion

		/// <summary>
		/// Corrects block processes (use this before save)
		/// </summary>
		void correctBlock()
		{
			foreach (var taskVm in TaskList.OfType<PPEditorTask>())
			{
				//processes don't follow the JIT model attachment strategy
				//so we need to manually attach their models prior to Save.

				//first remove those existing process models that there isn't any VM with the same Activity
				var tmp = taskVm.Model.Processes.Where(m=>
					!taskVm.ProcessList.Any(vm => 
						vm.SelectedChoice != null && vm.SelectedChoice.ActivityId == m.StateStationActivity.Activity.Id)
					).ToArray();
				foreach (var processModel in tmp)
				{
					TaskDataService.DeleteModel(processModel);
				}

				//then add new process Models (or attach if a process with the same Activity already exists)
				foreach (var processVm in taskVm.ProcessList.Where(x => x.SelectedChoice != null))
				{
					//check for existance
					var procModel = taskVm.Model.Processes.FirstOrDefault(x => 
						x.StateStationActivity.Activity.Id == processVm.SelectedChoice.ActivityId);
					//create new Model
					if (procModel == null)
					{
						procModel = new Model.Process();
						new Soheil.Dal.Repository<Model.Process>(_uow).Add(procModel);
						//and add to parent model
						taskVm.Model.Processes.Add(procModel);
					}
					//update common info (create/attach)
					procModel.Task = taskVm.Model;
					procModel.StateStationActivity = processVm.SelectedChoice.Model;
					procModel.TargetCount = processVm.TargetPoint;

					//...
					//do the same for machines
					//...
					var tmp1 = procModel.SelectedMachines.Where(m =>
						!processVm.MachineList.Any(vm => 
							vm.MachineId == m.StateStationActivityMachine.Machine.Id)
						).ToArray();
					foreach (var smModel in tmp1)
					{
						TaskDataService.DeleteModel(smModel);
					}
					foreach (var smVm in processVm.MachineList.Where(x => x.IsUsed))
					{
						//check for existance
						var smModel = procModel.SelectedMachines.FirstOrDefault(x => 
							x.StateStationActivityMachine.Machine.Id == smVm.MachineId);
						//create new Model
						if (smModel == null)
						{
							smModel = new Model.SelectedMachine();
							new Soheil.Dal.Repository<Model.SelectedMachine>(_uow).Add(smModel);
							//and add to parent model
							procModel.SelectedMachines.Add(smModel);
						}
						//update common info (create/attach)
						smModel.Process = procModel;
						smModel.StateStationActivityMachine = smVm.StateStationActivityMachineModel;
					}

					//...
					//do the same for operators
					//...
					var tmp2 = procModel.ProcessOperators.Where(m =>
						!processVm.OperatorList.Any(vm => 
							vm.OperatorId == m.Operator.Id)
						).ToArray();
					foreach (var poModel in tmp2)
					{
						TaskDataService.DeleteModel(poModel);
					}
					foreach (var poVm in processVm.OperatorList.Where(x => x.IsSelected))
					{
						//check for existance
						var poModel = procModel.ProcessOperators.FirstOrDefault(x =>
							x.Operator.Id == poVm.OperatorId);
						//create new Model
						if (poModel == null)
						{
							poModel = new Model.ProcessOperator();
							new Soheil.Dal.Repository<Model.ProcessOperator>(_uow).Add(poModel);
							//and add to parent model
							poModel.Process = procModel;
							poModel.Operator = poVm.OperatorModel;
							procModel.ProcessOperators.Add(poModel);
						}
						//update common info (create/attach)
						poModel.Role = poVm.Role;
					}
				}
				taskVm.ForceCalculateDuration();
			}

			Model.DurationSeconds = TaskList.OfType<PPEditorTask>().Sum(t => t.DurationSeconds);
			Model.EndDateTime = Model.StartDateTime.AddSeconds(Model.DurationSeconds);
		}

		internal void Save()
		{
			correctBlock();

			var nptDs = new DataServices.NPTDataService(_uow);

			//check if it fits
			if(!IsAutoStart)
			{
				var inRangeBlocks = BlockDataService.GetInRange(Model.StartDateTime, Model.EndDateTime);
				var inRangeNPTs = nptDs.GetInRange(Model.StartDateTime, Model.EndDateTime);

				if (inRangeBlocks.Any() || inRangeNPTs.Any()) IsAutoStart = true;
			}

			//check if should use auto start
			if (IsAutoStart)
			{
				// Updates the start datetime of this block to fit the first empty space
				Core.PP.Smart.SmartManager sman = new Core.PP.Smart.SmartManager(BlockDataService, nptDs);
				var seq = sman.FindNextFreeSpace(StationId, State.ProductRework.Id, DateTime.Now, (int)Duration.TotalSeconds);
				var block = seq.FirstOrDefault(x => x.Type == Core.PP.Smart.SmartRange.RangeType.NewTask);
				StartDate = block.StartDT.Date;
				StartTime = block.StartDT.TimeOfDay;
			
				if (!sman.SaveSetups(seq))
					Message.AddEmbeddedException("Some setups could not be added. check setup times table.");
			}

			BlockDataService.SaveBlock(Model);
			if (BlockAdded != null) BlockAdded(Model);
		}
	}
}
