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

		DataServices.BlockDataService _blockDataService;

		Dal.SoheilEdmContext _uow;


		#region Ctor & init
		/// <summary>
		/// Creates an instance of BlockEditor viewModel for an existing block model
		/// <para>Each instance has an exclusive uow</para>
		/// </summary>
		/// <param name="blockModel">block containing some (or no) tasks to edit (must be obtained from the same context)</param>
		public BlockEditorVm(Model.Block blockModel)
		{
			_uow = new Dal.SoheilEdmContext();
			_blockDataService = new DataServices.BlockDataService(_uow);
			Message = new Common.SoheilException.EmbeddedException();

			//change context graph
			Model = _blockDataService.GetSingle(blockModel.Id);
			State = new StateVm(Model.StateStation.State);
			StateStation = State.StateStationList.First(x => x.StateStationId == Model.StateStation.Id);
            SelectedStateStation = StateStation;
			StartDate = Model.StartDateTime.Date;
			StartTime = Model.StartDateTime.TimeOfDay;

			initOperatorManager();
			initTask();
			initializeCommands();
		}
		/// <summary>
		/// Creates an instance of BlockEditor viewModel for an existing fpcState model
		/// </summary>
		/// <param name="stateModel">state model to create a block (can be obtained from a different context)</param>
		public BlockEditorVm(Model.State stateModel)
		{
			_uow = new Dal.SoheilEdmContext();
			_blockDataService = new DataServices.BlockDataService(_uow);
			Message = new Common.SoheilException.EmbeddedException();

			var stateEntity = new Soheil.Core.DataServices.StateDataService(_uow).GetSingle(stateModel.Id);
			State = new StateVm(stateEntity);
			SelectedStateStation = State.StateStationList.FirstOrDefault();

			initOperatorManager();
			initializeCommands();
		}

		void initOperatorManager()
		{
			OperatorManager = new OperatorManagerVm(_uow);
			//update operators quicklist (readonly list inside a process rectangle)
			//also update SelectedOperatorsCount of a process
			OperatorManager.SelectionChanged += OperatorManager_SelectionChanged;
		}

		void initTask()
		{
			OperatorManager.Block = Model;

			if (!Model.Tasks.Any())
			{
				Model.Tasks.Add(new Model.Task
				{
					Block = Model,
					StartDateTime = Model.StartDateTime,
					EndDateTime = Model.EndDateTime,
					DurationSeconds = Model.DurationSeconds,
					TaskTargetPoint = Model.BlockTargetPoint,
					Code = Model.Code,
					ModifiedBy = Model.ModifiedBy,
				});
			}

			//add event handlers to ActivityList
			foreach (var group in Model.StateStation.StateStationActivities.GroupBy(x => x.Activity))
			{
				var activityVm = new ActivityEditorVm(Model.Tasks.First(), _uow, group);

				//refresh operator manager upon selecting a process
				activityVm.Selected += Activity_Selected;

				//update times of Block when times of a process changed
				activityVm.TimesChanged += Activity_TimesChanged;

				//update process Tp and duration upon changing choice
				activityVm.SelectedChoiceChanged += activityVm_SelectedChoiceChanged;

				ActivityList.Add(activityVm);
			}

			OperatorManager.refresh();
		}

		void activityVm_SelectedChoiceChanged(ProcessEditorVm processVm, ChoiceEditorVm newChoice)
		{
			if (newChoice == null) return;

			if (IsTargetPointFixed)
			{
				processVm.TargetPoint = 0;
				processVm.TargetPoint = FixedTargetPoint;
			}
			else if (IsDurationFixed)
			{
				processVm.DurationSeconds = 0;
				processVm.DurationSeconds = FixedDurationSeconds;
			}
			else if (IsDeferred)
			{
				//set target point
				if (processVm.DurationSeconds > 0)
					processVm.TargetPoint = (int)(processVm.DurationSeconds / newChoice.CycleTime);
				//or set duration seconds
				else
					processVm.DurationSeconds = (int)(processVm.TargetPoint * newChoice.CycleTime);
			}
		}
		#endregion

		#region Event Handlers
		//Selection is for user selection only (not automatic selection)
		void OperatorManager_SelectionChanged(OperatorEditorVm vm, bool isSelected, bool updateCount)
		{
			//update quick list
			var activityVm = ActivityList.FirstOrDefault(x => x.Id == OperatorManager.Activity.Id);
			if (activityVm != null)
			{
				var processVm = activityVm.ProcessList.FirstOrDefault(x => x.Model == OperatorManager.Process);
				if (processVm != null)
				{
					if (isSelected)
					{
						if (!processVm.SelectedOperators.Any(x => x.OperatorId == vm.OperatorId))
							processVm.SelectedOperators.Add(vm);
					}
					else
					{
						var operVm = processVm.SelectedOperators.FirstOrDefault(x => x.OperatorId == vm.OperatorId);
						if (operVm != null)
							processVm.SelectedOperators.Remove(operVm);
					}
				}
			}
		}

		void Activity_Selected(ProcessEditorVm processVm)
		{
			foreach (var activity in ActivityList)
			{
				foreach (var process in activity.ProcessList)
				{
					if (process != processVm) process.IsSelected = false;
				}
			}
			OperatorManager.Refresh(processVm);
		}
		void Activity_TimesChanged(ProcessEditorVm process, DateTime start, DateTime end)
		{
			if (start < Model.StartDateTime)
			{
				var diff = process.Model.StartDateTime - start;

				process.HoldEvents = true;
				process.StartTime = Model.StartDateTime.TimeOfDay;
				process.HoldEvents = false;

				process.EndTime = process.EndTime.Add(diff);
				//changing EndTime fires this event handler again
			}
			else//if start is wrong don't do the following
			{
				var max = ActivityList.Where(a => a.ProcessList.Any())
					.Max(a => a.ProcessList.Max(p => p.Date.Add(p.EndTime)));
				EndDate = max.Date;
				EndTime = max.TimeOfDay;
				Model.EndDateTime = max;
				Duration = Model.EndDateTime - Model.StartDateTime;
			}
			OperatorManager.refresh();
		}
		void StartChanged(DateTime start)
		{
			Model.StartDateTime = start;

			var end = start.AddSeconds(Model.DurationSeconds);
			EndDate = end.Date;
			EndTime = end.TimeOfDay;

			var task = Model.Tasks.FirstOrDefault();//??? for 1 task only
			if (task != null)
			{
				task.StartDateTime = start;
			}
		}
		#endregion

		#region Methods (Save)
		/// <summary>
		/// Saves this block and also finds the best empty space if needed
		/// </summary>
		internal void Save()
		{
			var task = Model.Tasks.FirstOrDefault();
			if (task == null) throw new Exception("No task is created");

			#region correct machines

			#endregion

			#region Evaluate times and space
			var nptDs = new DataServices.NPTDataService(_uow);

			//recalc Start/End/Duration
			if (task.Processes.Any(x => x.StartDateTime < Model.StartDateTime))
				throw new Exception("Start time of a process is wrong.");
			Model.EndDateTime = task.Processes.Max(x => x.EndDateTime);
			Model.DurationSeconds = (int)(Model.EndDateTime - Model.StartDateTime).TotalSeconds;

			task.StartDateTime = Model.StartDateTime;
			task.EndDateTime = Model.EndDateTime;
			task.DurationSeconds = Model.DurationSeconds;

			//check if it fits
			bool wasAutoStart = IsAutoStart;
			bool fits = true;
			if (!IsAutoStart)
			{
				var inRangeBlocks = _blockDataService.GetInRange(Model.StartDateTime, Model.EndDateTime, StationId);
				var inRangeNPTs = nptDs.GetInRange(Model.StartDateTime, Model.EndDateTime, StationId);

				//if not fit, make it auto start
				if (inRangeBlocks.Any() || inRangeNPTs.Any())
					fits = false;
			}

			//check if should use auto start
			if (IsAutoStart || !fits)
			{
				// Updates the start datetime of this block to fit the first empty space
				Core.PP.Smart.SmartManager sman = new Core.PP.Smart.SmartManager(_blockDataService, nptDs);
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
			#endregion

			_blockDataService.SaveBlock(Model);
			if (BlockAdded != null) BlockAdded(Model);
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets a collection of ActivityEditorVms that contains all processes of this block
		/// </summary>
		public ObservableCollection<ActivityEditorVm> ActivityList { get { return _activityList; } }
		private ObservableCollection<ActivityEditorVm> _activityList = new ObservableCollection<ActivityEditorVm>();


		/// <summary>
		/// Gets or sets the bindable State for this block
		/// </summary>
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
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (BlockEditorVm)d;
				vm.Model.BlockTargetPoint = (int)(e.NewValue);
				if (vm.Model.Tasks.Any())
					vm.Model.Tasks.First().TaskTargetPoint = (int)(e.NewValue);
			}));

		/// <summary>
		/// Gets or sets the bindable viewModel for Operators system
		/// </summary>
		public OperatorManagerVm OperatorManager
		{
			get { return (OperatorManagerVm)GetValue(OperatorManagerProperty); }
			set { SetValue(OperatorManagerProperty, value); }
		}
		public static readonly DependencyProperty OperatorManagerProperty =
			DependencyProperty.Register("OperatorManager", typeof(OperatorManagerVm), typeof(BlockEditorVm), new UIPropertyMetadata(null));

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
		/// <para>Changing the value causes change to StartDateTime of block model and task model</para>
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(BlockEditorVm),
			new UIPropertyMetadata(DateTime.Now.Date, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (DateTime)e.NewValue;
				vm.StartChanged(val.Add(vm.StartTime));
			}));

		/// <summary>
		/// Gets or sets the bindable StartTime manually defined for this block
		/// <para>Changing the value causes change to StartDateTime of block model and task model</para>
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(BlockEditorVm),
			new UIPropertyMetadata(DateTime.Now.TimeOfDay, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (TimeSpan)e.NewValue;
				vm.StartChanged(vm.StartDate.Add(val));
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
		/// <para>Changing the value causes change to EndDateTime of block model and task model</para>
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(BlockEditorVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (DateTime)e.NewValue;
				vm.Model.EndDateTime = val.Add(vm.EndTime);

				var task = vm.Model.Tasks.FirstOrDefault();
				if (task != null) task.EndDateTime = vm.Model.EndDateTime;
			}));
		/// <summary>
		/// Gets or sets the bindable auto-calculated EndTime of this block
		/// <para>Changing the value causes change to EndDateTime of block model and task model</para>
		/// </summary>
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(BlockEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (TimeSpan)e.NewValue;
				vm.Model.EndDateTime = vm.EndDate.Add(val);

				var task = vm.Model.Tasks.FirstOrDefault();
				if (task != null) task.EndDateTime = vm.Model.EndDateTime;
			}));

		/// <summary>
		/// Gets or sets the bindable auto-calculated Duration of this block
		/// <para>Changing the value causes change to Duration of block model and task model</para>
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
				var vm = d as BlockEditorVm;
				var val = (TimeSpan)e.NewValue;
				vm.Model.DurationSeconds = (int)val.TotalSeconds;

				var task = vm.Model.Tasks.FirstOrDefault();
				if (task != null) task.DurationSeconds = vm.Model.DurationSeconds;
			}));
		
		//IsTargetPointFixed Dependency Property
		public bool IsTargetPointFixed
		{
			get { return (bool)GetValue(IsTargetPointFixedProperty); }
			set { SetValue(IsTargetPointFixedProperty, value); }
		}
		public static readonly DependencyProperty IsTargetPointFixedProperty =
			DependencyProperty.Register("IsTargetPointFixed", typeof(bool), typeof(BlockEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				if ((bool)e.NewValue)
				{
					vm.IsDurationFixed = false;
					vm.IsDeferred = false;
				}
			}));
		//IsDurationFixed Dependency Property
		public bool IsDurationFixed
		{
			get { return (bool)GetValue(IsDurationFixedProperty); }
			set { SetValue(IsDurationFixedProperty, value); }
		}
		public static readonly DependencyProperty IsDurationFixedProperty =
			DependencyProperty.Register("IsDurationFixed", typeof(bool), typeof(BlockEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				if ((bool)e.NewValue)
				{
					vm.IsTargetPointFixed = false;
					vm.IsDeferred = false;
				}
			}));
		//IsDeferred Dependency Property
		public bool IsDeferred
		{
			get { return (bool)GetValue(IsDeferredProperty); }
			set { SetValue(IsDeferredProperty, value); }
		}
		public static readonly DependencyProperty IsDeferredProperty =
			DependencyProperty.Register("IsDeferred", typeof(bool), typeof(BlockEditorVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				if ((bool)e.NewValue)
				{
					vm.IsTargetPointFixed = false;
					vm.IsDurationFixed = false;
				}
			}));
		//FixedTargetPoint Dependency Property
		public int FixedTargetPoint
		{
			get { return (int)GetValue(FixedTargetPointProperty); }
			set { SetValue(FixedTargetPointProperty, value); }
		}
		public static readonly DependencyProperty FixedTargetPointProperty =
			DependencyProperty.Register("FixedTargetPoint", typeof(int), typeof(BlockEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (int)e.NewValue;
				vm.IsTargetPointFixed = true;
				foreach (var activity in vm.ActivityList)
				{
					foreach (var process in activity.ProcessList)
					{
						process.TargetPoint = val;
					}
				}
			}));
		//FixedDuration Dependency Property
		public int FixedDurationSeconds
		{
			get { return (int)GetValue(FixedDurationSecondsProperty); }
			set { SetValue(FixedDurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty FixedDurationSecondsProperty =
			DependencyProperty.Register("FixedDurationSeconds", typeof(int), typeof(BlockEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = d as BlockEditorVm;
				var val = (int)e.NewValue;
				vm.IsDurationFixed = true;
				foreach (var activity in vm.ActivityList)
				{
					foreach (var process in activity.ProcessList)
					{
						process.DurationSeconds = val;
					}
				}
			}));
		#endregion


		#region Commands
		void initializeCommands()
		{
			ChangeStationCommand = new Commands.Command(o =>
			{
				if (Model != null)
					try { _blockDataService.DeleteModelRecursive(Model); }
					catch { }
				ActivityList.Clear();

				StateStation = SelectedStateStation;
				Model = new Model.Block
				{
					Code = StateStation.Code,
					StateStation = StateStation.Model,
					StartDateTime = DateTime.Now,
					EndDateTime = DateTime.Now,
					DurationSeconds = 0,
					ModifiedBy = LoginInfo.Id,
				};
				initTask();
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
