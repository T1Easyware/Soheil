using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorActivity : DependencyObject
	{
		#region Ctor
		internal PPEditorActivity(Model.Process model)
		{
			_model = model;
		}
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorActivity(PPEditorStation parent, Model.Process processModel)
			: this(processModel)
		{
			_parent = parent;
			ActivityId = processModel.StateStationActivity.Activity.Id;
			CycleTime = processModel.StateStationActivity.CycleTime;
			ManHour = processModel.StateStationActivity.ManHour;
			Name = processModel.StateStationActivity.Activity.Name;
			ProcessId = processModel.Id;
			StateStationActivityId = processModel.StateStationActivity.Id;
			TargetPoint = processModel.TargetCount;
			var allOperatorModels = new DataServices.OperatorDataService().GetActives();
			foreach (var operatorModel in allOperatorModels)
			{
				var processOperatorModel = processModel.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operatorModel.Id);
				var operatorVm = (processOperatorModel == null) ?
					new PPEditorOperator(operatorModel) :
					new PPEditorOperator(processOperatorModel);
				OperatorList.Add(operatorVm);
			}
			foreach (var ssamModel in processModel.StateStationActivity.StateStationActivityMachines)
			{
				var selectedMachineModel = processModel.SelectedMachines.FirstOrDefault(x => x.StateStationActivityMachine.Id == ssamModel.Id);
				var machineVm = (selectedMachineModel == null) ?
					new PPEditorMachine(ssamModel) :
					new PPEditorMachine(selectedMachineModel);
				MachineList.Add(machineVm);
			}
		}
		public PPEditorActivity(PPEditorStation parent, Fpc.StateStationActivityVm ssa)
		{
			_parent = parent;
			ActivityId = ssa.Containment.Id;
			StateStationActivityId = ssa.Id;
			Name = ssa.Name;
			CycleTime = ssa.CycleTime;
			ManHour = ssa.ManHour;
			foreach (Fpc.StateStationActivityMachineVm ssam in ssa.ContentsList)
			{
				MachineList.Add(new PPEditorMachine(ssam));
			}
			var allOperatorModels = new DataServices.OperatorDataService().GetActives();
			foreach (var operatorModel in allOperatorModels)
			{
				OperatorList.Add(new PPEditorOperator(operatorModel));
			}
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

		PPEditorStation _parent;
		Model.Process _model;
		public int ActivityId { get; set; }
		public int StateStationActivityId { get; set; }
		public int ProcessId { get; set; }

		#region DpProps
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorActivity), new UIPropertyMetadata(null));
		//CycleTime Dependency Property
		public float CycleTime
		{
			get { return (float)GetValue(CycleTimeProperty); }
			set { SetValue(CycleTimeProperty, value); }
		}
		public static readonly DependencyProperty CycleTimeProperty =
			DependencyProperty.Register("CycleTime", typeof(float), typeof(PPEditorActivity), new UIPropertyMetadata(0f));
		//ManHour Dependency Property
		public float ManHour
		{
			get { return (float)GetValue(ManHourProperty); }
			set { SetValue(ManHourProperty, value); }
		}
		public static readonly DependencyProperty ManHourProperty =
			DependencyProperty.Register("ManHour", typeof(float), typeof(PPEditorActivity), new UIPropertyMetadata(0f));
		//TargetPoint Dependency Property
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(PPEditorActivity),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var val = (int)e.NewValue;
				var vm = (PPEditorActivity)d;
				if (vm._model == null)
				{
					vm.HasUnsavedChanges = (val > 0);
				}
				else vm.HasUnsavedChanges = (val != vm._model.TargetCount);
			}));
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
			DependencyProperty.Register("SelectedOperators", typeof(int), typeof(PPEditorActivity), new UIPropertyMetadata(0));
		//DoesParentDeferToActivities Dependency Property
		public bool DoesParentDeferToActivities
		{
			get { return (bool)GetValue(DoesParentDeferToActivitiesProperty); }
			set { SetValue(DoesParentDeferToActivitiesProperty, value); }
		}
		public static readonly DependencyProperty DoesParentDeferToActivitiesProperty =
			DependencyProperty.Register("DoesParentDeferToActivities", typeof(bool), typeof(PPEditorActivity), new UIPropertyMetadata(true));
		//HasUnsavedChanges Dependency Property
		public bool HasUnsavedChanges
		{
			get { return (bool)GetValue(HasUnsavedChangesProperty); }
			set { SetValue(HasUnsavedChangesProperty, value); }
		}
		public static readonly DependencyProperty HasUnsavedChangesProperty =
			DependencyProperty.Register("HasUnsavedChanges", typeof(bool), typeof(PPEditorActivity),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
					((PPEditorActivity)d)._parent.HasUnsavedChanges = true;
			}));
		#endregion
	}
}
