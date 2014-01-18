using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorProcess : DependencyObject
	{
		PPEditorTask _parent;
		Model.Process _model;
		bool _isInitializing = true;
		public int ProcessId { get { return _model.Id; } }

		#region Ctor
		/// <summary>
		/// Creates an instance of PPEditorProcess for an existing process 
		/// <para>within the given PPEditorTask</para>
		/// </summary>
		/// <param name="model"></param>
		public PPEditorProcess(PPEditorTask parent, Model.Process processModel)
		{
			_model = processModel;
			_parent = parent;
			
			//choices
			foreach (var choice in processModel.StateStationActivity.GetIdenticals())
			{
				Choices.Add(new PPEditorActivityChoice(choice));
			}
			SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == processModel.ProcessOperators.Count);

			Name = processModel.StateStationActivity.Activity.Name;
			TargetPoint = processModel.TargetCount;

			//operators
			var allOperatorModels = new DataServices.OperatorDataService().GetActives();
			foreach (var operatorModel in allOperatorModels)
			{
				var processOperatorModel = processModel.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operatorModel.Id);
				var operatorVm = (processOperatorModel == null) ?
					new PPEditorOperator(operatorModel) :
					new PPEditorOperator(processOperatorModel);
				OperatorList.Add(operatorVm);
			}
			OperatorList.CollectionChanged += (s, e) =>
			{
				if (!_isInitializing)
					SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == OperatorList.Count);
			};

			//machines
			foreach (var ssamModel in processModel.StateStationActivity.StateStationActivityMachines)
			{
				var selectedMachineModel = processModel.SelectedMachines.FirstOrDefault(x => x.StateStationActivityMachine.Id == ssamModel.Id);
				var machineVm = (selectedMachineModel == null) ?
					new PPEditorMachine(ssamModel) :
					new PPEditorMachine(selectedMachineModel);
				MachineList.Add(machineVm);//aya bayad hameye machine ha add shavand?
			}

			_isInitializing = false;
		}
		public void Reset()
		{
			if (_model == null)
			{
				TargetPoint = 0;
			}
			else
			{
				TargetPoint = _model.TargetCount;
			}
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
			new UIPropertyMetadata(0));
		//MachineList Observable Collection
		private ObservableCollection<PPEditorMachine> _machineList = new ObservableCollection<PPEditorMachine>();
		public ObservableCollection<PPEditorMachine> MachineList { get { return _machineList; } }
		//OperatorList Observable Collection
		private ObservableCollection<PPEditorOperator> _operatorList = new ObservableCollection<PPEditorOperator>();
		public ObservableCollection<PPEditorOperator> OperatorList { get { return _operatorList; } }
		//SelectedOperators Dependency Property
		public int SelectedOperators
		{
			get { return (int)GetValue(SelectedOperatorsProperty); }
			set { SetValue(SelectedOperatorsProperty, value); }
		}
		public static readonly DependencyProperty SelectedOperatorsProperty =
			DependencyProperty.Register("SelectedOperators", typeof(int), typeof(PPEditorProcess), new UIPropertyMetadata(0));
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
				var val = e.NewValue as PPEditorActivityChoice;
				if (val == null) return;
			}));
	}
}
