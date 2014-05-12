using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	/// <summary>
	/// ViewModel for a <see cref="Soheil.Model.WorkShift"/> which includes a <see cref="WorkShiftPrototypeVm"/> and a collection of <see cref="WorkBreakVm"/>s
	/// </summary>
	public class WorkShiftVm : DependencyObject
	{
		/// <summary>
		/// Occurs when IsOpen changed on this shift
		/// </summary>
		public event Action<bool> IsOpenChanged;

		public Model.WorkShift Model { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="WorkShiftVm"/> with the given model and work shift prototype viewModel
		/// </summary>
		/// <param name="model"></param>
		/// <param name="prototype"></param>
		public WorkShiftVm(Model.WorkShift model, WorkShiftPrototypeVm prototype)
		{
			Model = model;
			Prototype = prototype;
			StartSeconds = model.StartSeconds;
			EndSeconds = model.EndSeconds;
			IsOpen = model.IsOpen;

			//add workbreak models
			foreach (var workBreak in model.WorkBreaks)
			{
				var wbreak = new WorkBreakVm(workBreak);
				wbreak.DeleteCommand = new Commands.Command(o => Breaks.Remove(wbreak));
				Breaks.Add(wbreak);
			}

			//auto add future workbreak models
			Breaks.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
					foreach (WorkBreakVm wbreak in e.NewItems)
					{
						Model.WorkBreaks.Add(wbreak.Model);
					}
				if(e.OldItems!=null)
					foreach (WorkBreakVm wbreak in e.OldItems)
					{
						Model.WorkBreaks.Remove(wbreak.Model);
					}
			};

			ToggleIsOpenCommand = new Commands.Command(o => IsOpen = !IsOpen);
		}


		/// <summary>
		/// Gets or sets a bindable value for start of this time range
		/// <para>The number of seconds after 0:00AM</para>
		/// <para>Changing this value updates model's StartSeconds</para>
		/// <remarks>The value received by this property will be rounded by 5mins and trimmed bases on constraints, EndSeconds</remarks>
		/// </summary>
		public int StartSeconds
		{
			get { return (int)GetValue(StartSecondsProperty); }
			set { SetValue(StartSecondsProperty, value); }
		}
		public static readonly DependencyProperty StartSecondsProperty =
			DependencyProperty.Register("StartSeconds", typeof(int), typeof(WorkShiftVm), 
			new UIPropertyMetadata(SoheilConstants.EDITOR_START_SECONDS, (d, e) => 
			{
				var vm = (WorkShiftVm)d;
				var val = (int)e.NewValue;
				vm.Model.StartSeconds = val;
			}, (d, v) =>
			{
				var val = (int)v;
				var vm = (WorkShiftVm)d;
				if (vm.Model.EndSeconds - val < 3600) return vm.Model.EndSeconds - 3600;
				if (val < SoheilConstants.EDITOR_START_SECONDS) return SoheilConstants.EDITOR_START_SECONDS;
				if (val > SoheilConstants.EDITOR_END_SECONDS - 3600) return SoheilConstants.EDITOR_END_SECONDS - 3600;
				return SoheilFunctions.RoundFiveMinutes(val);
			}));
		/// <summary>
		/// Gets or sets a bindable value for end of this time range
		/// <para>The number of seconds after 0:00AM</para>
		/// <para>Changing this value updates model's EndSeconds</para>
		/// <remarks>The value received by this property will be rounded by 5mins and trimmed bases on constraints, StartSeconds</remarks>
		/// </summary>
		public int EndSeconds
		{
			get { return (int)GetValue(EndSecondsProperty); }
			set { SetValue(EndSecondsProperty, value); }
		}
		public static readonly DependencyProperty EndSecondsProperty =
			DependencyProperty.Register("EndSeconds", typeof(int), typeof(WorkShiftVm), 
			new UIPropertyMetadata(SoheilConstants.EDITOR_START_SECONDS, (d, e) => 
			{
				var vm = (WorkShiftVm)d;
				var val = (int)e.NewValue;
				vm.Model.EndSeconds = val;
			}, (d, v) =>
			{
				var val = (int)v;
				var vm = (WorkShiftVm)d;
				if (val < vm.Model.StartSeconds + 3600) return vm.Model.StartSeconds + 3600;
				if (val < SoheilConstants.EDITOR_START_SECONDS + 3600) return SoheilConstants.EDITOR_START_SECONDS + 3600;
				if (val > SoheilConstants.EDITOR_END_SECONDS) return SoheilConstants.EDITOR_END_SECONDS;
				return SoheilFunctions.RoundFiveMinutes(val);
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this shift is open in current work day
		/// </summary>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(WorkShiftVm),
			new UIPropertyMetadata(true, (d, e) =>
			{
				var vm = (WorkShiftVm)d;
				var val = (bool)e.NewValue;
				vm.Model.IsOpen = val;
				if (vm.IsOpenChanged != null) vm.IsOpenChanged(val);
			}));

		/// <summary>
		/// Gets or sets a bindable command that toggles the IsOpen value
		/// </summary>
		public Commands.Command ToggleIsOpenCommand
		{
			get { return (Commands.Command)GetValue(ToggleIsOpenCommandProperty); }
			set { SetValue(ToggleIsOpenCommandProperty, value); }
		}
		public static readonly DependencyProperty ToggleIsOpenCommandProperty =
			DependencyProperty.Register("ToggleIsOpenCommand", typeof(Commands.Command), typeof(WorkShiftVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the bindable Prototype of this shift which includes additional information about this shift
		/// </summary>
		public WorkShiftPrototypeVm Prototype
		{
			get { return (WorkShiftPrototypeVm)GetValue(PrototypeProperty); }
			set { SetValue(PrototypeProperty, value); }
		}
		public static readonly DependencyProperty PrototypeProperty =
			DependencyProperty.Register("Prototype", typeof(WorkShiftPrototypeVm), typeof(WorkShiftVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets an observable collection of WorkBreaks for this shift
		/// </summary>
		public ObservableCollection<WorkBreakVm> Breaks { get { return _breaks; } }
		private ObservableCollection<WorkBreakVm> _breaks = new ObservableCollection<WorkBreakVm>();

		/// <summary>
		/// Creates a temporary break at the given time with duration of zero, and adds it to Breaks
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public WorkBreakVm AddTemporaryBreak(int seconds)
		{
			var wbreak = new WorkBreakVm(new Model.WorkBreak
			{
				WorkShift = Model,
				StartSeconds = seconds,
				EndSeconds = seconds,
			});
			wbreak.DeleteCommand = new Commands.Command(o => Breaks.Remove(wbreak));
			Breaks.Add(wbreak);
			return wbreak;
		}
	}
}
