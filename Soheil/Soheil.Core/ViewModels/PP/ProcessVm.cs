using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Core.ViewModels.PP.Editor;

namespace Soheil.Core.ViewModels.PP
{
	public class ProcessVm : DependencyObject
	{
		public ProcessVm(
			Model.StateStationActivity ssaModel,
			IList<PPEditorOperator> operators,
			IList<Model.ProcessOperator> usedOperators)
			: this(ssaModel)
		{
			foreach (var ssam in ssaModel.StateStationActivityMachines)
			{
				Machines.Add(new PPEditorMachine(ssam));
			}
			foreach (var oper in operators)
			{
				Operators.Add(oper);
			}
			foreach (var processOperator in usedOperators)
			{
				Operators.First(x => x.OperatorId == processOperator.Id).IsSelected = true;
			}
		}

		protected ProcessVm(Model.StateStationActivity ssaModel)
		{
			StateStationActivity = new StateStationActivityVm(ssaModel);
		}

		/// <summary>
		/// If you use this constructor, you can't use Machines or Operators
		/// </summary>
		/// <param name="model"></param>
		public ProcessVm(Model.Process model, TaskVm task)
			: this(model.StateStationActivity)
		{
			TargetCount = model.TargetCount;
			ProcessId = model.Id;
			Task = task;
		}
		/// <summary>
		/// Only used in ReportDetailedView, i.e. when ctor:ProcessVm(Model.Process model) is used
		/// </summary>
		public int ProcessId { get; set; }

		//Machines Observable Collection
		private ObservableCollection<PPEditorMachine> _machines = new ObservableCollection<PPEditorMachine>();
		public ObservableCollection<PPEditorMachine> Machines { get { return _machines; } }
		//Operators Observable Collection
		private ObservableCollection<PPEditorOperator> _operators = new ObservableCollection<PPEditorOperator>();
		public ObservableCollection<PPEditorOperator> Operators { get { return _operators; } }

		//TargetCount Dependency Property
		public int TargetCount
		{
			get { return (int)GetValue(TargetCountProperty); }
			set { SetValue(TargetCountProperty, value); }
		}
		public static readonly DependencyProperty TargetCountProperty =
			DependencyProperty.Register("TargetCount", typeof(int), typeof(ProcessVm), new UIPropertyMetadata(0));

		//StateStationActivity Dependency Property
		public StateStationActivityVm StateStationActivity
		{
			get { return (StateStationActivityVm)GetValue(StateStationActivityProperty); }
			set { SetValue(StateStationActivityProperty, value); }
		}
		public static readonly DependencyProperty StateStationActivityProperty =
			DependencyProperty.Register("StateStationActivity", typeof(StateStationActivityVm), typeof(ProcessVm), new UIPropertyMetadata(null));

		//Task Dependency Property
		public TaskVm Task
		{
			get { return (TaskVm)GetValue(TaskProperty); }
			set { SetValue(TaskProperty, value); }
		}
		public static readonly DependencyProperty TaskProperty =
			DependencyProperty.Register("Task", typeof(TaskVm), typeof(ProcessVm), new UIPropertyMetadata(null));

		//IsExpanded Dependency Property
		public event EventHandler<ProcessVm> IsExpandedCallback;
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ProcessVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
					if (((ProcessVm)d).IsExpandedCallback != null)
						((ProcessVm)d).IsExpandedCallback(d, (ProcessVm)d);
			}));
	}
}
