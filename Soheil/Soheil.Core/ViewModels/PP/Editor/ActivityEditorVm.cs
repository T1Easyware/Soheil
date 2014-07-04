using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.ViewModels.PP.Report;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class ActivityEditorVm : ActivityVm
	{
		/// <summary>
		/// Occured when start or end of a process is changed
		/// </summary>
		public event Action<ProcessEditorVm, DateTime, DateTime> TimesChanged;
		/// <summary>
		/// Occurs when the specifed process is selected in this activity
		/// <para>Also when last process is deleted and therefore no selected process exists</para>
		/// </summary>
		public event Action<ProcessEditorVm, bool> Selected;
		/// <summary>
		/// Occurs when selected choice of SSAs for this Process is changed
		/// <para>second parameter can be null</para>
		/// </summary>
		public event Action<ProcessEditorVm, ChoiceEditorVm> SelectedChoiceChanged;
		public Func<DateTime> GetTaskStart;
		public ActivityEditorVm(
			Model.Task task, 
			Dal.SoheilEdmContext uow,
			IGrouping<Model.Activity, Model.StateStationActivity> ssaGroup)
			: base(ssaGroup.Key)
		{
			Message = new Common.SoheilException.EmbeddedException();

			if (!ssaGroup.Any())
			{
				Message.AddEmbeddedException("فعالیتی وجود ندارد");
				return;
			}

			//make ProcessList self-aware of all changes
			ProcessList.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
					foreach (ProcessEditorVm processVm in e.NewItems)
					{
						ProcessList_Added(processVm);
					}
			};

			//Add Choices
			foreach (var choice in ssaGroup.OrderBy(ssa => ssa.ManHour))
			{
				Choices.Add(new ChoiceEditorVm(choice));
			}

			//Add existing processes
			foreach (var process in task.Processes.Where(x => x.StateStationActivity.Activity.Id == ssaGroup.Key.Id))
			{
				ProcessList.Add(new ProcessEditorVm(process, Model, uow));
			}

			//Add process command
			AddProcessCommand = new Commands.Command(o =>
			{
				
				DateTime dt;
				if (GetTaskStart == null)
					dt = ProcessList.Any() ?
						ProcessList
							.Where(x => x.ActivityModel.Id == ssaGroup.Key.Id)
							.Max(x => x.Model.EndDateTime)
						: task.StartDateTime;
				else
					dt = GetTaskStart();

				var minMH = ssaGroup.Min(x => x.ManHour);

				var processVm = new ProcessEditorVm(
					new Model.Process
					{
						StartDateTime = dt,
						EndDateTime = dt,
						StateStationActivity = ssaGroup.First(x=>x.ManHour == minMH),
						TargetCount = 0,
						Task = task,
					}, Model, uow);//activity Model is set here
				ProcessList.Add(processVm);
				processVm.IsSelected = true;
			});
		}

		#region Event Handlers
		void ProcessList_Added(ProcessEditorVm processVm)
		{
			//create choice menuitems for process
			processVm.Choices.Clear();
			foreach (var choice in Choices)
			{
				var choiceVm = new ChoiceEditorVm(choice.Model);
				choiceVm.Selected += ch =>
					processVm.SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == ch.ManHour);
				processVm.Choices.Add(choiceVm);
			}

			//notify Block about selection and delete
			processVm.Selected += Process_Selected;
			processVm.Deleted += Process_Deleted;

			//notify Block about changes in times
			processVm.TimesChanged += Process_TimesChanged;

			//change selected choice for the process
			processVm.SelectedOperatorsCountChanged += Process_OperatorsCountChanged;

			//Updates process TargetPoint or DurationSeconds based on Block fixed data (deferred, fixedDuration, fixedTP)
			processVm.SelectedChoiceChanged += Process_SelectedChoiceChanged;

			if (processVm.Model.StateStationActivity != null)
			{
				//select the right choice based on manHour
				//processVm.SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == processVm.SelectedOperatorsCount);

				//select the right choice based on SSA
				processVm.SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == processVm.Model.StateStationActivity.ManHour);
			}
		}

		void Process_Selected(ProcessEditorVm processVm)
		{
			if (Selected != null)
				Selected(processVm, true);
		}
		void Process_Deleted(ProcessEditorVm processVm)
		{
			processVm.IsSelected = false;
			ProcessList.Remove(processVm);
			if (ProcessList.Any()) ProcessList.First().IsSelected = true;
			else
			{
				if (Selected != null)
					Selected(processVm, false);
			}
		}
		
		void Process_TimesChanged(ProcessEditorVm processVm, DateTime start, DateTime end)
		{
			if (TimesChanged != null)
				TimesChanged(processVm, start, end);
		}
		void Process_OperatorsCountChanged(ProcessEditorVm processVm, int count)
		{
			processVm.SelectedChoice = Choices.FirstOrDefault(x => x.ManHour == count);
		}
		void Process_SelectedChoiceChanged(ProcessEditorVm processVm, ChoiceEditorVm newChoice)
		{
			if (SelectedChoiceChanged != null)
				SelectedChoiceChanged(processVm, newChoice);
		}
		#endregion

		/// <summary>
		/// Gets a bindable collection of choices (StateStationActivities) that can be used to create a process for this activity
		/// <para>These choices all have the same Activity but different StateStationActivities (each of which has a unique ManHour)</para>
		/// </summary>
		public ObservableCollection<ChoiceEditorVm> Choices { get { return _choices; } }
		private ObservableCollection<ChoiceEditorVm> _choices = new ObservableCollection<ChoiceEditorVm>();

		/// <summary>
		/// Gets a bindable collection of processes (all have same activity)
		/// </summary>
		public ObservableCollection<ProcessEditorVm> ProcessList { get { return _processList; } }
		private ObservableCollection<ProcessEditorVm> _processList = new ObservableCollection<ProcessEditorVm>();


		//Message Dependency Property
		public Soheil.Common.SoheilException.EmbeddedException Message
		{
			get { return (Soheil.Common.SoheilException.EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(Soheil.Common.SoheilException.EmbeddedException), typeof(ActivityEditorVm), new UIPropertyMetadata(null));

		//AddProcessCommand Dependency Property
		public Commands.Command AddProcessCommand
		{
			get { return (Commands.Command)GetValue(AddProcessCommandProperty); }
			set { SetValue(AddProcessCommandProperty, value); }
		}
		public static readonly DependencyProperty AddProcessCommandProperty =
			DependencyProperty.Register("AddProcessCommand", typeof(Commands.Command), typeof(ActivityEditorVm), new UIPropertyMetadata(null));
	}
}
