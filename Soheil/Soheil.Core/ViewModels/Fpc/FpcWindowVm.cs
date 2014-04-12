using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Common.SoheilException;
using System.Runtime.CompilerServices;

namespace Soheil.Core.ViewModels.Fpc
{
	public class FpcWindowVm : FpcVm
	{
		Dal.SoheilEdmContext _uow;

		#region Ctor, ChangeFPC & Reset
		public FpcWindowVm()
		{
			initCommands();
		}
		public FpcWindowVm(Dal.SoheilEdmContext uow)
		{
			_uow = uow;
			initCommands();
		}

		/// <summary>
		/// Updates the ViewModel with an FPC Model with the given Id
		/// </summary>
		/// <param name="id">The id of the FPC Model which is used for updating the FpcWindowVm</param>
		public void ChangeFpc(int id)
		{
			ResetFPC();
			var model = fpcDataService.GetSingle(id);
			try
			{
				initByModel(model);

				//-----------
				//load states
				//-----------
				fpcDataService.CorrectFPCStates(model);
				//reload states
				var states = fpcDataService.stateDataService.GetStatesByFpcId(Id);
				//show all states
				foreach (var item in states)
				{
					States.Add(new StateVm(item, this));
				}

				//----------
				//load conns
				//----------
				var conns = fpcDataService.connectorDataService.GetByFpcId(Id);
				foreach (var item in conns)
				{
					Connectors.Add(new ConnectorVm(item,
						States.FirstOrDefault(x => x.Id == item.StartState.Id),
						States.FirstOrDefault(x => x.Id == item.EndState.Id),
						fpcDataService.connectorDataService));
				}
			}
			catch (SoheilExceptionBase exp)
			{
				string msg = "";
				if (exp.Level == ExceptionLevel.Error)
					msg += "مشکلی در خواندن اطلاعات پیش آمد";
				msg += exp.Message;
				Message = new DependencyMessageBox(msg, exp.Caption, MessageBoxButton.OK, exp.Level);

			}
			catch (Exception exp)
			{
				Message = new DependencyMessageBox(exp);
			}

			if (States.Count(x => x.StateType == StateType.Mid) > 0)
				FocusedState = States.Where(x => x.StateType == StateType.Mid).First();
		}

		/// <summary>
		/// Resets a viewer by changing its model to null
		/// </summary>
		public void ResetFPC(bool clearModel = false)
		{
			_lock = false;
			Message = new DependencyMessageBox();
			if(_uow == null)
				fpcDataService = new FPCDataService();
			else
				fpcDataService = new FPCDataService(_uow);

			//Stations
			Stations.Clear();
			var stations = fpcDataService.stationDataService.GetAllIncludingMachines();
			foreach (var item in stations)
				Stations.Add(new StationVm(item));

			//ActivityGroups
			ActivityGroups.Clear();
			var actgs = fpcDataService.activityGroupDataService.GetAllWithActivities();
			foreach (var item in actgs)
				ActivityGroups.Add(new ActivityGroupVm(item));

			//Drawing area
			States.Clear();
			Connectors.Clear();

			FocusedState = null;
			if (clearModel) 
				Model = new Model.FPC();
		}
		#endregion

		#region OnScreenToolbox (stations, activities, machines)
		/// <summary>
		/// The Clone of an OnScreen Toolbox item which is being dragged by the mouse
		/// </summary>
		public ToolboxItemVm SelectedToolboxItem
		{
			get { return (ToolboxItemVm)GetValue(SelectedToolboxItemProperty); }
			set { SetValue(SelectedToolboxItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedToolboxItemProperty =
			DependencyProperty.Register("SelectedToolboxItem", typeof(ToolboxItemVm), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		//MachineQuery Dependency Property
		public string MachineQuery
		{
			get { return (string)GetValue(MachineQueryProperty); }
			set { SetValue(MachineQueryProperty, value); }
		}
		public static readonly DependencyProperty MachineQueryProperty =
			DependencyProperty.Register("MachineQuery", typeof(string), typeof(FpcWindowVm),
			new UIPropertyMetadata("", (d, e) =>
			{
				var vm = d as FpcWindowVm;
				var val = e.NewValue as string;
				if (string.IsNullOrEmpty(val))
				{
					foreach (var mf in vm.MachineFamilies)
					{
						foreach (var m in mf.Machines)
						{
							m.IsVisible = true;
						}
						mf.IsExpanded = false;
					}
				}
				else
				{
					foreach (var mf in vm.MachineFamilies)
					{
						bool isAnyVisible = false;
						foreach (var m in mf.Machines)
						{
							if (m.Name.Contains(val))
							{
								m.IsVisible = true;
								isAnyVisible = true;
							}
							else m.IsVisible = false;
						}
						mf.IsExpanded = isAnyVisible;
					}
				}
			}));

		//ActivityQuery Dependency Property
		public string ActivityQuery
		{
			get { return (string)GetValue(ActivityQueryProperty); }
			set { SetValue(ActivityQueryProperty, value); }
		}
		public static readonly DependencyProperty ActivityQueryProperty =
			DependencyProperty.Register("ActivityQuery", typeof(string), typeof(FpcWindowVm),
			new UIPropertyMetadata("", (d, e) =>
			{
				var vm = d as FpcWindowVm;
				var val = e.NewValue as string;
				if (string.IsNullOrEmpty(val))
				{
					foreach (var ag in vm.ActivityGroups)
					{
						foreach (var a in ag.Activities)
						{
							a.IsVisible = true;
						}
						ag.IsExpanded = false;
					}
				}
				else
				{
					foreach (var ag in vm.ActivityGroups)
					{
						bool isAnyVisible = false;
						foreach (var a in ag.Activities)
						{
							if (a.Name.Contains(val))
							{
								a.IsVisible = true;
								isAnyVisible = true;
							}
							else a.IsVisible = false;
						}
						ag.IsExpanded = isAnyVisible;
					}
				}
			}));

		//Stations Observable Collection
		public ObservableCollection<StationVm> Stations { get { return _stations; } }
		private ObservableCollection<StationVm> _stations = new ObservableCollection<StationVm>();
		//ActivityGroups Observable Collection
		public ObservableCollection<ActivityGroupVm> ActivityGroups { get { return _activityGroups; } }
		private ObservableCollection<ActivityGroupVm> _activityGroups = new ObservableCollection<ActivityGroupVm>();
		//MachineFamilies Observable Collection
		public ObservableCollection<MachineFamilyVm> MachineFamilies { get { return _machineFamilies; } }
		private ObservableCollection<MachineFamilyVm> _machineFamilies = new ObservableCollection<MachineFamilyVm>();

		#endregion

		#region MainToolbox
		bool _lock;
		//MainToolbox item : Select
		public bool ToolSelection
		{
			get { return (bool)GetValue(ToolSelectionProperty); }
			set { SetValue(ToolSelectionProperty, value); }
		}
		public static readonly DependencyProperty ToolSelectionProperty =
			DependencyProperty.Register("ToolSelection", typeof(bool), typeof(FpcWindowVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				if (((FpcWindowVm)d)._lock) return;
				((FpcWindowVm)d)._lock = true;
				if ((bool)e.NewValue)
				{
					if ((bool)d.GetValue(ToolConnectorProperty)) d.SetValue(ToolConnectorProperty, false);
					if ((bool)d.GetValue(ToolStateProperty)) d.SetValue(ToolStateProperty, false);
				}
				else if ((bool)e.OldValue) d.SetValue(ToolSelectionProperty, true);
				((FpcWindowVm)d)._lock = false;
			}));
		//MainToolbox item : State
		public bool ToolState
		{
			get { return (bool)GetValue(ToolStateProperty); }
			set { SetValue(ToolStateProperty, value); }
		}
		public static readonly DependencyProperty ToolStateProperty =
			DependencyProperty.Register("ToolState", typeof(bool), typeof(FpcWindowVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as FpcWindowVm;
				vm.RemoveNewDraggingState();
				if (((FpcWindowVm)d)._lock) return;
				((FpcWindowVm)d)._lock = true;
				if ((bool)e.NewValue)
				{
					if ((bool)d.GetValue(ToolConnectorProperty)) d.SetValue(ToolConnectorProperty, false);
					if ((bool)d.GetValue(ToolSelectionProperty)) d.SetValue(ToolSelectionProperty, false);
					vm.addStateByTool();
				}
				else if ((bool)e.OldValue) d.SetValue(ToolStateProperty, true);
				((FpcWindowVm)d)._lock = false;
			}));
		private void addStateByTool()
		{
			var stateModel = new Model.State
			{
				StateType = StateType.Mid,
				X = -50,
				Y = -20,
			};
			this.Model.States.Add(stateModel);

			_newDraggingStateVm = new StateVm(stateModel, this)
			{
				Opacity = 0.4d,
			};
			_newDraggingStateVm.Config = new StateConfigVm(_newDraggingStateVm);
			this.States.Add(_newDraggingStateVm);
			DragTarget = _newDraggingStateVm;
			RelativeDragPoint = new Point(50, 20);
		}
		//MainToolbox item : Connector
		public bool ToolConnector
		{
			get { return (bool)GetValue(ToolConnectorProperty); }
			set { SetValue(ToolConnectorProperty, value); }
		}
		public static readonly DependencyProperty ToolConnectorProperty =
			DependencyProperty.Register("ToolConnector", typeof(bool), typeof(FpcWindowVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				(d as FpcWindowVm).RemoveHalfDrawnConnector();
				if (((FpcWindowVm)d)._lock) return;
				((FpcWindowVm)d)._lock = true;
				if ((bool)e.NewValue)
				{
					if ((bool)d.GetValue(ToolStateProperty)) d.SetValue(ToolStateProperty, false);
					if ((bool)d.GetValue(ToolSelectionProperty)) d.SetValue(ToolSelectionProperty, false);
				}
				else if ((bool)e.OldValue) d.SetValue(ToolConnectorProperty, true);
				((FpcWindowVm)d)._lock = false;
			}));
		#endregion

		//Message Dependency Property
		public DependencyMessageBox Message
		{
			get { return (DependencyMessageBox)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(DependencyMessageBox), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		#region Select and focus
		/// <summary>
		/// Gets FocusedState or 
		/// <para>Sets FocusedState and FocusedStateStation and selects a Station</para>
		/// </summary>
		public StateVm FocusedState
		{
			get { return (StateVm)GetValue(FocusedStateProperty); }
			set { SetValue(FocusedStateProperty, value); }
		}
		public static readonly DependencyProperty FocusedStateProperty =
			DependencyProperty.Register("FocusedState", typeof(StateVm), typeof(FpcWindowVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as FpcWindowVm;
				var state = e.NewValue as StateVm;
				if (state == null) { vm.FocusedStateStation = null; return; }
				if (state.Config == null) { vm.FocusedStateStation = null; return; }
				var station = state.Config.ContentsList.FirstOrDefault(x => x.IsExpanded);
				vm.FocusedStateStation = station as StateStationVm;
				vm.OnStationSelected(vm.FocusedStateStation);
			}));
		/// <summary>
		/// Gets FocusedStateStation or 
		/// <para>Sets FocusedStateStation and FocusedState and selects a Station</para>
		/// </summary>
		public StateStationVm FocusedStateStation
		{
			get { return (StateStationVm)GetValue(FocusedStateStationProperty); }
			set { SetValue(FocusedStateStationProperty, value); }
		}
		public static readonly DependencyProperty FocusedStateStationProperty =
			DependencyProperty.Register("FocusedStateStation", typeof(StateStationVm), typeof(FpcWindowVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as FpcWindowVm;
				var ss = e.NewValue as StateStationVm;
				if (ss == null) return;
				var s = (ss.Container as StateConfigVm).State;
				if (s != vm.FocusedState)
					(d as FpcWindowVm).FocusedState = s;
				vm.OnStationSelected(ss);
			}));
		/// <summary>
		/// Updates the OnScreenToolbox according to specified stateStation
		/// </summary>
		/// <param name="ss"></param>
		public void OnStationSelected(StateStationVm ss)
		{
			MachineFamilies.Clear();
			if (ss == null) return;
			foreach (var item in (ss.Containment as StationVm).StationMachines)
			{
				//find existing family
				var family = MachineFamilies.FirstOrDefault(x => x.Id == item.Machine.Family.Id);
				if (family == null)
					MachineFamilies.Add(item.Machine.Family);
				else if (!family.Machines.Any(x => x.Id == item.Machine.Id))
					family.Machines.Add(item.Machine);
			}
		}

		#endregion

		#region Commands and etc
		/// <summary>
		/// Initialize Commands
		/// </summary>
		private void initCommands()
		{
			ExpandAllCommand = new Commands.Command(o =>
			{
				var items = States.Where(x => x.StateType == StateType.Mid);
				foreach (var item in items)
				{
					item.ShowDetails = true;
				}
			});
			CollapseAllCommand = new Commands.Command(o =>
			{
				foreach (var item in States.Where(x => x.StateType == StateType.Mid))
				{
					item.ShowDetails = false;
				}
			});
			SaveAllCommand = new Commands.Command(o =>
			{
				try
				{
					foreach (var state in States)
					{
						state.PromptSave();
					}
					fpcDataService.ApplyChanges();
					foreach (var state in States)
					{
						state.IsChanged = false;
					}

					//check for tracability
					Soheil.Core.PP.Smart.SmartJob.AutoRouteCheck(this.Id);
					//always throws, so no after lines
				}
				catch (SoheilExceptionBase exp)
				{
					string msg = "";
					if (exp.Level == ExceptionLevel.Error)
						msg += "FPC تعریف شده قابل مسیریابی نمی باشد.\nدر صورتی که می خواهید از قابلیت افزودن خودکار Job استفاده نمایید بایستی FPC را اصلاح کنید.\n";
					msg += exp.Message;
					Message = new DependencyMessageBox(msg, exp.Caption, MessageBoxButton.OK, exp.Level);
					//this is actual after line for _fpcDataService.ApplyChanges(this); because it throws
					if (exp.Level == ExceptionLevel.Info)
						foreach (var state in States)
							state.IsChanged = false;
				}
				catch (Exception exp)
				{
					Message = new DependencyMessageBox(
						exp,
						"در ذخیره سازی خطای پیش آمد.",
						"خطا",
						MessageBoxButton.OK,
						ExceptionLevel.Error);
				}
			});
		}

		//FpcVm will call this method
		public override void IsDefaultChanged(bool newValue)
		{
			try
			{
				fpcDataService.ChangeDefault(Model, newValue);
			}
			catch (Exception exp)
			{
				Message = new DependencyMessageBox(exp.Message);
			}
		}
		/*//Check For being Tracable
		public Commands.Command CheckForTracableCommand
		{
			get { return (Commands.Command)GetValue(CheckForTracableCommandProperty); }
			set { SetValue(CheckForTracableCommandProperty, value); }
		}
		public static readonly DependencyProperty CheckForTracableCommandProperty =
			DependencyProperty.Register("CheckForTracableCommand", typeof(Commands.Command), typeof(FpcWindowVm), new PropertyMetadata(null));
		*/

		//Save All
		public Commands.Command SaveAllCommand
		{
			get { return (Commands.Command)GetValue(SaveAllCommandProperty); }
			set { SetValue(SaveAllCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllCommandProperty =
			DependencyProperty.Register("SaveAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new PropertyMetadata(null));
		
		//ExpandAllCommand Dependency Property
		public Commands.Command ExpandAllCommand
		{
			get { return (Commands.Command)GetValue(ExpandAllCommandProperty); }
			set { SetValue(ExpandAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ExpandAllCommandProperty =
			DependencyProperty.Register("ExpandAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new UIPropertyMetadata(null));
		//CollapseAllCommand Dependency Property
		public Commands.Command CollapseAllCommand
		{
			get { return (Commands.Command)GetValue(CollapseAllCommandProperty); }
			set { SetValue(CollapseAllCommandProperty, value); }
		}
		public static readonly DependencyProperty CollapseAllCommandProperty =
			DependencyProperty.Register("CollapseAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		#endregion


		#region Mouse props
		/// <summary>
		/// current dragged connector, state or toolbox item
		/// </summary>
		public DragTarget DragTarget { get; protected set; }
		/// <summary>
		/// some drag point??? related to drag target
		/// </summary>
		public Point RelativeDragPoint { get; protected set; }
		/// <summary>
		/// The new state where the End-point of current incomplete connector enters
		/// </summary>
		StateVm _connectorDropTargetStateVm;
		/// <summary>
		/// A new state when not placed yet
		/// </summary>
		StateVm _newDraggingStateVm;
		/// <summary>
		/// Initial location of mouse on the drag target visual
		/// </summary>
		Point _initialDragPoint;
		/// <summary>
		/// A TreeItemVm temporarily added to TreeItemVm container willing to accept current OnScreenToolbox item
		/// </summary>
		TreeItemVm _dropIndicator;

		#endregion

		#region Mouse methods
		public void MouseEntersState(StateVm state)
		{
			var dt = DragTarget as StateVm;
			if (dt == null) return;
			if (dt.StateType != StateType.Temp) return;

			var conn = Connectors.FirstOrDefault(x => x.End == dt);
			if (conn != null)
			{
				if (state != conn.Start && !Connectors.Any(x => x.End == state && x.Start == conn.Start))
				{
					conn.IsLoose = false;
					_connectorDropTargetStateVm = state;
				}
			}
		}
		public void MouseLeavesState(StateVm state)
		{
			var dt = DragTarget as StateVm;
			if (dt == null) return;
			if (dt.StateType != StateType.Temp) return;

			var conn = Connectors.FirstOrDefault(x => x.End == dt);
			if (conn != null)
			{
				conn.IsLoose = true;
				_connectorDropTargetStateVm = null;
			}
		}
		public void MouseClicksState(StateVm state, Point pos_drawingArea, Point pos_firstChild, Point pos_canvas)
		{
			FocusedState = state;
			//new state is dragging
			if (state == _newDraggingStateVm)
			{
				state.PlaceInFpc();
				_newDraggingStateVm = null;
				ToolSelection = true;
			}
			else
			{
				//connector is dragging
				if (ToolConnector)
				{
					RemoveHalfDrawnConnector();
					
					//create a temporary state attached to the end of connector
					var end = new StateVm(new Model.State
					{
						X = (float)pos_drawingArea.X,
						Y = (float)pos_drawingArea.Y,
					}, 
					this, true);

					DragTarget = end;
					States.Add(end);
					Connectors.Add(new ConnectorVm(null, state, end, fpcDataService.connectorDataService, true));
					RelativeDragPoint = new Point(0, 0);
				}
				//state is dragging
				else
				{
					DragTarget = state;
					RelativeDragPoint = pos_firstChild;
				}
			}
			_initialDragPoint = pos_canvas;
		}

		/// <summary>
		/// View calls this method when any kind of mouse release happens (drag&drop|click&release)
		/// </summary>
		/// <param name="curPos">release point</param>
		public void ReleaseDragAt(Point curPos)
		{
			if (DragTarget is StateVm)
			{
				var draggedState = DragTarget as StateVm;

				//click-released
				if (Math.Abs(curPos.X - _initialDragPoint.X) < 2 && Math.Abs(curPos.Y - _initialDragPoint.Y) < 2)
				{
					//if midState is clicked
					if (draggedState.StateType == StateType.Mid)
					{
						if (!draggedState.ShowDetails)
						{
							draggedState.ShowDetails = true;
						}
					}
				}

				//drag-released
				else
				{
					//if connector is dragging
					if (draggedState.StateType == StateType.Temp)
					{
						var conn = Connectors.FirstOrDefault(x => x.End == draggedState);
						if (conn != null)
						{
							//if can add this connector to fpc
							if (!conn.IsLoose)
							{
								States.Remove(draggedState);
								conn.End = _connectorDropTargetStateVm;
								fpcDataService.connectorDataService.AddConnector(conn.Start.Id, conn.End.Id);
							}
						}
					}
					RemoveHalfDrawnConnector();
				}
			}
			DragTarget = null;
		}

		public void UpdateDropIndicator(Point mouse)
		{
			TreeItemVm item = null;
			if (SelectedToolboxItem.ContentData is StationVm)
				item = SelectedToolboxItem.GetUnderlyingStateConfig(mouse);
			else if (SelectedToolboxItem.ContentData is ActivityVm)
				item = SelectedToolboxItem.GetUnderlyingStateStation(mouse);
			else if (SelectedToolboxItem.ContentData is MachineVm)
				item = SelectedToolboxItem.GetUnderlyingStateStationActivity(mouse);
			//if can't drop
			if (item == null)
			{
				SelectedToolboxItem.CanDrop = false;
				RemoveDropIndicator();
			}
			//if can drop (transition from can't drop)
			else if (!item.ContentsList.Any(x => x.IsDropIndicator))
			{
				SelectedToolboxItem.CanDrop = true;
				_dropIndicator = new DropIndicatorVm(this, item, SelectedToolboxItem.ContentData);
				item.ContentsList.Add(_dropIndicator);
			}
		}

		#endregion

		#region Remove extra visuals (incomplete connectors or temp states)
		/// <summary>
		/// Removes the current dotted line (an incomplete connector)
		/// </summary>
		public void RemoveHalfDrawnConnector()
		{
			var conn = Connectors.Where(x => x.End.StateType == StateType.Temp).FirstOrDefault();
			if (conn == null) return;
			States.Remove(conn.End);
			Connectors.Remove(conn);
			_connectorDropTargetStateVm = null;
		}
		/// <summary>
		/// Removes the hollow state (Status=temp) from the end of the current connector
		/// </summary>
		public void RemoveNewDraggingState()
		{
			if (_newDraggingStateVm != null)
				if (States.Contains(_newDraggingStateVm))
					States.Remove(_newDraggingStateVm);
		}
		/// <summary>
		/// Stops the current toolbox item from getting dragged
		/// </summary>
		public void StopDragToolboxItem()
		{
			SelectedToolboxItem = null;
			RemoveDropIndicator();
		}
		/// <summary>
		/// Removes the current drop indicator from its container
		/// </summary>
		public void RemoveDropIndicator()
		{
			if (_dropIndicator != null)
			{
				_dropIndicator.Container.ContentsList.Remove(_dropIndicator);
				_dropIndicator = null;
			}
		}
		#endregion


		#region Add new State
		internal delegate void SelectStateEventHandler(StateVm state);
		internal event SelectStateEventHandler SelectState;
		/// <summary>
		/// Fires SelectState event (for use in PPTaskEditor)
		/// </summary>
		/// <param name="state"></param>
		public void FireSelectState(StateVm state)
		{
			if (SelectState != null) SelectState(state);
		}
		#endregion
	}
}