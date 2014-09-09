﻿using System;
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
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Soheil.Tablet"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Soheil.Tablet;assembly=Soheil.Tablet"
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
		static TimeBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeBox), new FrameworkPropertyMetadata(typeof(TimeBox)));
		}
		private bool _suppress = false;


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
			DependencyProperty.Register("Hour", typeof(int), typeof(TimeBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
			{
				var vm = (TimeBox)d;
				if (vm._suppress) return;
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
				if (vm._suppress) return;
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
				if (vm._suppress) return;
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
	}
}