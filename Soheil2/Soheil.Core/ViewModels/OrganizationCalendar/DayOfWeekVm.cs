using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class DayOfWeekVm : DependencyObject
	{
		public DayOfWeekVm(int index, WorkDayVm dayStateVm)
		{
			DayOfWeek = index;
			Name = ((PersianDayOfWeek)index).ToString();
			SelectedDayStateVm = dayStateVm;
		}
		public int DayOfWeek { get; set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(DayOfWeekVm), new UIPropertyMetadata(null));
		//IsWeekStart Dependency Property
		public bool IsWeekStart
		{
			get { return (bool)GetValue(IsWeekStartProperty); }
			set { SetValue(IsWeekStartProperty, value); }
		}
		public static readonly DependencyProperty IsWeekStartProperty =
			DependencyProperty.Register("IsWeekStart", typeof(bool), typeof(DayOfWeekVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (DayOfWeekVm)d;
				var val = (bool)e.NewValue;
				if (vm.WeekStartChanged != null)
					vm.WeekStartChanged(vm, new PropertyChangedEventArgs(vm.DayOfWeek, val));

			}));

		#region State and Color
		//State Dependency Property
		public BusinessDayType State
		{
			get { return (BusinessDayType)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(BusinessDayType), typeof(DayOfWeekVm),
			new UIPropertyMetadata(BusinessDayType.Open, (d, e) =>
			{
				var vm = (DayOfWeekVm)d;
				var val = (BusinessDayType)e.NewValue;
				vm.Color = Model.WorkDay.GetColorByNr((int)e.NewValue);
				if (vm.BusinessStateChanged != null)
					vm.BusinessStateChanged(vm, new PropertyChangedEventArgs(vm.DayOfWeek, val));
			}));
		//SelectedDayStateVm Dependency Property
		public WorkDayVm SelectedDayStateVm
		{
			get { return (WorkDayVm)GetValue(SelectedDayStateVmProperty); }
			set { SetValue(SelectedDayStateVmProperty, value); }
		}
		public static readonly DependencyProperty SelectedDayStateVmProperty =
			DependencyProperty.Register("SelectedDayStateVm", typeof(WorkDayVm), typeof(DayOfWeekVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (DayOfWeekVm)d;
				var val = (WorkDayVm)e.NewValue;
				if (val == null) return;
				vm.State = val.BusinessState;
			}));
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(DayOfWeekVm), new UIPropertyMetadata(Colors.Transparent)); 
		#endregion


		#region Event stuff
		public event EventHandler<PropertyChangedEventArgs> BusinessStateChanged;
		public event EventHandler<PropertyChangedEventArgs> WeekStartChanged;
		
		public class PropertyChangedEventArgs : EventArgs
		{
			public PropertyChangedEventArgs(int dayIndex, BusinessDayType newState)
			{
				Index = dayIndex;
				State = newState;
			}
			public PropertyChangedEventArgs(int newWeekStart, bool isWeekStart)
			{
				Index = newWeekStart;
				IsWeekStart = isWeekStart;
			}

			public bool IsWeekStart { get; private set; }
			public int Index { get; private set; }
			public BusinessDayType State { get; private set; }
		} 
		#endregion
	}
}
