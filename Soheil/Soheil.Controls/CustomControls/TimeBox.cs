using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Soheil.Controls.CustomControls
{
	public class TimeBox : Control
	{
		public TimeBox()
		{
			SetTimeCommand = new CustomCommand(this);
		}
		static TimeBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeBox), new FrameworkPropertyMetadata(typeof(TimeBox)));
		}

		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TimeBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = d as TimeBox;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				vm.Time = TimeSpan.FromSeconds(val);
			}));
		//Time Dependency Property
		public TimeSpan Time
		{
			get { return (TimeSpan)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, value); }
		}
		public static readonly DependencyProperty TimeProperty =
			DependencyProperty.Register("Time", typeof(TimeSpan), typeof(TimeBox), 
			new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (TimeBox)d;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (TimeSpan)e.NewValue;

				if (vm.DurationSeconds != (int)val.TotalSeconds) vm.DurationSeconds = (int)val.TotalSeconds;

				vm.Hour = (int)val.TotalHours;
				vm.Minute = val.Minutes;
				vm.Second = val.Seconds;
			}));

		//Hour Dependency Property
		public int Hour
		{
			get { return (int)GetValue(HourProperty); }
			set { SetValue(HourProperty, value); }
		}
		public static readonly DependencyProperty HourProperty =
			DependencyProperty.Register("Hour", typeof(int), typeof(TimeBox), 
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (TimeBox)d;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if ((int)vm.Time.TotalHours != val) vm.Time = new TimeSpan(val, vm.Minute, vm.Second);
			}, (d, v) =>
			{
				var vm = (TimeBox)d;
				if ((int)v >= 999) return 999;
				if ((int)v <= 0) return 0;
				return v;
			}));
		//Minute Dependency Property
		public int Minute
		{
			get { return (int)GetValue(MinuteProperty); }
			set { SetValue(MinuteProperty, value); }
		}
		public static readonly DependencyProperty MinuteProperty =
			DependencyProperty.Register("Minute", typeof(int), typeof(TimeBox), 
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (TimeBox)d;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if (vm.Time.Minutes != val) vm.Time = new TimeSpan(vm.Hour, val, vm.Second);
			}, (d, v) =>
			{
				var vm = (TimeBox)d;
				if ((int)v >= 59) return 59;
				if ((int)v <= 0) return 0;
				return v;
			}));
		//Second Dependency Property
		public int Second
		{
			get { return (int)GetValue(SecondProperty); }
			set { SetValue(SecondProperty, value); }
		}
		public static readonly DependencyProperty SecondProperty =
			DependencyProperty.Register("Second", typeof(int), typeof(TimeBox), 
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (TimeBox)d;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if (vm.Time.Seconds != val) vm.Time = new TimeSpan(vm.Hour, vm.Minute, val);
			}, (d, v) =>
			{
				var vm = (TimeBox)d;
				if ((int)v >= 59) return 59;
				if ((int)v <= 0) return 0;
				return v;
			}));

		//IsReadOnly Dependency Property
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty =
			DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimeBox),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as TimeBox;
				if (vm.SetTimeCommand != null)
					vm.SetTimeCommand.Changed();
			}));

		public CustomCommand SetTimeCommand
		{
			get { return (CustomCommand)GetValue(SetTimeCommandProperty); }
			set { SetValue(SetTimeCommandProperty, value); }
		}
		public static readonly DependencyProperty SetTimeCommandProperty =
			DependencyProperty.Register("SetTimeCommand", typeof(CustomCommand), typeof(TimeBox), new PropertyMetadata(null));
		public class CustomCommand : ICommand
		{
			public bool CanExecute(object parameter)
			{
				return !_tb.IsReadOnly;
			}
			public void Changed() { if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs()); }
			public event EventHandler CanExecuteChanged;
			public void Execute(object parameter)
			{
				if (_tb == null) return;
				if (_tb.IsReadOnly) return;
				switch ((TimeBoxParameter)parameter)
				{
					case TimeBoxParameter.Now:
						_tb.Time = DateTime.Now.TimeOfDay;
						break;
					case TimeBoxParameter.FirstEmptySpace:
						_tb.Time = TimeSpan.Zero;
						break;
					case TimeBoxParameter.StartOfHour:
						_tb.Time = new TimeSpan((int)_tb.Time.TotalHours, 0, 0);
						break;
					case TimeBoxParameter.NextHour:
						_tb.Time = _tb.Time.Add(TimeSpan.FromHours(1));
						break;
					case TimeBoxParameter.PreviousHour:
						_tb.Time = _tb.Time.Add(TimeSpan.FromHours(-1));
						break;
					case TimeBoxParameter.AddHalfHour:
						_tb.Time = _tb.Time.Add(TimeSpan.FromMinutes(30));
						break;
					case TimeBoxParameter.Add5Minutes:
						_tb.Time = _tb.Time.Add(TimeSpan.FromMinutes(5));
						break;
					case TimeBoxParameter.Add1Minute:
						_tb.Time = _tb.Time.Add(TimeSpan.FromMinutes(1));
						break;
					default:
						break;
				}
			}
			TimeBox _tb;
			public CustomCommand(TimeBox tb)
			{
				_tb = tb;
			}
		}

	}
}
