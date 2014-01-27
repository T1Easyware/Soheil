using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// Does not have duration (it should be automatically calculated from tp*ct)
	/// </summary>
	public class PPEditorProcess : DependencyObject
	{
		PPEditorTask _parent;
		IEnumerable<Model.StateStationActivity> _ssaGroup;
		//Model.Process _model;
		bool _isInitializing = true;
		//public int ProcessId { get { return _model.Id; } }
		//public int ActivityId { get { return _model.StateStationActivity.Activity.Id; } }

		/// <summary>
		/// Use this event to notify Task about changes to ActivityChoice of this Process
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<PPEditorActivityChoice, PPEditorActivityChoice> ActivityChoiceChanged;
		/// <summary>
		/// Use this event to notify Task about changes to TargetPoint of this Process
		/// <para>first arg is oldValue, seconds arg is NewValue</para>
		/// </summary>
		public event Action<int, int> ProcessTargetPointChanged;

		#region Ctor
		/// <summary>
		/// Creates an instance of PPEditorProcess for an existing process 
		/// <para>within the given PPEditorTask</para>
		/// </summary>
		/// <param name="model"></param>
		public PPEditorProcess(PPEditorTask parent, IEnumerable<Model.StateStationActivity> ssaGroup, Dal.SoheilEdmContext uow)
		{
			//_model = processModel;
			_ssaGroup = ssaGroup;
			_parent = parent;
			
			//choices
			foreach (var choice in ssaGroup)
			{
				Choices.Add(new PPEditorActivityChoice(choice, this));
			}
			//SelectedChoice = Choices.FirstOrDefault();

			Name = ssaGroup.First().Activity.Name;

			//operators
			var allOperatorModels = new DataServices.OperatorDataService(uow).GetActives();
			foreach (var operatorModel in allOperatorModels)
			{
				var operatorVm = new PPEditorOperator(operatorModel);
				operatorVm.SelectedOperatorsChanged += () =>
				{
					SelectedOperatorsCount = OperatorList.Count(x => x.IsSelected);
					if (!_isInitializing)
						SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == SelectedOperatorsCount);
				};
				OperatorList.Add(operatorVm);
			}

			_isInitializing = false;
		}

		#endregion

		#region DpProps
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorProcess), new UIPropertyMetadata(null));
		//TargetPoint Dependency Property
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
				vm.TargetPoint = val;
				vm.DurationSeconds = vm.SelectedChoice == null ? 0 :
					(int)Math.Floor(val * vm.SelectedChoice.CycleTime);
				if (vm.ProcessTargetPointChanged != null)
					vm.ProcessTargetPointChanged((int)e.OldValue, val);
			}));
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(PPEditorProcess),
			new UIPropertyMetadata(0, (d, e) => ((PPEditorProcess)d).DurationSeconds = (int)e.NewValue));


		//MachineList Observable Collection
		private ObservableCollection<PPEditorMachine> _machineList = new ObservableCollection<PPEditorMachine>();
		public ObservableCollection<PPEditorMachine> MachineList { get { return _machineList; } }
		//OperatorList Observable Collection
		private ObservableCollection<PPEditorOperator> _operatorList = new ObservableCollection<PPEditorOperator>();
		public ObservableCollection<PPEditorOperator> OperatorList { get { return _operatorList; } }
		public int SelectedOperatorsCount
		{
			get { return (int)GetValue(SelectedOperatorsCountProperty); }
			set { SetValue(SelectedOperatorsCountProperty, value); }
		}
		public static readonly DependencyProperty SelectedOperatorsCountProperty =
			DependencyProperty.Register("SelectedOperatorsCount", typeof(int), typeof(PPEditorProcess), new UIPropertyMetadata(0));
		//DoesParentDeferToActivities Dependency Property
		public bool DoesParentDeferToActivities
		{
			get { return (bool)GetValue(DoesParentDeferToActivitiesProperty); }
			set { SetValue(DoesParentDeferToActivitiesProperty, value); }
		}
		public static readonly DependencyProperty DoesParentDeferToActivitiesProperty =
			DependencyProperty.Register("DoesParentDeferToActivities", typeof(bool), typeof(PPEditorProcess), new UIPropertyMetadata(true));
		#endregion

		//Choices Observable Collection
		public ObservableCollection<PPEditorActivityChoice> Choices { get { return _choices; } }
		private ObservableCollection<PPEditorActivityChoice> _choices = new ObservableCollection<PPEditorActivityChoice>();
		//SelectedChoice Dependency Property
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
				if (newVal == null)
				{
					vm.DurationSeconds = 0;
				}
				else
				{
					vm.DurationSeconds = (int)Math.Floor(newVal.CycleTime * vm.TargetPoint);

					//machines
					vm.MachineList.Clear();
					foreach (var ssa in vm._ssaGroup)
					{
						foreach (var ssam in ssa.StateStationActivityMachines)
						{
							//var selectedMachineModel = processModel.SelectedMachines.FirstOrDefault(x => x.StateStationActivityMachine.Id == ssam.Id);
							//	var machineVm = (selectedMachineModel == null) ?
							//new PPEditorMachine(ssam) :
							//	new PPEditorMachine(selectedMachineModel);
							var machineVm = new PPEditorMachine(ssam);
							//i.e. add from model if exists or else do this:
							vm.MachineList.Add(machineVm);//aya bayad hameye machine ha add shavand?
						}
					}
				}
				if (vm.ActivityChoiceChanged != null)
					vm.ActivityChoiceChanged(oldVal, newVal);
			}));
	}
}
