using Soheil.Common;
using Soheil.Core.Index;
using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }

		public OeeVm(AccessType access)
		{
			Access = access;
			DayStart = TimeSpan.FromHours(7);
			var ds = new DataServices.MachineFamilyDataService();
			var list = ds.GetActives();
			foreach (var item in list.Where(x => x.Machines.Any(y => y.Status == (byte)Common.Status.Active && y.HasOEE)))
			{
				var vm = new OeeMachineFamilyVm(item);
				vm.Selected += machine =>
				{
					CurrentMachine = machine;
					machine.Load(GetCurrentInfo());
				};
				MachineFamilies.Add(vm);
			}
		}

		public void Reload()
		{
			if (CurrentMachine != null)
				CurrentMachine.Load(GetCurrentInfo());
		}
		private OeeType GetCurrentInfo()
		{
			var oeeType = new OeeType();
			if (IsMonthly) oeeType.Interval = OeeType.OeeInterval.Monthly;
			else if (IsWeekly) oeeType.Interval = OeeType.OeeInterval.Weekly;
			else if (IsDaily) oeeType.Interval = OeeType.OeeInterval.Daily;
			oeeType.DayStart = DayStart;
			oeeType.MonthStart = MonthStart;
			return oeeType;
		}

		public ObservableCollection<OeeMachineFamilyVm> MachineFamilies { get { return _machineFamilies; } }
		private ObservableCollection<OeeMachineFamilyVm> _machineFamilies = new ObservableCollection<OeeMachineFamilyVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates the Current selected Machine in the list
		/// </summary>
		public OeeMachineVm CurrentMachine
		{
			get { return (OeeMachineVm)GetValue(CurrentMachineProperty); }
			set { SetValue(CurrentMachineProperty, value); }
		}
		public static readonly DependencyProperty CurrentMachineProperty =
			DependencyProperty.Register("CurrentMachine", typeof(OeeMachineVm), typeof(OeeVm), new PropertyMetadata(null));

		#region Tools
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowUnreported
		/// </summary>
		public bool ShowUnreported
		{
			get { return (bool)GetValue(ShowUnreportedProperty); }
			set { SetValue(ShowUnreportedProperty, value); }
		}
		public static readonly DependencyProperty ShowUnreportedProperty =
			DependencyProperty.Register("ShowUnreported", typeof(bool), typeof(OeeVm),
			new PropertyMetadata(true, (d, e) =>
			{
				var vm = (OeeVm)d;
				var val = (bool)e.NewValue;
				if (vm.CurrentMachine != null)
				{
					foreach (var time in vm.CurrentMachine.Timeline)
					{
						time.ShowUnreported(val);
					}
				}
			}));

		/// <summary>
		/// Gets or sets a bindable value that indicates MonthStart
		/// </summary>
		public int MonthStart
		{
			get { return (int)GetValue(MonthStartProperty); }
			set { SetValue(MonthStartProperty, value); }
		}
		public static readonly DependencyProperty MonthStartProperty =
			DependencyProperty.Register("MonthStart", typeof(int), typeof(OeeVm), new PropertyMetadata(1, (d, e) => { }, (d, v) =>
				{
					if ((int)v > 30) return 30;
					if ((int)v < 1) return 1;
					return v;
				}));
		/// <summary>
		/// Gets or sets a bindable value that indicates DayStart
		/// </summary>
		public TimeSpan DayStart
		{
			get { return (TimeSpan)GetValue(DayStartProperty); }
			set { SetValue(DayStartProperty, value); }
		}
		public static readonly DependencyProperty DayStartProperty =
			DependencyProperty.Register("DayStart", typeof(TimeSpan), typeof(OeeVm), new PropertyMetadata(TimeSpan.Zero));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsMonthly
		/// </summary>
		public bool IsMonthly
		{
			get { return (bool)GetValue(IsMonthlyProperty); }
			set { SetValue(IsMonthlyProperty, value); }
		}
		public static readonly DependencyProperty IsMonthlyProperty =
			DependencyProperty.Register("IsMonthly", typeof(bool), typeof(OeeVm),
			new PropertyMetadata(true, (d, e) =>
			{
				var vm = (OeeVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{
					d.SetValue(IsWeeklyProperty, false);
					d.SetValue(IsDailyProperty, false);
					vm.Reload();
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsWeekly
		/// </summary>
		public bool IsWeekly
		{
			get { return (bool)GetValue(IsWeeklyProperty); }
			set { SetValue(IsWeeklyProperty, value); }
		}
		public static readonly DependencyProperty IsWeeklyProperty =
			DependencyProperty.Register("IsWeekly", typeof(bool), typeof(OeeVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (OeeVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{
					d.SetValue(IsMonthlyProperty, false);
					d.SetValue(IsDailyProperty, false);
					vm.Reload();
				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsDaily
		/// </summary>
		public bool IsDaily
		{
			get { return (bool)GetValue(IsDailyProperty); }
			set { SetValue(IsDailyProperty, value); }
		}
		public static readonly DependencyProperty IsDailyProperty =
			DependencyProperty.Register("IsDaily", typeof(bool), typeof(OeeVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (OeeVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{
					d.SetValue(IsMonthlyProperty, false);
					d.SetValue(IsWeeklyProperty, false);
					vm.Reload();
				}
			}));

		#endregion
	}
}
