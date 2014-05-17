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
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls;assembly=Soheil.Controls.CustomControls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:TimeBox/>
	///
	/// </summary>
	public class TimeBox : Control
	{
		public TimeBox()
		{
			SetDurationMinutesCommand = new CustomCommand(this);
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
			new UIPropertyMetadata(0, (d, e) =>
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
			DependencyProperty.Register("Time", typeof(TimeSpan), typeof(TimeBox), new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
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
			DependencyProperty.Register("Hour", typeof(int), typeof(TimeBox), new UIPropertyMetadata(0, (d, e) =>
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
			DependencyProperty.Register("Minute", typeof(int), typeof(TimeBox), new UIPropertyMetadata(0, (d, e) =>
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
			DependencyProperty.Register("Second", typeof(int), typeof(TimeBox), new UIPropertyMetadata(0, (d, e) =>
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
			DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimeBox), new UIPropertyMetadata(false));

		public ICommand SetDurationMinutesCommand
		{
			get { return (ICommand)GetValue(SetDurationMinutesCommandProperty); }
			set { SetValue(SetDurationMinutesCommandProperty, value); }
		}
		public static readonly DependencyProperty SetDurationMinutesCommandProperty =
			DependencyProperty.Register("SetDurationMinutesCommand", typeof(ICommand), typeof(DurationToolbox), new PropertyMetadata(null));
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
				_tb.DurationSeconds = (int)parameter;
			}
			TimeBox _tb;
			public CustomCommand(TimeBox tb)
			{
				_tb = tb;
			}
		}
	}
}
