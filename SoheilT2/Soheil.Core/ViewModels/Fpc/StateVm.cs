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
	public class StateVm : DragTarget
	{
		public StateDataService _stateDataService { get { return ParentWindowVm.fpcDataService.stateDataService; } }

		public event EventHandler<ModelRemovedEventArgs> StateDeleted;
		public Model.State Model { get; private set; }

		#region Ctor
		/// <summary>
		/// Create a temporary state for drag and drop
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="parentWindowVm"></param>
		public StateVm(Point pos, FpcWindowVm parentWindowVm)
		{
			InitializingPhase = true;
			ParentWindowVm = parentWindowVm;
			Location = new Vector(pos.X, pos.Y);
			Width = 1;
			Height = 1;
			StateType = StateType.Temp;
		}
		/// <summary>
		/// Create a state from the model (if specified or empty state otherwise) on parentWindow with specified dataService
		/// </summary>
		/// <param name="model">Must be full of data or completely null</param>
		/// <param name="parentWindowVm"></param>
		/// <param name="stateDataService"></param>
		public StateVm(Model.State model, FpcWindowVm parentWindowVm)
		{
			InitializingPhase = true;
			ParentWindowVm = parentWindowVm;
			Model = model;

			if (model != null)
			{
				Id = model.Id;
				Name = model.Name;
				Code = model.Code;
				Location = new Vector(model.X, model.Y);
				StateType = model.StateType;
				if (StateType == Common.StateType.Mid || StateType == Common.StateType.Rework)
					if(model.OnProductRework!=null)//***dlete
					ProductRework = parentWindowVm.ProductReworks.FirstOrDefault(x => x.Id == model.OnProductRework.Id);
			}
			initCommands();
			ResetCommand.Execute(null);//Updates StateConfigVm
		}
		#endregion

		#region Basic Props
		//ParentWindowVm Dependency Property
		public FpcWindowVm ParentWindowVm
		{
			get { return (FpcWindowVm)GetValue(ParentWindowVmProperty); }
			set { SetValue(ParentWindowVmProperty, value); }
		}
		public static readonly DependencyProperty ParentWindowVmProperty =
			DependencyProperty.Register("ParentWindowVm", typeof(FpcWindowVm), typeof(StateVm), new UIPropertyMetadata(null));
		
		//FPC Dependency Property
		public FpcVm FPC { get { return (FpcVm)ParentWindowVm; } }
		
		//Config Dependency Property
		public StateConfigVm Config
		{
			get { return (StateConfigVm)GetValue(ConfigProperty); }
			set { SetValue(ConfigProperty, value); }
		}
		public static readonly DependencyProperty ConfigProperty =
			DependencyProperty.Register("Config", typeof(StateConfigVm), typeof(StateVm), new UIPropertyMetadata(null)); 

		//IsChanged Dependency Property
		public bool IsChanged
		{
			get { return (bool)GetValue(IsChangedProperty); }
			set { SetValue(IsChangedProperty, value); }
		}
		public static readonly DependencyProperty IsChangedProperty =
			DependencyProperty.Register("IsChanged", typeof(bool), typeof(StateVm), new UIPropertyMetadata(false));

		private bool _isSavedAtLeastOnce = false;
		public bool InitializingPhase { get; set; }

		public static void AnyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var vm = d as StateVm;
			if (vm != null)
			{
				if (!vm.InitializingPhase)
				{
					vm.IsChanged = true;
					if (e.Property == NameProperty) vm.Model.Name = (string)e.NewValue;
					else if (e.Property == CodeProperty) vm.Model.Code = (string)e.NewValue;
					else if (e.Property == ProductReworkProperty)
					{
						var prvm = (ProductReworkVm)e.NewValue;
						vm.Model.OnProductRework = prvm == null ? vm.ParentWindowVm.Model.Product.MainProductRework : prvm.Model;
					}
				}
			}
		}
		
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(StateVm), new UIPropertyMetadata("مرحله بدون نام", AnyPropertyChangedCallback));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(StateVm), 
			new UIPropertyMetadata("", AnyPropertyChangedCallback));
		//StateType Dependency Property
		public StateType StateType
		{
			get { return (StateType)GetValue(StateTypeProperty); }
			set { SetValue(StateTypeProperty, value); }
		}
		public static readonly DependencyProperty StateTypeProperty =
			DependencyProperty.Register("StateType", typeof(StateType), typeof(StateVm), new UIPropertyMetadata(StateType.Mid));
		
		//ProductRework Dependency Property
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(StateVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				AnyPropertyChangedCallback(d, e);
				var vm = (StateVm)d;
				if (vm.StateType != StateType.Mid) return;
				var val = (ProductReworkVm)e.NewValue;
				vm._lockPR = true;
				if (val == null) vm.IsRework = false;
				else vm.IsRework = !val.IsMainProduction;
				vm._lockPR = false;
			}));
		//IsMainProduction Dependency Property
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
				if (vm._lockPR) return;
				if (!(bool)e.NewValue) vm.ProductRework = null;
				else vm.ProductRework = vm.ParentWindowVm.ProductReworks.FirstOrDefault();
			}));
		private bool _lockPR = false;
		#endregion

		#region Visual
		//Width Dependency Property
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(StateVm), new UIPropertyMetadata(40d));
		//Height Dependency Property
		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(StateVm), new UIPropertyMetadata(38d));

		
		//ShowDetails Dependency Property
		public bool ShowDetails
		{
			get { return (bool)GetValue(ShowDetailsProperty); }
			set { SetValue(ShowDetailsProperty, value); }
		}
		public static readonly DependencyProperty ShowDetailsProperty =
			DependencyProperty.Register("ShowDetails", typeof(bool), typeof(StateVm),
			new UIPropertyMetadata(false, (d, e) => { }, (d, v) =>
			{
				if (((StateVm)d).IsChanged) return true;
				return v;
			}));
		//Opacity Dependency Property
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
			ResetCommand = new Commands.Command(o =>
			{
				ShowDetails = false;
				Config = new StateConfigVm(this, ParentWindowVm);
				if (Model == null) return;
				//fetch from db and update state config
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
				IsChanged = _isSavedAtLeastOnce;
			});

			SaveCommand = new Commands.Command(o =>
			{
				try
				{
					_stateDataService.AttachModel(this);
					IsChanged = false;
					_isSavedAtLeastOnce = true;
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
					var connectors = FPC.Connectors.Where(x => x.Start.Id == Id || x.End.Id == Id).ToList();
					_stateDataService.DeleteModel(this);

					if (StateDeleted != null)
						StateDeleted(this, new ModelRemovedEventArgs(Id));
					FPC.States.Remove(this);
					foreach (var connector in connectors)
					{
						FPC.Connectors.Remove(connector);
					}
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
				ParentWindowVm.FireSelectState(this);
			});
		}

		//SaveCommand
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(StateVm), new PropertyMetadata(null));
		//ResetCommand
		public Commands.Command ResetCommand
		{
			get { return (Commands.Command)GetValue(ResetCommandProperty); }
			set { SetValue(ResetCommandProperty, value); }
		}
		public static readonly DependencyProperty ResetCommandProperty =
			DependencyProperty.Register("ResetCommand", typeof(Commands.Command), typeof(StateVm), new PropertyMetadata(null));
		//DeleteCommand
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(StateVm), new PropertyMetadata(null));
		//SelectCommand Dependency Property
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
