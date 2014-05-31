using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Core.Commands;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// View Model for State in a fpc
	/// </summary>
	public class StateVm : DragTarget
	{
		/// <summary>
		/// Occurs when this state is deleted from its fpc
		/// </summary>
		public event Action<IEnumerable<ConnectorVm>> StateDeleted;
		/// <summary>
		/// Occurs when this state is selected
		/// </summary>
		public event Action StateSelected;
		/// <summary>
		/// Occurs when model is changed but not saved
		/// </summary>
		public event Action ModelChanged;
		protected void OnModelChanged() { if (ModelChanged != null) ModelChanged(); }

		/// <summary>
		/// Gets the model for this vm
		/// </summary>
		public Model.State Model { get; private set; }
		/// <summary>
		/// Gets the Id of this state
		/// </summary>
		public override int Id { get { return Model == null ? -1 : Model.Id; } }
		/// <summary>
		/// Gets or sets the Name of this state
		/// </summary>
		public string Name
		{
			get { return Model.Name; }
			set { Model.Name = value; OnModelChanged(); OnPropertyChanged("Name"); }
		}
		/// <summary>
		/// Gets or sets the Code of this state
		/// </summary>
		public string Code
		{
			get { return Model.Code; }
			set { Model.Code = value; OnModelChanged(); OnPropertyChanged("Code"); }
		}
		/// <summary>
		/// Gets the type of this state (Start, Mid, End, Rework)
		/// <para>If model is null returns Temp</para>
		/// </summary>
		public StateType StateType
		{
			get { return Model == null ? StateType.Temp : Model.StateType; }
			private set { Model.StateType = value; OnPropertyChanged("StateType"); }
		}

		#region Ctor
		private StateVm() { }
		/// <summary>
		/// Create a temporary state just for drag and drop
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="parentWindowVm"></param>
		/// <returns></returns>
		public static StateVm CreateTemp(double x, double y, FpcWindowVm parentWindowVm)
		{
			var vm = new StateVm
			{
				InitializingPhase = true,
				ParentWindowVm = parentWindowVm,
				Width = 1,
				Height = 1,
				Location = new Vector(x, y),
				//since model is null, StateType will be Temp
			};
			return vm;
		}
		/// <summary>
		/// Creates an instance of StateVm from the given model
		/// </summary>
		/// <param name="model">Must be full of data</param>
		/// <param name="parentWindowVm">parent window vm</param>
		/// <param name="isPlaced">a value that indicates whether this state is officially a part of its fpc</param>
		public StateVm(Model.State model, FpcWindowVm parentWindowVm, bool isPlaced = true)
		{
			InitializingPhase = true;
			ParentWindowVm = parentWindowVm;
			IsPlaced = isPlaced;
			if (isPlaced) { IsFixed = model.StateStations.Any(ss => ss.Blocks.Any()); }

			initCommands();

			//fetch from db and update state basics
			Model = model;
			Name = Model.Name;
			Code = Model.Code;
			if (StateType == Common.StateType.Mid || StateType == Common.StateType.Rework)
				if (Model.OnProductRework != null)
					ProductRework = ParentWindowVm.ProductReworks.FirstOrDefault(x => x.Id == Model.OnProductRework.Id);
			Location = new Vector(model.X, model.Y);


			//fetch from db and update state config
			Config = new StateConfigVm(this);
			foreach (var ss in Model.StateStations)
			{
				var stateStation = new StateStationVm(ParentWindowVm, ss)
				{
					Container = Config,
					Containment = new StationVm(ss.Station),
				};
				foreach (var ssa in ss.StateStationActivities)
				{
					var stateStationActivity = new StateStationActivityVm(ParentWindowVm, ssa)
					{
						Container = stateStation,
						ManHour = ssa.ManHour,
						CycleTime = ssa.CycleTime,
						IsMany = ssa.IsMany,
						Containment = new ActivityVm(ssa.Activity, null),
						//CreatedDate
						//ModifiedBy
						//ModifiedDate
						//Status
					};
					foreach (var ssam in ssa.StateStationActivityMachines)
					{
						stateStationActivity.ContentsList.Add(new StateStationActivityMachineVm(ParentWindowVm, ssam)
						{
							Container = stateStationActivity,
							Containment = new MachineVm(ssam.Machine, null),
							IsDefault = ssam.IsFixed,
						});
					}
					stateStation.ContentsList.Add(stateStationActivity);
				}
				Config.ContentsList.Add(stateStation);
			}
		}

		/// <summary>
		/// Place the state into the fpc and Executes SaveCommand
		/// </summary>
		internal void PlaceInFpc()
		{
			Name = "*";
			Code = "*";
			Model.X = (float)Location.X;
			Model.Y = (float)Location.Y;
			SaveCommand.Execute(null);
			Opacity = 1;
			Config.IsExpanded = true;
			IsPlaced = true;
		}

		/// <summary>
		/// Prepares the state for save by checking ManHours of activities
		/// </summary>
		internal void PromptSave()
		{
			Model.X = (float)this.Location.X;
			Model.Y = (float)this.Location.Y;
			//check for manhour duplication for ssa's with same activity defined in a stateStation
			if (Config.ContentsList.Any(ss =>
				ss.ContentsList.GroupBy(ssa => ssa.Containment.Id)
				.Any(ssaWithSameActivity =>
					ssaWithSameActivity.GroupBy(x => ((StateStationActivityVm)x).ManHour)
					.Any(ssaWSA_grpByMH =>
					ssaWSA_grpByMH.Count() > 1))))
				throw new Soheil.Common.SoheilException.SoheilExceptionBase(
					"بعضی از فعالیتهای همسان نفرساعت یکسان دارند",
					Common.SoheilException.ExceptionLevel.Error,
					"ذخیره نشد");
		}
		#endregion

		#region Parent and Child
		/// <summary>
		/// Gets or sets a bindable value for Parent view model of this state
		/// </summary>
		public FpcWindowVm ParentWindowVm
		{
			get { return (FpcWindowVm)GetValue(ParentWindowVmProperty); }
			set { SetValue(ParentWindowVmProperty, value); }
		}
		public static readonly DependencyProperty ParentWindowVmProperty =
			DependencyProperty.Register("ParentWindowVm", typeof(FpcWindowVm), typeof(StateVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets or sets a bindable value for configuration of this state (which contains all children)
		/// </summary>
		public StateConfigVm Config
		{
			get { return (StateConfigVm)GetValue(ConfigProperty); }
			set { SetValue(ConfigProperty, value); }
		}
		public static readonly DependencyProperty ConfigProperty =
			DependencyProperty.Register("Config", typeof(StateConfigVm), typeof(StateVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this state is showing its details (<see cref="StateConfigVm"/>)
		/// </summary>
		public bool ShowDetails
		{
			get { return (bool)GetValue(ShowDetailsProperty); }
			set { SetValue(ShowDetailsProperty, value); }
		}
		public static readonly DependencyProperty ShowDetailsProperty =
			DependencyProperty.Register("ShowDetails", typeof(bool), typeof(StateVm),
			new UIPropertyMetadata(false, (d, e) => {
				var vm = (StateVm)d;
				if (vm.Config != null) vm.Config.IsExpanded = (bool)e.NewValue;
			}));
		#endregion

		#region Product Rework
		/// <summary>
		/// locks IsRework or ProductRework when other one is changing
		/// </summary>
		private bool _lockPR = false;
		/// <summary>
		/// Gets or sets a bindable value that indicates the rework on which this state is working on
		/// <para>Null if no rework is associated with this state</para>
		/// </summary>
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(StateVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (StateVm)d;
				var val = (ProductReworkVm)e.NewValue;
				if (vm.StateType != StateType.Mid) return;

				//lock PR to prevent loop in IsRework property callback
				vm._lockPR = true;
				//update IsRework property
				if (val == null) vm.IsRework = false;
				else vm.IsRework = !val.IsMainProduction;
				vm._lockPR = false;

				//update product rework in database
				AnyPropertyChangedCallback(d, e);
			}));
		
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this state is doing work on a rework
		/// </summary>
		public bool IsRework
		{
			get { return (bool)GetValue(IsReworkProperty); }
			set { SetValue(IsReworkProperty, value); }
		}
		public static readonly DependencyProperty IsReworkProperty =
			DependencyProperty.Register("IsRework", typeof(bool), typeof(StateVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (StateVm)d;
				if (vm.StateType != StateType.Mid) return;

				//exit if called from ProductRework property
				if (vm._lockPR) return;
				//otherwise update ProductRework property
				if (!(bool)e.NewValue) vm.ProductRework = null;
				else vm.ProductRework = vm.ParentWindowVm.ProductReworks.FirstOrDefault();

				//update product rework in database through ProductRework property callback
			}));
		#endregion

		#region Changes (AnyPropertyChangedCallback, InitializingPhase,...)
		/// <summary>
		/// Gets a value that indicates whether this state is officially a part of its fpc
		/// </summary>
		public bool IsPlaced { get; private set; }
		/// <summary>
		/// Gets or sets a value that indicates whether this state is in its initializing phase (loading data for the first time)
		/// </summary>
		public bool InitializingPhase { get; set; }
		/// <summary>
		/// Gets a bindable value that indicates whether this state is in use in PPTable
		/// </summary>
		public bool IsFixed
		{
			get { return (bool)GetValue(IsFixedProperty); }
			protected set { SetValue(IsFixedProperty, value); }
		}
		public static readonly DependencyProperty IsFixedProperty =
			DependencyProperty.Register("IsFixed", typeof(bool), typeof(StateVm), new PropertyMetadata(false));

		/// <summary>
		/// Saves if not in InitializingPhase
		/// <para>Also do some correction before saving OnProductRework and Location</para>
		/// <para>Also checks to ensure it's safe to change ManHour, CycleTime or OnProductRework</para>
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		public static void AnyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StateVm stateVm = null;
			if (d is StateVm)
			{
				stateVm = d as StateVm;
				if (!stateVm.InitializingPhase && !stateVm.ParentWindowVm.IsReadonly)
				{
					if(e.Property == DragTarget.LocationProperty)
					{
						if (stateVm.Model != null)
						{
							var val = (Thickness)e.NewValue;
							var vec = new Vector(val.Left, val.Top);
							stateVm.Model.X = (float)vec.X;
							stateVm.Model.Y = (float)vec.Y;
						}
					}
					else if (e.Property == ProductReworkProperty)
					{
						if (stateVm.Model.StateStations.Any(ss => ss.Blocks.Any()))
						{
							stateVm.ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox("این مرحله (بعضی از ایستگاهها) در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
							return;
						}
						var newProductReworkVm = (ProductReworkVm)e.NewValue;
						stateVm.Model.OnProductRework = newProductReworkVm == null ? stateVm.ParentWindowVm.Model.Product.MainProductRework : newProductReworkVm.Model;
					}
				}
			}
			else if (d is StateStationActivityVm)
			{
				stateVm = ((StateStationActivityVm)d).ContainerSS.ContainerS.State;
				if (!stateVm.InitializingPhase && !stateVm.ParentWindowVm.IsReadonly)
				{
					var ssa = ((StateStationActivityVm)d);
					if (e.Property == StateStationActivityVm.CycleTimeProperty || e.Property == StateStationActivityVm.ManHourProperty)
					{
						//check if no process is using this SSA
						if (ssa.Model.Processes.Any())
						{
							stateVm.ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox("این فعالیت در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
							return;
						}
					}
				}
			}

			//notify parents to save database
			if (stateVm != null)
				stateVm.OnModelChanged();
		}
		#endregion

		#region Visual (Width, Height, Opacity)
		/// <summary>
		/// Gets or sets a bindable value for width of this state
		/// </summary>
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(StateVm), new UIPropertyMetadata(40d));
		/// <summary>
		/// Gets or sets a bindable value for height of this state
		/// </summary>
		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(StateVm), new UIPropertyMetadata(38d));
		/// <summary>
		/// Gets or sets a bindable value for Opacity of this state
		/// </summary>
		public double Opacity
		{
			get { return (double)GetValue(OpacityProperty); }
			set { SetValue(OpacityProperty, value); }
		}
		public static readonly DependencyProperty OpacityProperty =
			DependencyProperty.Register("Opacity", typeof(double), typeof(StateVm), new UIPropertyMetadata(1d)); 
		#endregion

		#region Commands
		private void initCommands()
		{
			SaveCommand = new Commands.Command(o =>
			{
				try
				{
					PromptSave();
					ParentWindowVm.fpcDataService.stateDataService.AttachModel(Model);
				}
				catch (Common.SoheilException.SoheilExceptionBase exp)
				{
					string msg = "";
					if (exp.Level == Common.SoheilException.ExceptionLevel.Error)
						msg += "قادر به ذخیره مرحله نیست\n";
					msg += exp.Message;
					ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox(msg, exp.Caption, MessageBoxButton.OK, exp.Level);
				}
				catch (Exception exp)
				{
					ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox(exp);
				}
			});

			DeleteCommand = new Commands.Command(o =>
			{
				try
				{
					//check for constraints
					var entity = new Dal.Repository<Model.State>(new Dal.SoheilEdmContext()).Single(x => x.Id == Id);
					if (entity != null && entity.StateStations.Any(ss => ss.Blocks.Any()))
					{
						ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox("این مرحله (بعضی از ایستگاهها) در برنامه تولید استفاده شده است", "Error", MessageBoxButton.OK, Common.SoheilException.ExceptionLevel.Error);
						return;
					}

					var connectors = ParentWindowVm.Connectors.Where(x => x.Start.Id == Id || x.End.Id == Id).ToList();
					ParentWindowVm.fpcDataService.stateDataService.DeleteModel(Model);

					//notify deletion (to remove state and its connectors from parent)
					if (StateDeleted != null) StateDeleted(connectors);
				}
				catch (Common.SoheilException.SoheilExceptionBase exp)
				{
					string msg = "";
					if (exp.Level == Common.SoheilException.ExceptionLevel.Error)
						msg += "قادر به حذف مرحله نیست\n";
					msg += exp.Message;
					ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox(msg, exp.Caption, MessageBoxButton.OK, exp.Level);
				}
				catch (Exception exp)
				{
					ParentWindowVm.Message = new Common.SoheilException.DependencyMessageBox(exp);
				}
			});

			SelectCommand = new Command(o =>
			{
				if (StateSelected != null) StateSelected();
			});
		}

		/// <summary>
		/// Gets or sets a bindable command to save this state
		/// </summary>
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(StateVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to delete this state and its connectors
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(StateVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to select this state
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(StateVm), new UIPropertyMetadata(null));
		#endregion


	}
}
