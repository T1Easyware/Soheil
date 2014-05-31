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
	/// <summary>
	/// ViewModel for a day of persian week
	/// </summary>
	public class DayOfWeekVm : DependencyObject
	{
		/// <summary>
		/// Occurs when this day of week changes its openness state of business
		/// </summary>
		public event Action<int, BusinessDayType> BusinessStateChanged;
		/// <summary>
		/// Occurs when this day of week changes its IsWeekStart value
		/// </summary>
		public event Action<int, bool> WeekStartChanged;
	
		/// <summary>
		/// Creates an instance of DayOfWeekVm with the given openness state
		/// </summary>
		/// <param name="index">indicates which day of week this is</param>
		/// <param name="dayStateVm">openness state of this day of week</param>
		public DayOfWeekVm(int index, WorkDayVm dayStateVm)
		{
			DayOfWeek = index;
			Name = ((PersianDayOfWeek)index).ToString();
			SelectedDayStateVm = dayStateVm;
		}
		
		/// <summary>
		/// Gets the day of week (zero-biased index)
		/// </summary>
		public int DayOfWeek { get; private set; }

		/// <summary>
		/// Gets the bindable text for name of this day
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			private set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(DayOfWeekVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this day starts the week
		/// <para>Changing this value fires WeekStartChanged event</para>
		/// </summary>
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
					vm.WeekStartChanged(vm.DayOfWeek, val);
			}));

		/// <summary>
		/// Gets or sets a bindable value for openness state of business for this day of week
		/// </summary>
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
				//vm.Color = Model.WorkDay.GetColorByNr((int)e.NewValue);
				if (vm.BusinessStateChanged != null)
					vm.BusinessStateChanged(vm.DayOfWeek, val);
			}));
		
		/// <summary>
		/// Gets or sets a bindable value for an instance of WorkDay representing the openness state of business of this day of week
		/// </summary>
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
	}
}
