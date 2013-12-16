using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public class MonthSlideItemVm: DependencyObject
    {
		public MonthSlideItemVm(DateTime dt, Soheil.Core.PP.DayCollection daysOfMonth, ref int daysFromStartOfYear, PPTableVm parent)
		{
			_daysOfMonth = daysOfMonth;
			_parent = parent;
			Data = dt;
			NumOfDays = dt.GetPersianMonthDays();
			DaysFromStartOfYear = daysFromStartOfYear;
			daysFromStartOfYear += NumOfDays;
			ColumnIndex = (int)dt.GetPersianMonth() - 1;
			Text = string.Format("{0}/{1}", dt.GetPersianYear(), dt.GetPersianMonth());
		}
		Soheil.Core.PP.DayCollection _daysOfMonth;
		PPTableVm _parent;

		//Data Dependency Property
		public DateTime Data
		{
			get { return (DateTime)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(DateTime), typeof(MonthSlideItemVm), new UIPropertyMetadata(default(DateTime)));
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(MonthSlideItemVm), new UIPropertyMetadata(null));
		//NumOfDays Dependency Property
		public int NumOfDays
		{
			get { return (int)GetValue(NumOfDaysProperty); }
			set { SetValue(NumOfDaysProperty, value); }
		}
		public static readonly DependencyProperty NumOfDaysProperty =
			DependencyProperty.Register("NumOfDays", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(31));
		//ColumnIndex Dependency Property (in months, from the start of the year)
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(0));
		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(MonthSlideItemVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				if ((bool)e.NewValue)
				{
					var vm = (MonthSlideItemVm)d;
					vm._daysOfMonth.Reload(vm.Data);
					foreach (var month in vm._parent.Months.Where(x => x != vm))
					{
						month.IsSelected = false;
					}
					vm._parent.SelectedMonth = vm;
				}
			}, (d, v) =>
			{
				if ((bool)v) return true;
				var vm = (MonthSlideItemVm)d;
				if (vm._parent.Months.Where(x => x != vm).Any(x => x.IsSelected)) return false;
				return true;
			}));
		//DaysFromStartOfYear Dependency Property
		public int DaysFromStartOfYear
		{
			get { return (int)GetValue(DaysFromStartOfYearProperty); }
			set { SetValue(DaysFromStartOfYearProperty, value); }
		}
		public static readonly DependencyProperty DaysFromStartOfYearProperty =
			DependencyProperty.Register("DaysFromStartOfYear", typeof(int), typeof(MonthSlideItemVm), new UIPropertyMetadata(0));
    }
}
