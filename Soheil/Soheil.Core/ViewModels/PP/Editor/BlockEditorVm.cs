using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class BlockEditorVm : DependencyObject
	{
		public Model.Block Model { get; protected set; }
		public int StateId { get { return State.Id; } }
		public int StationId { get { return StateStation == null ? 0 : StateStation.StationId; } }

		public event Action<Model.Block> BlockAdded;

		public DataServices.TaskDataService TaskDataService { get; private set; }
		public DataServices.BlockDataService BlockDataService { get; private set; }

		Dal.SoheilEdmContext _uow;

		#region Ctor and methods
		/// <summary>
		/// Creates an instance of BlockEditor viewModel for an existing block model
		/// <para>Each instance has an exclusive uow</para>
		/// </summary>
		/// <param name="blockModel">block containing some (or no) tasks to edit</param>
		public BlockEditorVm(Model.Block blockModel)
		{
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
				TaskList.Add(new TaskEditorVm(task, _uow));
			AppendHolder();
		}
		/// <summary>
		/// Creates an instance of BlockEditor viewModel for an existing fpcState model
		/// </summary>
		/// <param name="stateModel">state model to create a block</param>
		public BlockEditorVm(Model.State stateModel)
		{
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
			AppendHolder();
		}

		protected void AppendHolder()
		{
			var holder = new TaskEditorHolderVm();
			holder.TaskCreated += () =>
			{
				try
				{
					InsertTask();
				}
				catch (Soheil.Common.SoheilException.RoutedException ex)
				{
					Message.AddEmbeddedException(ex.Message);
				}
			};
			TaskList.Add(holder);

		}

		/// <summary>
		/// Creates DataServices and sets event handlers for tasks
		/// </summary>
		void initMembers()
		{
			//DS
			_uow = new Dal.SoheilEdmContext();
			TaskDataService = new DataServices.TaskDataService(_uow);
			BlockDataService = new DataServices.BlockDataService(_uow);
			//TaskList
			TaskList.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null) foreach (var item in e.NewItems.OfType<TaskEditorVm>())
				{
					item.TaskDurationChanged += (d1, d2) => Duration += (d2 - d1);
					item.TaskTargetPointChanged += (t1, t2) => BlockTargetPoint = TaskList.OfType<TaskEditorVm>().Sum(x => x.TaskTargetPoint);
					item.DeleteTaskConfirmed += () =>
					{
						try
						{
							TaskDataService.DeleteModel(item.Model);
							TaskList.Remove(this);
						}
						catch (Exception ex)
						{
							Message.AddEmbeddedException(ex.Message);
						}
					};
					Duration += TimeSpan.FromSeconds(item.DurationSeconds);
					BlockTargetPoint += item.TaskTargetPoint;
				}
				if (e.OldItems != null) foreach (var item in e.OldItems.OfType<TaskEditorVm>())
				{
					item.ForceCalculateDuration();
					Duration -= TimeSpan.FromSeconds(item.DurationSeconds);
					BlockTargetPoint -= item.TaskTargetPoint;
				}
			};
			//Errors
			Message = new Common.SoheilException.EmbeddedException();	
		}

		/// <summary>
		/// Resets all tasks within this block (tries not to delete anything)
		/// <para>Reseting a task causes its processes to be corrected</para>
		/// <para>If no tasks in this block create one</para>
		/// </summary>
		internal void Reset()
		{
			foreach (var task in TaskList.OfType<TaskEditorVm>())
			{
				task.RebuildProcesses();
			}
			if (!TaskList.OfType<TaskEditorVm>().Any())
				InsertTask();
		}

		/// <summary>
		/// Creates a tasks and adds it to the end of tasks
		/// </summary>
		internal void InsertTask()
		{
			//make sure stateStation is specified
			if (SelectedStateStation == null)
				throw new Soheil.Common.SoheilException.SoheilExceptionBase(
					"ایستگاه انتخاب نشده است",
					Common.SoheilException.ExceptionLevel.Warning);

			var last = TaskList.OfType<TaskEditorVm>().LastOrDefault();
			var startDt = (last == null) ? StartDate.Add(StartTime) : last.EndDateTime;

			//create a new task model after last one
			var taskModel = new Model.Task
			{
				Block = Model,
				Code = string.Format("{0}{1:D2}", Model.Code, TaskList.Count),
				DurationSeconds = 0,
				StartDateTime = startDt,
				EndDateTime = startDt,
				TaskTargetPoint = 0,
			};
			//add it to block
			Model.Tasks.Add(taskModel);

			//create and add the VM to TaskList
			TaskList.Insert(TaskList.Any() ? TaskList.Count - 1 : 0, new TaskEditorVm(taskModel, _uow));
			last = TaskList.OfType<TaskEditorVm>().LastOrDefault();

			//select the new task
			//the selection works fine as long as a selector is used in view (TabControl has a selector)
			if (last != null) 
				last.IsSelected = true;
		}



		/// <summary>
		/// Corrects block processes (use this before save)
		/// </summary>
		void correctBlock()
		{
			//processes don't follow the JIT model attachment strategy
			//so we need to manually attach or remove their models prior to Save.
			var processRepository = new Dal.Repository<Model.Process>(_uow);
			var operatorRepository = new Dal.Repository<Model.Operator>(_uow);
			var processOperatorRepository = new Dal.Repository<Model.ProcessOperator>(_uow);
			var selectedMachineRepository = new Soheil.Dal.Repository<Model.SelectedMachine>(_uow);

			foreach (var taskVm in TaskList.OfType<TaskEditorVm>())
			{
				/* SSA of each process model can be converted to another SSA (with same activity) when ManHour changes
				 * 
				 *				ProcessModel	validProcessVm				change
				 *				ssa#			ssa# of selected choice
				 * process1		1				1							= same
				 * process2		2				22							↔ changed
				 * process3		3											x deleted
				 * process4						4							* new
				 * process5													= same
				 * 
				*/

				//Some (or all) of processes have (SelectedChoice!=null) and (TargetPoint>0)
				//these are validProcessVms and will have a process in model
				//other processes won't
				var validProcessVms = taskVm.ProcessList.Where(procVm => procVm.SelectedChoice != null && procVm.TargetPoint > 0);
				//existingProcesses is a list of all process models in database
				var existingProcesses = taskVm.Model.Processes.ToArray();

				//update existing process models that are in validProcessVms
				//otherwise delete them
				foreach (var processModel in existingProcesses)
				{
					//find the validProcessVm for this process model
					ProcessEditorVm validProcessVm = null;
					//if current process does not have SSA it means it's not saved yet
					if (processModel.StateStationActivity != null)
						validProcessVms.FirstOrDefault(x =>
							x.SelectedChoice.ActivityId == processModel.StateStationActivity.Activity.Id);

					if (validProcessVm == null)
					{
						#region [x]
						//if no validProcessVm found delete the process model
						TaskDataService.DeleteModel(processModel); 
						#endregion
					}
					else
					{
						//if a validProcessVm is found for this process model
						//	update its things
						processModel.TargetCount = validProcessVm.TargetPoint;
						#region selected machines
						//delete SelectedMachine models that aren't in Vm
						foreach (var smModel in processModel.SelectedMachines.ToArray())
						{
							if (!validProcessVm.MachineList.Any(x =>
								x.CanBeUsed && 
								x.IsUsed && 
								x.MachineId == smModel.StateStationActivityMachine.Machine.Id))
							{
								processModel.SelectedMachines.Remove(smModel);
								selectedMachineRepository.Delete(smModel);
							}
						}
						//add or update SelectedMachine models that are in Vm
						foreach (var machineVm in validProcessVm.MachineList.Where(x => x.CanBeUsed && x.IsUsed))
						{
							//find the SSAM model that matches both machineVm and SSA
							var ssamModel = processModel.StateStationActivity.StateStationActivityMachines.FirstOrDefault(x => x.Machine.Id == machineVm.MachineId);
							//if SSAM is not available it must be a mistake, skip it
							if (ssamModel == null) continue;

							//find machineVm in processModel's SelectedMachines
							var smModel = processModel.SelectedMachines.FirstOrDefault(po => po.StateStationActivityMachine.Machine.Id == machineVm.MachineId);
							if (smModel != null)
							{
								//if machineVm is already in process model's SelectedMachines
								//	update its SSAM (not necessary if SSA is the same[=], but just in case, we set it again)
								smModel.StateStationActivityMachine = ssamModel;
							}
							else
							{
								//if machineVm is not in process model's SelectedMachines
								//	create a new SelectedMachine model
								smModel = new Model.SelectedMachine
								{
									Process = processModel,
									StateStationActivityMachine = ssamModel,
								};
								//add SelectedMachine
								selectedMachineRepository.Add(smModel);
								processModel.SelectedMachines.Add(smModel);
							}
						}
						#endregion

						//[↔]
						if (validProcessVm.SelectedChoice.StateStationActivityId != processModel.StateStationActivity.Id)
						{
							//if validProcessVm has a different SSA from its original model's SSA
							//	update its SSA
							processModel.StateStationActivity = TaskDataService.GetStateStationActivity(validProcessVm.SelectedChoice.StateStationActivityId);
						}
						//else [=]
					}
				}

				#region [*]
				//add validProcessVms to model that aren't in process models
				foreach (var validProcessVm in validProcessVms)
				{
					//if validProcessVm is not found among process models create a new model
					if (!taskVm.Model.Processes.Any(x => x.StateStationActivity.Activity.Id == validProcessVm.SelectedChoice.ActivityId))
					{
						var ssaModel = TaskDataService.GetStateStationActivity(validProcessVm.SelectedChoice.StateStationActivityId);
						//create a new Process model
						var processModel = new Model.Process
						{
							Task = taskVm.Model,
							StateStationActivity = ssaModel,
							TargetCount = validProcessVm.TargetPoint,
							Code = taskVm.Model.Code + "." + ssaModel.Activity.Code,
						};
						#region add selected machines
						foreach (var machineVm in validProcessVm.MachineList.Where(x => x.CanBeUsed && x.IsUsed))
						{
							//find the SSAM model
							var ssamModel = processModel.StateStationActivity.StateStationActivityMachines.FirstOrDefault(x => x.Machine.Id == machineVm.MachineId);

							if (ssamModel != null)
							{
								//create a new SelectedMachine model
								var smModel = new Model.SelectedMachine
								{
									Process = processModel,
									StateStationActivityMachine = ssamModel,
								};
								//add SelectedMachine
								selectedMachineRepository.Add(smModel);
								processModel.SelectedMachines.Add(smModel);
							}
						}
						#endregion

						//add Process
						processRepository.Add(processModel);
						taskVm.Model.Processes.Add(processModel);
					}
				} 
				#endregion

				//correct task times
				taskVm.ForceCalculateDuration();
			}
			//correct block times
			Model.StartDateTime = StartDate.Add(StartTime);
			Model.DurationSeconds = TaskList.OfType<TaskEditorVm>().Sum(t => t.DurationSeconds);
			Model.EndDateTime = Model.StartDateTime.AddSeconds(Model.DurationSeconds);
		}

		/// <summary>
		/// Saves this block and also finds the best empty space if needed
		/// </summary>
		internal void Save()
		{
			correctBlock();

			var nptDs = new DataServices.NPTDataService(_uow);

			//check if it fits
			bool wasAutoStart = IsAutoStart;
			if (!IsAutoStart)
			{
				var inRangeBlocks = BlockDataService.GetInRange(Model.StartDateTime, Model.EndDateTime, StationId);
				var inRangeNPTs = nptDs.GetInRange(Model.StartDateTime, Model.EndDateTime, StationId);

				//if not fit, make it auto start
				if (inRangeBlocks.Any() || inRangeNPTs.Any())
					IsAutoStart = true;
			}

			//check if should use auto start
			if (IsAutoStart)
			{
				// Updates the start datetime of this block to fit the first empty space
				Core.PP.Smart.SmartManager sman = new Core.PP.Smart.SmartManager(BlockDataService, nptDs);
				var seq = sman.FindNextFreeSpace(
					StationId, 
					State.ProductRework.Id,
					!wasAutoStart ? StartDate.Add(StartTime) : DateTime.Now, //put it after specifed time if it wasn't auto start
					(int)Duration.TotalSeconds);
				var block = seq.FirstOrDefault(x => x.Type == Core.PP.Smart.SmartRange.RangeType.NewTask);
				StartDate = block.StartDT.Date;
				StartTime = block.StartDT.TimeOfDay;

				if (!sman.SaveSetups(seq))
					Message.AddEmbeddedException("Some setups could not be added. check setup times table.");
			}

			BlockDataService.SaveBlock(Model);
			if (BlockAdded != null) BlockAdded(Model);
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

		/// <summary>
		/// Gets a bindable collection of Tasks (<see cref="PPEditorTaskHolder"/>) in this block including the <see cref="PPEditorTaskHolder"/>
		/// </summary>
		public ObservableCollection<DependencyObject> TaskList { get { return _taskList; } }
		private ObservableCollection<DependencyObject> _taskList = new ObservableCollection<DependencyObject>();


		#region Fpc links
		public StateVm State
		{
			get { return (StateVm)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(StateVm), typeof(BlockEditorVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable StateStation that represents this block
		/// </summary>
		public StateStationVm StateStation
		{
			get { return (StateStationVm)GetValue(StateStationProperty); }
			set { SetValue(StateStationProperty, value); }
		}
		public static readonly DependencyProperty StateStationProperty =
			DependencyProperty.Register("StateStation", typeof(StateStationVm), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable selected StateStation from a list of available StateStations for block's State
		/// </summary>
		public StateStationVm SelectedStateStation
		{
			get { return (StateStationVm)GetValue(SelectedStateStationProperty); }
			set { SetValue(SelectedStateStationProperty, value); }
		}
		public static readonly DependencyProperty SelectedStateStationProperty =
			DependencyProperty.Register("SelectedStateStation", typeof(StateStationVm), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		#endregion

		#region Date & Time
		/// <summary>
		/// Gets or sets the bindable value that indicates whether this block will fit in the earliest possible space
		/// </summary>
		public bool IsAutoStart
		{
			get { return (bool)GetValue(IsAutoStartProperty); }
			set { SetValue(IsAutoStartProperty, value); }
		}
		public static readonly DependencyProperty IsAutoStartProperty =
			DependencyProperty.Register("IsAutoStart", typeof(bool), typeof(BlockEditorVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (bool)e.NewValue;
				if(val)
				{
					var end = vm.AutoStartDateTime.Add(vm.Duration);
					d.SetValue(EndDateProperty, end.Date);
					d.SetValue(EndTimeProperty, end.TimeOfDay);
				}
			}));
		/// <summary>
		/// Gets or sets the bindable StartDate manually defined for this block
		/// <para>Changing the value causes change to EndDate and EndTime of ViewModel</para>
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(BlockEditorVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (DateTime)e.NewValue;
				DateTime start = val.Add(vm.StartTime);
				DateTime end = start.AddSeconds(vm.TaskList.OfType<TaskEditorVm>().Sum(x => x.DurationSeconds));
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));
		/// <summary>
		/// Gets or sets the bindable StartTime manually defined for this block
		/// <para>Changing the value causes change to EndDate and EndTime of ViewModel</para>
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(BlockEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (TimeSpan)e.NewValue;
				DateTime start = vm.StartDate.Add(val);
				DateTime end = start.AddSeconds(vm.TaskList.OfType<TaskEditorVm>().Sum(x => x.DurationSeconds));
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));

		//AutoStartDateTime Dependency Property
		public DateTime AutoStartDateTime
		{
			get { return (DateTime)GetValue(AutoStartDateTimeProperty); }
			set { SetValue(AutoStartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty AutoStartDateTimeProperty =
			DependencyProperty.Register("AutoStartDateTime", typeof(DateTime), typeof(BlockEditorVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (DateTime)e.NewValue;
				if(vm.IsAutoStart)
				{
					var end = val.Add(vm.Duration);
					d.SetValue(EndDateProperty, end.Date);
					d.SetValue(EndDateProperty, end.TimeOfDay);
				}
			}));

		/// <summary>
		/// Gets or sets the bindable auto-calculated EndDate of this block
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(BlockEditorVm), new UIPropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets the bindable auto-calculated EndTime of this block
		/// </summary>
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(BlockEditorVm), new UIPropertyMetadata(TimeSpan.Zero));
		/// <summary>
		/// Gets or sets the bindable auto-calculated Duration of this block
		/// <para>Changing the value causes change to Duration of model and EndDate and EndTime of ViewModel</para>
		/// </summary>
		public TimeSpan Duration
		{
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(BlockEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = (BlockEditorVm)d;
				var val = (TimeSpan)e.NewValue;
				vm.Model.DurationSeconds = (int)val.TotalSeconds;
				DateTime end ;
				if(vm.IsAutoStart)
				{
					end = vm.AutoStartDateTime.Add(val);
				}
				else
				{
					end = vm.StartDate.Add(vm.StartTime).Add(val);
				}
				vm.EndDate = end.Date;
				vm.EndTime = end.TimeOfDay;
			}));
		#endregion

		#region other
		/// <summary>
		/// Gets or sets the bindable auto-calculate TargetPoint of this block
		/// <para>Changing this value causes change to TargetPoint of model</para>
		/// </summary>
		public int BlockTargetPoint
		{
			get { return (int)GetValue(BlockTargetPointProperty); }
			set { SetValue(BlockTargetPointProperty, value); }
		}
		public static readonly DependencyProperty BlockTargetPointProperty =
			DependencyProperty.Register("BlockTargetPoint", typeof(int), typeof(BlockEditorVm),
			new UIPropertyMetadata(0, (d, e) => ((BlockEditorVm)d).Model.BlockTargetPoint = (int)(e.NewValue)));

		/// <summary>
		/// Gets or sets the bindable message box
		/// </summary>
		public Soheil.Common.SoheilException.EmbeddedException Message
		{
			get { return (Soheil.Common.SoheilException.EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(Soheil.Common.SoheilException.EmbeddedException), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		#endregion


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
				var planEditor = vm as PlanEditorVm;
				if (planEditor != null) planEditor.RemoveBlock(this);
			});
			SelectTodayCommand = new Commands.Command(o => StartDate = DateTime.Now.Date);
			SelectTomorrowCommand = new Commands.Command(o => StartDate = DateTime.Now.AddDays(1).Date);
			SelectThisHourCommand = new Commands.Command(o =>
			{
				SelectTodayCommand.Execute(o);
				StartTime = TimeSpan.FromHours(DateTime.Now.Hour);
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
			SubtractOneHourCommand = new Commands.Command(o =>
			{
				if (StartTime.CompareTo(TimeSpan.FromHours(1)) < 1)
				{
					StartTime = StartTime.Add(TimeSpan.FromHours(23));
					StartDate = StartDate.AddDays(-1);
				}
				else
					StartTime = StartTime.Add(TimeSpan.FromHours(-1));
			});
		}

		/// <summary>
		/// Gets or sets a bindable command to set StateStation of this block according to SelectedStateStation
		/// </summary>
		public Commands.Command ChangeStationCommand
		{
			get { return (Commands.Command)GetValue(ChangeStationCommandProperty); }
			set { SetValue(ChangeStationCommandProperty, value); }
		}
		public static readonly DependencyProperty ChangeStationCommandProperty =
			DependencyProperty.Register("ChangeStationCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to cancel the setting of StateStation of this block
		/// </summary>
		public Commands.Command DontChangeStationCommand
		{
			get { return (Commands.Command)GetValue(DontChangeStationCommandProperty); }
			set { SetValue(DontChangeStationCommandProperty, value); }
		}
		public static readonly DependencyProperty DontChangeStationCommandProperty =
			DependencyProperty.Register("DontChangeStationCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to delete this block from the list of States (Blocks) in PPEditor
		/// </summary>
		public Commands.Command DeleteBlockFromList
		{
			get { return (Commands.Command)GetValue(DeleteBlockFromListProperty); }
			set { SetValue(DeleteBlockFromListProperty, value); }
		}
		public static readonly DependencyProperty DeleteBlockFromListProperty =
			DependencyProperty.Register("DeleteBlockFromList", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to set the StartDate of this block to Today
		/// </summary>
		public Commands.Command SelectTodayCommand
		{
			get { return (Commands.Command)GetValue(SelectTodayCommandProperty); }
			set { SetValue(SelectTodayCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectTodayCommandProperty =
			DependencyProperty.Register("SelectTodayCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to set the StartDate of this block to Tomorrow
		/// </summary>
		public Commands.Command SelectTomorrowCommand
		{
			get { return (Commands.Command)GetValue(SelectTomorrowCommandProperty); }
			set { SetValue(SelectTomorrowCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectTomorrowCommandProperty =
			DependencyProperty.Register("SelectTomorrowCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to set the StartTime of this block to start of this hour
		/// </summary>
		public Commands.Command SelectThisHourCommand
		{
			get { return (Commands.Command)GetValue(SelectThisHourCommandProperty); }
			set { SetValue(SelectThisHourCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectThisHourCommandProperty =
			DependencyProperty.Register("SelectThisHourCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to add 1 hour to the StartTime of this block
		/// </summary>
		public Commands.Command AddOneHourCommand
		{
			get { return (Commands.Command)GetValue(AddOneHourCommandProperty); }
			set { SetValue(AddOneHourCommandProperty, value); }
		}
		public static readonly DependencyProperty AddOneHourCommandProperty =
			DependencyProperty.Register("AddOneHourCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		//SubtractOneHourCommand Dependency Property
		public Commands.Command SubtractOneHourCommand
		{
			get { return (Commands.Command)GetValue(SubtractOneHourCommandProperty); }
			set { SetValue(SubtractOneHourCommandProperty, value); }
		}
		public static readonly DependencyProperty SubtractOneHourCommandProperty =
			DependencyProperty.Register("SubtractOneHourCommand", typeof(Commands.Command), typeof(BlockEditorVm), new UIPropertyMetadata(null));
		#endregion

	}
}
