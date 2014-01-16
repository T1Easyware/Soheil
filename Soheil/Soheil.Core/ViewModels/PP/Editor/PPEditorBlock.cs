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
		#region Ctor
		/// <summary>
		/// Creates an instance of PPEditorState viewModel for an existing block model
		/// </summary>
		/// <param name="blockModel">block containing some tasks to edit</param>
		public PPEditorBlock(Model.Block blockModel)
		{
			_model = blockModel;
			State = new StateVm(blockModel.StateStation.State);
			StateStation = State.StateStationList.First(x => x.StateStationId == blockModel.StateStation.Id);
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
			State = new StateVm(stateModel);
			StateStation = State.StateStationList.FirstOrDefault();
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

		Model.Block _model;
		public int StateId { get { return State.Id; } }
		public int StationId { get { return StateStation == null ? 0 : StateStation.StationId; } }
		public int StateStationId { get { return StateStation == null ? 0 : StateStation.StateStationId; } }

		#region DpProps
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
				
			}));
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
		}
		//DeleteBlockFromList Dependency Property
		public Commands.Command DeleteBlockFromList
		{
			get { return (Commands.Command)GetValue(DeleteBlockFromListProperty); }
			set { SetValue(DeleteBlockFromListProperty, value); }
		}
		public static readonly DependencyProperty DeleteBlockFromListProperty =
			DependencyProperty.Register("DeleteBlockFromList", typeof(Commands.Command), typeof(PPEditorBlock), new UIPropertyMetadata(null)); 
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
