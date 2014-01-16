using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorBlock : DependencyObject
	{
		Model.Block _model;
		public int StateId { get { return State.Id; } }
		public int StationId { get { return StateStation == null ? 0 : StateStation.StationId; } }
		public int StateStationId { get { return StateStation == null ? 0 : StateStation.StateStationId; } }

		#region Ctor
		/// <summary>
		/// Creates an instance of PPEditorState viewModel for an existing block model
		/// </summary>
		/// <param name="blockModel">block containing some (or no) tasks to edit</param>
		public PPEditorBlock(Model.Block blockModel)
		{
			_model = blockModel;
			//TOP MEMBERS
			State = new StateVm(blockModel.StateStation.State);
			StateStation = State.StateStationList.First(x => x.StateStationId == blockModel.StateStation.Id);
			initMembers();
			//TaskList
			foreach (var task in blockModel.Tasks)
			{
				TaskList.Add(new PPEditorTask(task, this));
			}
		}
		/// <summary>
		/// Creates an instance of PPEditorState viewModel for an existing fpcState model
		/// </summary>
		/// <param name="stateModel">state model to create a block</param>
		public PPEditorBlock(Model.State stateModel)
		{
			_model = new Model.Block();
			//TOP MEMBERS
			State = new StateVm(stateModel);
			StateStation = State.StateStationList.FirstOrDefault();
			initMembers();
		}
		void initMembers()
		{
			StartDate = _model.StartDateTime.Date;
			StartTime = _model.StartDateTime.TimeOfDay;
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
			DependencyProperty.Register("StateStation", typeof(StateStationVm), typeof(PPEditorBlock),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as PPEditorBlock;
				var val = e.NewValue as StateStationVm;
				if (val == null) return;
				vm._model.StateStation = val.Model;
			}));
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
				vm._model.StartDateTime = val.Add(vm.StartTime);
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
				vm._model.StartDateTime = vm.StartDate.Add(val);
			}));
		//IsAutoStart Dependency Property
		public bool IsAutoStart
		{
			get { return (bool)GetValue(IsAutoStartProperty); }
			set { SetValue(IsAutoStartProperty, value); }
		}
		public static readonly DependencyProperty IsAutoStartProperty =
			DependencyProperty.Register("IsAutoStart", typeof(bool), typeof(PPEditorBlock), new PropertyMetadata(true));
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
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPEditorBlock), new UIPropertyMetadata(TimeSpan.Zero));
		//BlockTargetPoint Dependency Property
		public int BlockTargetPoint
		{
			get { return (int)GetValue(BlockTargetPointProperty); }
			set { SetValue(BlockTargetPointProperty, value); }
		}
		public static readonly DependencyProperty BlockTargetPointProperty =
			DependencyProperty.Register("BlockTargetPoint", typeof(int), typeof(PPEditorBlock), new UIPropertyMetadata(0));
		#endregion
		
		//TaskList Observable Collection
		public ObservableCollection<PPEditorTask> TaskList { get { return _taskList; } }
		private ObservableCollection<PPEditorTask> _taskList = new ObservableCollection<PPEditorTask>();

		#region Commands
		void initializeCommands()
		{
			DeleteBlockFromList = new Commands.Command(vm =>
			{
				((PPTaskEditorVm)vm).FpcViewer_RemoveBlock(this);
			});
			SelectTodayCommand = new Commands.Command(o => StartDate = DateTime.Now.Date);
			SelectTomorrowCommand = new Commands.Command(o => StartDate = DateTime.Now.AddDays(1).Date);
			SelectNextHourCommand = new Commands.Command(o => { SelectTodayCommand.Execute(o); StartTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0); });
		}
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
		#endregion


		internal void Reset()
		{
			foreach (var task in TaskList)
			{
				task.Reset();
			}
		}
	}
}
