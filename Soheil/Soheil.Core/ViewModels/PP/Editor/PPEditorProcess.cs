using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common.SoheilException;

/* process	p1		->	ssa1  -> a1
 * 
 * activity a1		->	ssa1 (ManHour = 1) choice 1
 *						ssa2 (ManHour = 2) choice 2
 *						ssa2 (ManHour = 3) choice 3
 * 
 * PPEditorProcess	->	ssaGroup1 (ssa1, ssa2, ssa3)
 * 
 */

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// Does not have duration (it should be automatically calculated from tp*ct)
	/// </summary>
	public class PPEditorProcess : DependencyObject
	{
        IEnumerable<Model.StateStationActivity> _ssaGroup;

        Dal.SoheilEdmContext _uow;

		/// <summary>
		/// Gets the process model
		/// </summary>
		public Model.Process Model { get; protected set; }

		/// <summary>
		/// Use this event to notify Task about changes to ActivityChoice of this Process
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<PPEditorActivityChoice, PPEditorActivityChoice> ActivityChoiceChanged;
		/// <summary>
		/// Occurs when TargetPoint of this Process is changed
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<int, int> ProcessTargetPointChanged;
		/// <summary>
		/// Occurs when DurationSeconds of this Process is changed
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<int, int> ProcessDurationChanged;

		#region Ctor
        /// <summary>
		/// Creates an instance of PPEditorProcess with given model within the given PPEditorTask
        /// <para>Updates its choices and operators and machines as well</para>
        /// </summary>
        /// <param name="model"></param>
		public PPEditorProcess(Model.Process model, Dal.SoheilEdmContext uow, IGrouping<int, Model.StateStationActivity> ssaGroup = null)
		{
			HoldEvents = true;

			Message = new Common.SoheilException.EmbeddedException();
			Model = model;
			_uow = uow;

			#region Load name and choices
			//find ssaGroup (same activity, different SSAs)
			if (ssaGroup == null)
				_ssaGroup = model.StateStationActivity.GetIdenticals();
			else
				_ssaGroup = ssaGroup;

			if (!_ssaGroup.Any())
			{
				Message.AddEmbeddedException("فعالیتی وجود ندارد");
				return;
			}
			//set sampleSSA (this SSA is used for operators skills) see ctor in Soheil.Core.ViewModels.PP.OperatorVm
			var sampleSSA = _ssaGroup.First();
			Name = sampleSSA.Activity.Name;

			//Add Choices
			foreach (var choice in _ssaGroup.OrderBy(ssa => ssa.ManHour))
			{
				Choices.Add(new PPEditorActivityChoice(choice, this));
			}
			#endregion

			#region Load operators
			//Find all Operators
			var allOperatorModels = new DataServices.OperatorDataService(_uow).GetActives();
			var operatorVms = new List<PPEditorOperator>();
			foreach (var operatorModel in allOperatorModels)
			{
				var operatorVm = new PPEditorOperator(operatorModel, sampleSSA);
				//set IsSelected of operator
				operatorVm.IsSelected = model.ProcessOperators.Any(po => po.Operator.Id == operatorModel.Id);
				//add event handler to update SelectedOperatorsCount
				//(event handler is set after IsSelected is set, so remember to set SelectedOperatorsCount manually)
				operatorVm.SelectedOperatorsChanged += () => SelectedOperatorsCount = OperatorList.Count(x => x.IsSelected);
				operatorVms.Add(operatorVm);
			}
			//Add Operators
			foreach (var operatorVm in operatorVms.OrderByDescending(x => x.EffectiveSkill))
			{
				OperatorList.Add(operatorVm);
			} 
			#endregion

			#region Load machines
			//Find all valid Machines for the whole ssaGroup
			foreach (var ssa in _ssaGroup)
			{
				foreach (var ssam in ssa.StateStationActivityMachines)
				{
					if (!MachineList.Any(x => x.MachineId == ssam.Machine.Id))
					{
						//add the unique machines to MachineList
						var machineVm = new PPEditorMachine(ssam.Machine);
						//select it if it is in SelectedMachines of process model
						machineVm.IsUsed = model.SelectedMachines.Any(x => x.StateStationActivityMachine.Machine.Id == machineVm.MachineId);
						MachineList.Add(machineVm);
					}
				}
			} 
			#endregion

			#region Select the right choice
			//select the right choice based on ManHour
			SelectedOperatorsCount = model.ProcessOperators.Count;
			if (model.StateStationActivity != null)
			{
				//select the right choice based on model.StateStationActivity.Id
				SelectedChoice = Choices.FirstOrDefault(x => x.StateStationActivityId == model.StateStationActivity.Id);
			}
			#endregion

			TargetPoint = model.TargetCount;
			HoldEvents = false;

			//Set the command
			SetDurationMinutesCommand = new Commands.Command(min => DurationSeconds = (int)min * 60);
		}
		#endregion

		#region Name, TP, Duration
		/// <summary>
		/// Gets or sets a value that indicates whether this viewModel should prevent TargetPointChanged and DurationChanged events from firing
		/// </summary>
		public bool HoldEvents { get; set; }

		/// <summary>
		/// Gets or sets a bindable text as the name of this process
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorProcess), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value for target point of this process
		/// <para>Changing this value updates DurationSeconds and model's TargetCount and fires ProcessTargetPointChanged event</para>
		/// </summary>
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(PPEditorProcess),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorProcess)d;
				var val = (int)e.NewValue;
				vm.Model.TargetCount = val;

				//update DurationSeconds
				if (vm.SelectedChoice != null)
				{
					int newDuration = (int)Math.Floor(val * vm.SelectedChoice.CycleTime);
					if (newDuration != vm.DurationSeconds)
						vm.DurationSeconds = newDuration;
				}

				//fire event
				if (!vm.HoldEvents && vm.ProcessTargetPointChanged != null)
					vm.ProcessTargetPointChanged((int)e.OldValue, val);
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
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(PPEditorProcess),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorProcess)d;
				var val = (int)e.NewValue;

				//update TargetPoint
				if (vm.SelectedChoice != null)
				{
					int newTargetPoint = (int)Math.Floor(val / vm.SelectedChoice.CycleTime);
					if (newTargetPoint != vm.TargetPoint)
						vm.TargetPoint = newTargetPoint;
				}

				//fire event
				if (!vm.HoldEvents && vm.ProcessDurationChanged != null)
					vm.ProcessDurationChanged((int)e.OldValue, val);
			}));

		#endregion

		#region Operators and machines
		/// <summary>
		/// Gets a collection of machines that are in at least one choice of this process
		/// <para>Selected machines have IsUsed = true</para>
		/// <para>Machines in the selected choice have CanBeUsed = true</para>
		/// </summary>
		public ObservableCollection<PPEditorMachine> MachineList { get { return _machineList; } }
		private ObservableCollection<PPEditorMachine> _machineList = new ObservableCollection<PPEditorMachine>();
		/// <summary>
		/// Gets a collection of all operators
		/// <para>Selected operators have IsSelected = true</para>
		/// </summary>
		public ObservableCollection<PPEditorOperator> OperatorList { get { return _operatorList; } }
		private ObservableCollection<PPEditorOperator> _operatorList = new ObservableCollection<PPEditorOperator>();
		/// <summary>
		/// Gets or sets the bindable number of used operators in this process
		/// </summary>
		public int SelectedOperatorsCount
		{
			get { return (int)GetValue(SelectedOperatorsCountProperty); }
			set { SetValue(SelectedOperatorsCountProperty, value); }
		}
		public static readonly DependencyProperty SelectedOperatorsCountProperty =
			DependencyProperty.Register("SelectedOperatorsCount", typeof(int), typeof(PPEditorProcess),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorProcess)d;
				var val = (int)e.NewValue;
				vm.SelectedChoice = vm.Choices.FirstOrDefault(x => x.ManHour == val);
			}));
		#endregion

		//Message Dependency Property
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(PPEditorProcess), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable collection of choices (StateStationActivities) of this process
		/// <para>These choices all have the same Activity but different StateStationActivities (each of which has a unique ManHour)</para>
		/// </summary>
		public ObservableCollection<PPEditorActivityChoice> Choices { get { return _choices; } }
		private ObservableCollection<PPEditorActivityChoice> _choices = new ObservableCollection<PPEditorActivityChoice>();
		/// <summary>
		/// Gets or sets a bindable value to represent the StateStationActivity compatible with the number of used operators
		/// <para>Changing this value updates Model.StateStationActivity, valid machines vm and fires the ActivityChoiceChanged event</para>
		/// </summary>
		public PPEditorActivityChoice SelectedChoice
		{
			get { return (PPEditorActivityChoice)GetValue(SelectedChoiceProperty); }
			set { SetValue(SelectedChoiceProperty, value); }
		}
		public static readonly DependencyProperty SelectedChoiceProperty =
			DependencyProperty.Register("SelectedChoice", typeof(PPEditorActivityChoice), typeof(PPEditorProcess),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as PPEditorProcess;
				var newVal = e.NewValue as PPEditorActivityChoice;
				var oldVal = e.OldValue as PPEditorActivityChoice;
				vm.choiceIsChanged(oldVal, newVal);
			}));
		private void choiceIsChanged(PPEditorActivityChoice oldVal, PPEditorActivityChoice newVal)
		{
			if (newVal == null)
			{
				//invalid choice
				Model.StateStationActivity = null;
				DurationSeconds = 0;
				Message.AddEmbeddedException("نفرساعت مورد استفاده این فعالیت نامعتبر است");

				//Update Machines according to the choice
				foreach (var machineVm in MachineList)
				{
					machineVm.CanBeUsed = false;
				}
			}
			else
			{
				//valid choice
				//DurationSeconds will be set through ActivityChoiceChanged event
				Model.StateStationActivity = newVal.Model;
				Message.ResetEmbeddedException();
				//compare manhour of selected choice with number of assigned operators
				SelectedChoice.OperatorCountError = ((int)Math.Ceiling(newVal.ManHour) != OperatorList.Count(x=>x.IsSelected));

				//Update Machines according to the choice
				foreach (var machineVm in MachineList)
				{
					machineVm.CanBeUsed = newVal.Model.StateStationActivityMachines.Any(ssam => ssam.Machine.Id == machineVm.MachineId);
				}
			}

			//fire the event
			if (ActivityChoiceChanged != null)
				ActivityChoiceChanged(oldVal, newVal);
		}

		//SetDurationMinutesCommand Dependency Property
		public Commands.Command SetDurationMinutesCommand
		{
			get { return (Commands.Command)GetValue(SetDurationMinutesCommandProperty); }
			set { SetValue(SetDurationMinutesCommandProperty, value); }
		}
		public static readonly DependencyProperty SetDurationMinutesCommandProperty =
			DependencyProperty.Register("SetDurationMinutesCommand", typeof(Commands.Command), typeof(PPEditorProcess), new UIPropertyMetadata(null));
	}
}
