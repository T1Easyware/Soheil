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

namespace Soheil.Tablet
{
	/// <summary>
	/// Interaction logic for DurationBox.xaml
	/// </summary>
	public partial class DurationBox : UserControl
	{
		public DurationBox()
		{
			InitializeComponent();
			SetDurationMinutesCommand = new CustomCommand(this);
		}

		private bool _suppress = false;
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(DurationBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = d as DurationBox;

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
			DependencyProperty.Register("Time", typeof(TimeSpan), typeof(DurationBox),
			new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (DurationBox)d;

				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (TimeSpan)e.NewValue;

				if (vm.DurationSeconds != (int)val.TotalSeconds) vm.DurationSeconds = (int)val.TotalSeconds;

				vm._suppress = true;
				vm.Hour = (int)val.TotalHours;
				vm.Minute = val.Minutes;
				vm.Second = val.Seconds;
				vm._suppress = false;
			}));

		//Hour Dependency Property
		public int Hour
		{
			get { return (int)GetValue(HourProperty); }
			set { SetValue(HourProperty, value); }
		}
		public static readonly DependencyProperty HourProperty =
			DependencyProperty.Register("Hour", typeof(int), typeof(DurationBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (DurationBox)d;
				if (vm._suppress) return;
				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if ((int)vm.Time.TotalHours != val) vm.Time = new TimeSpan(val, vm.Minute, vm.Second);
			}, (d, v) =>
			{
				var vm = (DurationBox)d;
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
			DependencyProperty.Register("Minute", typeof(int), typeof(DurationBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (DurationBox)d;
				if (vm._suppress) return;
				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if (vm.Time.Minutes != val) vm.Time = new TimeSpan(vm.Hour, val, vm.Second);
			}, (d, v) =>
			{
				var vm = (DurationBox)d;
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
			DependencyProperty.Register("Second", typeof(int), typeof(DurationBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (DurationBox)d;
				if (vm._suppress) return;
				if (e.NewValue == DependencyProperty.UnsetValue) return;
				var val = (int)e.NewValue;

				if (vm.Time.Seconds != val) vm.Time = new TimeSpan(vm.Hour, vm.Minute, val);
			}, (d, v) =>
			{
				var vm = (DurationBox)d;
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
			DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DurationBox),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as DurationBox;
				if (vm.SetDurationMinutesCommand != null)
					vm.SetDurationMinutesCommand.Changed();
			}));

		public CustomCommand SetDurationMinutesCommand
		{
			get { return (CustomCommand)GetValue(SetDurationMinutesCommandProperty); }
			set { SetValue(SetDurationMinutesCommandProperty, value); }
		}
		public static readonly DependencyProperty SetDurationMinutesCommandProperty =
			DependencyProperty.Register("SetDurationMinutesCommand", typeof(CustomCommand), typeof(DurationBox), new PropertyMetadata(null));
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
				_tb.DurationSeconds = (int)parameter * 60;
			}
			DurationBox _tb;
			public CustomCommand(DurationBox tb)
			{
				_tb = tb;
			}
		}

	}
}
