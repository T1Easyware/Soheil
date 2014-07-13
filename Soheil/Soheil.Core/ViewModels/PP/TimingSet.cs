using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public class TimingSet : DependencyObject
	{
		#region Events
		public event Action<int> TargetPointChanged;
		public event Action<int> DurationChanged;
		public event Action<DateTime> StartChanged;
		public event Action<DateTime> EndChanged;
		public event Action Saved;
		/// <summary>
		/// Occurs when Start or end of this vm is changed
		/// <para>first arg is Start, seconds arg is End</para>
		/// </summary>
		public event Action<DateTime, DateTime> TimesChanged;
		#endregion

		public TimingSet(Soheil.Core.ViewModels.PP.Report.ProcessReportVm vm)
		{
			IsInitializing = () => vm._isInInitializingPhase;
			SetIsInitializing = v => vm._isInInitializingPhase = v;
			GetCycleTime = () => vm.Model.Process.StateStationActivity.CycleTime;
			GetLowerBound = () => vm.Model.Process.StartDateTime;
			GetUpperBound = () => vm.Model.Process.EndDateTime;

			TargetPoint = vm.Model.ProcessReportTargetPoint;
			DurationSeconds = vm.Model.DurationSeconds;
			StartDateTime = vm.Model.StartDateTime;
			EndDateTime = vm.Model.EndDateTime;

			AutoDurationCommand = new Commands.Command(o =>
			{
				DurationSeconds = (int)(TargetPoint * GetCycleTime());
			});
			AutoTargetPointCommand = new Commands.Command(o =>
			{
				TargetPoint = (int)(DurationSeconds / GetCycleTime());
			});
		}

		public TimingSet(Soheil.Core.ViewModels.PP.Editor.ProcessEditorVm vm)
		{
			IsInitializing = () => vm._isInitializing;
			SetIsInitializing = v => vm._isInitializing = v;
			GetCycleTime = () => vm.Model.StateStationActivity == null ? 0f : vm.Model.StateStationActivity.CycleTime;
			GetLowerBound = () => DateTime.MinValue;//vm.Model.Task.StartDateTime.TruncateMilliseconds();
			GetUpperBound = () => DateTime.MaxValue;

			_suppress = true;
			vm.Model.TargetCount = vm.Model.TargetCount <= 0 ? 1 : vm.Model.TargetCount;
			vm.Model.DurationSeconds = vm.Model.DurationSeconds < GetCycleTime() ? (int)GetCycleTime() : vm.Model.DurationSeconds;
			vm.Model.EndDateTime = vm.Model.EndDateTime - vm.Model.StartDateTime < TimeSpan.FromSeconds(GetCycleTime()) ?
				vm.Model.StartDateTime + TimeSpan.FromSeconds(GetCycleTime()) :
				vm.Model.EndDateTime;
			TargetPoint = vm.Model.TargetCount;
			StartDateTime = vm.Model.StartDateTime;
			DurationSeconds = vm.Model.DurationSeconds;
			EndDateTime = vm.Model.EndDateTime;

			AutoDurationCommand = new Commands.Command(o =>
			{
				DurationSeconds = (int)(TargetPoint * GetCycleTime());
			});
			AutoTargetPointCommand = new Commands.Command(o =>
			{
				TargetPoint = (int)(DurationSeconds / GetCycleTime());
			});
		}

		#region Timing props

		bool _suppress = false;
		public Func<bool> IsInitializing;
		public Action<bool> SetIsInitializing;
		public Func<DateTime> GetLowerBound;
		public Func<DateTime> GetUpperBound;
		public Func<float> GetCycleTime;

		public TimingSet PreviousReport { get; set; }
		public TimingSet NextReport { get; set; }

		public DateTime LowerBound { get { return PreviousReport == null ? GetLowerBound() : PreviousReport.EndDateTime; } }
		public DateTime UpperBound { get { return NextReport == null ? GetUpperBound() : NextReport.StartDateTime; } }

		#endregion

		#region Timing dependency props
		/// <summary>
		/// Gets or sets the bindable number of target point for this process report
		/// </summary>
		public int TargetPoint
		{
			get { return (int)GetValue(TargetPointProperty); }
			set { SetValue(TargetPointProperty, value); }
		}
		public static readonly DependencyProperty TargetPointProperty =
			DependencyProperty.Register("TargetPoint", typeof(int), typeof(TimingSet),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (TimingSet)d;
				var val = (int)e.NewValue;
				if (vm.TargetPointChanged != null) vm.TargetPointChanged(val);

				if (vm.IsInitializing()) return;

				//set Duration if [÷]
				if (vm.IsDurationDividable)
					vm.DurationSeconds = (int)vm.GetCycleTime() * val;

				if (vm.Saved != null) vm.Saved();

			}, (d, v) =>
			{
				var vm = (TimingSet)d;
				var val = (int)v;
				if (val < 1) return 1;
				return v;
			}));

		#region Duration
		/// <summary>
		/// Gets the biggest integer multiply of CT which is less than duration
		/// </summary>
		/// <param name="duration">uncoerced input</param>
		/// <returns></returns>
		int getDividableSeconds(TimeSpan duration)
		{
			return getDividableSeconds((int)duration.TotalSeconds);
		}
		/// <summary>
		/// Gets the biggest integer multiply of CT which is less than duration
		/// </summary>
		/// <param name="duration">uncoerced input</param>
		/// <returns></returns>
		int getDividableSeconds(int duration)
		{
			return
				(int)
					(GetCycleTime() *
					(int)
						(duration / GetCycleTime())
					);
		}

		//IsDurationDividable Dependency Property
		public bool IsDurationDividable
		{
			get { return (bool)GetValue(IsDurationDividableProperty); }
			set { SetValue(IsDurationDividableProperty, value); }
		}
		public static readonly DependencyProperty IsDurationDividableProperty =
			DependencyProperty.Register("IsDurationDividable", typeof(bool), typeof(TimingSet),
			new UIPropertyMetadata(true, (d, e) =>
			{
				var vm = d as TimingSet;
				if ((bool)e.NewValue)
				{
					//coerce all
					var dur = vm.getDividableSeconds(vm.DurationSeconds);
					if (dur != vm.DurationSeconds)
					{
						vm.DurationSeconds = dur;
					}
				}
			}));
		/// <summary>
		/// Gets or sets the bindable duration seconds
		/// </summary>
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TimingSet), new UIPropertyMetadata(TimeSpan.Zero));
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TimingSet),
			new UIPropertyMetadata(0, (d, e) =>
			{
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));
				((TimingSet)d).ProcessReportVm_DurationSecondsChanged((int)e.NewValue);
			}, (d, v) =>
			{
				var vm = (TimingSet)d;
				var val = (int)v;
				//if very small => set to 1CT
				if (val < vm.GetCycleTime())
				{
					return (int)vm.GetCycleTime();
				}
				//if very large => set to Whole space
				if (!vm._suppress && val > (vm.UpperBound - vm.LowerBound).TotalSeconds)
				{
					val = (int)(vm.UpperBound - vm.LowerBound).TotalSeconds;
				}
				//coerce if [÷]
				if (vm.IsDurationDividable)
				{
					return vm.getDividableSeconds(val);
				}
				else
				{
					//don't coerce
					return val;
				}
			}));
		void ProcessReportVm_DurationSecondsChanged(int newVal)
		{
			if (!IsInitializing())
			{
				//update TargetPoint
				TargetPoint = (int)(newVal / GetCycleTime());

				//update EndDateTime
				EndDateTime = StartDateTime.AddSeconds(newVal);

				if (DurationChanged != null) DurationChanged(newVal);
				if (Saved != null) Saved();
			}
		}
		#endregion

		#region Start
		/// <summary>
		/// Gets or sets the bindable Start dateTime
		/// </summary>
		public DateTime StartDateTime
		{
			get { return (DateTime)GetValue(StartDateTimeProperty); }
			set { SetValue(StartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty StartDateTimeProperty =
			DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(TimingSet),
			new UIPropertyMetadata(DateTime.Now,
				(d, e) => ((TimingSet)d).ProcessReportVm_StartDateTimeChanged((DateTime)e.NewValue),
				(d, v) =>
				{
					var vm = (TimingSet)d;
					if (vm.IsInitializing()) return v;

					var val = (DateTime)v;
					//check lower bound
					if (val < vm.LowerBound)
					{
						val = vm.LowerBound;
					}
					else if (!vm._suppress && val > vm.EndDateTime.AddSeconds(-vm.GetCycleTime()))
					{
						val = vm.EndDateTime.AddSeconds(-vm.GetCycleTime());
					}

					if (val.AddSeconds(vm.DurationSeconds) > vm.UpperBound)
					{
						val = vm.UpperBound.AddSeconds(-vm.DurationSeconds);
					}
					val = val.TruncateMilliseconds();
					if (val != (DateTime)v)
					{
						//update only startTime & startDate
						var tmp = vm.IsInitializing();
						vm.SetIsInitializing(true);
						d.SetValue(StartTimeProperty, val.TimeOfDay);
						d.SetValue(StartDateProperty, val.Date);
						vm.SetIsInitializing(tmp);
					}
					return val;
				}));
		void ProcessReportVm_StartDateTimeChanged(DateTime newVal)
		{
			//update Model, EndDateTime, StartDate & StartTime
			if (!IsInitializing())
			{
				//update Model
				if (StartChanged != null) StartChanged(newVal);

				//Set EndDateTime
				EndDateTime = newVal.AddSeconds(DurationSeconds);

				SetIsInitializing(true);
				SetValue(StartTimeProperty, newVal.TimeOfDay);
				SetValue(StartDateProperty, newVal.Date);
				SetIsInitializing(false);

				if (Saved != null) Saved();
				if (TimesChanged != null) TimesChanged(newVal, EndDateTime);
			}
			else
			{
				//update only startTime & startDate
				SetValue(StartTimeProperty, newVal.TimeOfDay);
				SetValue(StartDateProperty, newVal.Date);
			}
		}

		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(TimingSet),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TimingSet;
				if (vm.IsInitializing()) return;
				var val = (DateTime)e.NewValue;
				vm.StartDateTime = val.Add((TimeSpan)d.GetValue(StartTimeProperty));
			}));
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(TimingSet),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TimingSet;
				if (vm.IsInitializing()) return;
				var val = (TimeSpan)e.NewValue;
				vm.StartDateTime = ((DateTime)d.GetValue(StartDateProperty)).Add(val);
			}));
		#endregion

		#region End
		/// <summary>
		/// Gets or sets the bindable Start dateTime
		/// </summary>
		public DateTime EndDateTime
		{
			get { return (DateTime)GetValue(EndDateTimeProperty); }
			set { SetValue(EndDateTimeProperty, value); }
		}
		public static readonly DependencyProperty EndDateTimeProperty =
			DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(TimingSet),
			new UIPropertyMetadata(DateTime.Now,
				(d, e) => ((TimingSet)d).ProcessReportVm_EndDateTimeChanged((DateTime)e.NewValue),
				(d, v) =>
				{
					var vm = (TimingSet)d;
					var val = (DateTime)v;
					//check upper bound
					if (val > vm.UpperBound)
					{
						val = vm.UpperBound;
					}
					else if (!vm._suppress && val < vm.StartDateTime.AddSeconds(vm.GetCycleTime()))
					{
						val = vm.StartDateTime.AddSeconds(vm.GetCycleTime());
					}
					//coerce if [÷]
					if (vm.IsDurationDividable)
					{
						int dur = vm.getDividableSeconds(val - vm.StartDateTime);
						val = vm.StartDateTime.AddSeconds(dur);
					}
					val = val.TruncateMilliseconds();
					if (val != (DateTime)v)
					{
						//update only endTime & endDate
						var tmp = vm.IsInitializing();
						vm.SetIsInitializing(true);
						d.SetValue(EndTimeProperty, val.TimeOfDay);
						d.SetValue(EndDateProperty, val.Date);
						vm.SetIsInitializing(tmp);
					}
					return val;
				}));
		void ProcessReportVm_EndDateTimeChanged(DateTime newVal)
		{
			//update Model, DurationSeconds, EndDate & EndTime
			if (!IsInitializing())
			{
				//update Model
				if (EndChanged != null) EndChanged(newVal);

				//update DurationSeconds
				if(!_suppress)
					DurationSeconds = (int)(newVal - StartDateTime).TotalSeconds;

				SetIsInitializing(true);
				SetValue(EndTimeProperty, newVal.TimeOfDay);
				SetValue(EndDateProperty, newVal.Date);
				SetIsInitializing(false);
			}
			else
			{
				//update only EndTime & EndDate
				SetValue(EndTimeProperty, newVal.TimeOfDay);
				SetValue(EndDateProperty, newVal.Date);
			}
			if (TimesChanged != null) TimesChanged(StartDateTime, newVal);
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(TimingSet),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as TimingSet;
				if (vm.IsInitializing()) return;
				var val = (DateTime)e.NewValue;
				vm.EndDateTime = val.Add((TimeSpan)d.GetValue(EndTimeProperty));
			}));
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(TimingSet),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as TimingSet;
				if (vm.IsInitializing()) return;
				var val = (TimeSpan)e.NewValue;
				vm.EndDateTime = ((DateTime)d.GetValue(EndDateProperty)).Add(val);
			}));
		#endregion

		#endregion

		#region Commands
		/// <summary>
		/// Gets or sets the bindable command to automatically set the TargetPoint of this report according to its DurationSeconds
		/// </summary>
		public Commands.Command AutoTargetPointCommand
		{
			get { return (Commands.Command)GetValue(AutoTargetPointCommandProperty); }
			set { SetValue(AutoTargetPointCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoTargetPointCommandProperty =
			DependencyProperty.Register("AutoTargetPointCommand", typeof(Commands.Command), typeof(TimingSet), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets the bindable command to automatically set the DurationSeconds of this report according to its TargetPoint
		/// </summary>
		public Commands.Command AutoDurationCommand
		{
			get { return (Commands.Command)GetValue(AutoDurationCommandProperty); }
			set { SetValue(AutoDurationCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoDurationCommandProperty =
			DependencyProperty.Register("AutoDurationCommand", typeof(Commands.Command), typeof(TimingSet), new UIPropertyMetadata(null));

		#endregion

	}
}
