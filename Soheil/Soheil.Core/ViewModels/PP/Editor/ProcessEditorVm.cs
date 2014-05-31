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
		IEnumerable<Model.StateStationActivity> _ssaGroup;

		Dal.SoheilEdmContext _uow;

		/// <summary>
		/// Gets the process model
		/// </summary>
		public Model.Process Model { get; protected set; }
		/// <summary>
		/// Gets the first StateStationActivity in choices
		/// </summary>
		public Model.StateStationActivity SampleSSA { get; protected set; }

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
		public ProcessEditorVm(Model.Process model, Dal.SoheilEdmContext uow, IGrouping<int, Model.StateStationActivity> ssaGroup = null)
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

			//set sampleSSA
			SampleSSA = _ssaGroup.First();
			Name = SampleSSA.Activity.Name;

			//Add Choices
			foreach (var choice in _ssaGroup.OrderBy(ssa => ssa.ManHour))
			{
				Choices.Add(new PPEditorActivityChoice(choice, this));
			}

			//select the right choice based on ManHour
			SelectedOperatorsCount = model.ProcessOperators.Count;
			if (model.StateStationActivity != null)
			{
				//select the right choice based on model.StateStationActivity.Id
				SelectedChoice = Choices.FirstOrDefault(x => x.StateStationActivityId == model.StateStationActivity.Id);
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
						var machineVm = new MachineEditorVm(ssam.Machine);
						//select it if it is in SelectedMachines of process model
						machineVm.IsUsed = model.SelectedMachines.Any(x => x.StateStationActivityMachine.Machine.Id == machineVm.MachineId);
						MachineList.Add(machineVm);
					}
				}
			}
			#endregion


			TargetPoint = model.TargetCount;
			HoldEvents = false;
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
			DependencyProperty.Register("Name", typeof(string), typeof(ProcessEditorVm), new UIPropertyMetadata(null));
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
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(ProcessEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessEditorVm)d;
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
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(ProcessEditorVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (ProcessEditorVm)d;
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

		/// <summary>
		/// Gets a collection of machines that are in at least one choice of this process
		/// <para>Selected machines have IsUsed = true</para>
		/// <para>Machines in the selected choice have CanBeUsed = true</para>
		/// </summary>
		public ObservableCollection<MachineEditorVm> MachineList { get { return _machineList; } }
		private ObservableCollection<MachineEditorVm> _machineList = new ObservableCollection<MachineEditorVm>();
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
				var val = (int)e.NewValue;
				vm.SelectedChoice = vm.Choices.FirstOrDefault(x => x.ManHour == val);
			}));


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
			DependencyProperty.Register("SelectedChoice", typeof(PPEditorActivityChoice), typeof(ProcessEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = d as ProcessEditorVm;
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
				SelectedChoice.OperatorCountError = ((int)Math.Ceiling(newVal.ManHour) != Model.ProcessOperators.Count);

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

		//Message Dependency Property
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(ProcessEditorVm), new UIPropertyMetadata(null));

	}
}