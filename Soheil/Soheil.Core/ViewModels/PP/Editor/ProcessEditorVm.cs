using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common.SoheilException;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Editor
{

	/* process	p1		->	ssa1  -> a1
	 * 
	 * activity a1		->	ssa1 (ManHour = 1) choice 1
	 *						ssa2 (ManHour = 2) choice 2
	 *						ssa2 (ManHour = 3) choice 3
	 * 
	 * PPEditorProcess	->	ssaGroup1 (ssa1, ssa2, ssa3)
	 * 
	 */

	/// <summary>
	/// Does not have duration (it should be automatically calculated from tp*ct)
	/// </summary>
	public class ProcessEditorVm : DependencyObject
	{
		Dal.SoheilEdmContext _uow;

		/// <summary>
		/// Gets the process model
		/// </summary>
		public Model.Process Model { get; protected set; }
		/// <summary>
		/// Gets the model for Activity (it is fixed unlike SSA)
		/// </summary>
		public Soheil.Model.Activity ActivityModel { get; protected set; }

		/// <summary>
		/// Gets or sets a value that indicates whether this viewModel should prevent TargetPointChanged and DurationChanged events from firing
		/// </summary>
		public bool HoldEvents { get; set; }
		//indicates whether this viewModel should prevent changing Model
		bool _isInitializing;

		/// <summary>
		/// Occurs when DurationSeconds of this Process is changed
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<ProcessEditorVm, DateTime, DateTime> TimesChanged;
		/// <summary>
		/// Occurs when number of selected operators is changed for this process (2nd Parameter=number of operators)
		/// <para>This event should change SelectedChoice of this process</para>
		/// </summary>
		public event Action<ProcessEditorVm, int> SelectedOperatorsCountChanged;
		/// <summary>
		/// Occurs when this process is selected among other processes with same activity
		/// </summary>
		public event Action<ProcessEditorVm> Selected;
		public event Action<ProcessEditorVm> Deleted;
		/// <summary>
		/// Occurs when selected choice of SSAs for this Process is changed
		/// <para>second parameter can be null</para>
		/// </summary>
		public event Action<ProcessEditorVm, ChoiceEditorVm> SelectedChoiceChanged;


		#region Ctor & choiceIsChanged
		/// <summary>
		/// Creates an instance of PPEditorProcess with given model within the given PPEditorTask
		/// <para>Updates its choices and operators and machines as well</para>
		/// </summary>
		/// <param name="model"></param>
		public ProcessEditorVm(Model.Process model, Model.Activity activityModel, Dal.SoheilEdmContext uow)
		{
			_isInitializing = true;

			#region Basic
			_uow = uow;
			Model = model;
			ActivityModel = activityModel;
			HasReport = Model.ProcessReports.Any();
			Message = new Common.SoheilException.EmbeddedException();

			TargetPoint = model.TargetCount;
			DurationSeconds = model.DurationSeconds;

			Date = model.Task.Block.StartDateTime.Date;
			StartTime = model.StartDateTime.Subtract(Date);
			EndTime = model.EndDateTime.Subtract(Date); 
			#endregion

			#region Machines
			//SelectedMachines
			foreach (var sm in model.SelectedMachines)
			{
				var machineVm = new MachineEditorVm(sm);
				SelectedMachines.Add(machineVm);
			}
			NoSelectedMachines = !model.SelectedMachines.Any();
			SelectedMachines.CollectionChanged += (s, e) => NoSelectedMachines = !SelectedMachines.Any();

			//MachineFamilyList
			ShowAllMachinesCommand = new Commands.Command(o =>
			{
				ShowAllMachines = true;
				IsSelected = true;
				MachineFamilyList.Clear();

				//Load Model
				var ssams = new List<Model.StateStationActivityMachine>();
				foreach (var ssa in Model.StateStationActivity.StateStation.StateStationActivities)
				{
					ssams.AddRange(ssa.StateStationActivityMachines);
				}
				var machines = ssams.GroupBy(x => x.Machine);
				var machineFamilies = machines.GroupBy(x => x.Key.MachineFamily);

				//Create ViewModel
				foreach (var machineFamily in machineFamilies)
				{
					var machineFamilyVm = new MachineFamilyEditorVm(machineFamily);
					machineFamilyVm.Revalidate(Model);
					MachineFamilyList.Add(machineFamilyVm);
				}
			}); 
			#endregion

			//set operators after those 2, because when choice is selected we expect to have valid information in this vm
			#region Operators
			//select the right choice based on ManHour
			foreach (var oper in model.ProcessOperators)
			{
				SelectedOperators.Add(new OperatorVm(oper.Operator));
			}
			SelectedOperatorsCount = model.ProcessOperators.Count;
			SelectedOperators.CollectionChanged += (s, e) => SelectedOperatorsCount = SelectedOperators.Count;
			#endregion

			//command
			SelectCommand = new Commands.Command(o => IsSelected = true);
			DeleteCommand = new Commands.Command(o =>
			{
				var succeed = new DataServices.TaskDataService(uow).DeleteModel(Model, (bool)o);
				if (succeed)
				{
					uow.Commit();
					if (Deleted != null) Deleted(this);
				}
				else
					Message.AddEmbeddedException("Activity has reports");
			});

			_isInitializing = false;
		}

		/// <summary>
		/// Updates machines, model and operators and fires SelectedChoiceChanged event
		/// </summary>
		/// <param name="oldVal"></param>
		/// <param name="newVal"></param>
		private void choiceIsChanged(ChoiceEditorVm oldVal, ChoiceEditorVm newVal)
		{
			if (newVal == null)
			{
				//invalid choice
				Model.StateStationActivity = null;
				Message.AddEmbeddedException("نفرساعت مورد استفاده این فعالیت نامعتبر است");
				OperatorCountError = true;

				//Update Machines according to the choice
				foreach (var machineFamilyVm in MachineFamilyList)
				{
					foreach (var machineVm in machineFamilyVm.MachineList)
					{
						machineVm.CanBeUsed = false;
					}
				}
				foreach (var smVm in SelectedMachines)
				{
					smVm.CanBeUsed = false;
				}
			}
			else
			{
				//valid choice
				Model.StateStationActivity = newVal.Model;
				Message.ResetEmbeddedException();

				//compare manhour of selected choice with number of assigned operators
				OperatorCountError = ((int)Math.Ceiling(newVal.ManHour) != Model.ProcessOperators.Count);

				//Update Machines according to the choice
				foreach (var machineFamilyVm in MachineFamilyList)
				{
					foreach (var machineVm in machineFamilyVm.MachineList)
					{
						machineVm.CanBeUsed = newVal.Model.StateStationActivityMachines.Any(ssam => ssam.Machine.Id == machineVm.MachineId);
					}
				}
				foreach (var smVm in SelectedMachines)
				{
					smVm.CanBeUsed = newVal.Model.StateStationActivityMachines.Any(ssam => ssam.Machine.Id == smVm.MachineId);
				}
			}
			//fire event
			if (SelectedChoiceChanged != null)
				SelectedChoiceChanged(this, newVal);
		}
		#endregion

		#region TP & Time
		/// <summary>
		/// Gets or sets a bindable value for target point of this process
		/// <para>Changing this value updates DurationSeconds and model's TargetCount</para>
		/// </summary>
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(ProcessEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessEditorVm)d;
				if (vm._isInitializing) return;

				//update model
				var val = (int)e.NewValue;
				vm.Model.TargetCount = val;

				if (vm.HoldEvents) return;

				//update DurationSeconds
				if (vm.SelectedChoice != null)
				{
					int newDuration = (int)Math.Floor(val * vm.SelectedChoice.CycleTime);
					if (newDuration != vm.DurationSeconds)
						vm.DurationSeconds = newDuration;
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value for duration seconds of this process
		/// </summary>
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(ProcessEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessEditorVm)d;
				if (vm._isInitializing) return;

				//update model
				var val = (int)e.NewValue;
				vm.Model.DurationSeconds = val;
				
				if (vm.HoldEvents) return;

				//update EndTime
				vm.EndTime = vm.StartTime.Add(TimeSpan.FromSeconds(val));

				//update TargetPoint
				if (vm.SelectedChoice != null)
				{
					int newTargetPoint = (int)Math.Floor(val / vm.SelectedChoice.CycleTime);
					if (newTargetPoint != vm.TargetPoint)
						vm.TargetPoint = newTargetPoint;
				}

			}));

		/// <summary>
		/// Gets or sets Date of this process (StartTime and EndTime are considered relative to this Date)
		/// </summary>
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				if (!_isInitializing && Model != null)
				{
					Model.StartDateTime = value.Add(StartTime);
					Model.EndDateTime = value.Add(EndTime);
				}
			}
		}
		DateTime _date;

		/// <summary>
		/// Gets or sets a bindable value for starting timespan (offset from Date)
		/// </summary>
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(ProcessEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as ProcessEditorVm;
				if (vm._isInitializing) return;

				//update model
				vm.Model.StartDateTime = vm.Date.Add((TimeSpan)e.NewValue);

				if (vm.HoldEvents) return;

				//fire event
				if (vm.TimesChanged != null)
					vm.TimesChanged(vm, vm.Model.StartDateTime, vm.Model.EndDateTime);
			}));
		/// <summary>
		/// Gets or sets a bindable value for ending timespan (offset from Date)
		/// </summary>
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(ProcessEditorVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as ProcessEditorVm;
				if (vm._isInitializing) return;

				//update model
				vm.Model.EndDateTime = vm.Date.Add((TimeSpan)e.NewValue);

				if (vm.HoldEvents) return;

				//fire event
				if (vm.TimesChanged != null)
					vm.TimesChanged(vm, vm.Model.StartDateTime, vm.Model.EndDateTime);
			}));
		#endregion

		#region Operators and Choice
		/// <summary>
		/// Gets a bindable collection of SelectedOperators for this process (AKA quicklist)
		/// <para>Changing this collection has "NO" effect except for updating SelectedOperatorsCount</para>
		/// </summary>
		public ObservableCollection<OperatorVm> SelectedOperators { get { return _selectedOperators; } }
		private ObservableCollection<OperatorVm> _selectedOperators = new ObservableCollection<OperatorVm>();
		/// <summary>
		/// Gets or sets the bindable number of used operators in this process
		/// </summary>
		public int SelectedOperatorsCount
		{
			get { return (int)GetValue(SelectedOperatorsCountProperty); }
			set { SetValue(SelectedOperatorsCountProperty, value); }
		}
		public static readonly DependencyProperty SelectedOperatorsCountProperty =
			DependencyProperty.Register("SelectedOperatorsCount", typeof(int), typeof(ProcessEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessEditorVm)d;
				if (vm.SelectedOperatorsCountChanged != null)
					vm.SelectedOperatorsCountChanged(vm, (int)e.NewValue);
			}));
		/// <summary>
		/// Gets or sets a bindable value to represent the StateStationActivity compatible with the number of used operators
		/// <para>Changing this value updates Model.StateStationActivity and valid machines vm</para>
		/// </summary>
		public ChoiceEditorVm SelectedChoice
		{
			get { return (ChoiceEditorVm)GetValue(SelectedChoiceProperty); }
			set { SetValue(SelectedChoiceProperty, value); }
		}
		public static readonly DependencyProperty SelectedChoiceProperty =
			DependencyProperty.Register("SelectedChoice", typeof(ChoiceEditorVm), typeof(ProcessEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as ProcessEditorVm;
				var newVal = e.NewValue as ChoiceEditorVm;
				var oldVal = e.OldValue as ChoiceEditorVm;
				vm.choiceIsChanged(oldVal, newVal);
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicate whether Manhour does not match the number of operators assigned to this choice
		/// <para>This could be true when an auto planning considered this choice but not able to assign operators yet</para>
		/// </summary>
		public bool OperatorCountError
		{
			get { return (bool)GetValue(OperatorCountErrorProperty); }
			set { SetValue(OperatorCountErrorProperty, value); }
		}
		public static readonly DependencyProperty OperatorCountErrorProperty =
			DependencyProperty.Register("OperatorCountError", typeof(bool), typeof(ProcessEditorVm), new UIPropertyMetadata(true));
		#endregion

		#region Machines
		/// <summary>
		/// Gets a collection of MachineFamilies that have at least one machine in at least one choice of this process
		/// <para>Selected machines have IsUsed = true</para>
		/// <para>Machines in the selected choice have CanBeUsed = true</para>
		/// </summary>
		public ObservableCollection<MachineFamilyEditorVm> MachineFamilyList { get { return _machineFamilyList; } }
		private ObservableCollection<MachineFamilyEditorVm> _machineFamilyList = new ObservableCollection<MachineFamilyEditorVm>();

		/// <summary>
		/// Gets a bindable collection of SelectedMachines for this process
		/// <para>Changing this collection has "NO" effect except for updating NoSelectedMachines</para>
		/// </summary>
		public ObservableCollection<MachineEditorVm> SelectedMachines { get { return _selectedMachines; } }
		private ObservableCollection<MachineEditorVm> _selectedMachines = new ObservableCollection<MachineEditorVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this process no selected machines
		/// </summary>
		public bool NoSelectedMachines
		{
			get { return (bool)GetValue(NoSelectedMachinesProperty); }
			set { SetValue(NoSelectedMachinesProperty, value); }
		}
		public static readonly DependencyProperty NoSelectedMachinesProperty =
			DependencyProperty.Register("NoSelectedMachines", typeof(bool), typeof(ProcessEditorVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether all machines (families) are shown instead of selected machines
		/// </summary>
		public bool ShowAllMachines
		{
			get { return (bool)GetValue(ShowAllMachinesProperty); }
			set { SetValue(ShowAllMachinesProperty, value); }
		}
		public static readonly DependencyProperty ShowAllMachinesProperty =
			DependencyProperty.Register("ShowAllMachines", typeof(bool), typeof(ProcessEditorVm), new UIPropertyMetadata(false));

		/// <summary>
		/// Gets or sets a bindable command to show all machines (also selects the process)
		/// </summary>
		public Commands.Command ShowAllMachinesCommand
		{
			get { return (Commands.Command)GetValue(ShowAllMachinesCommandProperty); }
			set { SetValue(ShowAllMachinesCommandProperty, value); }
		}
		public static readonly DependencyProperty ShowAllMachinesCommandProperty =
			DependencyProperty.Register("ShowAllMachinesCommand", typeof(Commands.Command), typeof(ProcessEditorVm), new UIPropertyMetadata(null));


		#endregion

		#region Select,Delete, Report, Message,
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this process is selected among all processes of the block
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProcessEditorVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as ProcessEditorVm;
				if ((bool)e.NewValue)
				{
					if (vm.Selected != null)
						vm.Selected(vm);
				}
			}));
		//SelectCommand Dependency Property
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(ProcessEditorVm), new UIPropertyMetadata(null));

		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(ProcessEditorVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this process has reports
		/// </summary>
		public bool HasReport
		{
			get { return (bool)GetValue(HasReportProperty); }
			set { SetValue(HasReportProperty, value); }
		}
		public static readonly DependencyProperty HasReportProperty =
			DependencyProperty.Register("HasReport", typeof(bool), typeof(ProcessEditorVm), new UIPropertyMetadata(false));

		/// <summary>
		/// Gets or sets the bindable Error Message
		/// </summary>
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(ProcessEditorVm), new UIPropertyMetadata(null));

		#endregion

	}
}