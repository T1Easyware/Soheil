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
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.Fpc
{
	public class FpcWindowVm : ViewModelBase
	{
		/// <summary>
		/// Gets or sets a bindable value that indicates Access
		/// </summary>
		public AccessType Access
		{
			get { return (AccessType)GetValue(AccessProperty); }
			set { SetValue(AccessProperty, value); }
		}
		public static readonly DependencyProperty AccessProperty =
			DependencyProperty.Register("Access", typeof(AccessType), typeof(FpcWindowVm), new PropertyMetadata(AccessType.None));


		/// <summary>
		/// Gets the DataService instance for this fpc
		/// </summary>
		public FPCDataService fpcDataService { get; protected set; }

		/// <summary>
		/// Gets the Model for this fpc
		/// </summary>
		public Model.FPC Model { get; protected set; }

		/// <summary>
		/// Gets the Id for this fpc
		/// </summary>
		public int Id { get { return Model == null ? -1 : Model.Id; } }

		/// <summary>
		/// Occurs when a state is selected
		/// </summary>
		internal event Action<StateVm> SelectedStateChanged;

		/// <summary>
		/// Gets a value indicating whether this view model is readonly
		/// </summary>
		public bool IsReadonly { get; private set; }

		/// <summary>
		/// Gets or sets a value that indicated whether the location of states are readonly
		/// </summary>
		public bool IsLocationsLocked
		{
			get { return (bool)GetValue(IsLocationsLockedProperty); }
			set { SetValue(IsLocationsLockedProperty, value); }
		}
		public static readonly DependencyProperty IsLocationsLockedProperty =
			DependencyProperty.Register("IsLocationsLocked", typeof(bool), typeof(FpcWindowVm), new UIPropertyMetadata(false));

		#region Ctor, ChangeFPC & Reset
		/// <summary>
		/// Creates an instance of FpcWindowVm with its own Unit of work
		/// </summary>
		/// <param name="isReadonly">optional parameter to make the viewmodel readonly</param>
		public FpcWindowVm(bool isReadonly = false)
			: base()
		{
			initCommands();
			initStates();
			IsReadonly = isReadonly;
		}

		/// <summary>
		/// Initializes States and Connectors collections to automatically do some stuff for FpcWindowVm
		/// </summary>
		void initStates()
		{
			States.CollectionChanged += (s, e) =>
			{
				HasStates = States.Any();
				//new items inform FpcWindowVm upon selection
				if (e.NewItems != null)
					foreach (StateVm newItem in e.NewItems)
					{
						newItem.StateSelected += () =>
						{
							//fire SelectedStateChanged event
							if (SelectedStateChanged != null)
								SelectedStateChanged(newItem);
						};
						newItem.StateDeleted += (connectors) =>
						{
							States.Remove(newItem);
							foreach (var connector in connectors)
							{
								Connectors.Remove(connector);
							}
						};
						newItem.ModelChanged += () =>
						{
							if (newItem.IsPlaced)
								fpcDataService.ApplyChanges();
						};
					}
			};
			Connectors.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
					foreach (ConnectorVm newItem in e.NewItems)
					{
						newItem.ConnectorRemoved += () =>
						{
							Connectors.Remove(newItem);
						};
					}
			};
		}

		/// <summary>
		/// Updates ViewModel by the given product's active fpc
		/// </summary>
		/// <param name="productId"></param>
		public void ChangeFpcByProductId(int productId)
		{
			//clear all fpc data from the view model and reload stations and activities
			ResetFPC();
			//set model
			Model = fpcDataService.GetActiveForProduct(productId);
			//update view model from model
			ChangeFpcByModel();
		}

		/// <summary>
		/// Updates the ViewModel to FPC Model with the given Id
		/// </summary>
		/// <param name="id">The id of the FPC Model which is used for updating the FpcWindowVm</param>
		public void ChangeFpcByFpcId(int fpcId, AccessType access)
		{
			Access = access;
			ShowToolbox = true;//if access denied, value will be coerced
			//clear all fpc data from the view model and reloads stations and activities
			ResetFPC();
			//set model
			Model = fpcDataService.GetSingle(fpcId);
			//update view model from model
			ChangeFpcByModel();
		}
		/// <summary>
		/// Updates ViewModel from Model
		/// </summary>
		public void ChangeFpcByModel()
		{
			try
			{
				//-----------
				//load basics
				//-----------
				IsDefault = Model.IsDefault;
				Product = new ProductVm(Model.Product);

				//load all product reworks
				var productReworkModels = fpcDataService.GetProductReworks(Model, includeMainProduct: false);
				ProductReworks.Clear();
				foreach (var prodrew in productReworkModels)
				{
					ProductReworks.Add(new ProductReworkVm(prodrew));
				}

				//Reworks
				Reworks.Clear();
				var rws = new ReworkDataService(fpcDataService.Context).GetActives();
				foreach (var item in rws)
				{
					var rewVm = new ReworkVm(item);
					if (item.ProductReworks.Any(x => x.Product.Id == Model.Product.Id))
						rewVm.IsInProduct = true;
					if (productReworkModels.Any(x => x.Rework != null && x.Rework.Id == item.Id))
						rewVm.IsInFpc = true;
					Reworks.Add(rewVm);
				}


				//-----------
				//load states
				//-----------
				fpcDataService.CorrectFPCStates(Model);
				//reload states
				var states = fpcDataService.stateDataService.GetStatesByFpcId(Id);
				//show all states
				foreach (var item in states)
				{
					var stateVm = new StateVm(item, this);
					if (!ShowFpcReworks && 
						(stateVm.IsRework || stateVm.StateType == StateType.Rework))
						stateVm.IsVisible = false;
					States.Add(stateVm);
				}
	
				//----------
				//load conns
				//----------
				var conns = fpcDataService.connectorDataService.GetByFpcId(Id);
				foreach (var item in conns)
				{
					var connVm = new ConnectorVm(item,
						States.FirstOrDefault(x => x.Id == item.StartState.Id),
						States.FirstOrDefault(x => x.Id == item.EndState.Id),
						fpcDataService.connectorDataService);
					if(!ShowFpcReworks && 
						(connVm.Start.IsRework || connVm.Start.StateType == StateType.Rework
						|| connVm.End.IsRework || connVm.End.StateType == StateType.Rework))
						connVm.IsVisible = false;
					Connectors.Add(connVm);
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

			//select the first mid-state
			if (States.Count(x => x.StateType == StateType.Mid) > 0)
				FocusedState = States.Where(x => x.StateType == StateType.Mid).First();
		}

		/// <summary>
		/// Clears the states and connectors and reloads dataservice, stations and activities
		/// </summary>
		/// <param name="clearModel">specify 'true' to create a new model as well</param>
		public void ResetFPC(bool clearModel = false)
		{
			_lock = false;
			Message = new DependencyMessageBox();

			fpcDataService = new FPCDataService();

			//Stations
			Stations.Clear();
			var stations = fpcDataService.stationDataService.GetActives();
			foreach (var item in stations)
				Stations.Add(new StationVm(item));

			//ActivityGroups
			ActivityGroups.Clear();
			var actgs = fpcDataService.activityGroupDataService.GetActives();
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

		#region Props and lists
		/// <summary>
		/// Gets a bindable vm for product of this fpc
		/// </summary>
		public ProductVm Product
		{
			get { return (ProductVm)GetValue(ProductProperty); }
			protected set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(ProductVm), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this fpc is default fpc for this product
		/// <para>Changing the value calls ChangeDefault method of fpcDataService</para>
		/// </summary>
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(FpcWindowVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				var vm = (FpcWindowVm)d;
				try
				{
					vm.fpcDataService.ChangeDefault(vm.Model, (bool)e.NewValue);
				}
				catch (Exception exp)
				{
					vm.Message = new DependencyMessageBox(exp.Message);
				}
			}));
		/// <summary>
		/// Gets a bindable value that indicates whether this fpc has any states
		/// </summary>
		public bool HasStates
		{
			get { return (bool)GetValue(HasStatesProperty); }
			private set { SetValue(HasStatesProperty, value); }
		}
		public static readonly DependencyProperty HasStatesProperty =
			DependencyProperty.Register("HasStates", typeof(bool), typeof(FpcWindowVm), new UIPropertyMetadata(false));

		/// <summary>
		/// Gets or sets the current bindable message box
		/// </summary>
		public DependencyMessageBox Message
		{
			get { return (DependencyMessageBox)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(DependencyMessageBox), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable collection of Connectors in this fpc
		/// </summary>
		public ObservableCollection<ConnectorVm> Connectors { get { return _connectors; } }
		private ObservableCollection<ConnectorVm> _connectors = new ObservableCollection<ConnectorVm>();
		/// <summary>
		/// Gets a bindable collection of States in this fpc
		/// </summary>
		public ObservableCollection<StateVm> States { get { return _states; } }
		private ObservableCollection<StateVm> _states = new ObservableCollection<StateVm>();
		/// <summary>
		/// Gets a bindable collection of all possible ProductReworks related to this fpc
		/// </summary>
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();
		#endregion

		#region OnScreenToolbox (stations, activities, machines)
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowToolbox
		/// </summary>
		public bool ShowToolbox
		{
			get { return (bool)GetValue(ShowToolboxProperty); }
			set { SetValue(ShowToolboxProperty, value); }
		}
		public static readonly DependencyProperty ShowToolboxProperty =
			DependencyProperty.Register("ShowToolbox", typeof(bool), typeof(FpcWindowVm), new PropertyMetadata(false, (d, e) => { }, (d, v) =>
			{
				var vm = (FpcWindowVm)d;
				var val = (bool)v;
				if ((vm.Access & AccessType.Update) == AccessType.Update) return v;
				else return false;
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowAllMachines
		/// </summary>
		public bool ShowAllMachines
		{
			get { return (bool)GetValue(ShowAllMachinesProperty); }
			set { SetValue(ShowAllMachinesProperty, value); }
		}
		public static readonly DependencyProperty ShowAllMachinesProperty =
			DependencyProperty.Register("ShowAllMachines", typeof(bool), typeof(FpcWindowVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (FpcWindowVm)d;
				if ((bool)e.NewValue)
				{
					IEnumerable<Model.MachineFamily> list;
					list = new DataServices.MachineFamilyDataService(vm.fpcDataService.Context).GetActives();
					vm._allMachineFamilies = new List<MachineFamilyVm>();
					foreach (var mf in list)
					{
						var mfvm = new MachineFamilyVm(mf);
						foreach (var m in mf.Machines.Where(x => x.Status == (byte)Status.Active))
						{
							mfvm.Machines.Add(new MachineVm(m, mfvm));
						}
						vm._allMachineFamilies.Add(mfvm);
					}
				}
				vm.OnStationSelected(vm.FocusedStateStation);
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates ShowFpcReworks
		/// </summary>
		public bool ShowFpcReworks
		{
			get { return (bool)GetValue(ShowFpcReworksProperty); }
			set { SetValue(ShowFpcReworksProperty, value); }
		}
		public static readonly DependencyProperty ShowFpcReworksProperty =
			DependencyProperty.Register("ShowFpcReworks", typeof(bool), typeof(FpcWindowVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (FpcWindowVm)d;
				var val = (bool)e.NewValue;
				//find rework states
				foreach (var state in vm.States.Where(x => x.IsRework || x.StateType == StateType.Rework))
				{
					state.IsVisible = val;
				}
				//find rework connectors
				foreach (var conn in vm.Connectors.Where(x => !x.IsLoose && 
					(x.Start.IsRework || x.Start.StateType == StateType.Rework 
					|| x.End.IsRework || x.End.StateType == StateType.Rework)))
				{
					conn.IsVisible = val;
				}
			}));

		public void AddRework(ToolboxItemVm item)
		{
			var rewVm = item.ContentData as ReworkVm;
			if (rewVm == null) return;

			//if(rewVm.IsInFpc)
			if (Model.States.Any(x =>
					x.StateTypeNr == (int)StateType.Rework
					&& x.OnProductRework != null
					&& x.OnProductRework.Rework != null
					&& x.OnProductRework.Rework.Id == rewVm.Id))
			{
				Message = new DependencyMessageBox("این دوباره کاری در FPC موجود است", "توضیحات", MessageBoxButton.OK, ExceptionLevel.Info);
				return;
			}

			//if(!rewVm.IsInProduct)
			//add to product
			Model.ProductRework productRework;
			if (!Model.Product.ProductReworks.Any(x => x.Rework != null && x.Rework.Id == rewVm.Id))
			{
				productRework = new Model.ProductRework
				{
					Rework = new ReworkDataService(fpcDataService.Context).GetSingle(rewVm.Id),
					Product = new ProductDataService(fpcDataService.Context).GetSingle(Model.Product.Id),
					Name = rewVm.Name,
					Code = rewVm.Code,
					ModifiedBy = LoginInfo.Id,
				};
				new ProductReworkDataService(fpcDataService.Context).AddModel(productRework);
			}
			else
				productRework = rewVm.Model.ProductReworks.FirstOrDefault(x => x.Product.Id == Model.Product.Id);

			//add to fpc
			var rewState = new Model.State
			{
				FPC = Model,
				X = (float)item.Location.X,
				Y = (float)item.Location.Y,
				Name = rewVm.Name,
				Code = rewVm.Code,
				OnProductRework = productRework,
				StateType = StateType.Rework,
			};
			fpcDataService.stateDataService.AddModel(rewState);

			rewVm.IsInProduct = true;
			rewVm.IsInFpc = true;

			//add state vm
			var rewStateVm = new StateVm(rewState, this);
			States.Add(rewStateVm);
		}


		/// <summary>
		/// The bindable clone of a ToolboxItem which is being dragged by the mouse
		/// </summary>
		public ToolboxItemVm SelectedToolboxItem
		{
			get { return (ToolboxItemVm)GetValue(SelectedToolboxItemProperty); }
			set { SetValue(SelectedToolboxItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedToolboxItemProperty =
			DependencyProperty.Register("SelectedToolboxItem", typeof(ToolboxItemVm), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the bindable text for filtering the machine list
		/// </summary>
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
					val = val.ToUpper();
					foreach (var mf in vm.MachineFamilies)
					{
						bool isAnyVisible = false;
						foreach (var m in mf.Machines)
						{
							if (m.Name.ToUpper().Contains(val) || m.Code.ToUpper().StartsWith(val))
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

		/// <summary>
		/// Gets or sets the bindable text for filtering the activity list
		/// </summary>
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
					val = val.ToUpper();
					foreach (var ag in vm.ActivityGroups)
					{
						bool isAnyVisible = false;
						foreach (var a in ag.Activities)
						{
							if (a.Name.ToUpper().Contains(val) || a.Model.Code.ToUpper().StartsWith(val))
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

		/// <summary>
		/// Gets a bindable collection of all Stations
		/// </summary>
		public ObservableCollection<StationVm> Stations { get { return _stations; } }
		private ObservableCollection<StationVm> _stations = new ObservableCollection<StationVm>();
		/// <summary>
		/// Gets a bindable collection of all ActivityGroups with their Activities
		/// </summary>
		public ObservableCollection<ActivityGroupVm> ActivityGroups { get { return _activityGroups; } }
		private ObservableCollection<ActivityGroupVm> _activityGroups = new ObservableCollection<ActivityGroupVm>();
		/// <summary>
		/// Gets a bindable collection of all MachineFamilies with their Machines
		/// </summary>
		public ObservableCollection<MachineFamilyVm> MachineFamilies { get { return _machineFamilies; } }
		private ObservableCollection<MachineFamilyVm> _machineFamilies = new ObservableCollection<MachineFamilyVm>();
		List<MachineFamilyVm> _allMachineFamilies;

		/// <summary>
		/// Gets a bindable collection of all Reworks representing ProductReworks of this FPC
		/// </summary>
		public ObservableCollection<ReworkVm> Reworks { get { return _reworks; } }
		private ObservableCollection<ReworkVm> _reworks = new ObservableCollection<ReworkVm>();
		#endregion

		#region Menu bar items
		//Zoom Dependency Property
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register("Zoom", typeof(double), typeof(FpcWindowVm), new UIPropertyMetadata(1d, (d, e) => { }, (d, v) =>
				{
					var val = (double)v;
					if (val < _minZoom) return _minZoom;
					if (val > _maxZoom) return _maxZoom;
					return v;
				}));
		private const double _minZoom = 0.1d;
		private const double _maxZoom = 3d;

		/// <summary>
		/// locks on one of toolbar menu items (selection, states, connectors) while deselecting the other two
		/// </summary>
		bool _lock;
		/// <summary>
		/// Gets or sets a bindable value that indicates whether 'Selection' is selected in Toolbar menu
		/// <para>Selecting this item deselects other toolbar menu items</para>
		/// </summary>
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
		/// <summary>
		/// Gets or sets a bindable value that indicates whether 'State' is selected in Toolbar menu
		/// <para>Selecting this item deselects other toolbar menu items</para>
		/// </summary>
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
		/// <summary>
		/// Adds a state to this fpc (both model and viewModel) and stick it to mouse
		/// </summary>
		private void addStateByTool()
		{
			var stateModel = new Model.State
			{
				StateType = StateType.Mid,
				X = -50,
				Y = -20,
				FPC = Model,
			};
			this.Model.States.Add(stateModel);

			_newDraggingStateVm = new StateVm(stateModel, this, false)
			{
				Opacity = 0.4d,
			};
			_newDraggingStateVm.Config = new StateConfigVm(_newDraggingStateVm);
			this.States.Add(_newDraggingStateVm);
			DragTarget = _newDraggingStateVm;
			RelativeDragPoint = new Point(50, 20);
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates whether 'Connector' is selected in Toolbar menu
		/// <para>Selecting this item deselects other toolbar menu items</para>
		/// </summary>
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


		#region Select and focus
		/// <summary>
		/// Gets or sets a bindable value for FocusedState
		/// <para>Changing this value, automatically changes FocusedStateStation to match the first expanded station</para>
		/// <para>If no station is expanded, expands the first station</para>
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
				//find and change the focues station
				var station = state.Config.ContentsList.FirstOrDefault(x => x.IsExpanded);
				if (station == null)
				{
					station = state.Config.ContentsList.FirstOrDefault();
					if (station != null) station.IsExpanded = true;
				}
				vm.FocusedStateStation = station as StateStationVm;
			}));
		/// <summary>
		/// Gets or sets a bindable value for FocusedStateStation
		/// <para>Changing this value, automatically updates machines list</para>
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
				//make sure FocusedState is correct
				if (s != vm.FocusedState)
					(d as FpcWindowVm).FocusedState = s;
				//reload machines
				vm.OnStationSelected(ss);
			}));
		/// <summary>
		/// Updates the Machines list of Toolbox according to specified stateStation
		/// </summary>
		/// <param name="ss"></param>
		public void OnStationSelected(StateStationVm ss)
		{
			MachineFamilies.Clear();
			if (ss == null) return;
			if (ShowAllMachines)
			{
				foreach (var mf in _allMachineFamilies)
				{
					MachineFamilies.Add(mf);
				}
			}
			else
			{
				foreach (var item in (ss.Containment as StationVm).StationMachines)
				{
					//find existing family
					var family = MachineFamilies.FirstOrDefault(x => x.Id == item.Machine.Family.Id);

					//if family is not yet added, clear its machines and then add it
					if (family == null)
					{
						family = item.Machine.Family;
						family.Machines.Clear();
						MachineFamilies.Add(family);
					}

					//make sure the machine exists in that family
					if (!family.Machines.Any(x => x.Id == item.Machine.Id))
					{
						family.Machines.Add(item.Machine);
					}
				}
			}
		}

		#endregion

		#region Commands
		/// <summary>
		/// Initialize Commands
		/// </summary>
		private void initCommands()
		{
			SaveAllCommand = new Commands.Command(o =>
			{
				try
				{
					foreach (var state in States)
					{
						state.PromptSave();
					}
					fpcDataService.ApplyChanges();

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
			ResetZoomCommand = new Commands.Command(o => Zoom = 1d);
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

		/// <summary>
		/// Gets or sets a bindable command to save everything in this fpc
		/// </summary>
		public Commands.Command SaveAllCommand
		{
			get { return (Commands.Command)GetValue(SaveAllCommandProperty); }
			set { SetValue(SaveAllCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllCommandProperty =
			DependencyProperty.Register("SaveAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to expand all states in this fpc
		/// </summary>
		public Commands.Command ExpandAllCommand
		{
			get { return (Commands.Command)GetValue(ExpandAllCommandProperty); }
			set { SetValue(ExpandAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ExpandAllCommandProperty =
			DependencyProperty.Register("ExpandAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to collapse all states in this fpc
		/// </summary>
		public Commands.Command CollapseAllCommand
		{
			get { return (Commands.Command)GetValue(CollapseAllCommandProperty); }
			set { SetValue(CollapseAllCommandProperty, value); }
		}
		public static readonly DependencyProperty CollapseAllCommandProperty =
			DependencyProperty.Register("CollapseAllCommand", typeof(Commands.Command), typeof(FpcWindowVm), new UIPropertyMetadata(null));

		//ResetZoomCommand Dependency Property
		public Commands.Command ResetZoomCommand
		{
			get { return (Commands.Command)GetValue(ResetZoomCommandProperty); }
			set { SetValue(ResetZoomCommandProperty, value); }
		}
		public static readonly DependencyProperty ResetZoomCommandProperty =
			DependencyProperty.Register("ResetZoomCommand", typeof(Commands.Command), typeof(FpcWindowVm), new UIPropertyMetadata(null));
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
		/// <summary>
		/// Connects the dragging connector to the specified state
		/// <para>Exits if no connector is being dragged</para>
		/// <remarks>
		/// By 'dragging connector', we mean dragging state with StateType=Temp which holds the end of the connector
		/// </remarks>
		/// </summary>
		/// <param name="state">Target state as the end state for the connector</param>
		public void MouseEntersState(StateVm state)
		{
			//find the drag target (temp state)
			var dt = DragTarget as StateVm;
			if (dt == null) return;
			if (dt.StateType != StateType.Temp) return;

			//find the connector that is being dragged
			var conn = Connectors.FirstOrDefault(x => x.End == dt);
			if (conn != null)
			{
				//if not making a loop or repeating an existing connector
				if (state != conn.Start && !Connectors.Any(x => x.End == state && x.Start == conn.Start))
				{
					//mark the connector to be attached
					conn.IsLoose = false;
					_connectorDropTargetStateVm = state;
				}
			}
		}
		/// <summary>
		/// Disconnects the dragging connector from the specified state
		/// <para>Exits if no connector is being dragged</para>
		/// <remarks>
		/// By 'dragging connector', we mean dragging state with StateType=Temp which holds the end of the connector
		/// </remarks>
		/// </summary>
		/// <param name="state">Target state as the end state for the connector</param>
		public void MouseLeavesState(StateVm state)
		{
			//find the drag target (temp state)
			var dt = DragTarget as StateVm;
			if (dt == null) return;
			if (dt.StateType != StateType.Temp) return;

			//find the connector that is being dragged
			var conn = Connectors.FirstOrDefault(x => x.End == dt);
			if (conn != null)
			{
				//make the connector loose again
				conn.IsLoose = true;
				_connectorDropTargetStateVm = null;
			}
		}
		/// <summary>
		/// Sets FocusedState to the specified state and do the following actions:
		/// <para>Place the state into FpcWindowVm if it's not placed yet, Otherwise start dragging the state</para>
		/// <para>Start drawing a connector if 'connector' is selected from menu bar</para>
		/// </summary>
		/// <param name="state"></param>
		/// <param name="pos_drawingArea">location of mouse relative to drawing area</param>
		/// <param name="pos_firstChild">location of mouse relative to first child of state</param>
		/// <param name="pos_state">location of mouse relative to state</param>
		public void MouseClicksState(StateVm state, Point pos_drawingArea, Point pos_firstChild, Point pos_state)
		{
			FocusedState = state;
			//place the newly created state which was dragging
			if (state == _newDraggingStateVm)
			{
				state.PlaceInFpc();
				_newDraggingStateVm = null;
				ToolSelection = true;
			}
			else
			{
				_initialDragPoint = pos_state;
				//connector is dragging
				if (ToolConnector)
				{
					RemoveHalfDrawnConnector();

					//create a temporary state and attach it to the end of connector
					var end = StateVm.CreateTemp(pos_drawingArea.X, pos_drawingArea.Y, this);
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
		}

		/// <summary>
		/// Expand the DragTarget or attach the dragging connector to _connectorDropTargetStateVm
		/// <remarks>DragTarget must be a StateVm (Action is chosen based on its StateType)</remarks>
		/// </summary>
		/// <param name="pos_drawingArea">location of mouse relative to drawing area</param>
		public void ReleaseDragAt(Point pos_drawingArea)
		{
			var draggedState = DragTarget as StateVm;
			if (draggedState != null)
			{
				//if not moved a lot then it's a click-released
				if (Math.Abs(pos_drawingArea.X - _initialDragPoint.X) < 2 && Math.Abs(pos_drawingArea.Y - _initialDragPoint.Y) < 2)
				{
					//show details if midState is clicked
					if (draggedState.StateType == StateType.Mid)
					{
						if (!draggedState.ShowDetails)
						{
							draggedState.ShowDetails = true;
						}
					}
				}

				//else it's a drag-released
				else
				{
					//if connector is dragging
					if (draggedState.StateType == StateType.Temp)
					{
						var conn = Connectors.FirstOrDefault(x => x.End == draggedState);
						if (conn != null)
						{
							//if connector isn't loose (ready to add) it can be added to fpc
							if (!conn.IsLoose)
							{
								//remove temp state
								States.Remove(draggedState);
								//attach the end of the connector
								conn.End = _connectorDropTargetStateVm;
								//add the connector to database and save
								fpcDataService.connectorDataService.AddConnector(conn.Start.Id, conn.End.Id);
							}
						}
					}
					//remove the dotted connector
					RemoveHalfDrawnConnector();
				}
			}
			//undrag DragTarget
			DragTarget = null;
		}



		#endregion

		#region Extra visuals (DropIndicator, incomplete connectors or temp states)
		/// <summary>
		/// Adds or removes the drap indicator object according to the tree item under the mouse
		/// </summary>
		/// <param name="pos_drawingArea">relative location of mouse in drawing area</param>
		public void UpdateDropIndicator(Point pos_drawingArea)
		{
			//find the underlying tree item
			TreeItemVm item = null;
			if (SelectedToolboxItem.ContentData is StationVm)
				item = SelectedToolboxItem.GetUnderlyingStateConfig(pos_drawingArea);
			else if (SelectedToolboxItem.ContentData is ActivityVm)
				item = SelectedToolboxItem.GetUnderlyingStateStation(pos_drawingArea);
			else if (SelectedToolboxItem.ContentData is MachineVm)
				item = SelectedToolboxItem.GetUnderlyingStateStationActivity(pos_drawingArea);
			else if (SelectedToolboxItem.ContentData is ReworkVm)
			{
				SelectedToolboxItem.CanDrop = (SelectedToolboxItem.Location.X > 10 && SelectedToolboxItem.Location.Y > 10);
				return;
			}

			//remove the indicator if can't drop
			if (item == null)
			{
				SelectedToolboxItem.CanDrop = false;
				RemoveDropIndicator();
			}

			//add an indicator if can drop (transition from can't drop)
			else if (!item.ContentsList.Any(x => x.IsDropIndicator))
			{
				SelectedToolboxItem.CanDrop = true;
				_dropIndicator = new DropIndicatorVm(this, item, SelectedToolboxItem.ContentData);
				item.ContentsList.Add(_dropIndicator);
			}
		}

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



	}
}