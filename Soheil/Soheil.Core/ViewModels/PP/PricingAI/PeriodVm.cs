using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.PP.PricingAI;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class PeriodVm : DependencyObject
	{
		public PeriodVm(Period period)
		{
			Name = period.Name;
			Duration = period.Duration;
			TotalBudget = (period.TotalBudget == int.MaxValue || period.TotalBudget < 0) ? null : (int?)period.TotalBudget;
			TotalCapacity = (period.TotalCapacity == int.MaxValue || period.TotalCapacity < 0) ? null : (int?)period.TotalCapacity;
			StartDate = period.StartDate;
			EndDate = period.EndDate;
			Index = period.Index;
		}

		public int Index { get; set; }

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Duration
		/// </summary>
		public int Duration
		{
			get { return (int)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(int), typeof(PeriodVm), new PropertyMetadata(90));
		/// <summary>
		/// Gets or sets a bindable value that indicates TotalCapacity
		/// </summary>
		public int? TotalCapacity
		{
			get { return (int?)GetValue(TotalCapacityProperty); }
			set { SetValue(TotalCapacityProperty, value); }
		}
		public static readonly DependencyProperty TotalCapacityProperty =
			DependencyProperty.Register("TotalCapacity", typeof(int?), typeof(PeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates TotalBudget
		/// </summary>
		public int? TotalBudget
		{
			get { return (int?)GetValue(TotalBudgetProperty); }
			set { SetValue(TotalBudgetProperty, value); }
		}
		public static readonly DependencyProperty TotalBudgetProperty =
			DependencyProperty.Register("TotalBudget", typeof(int?), typeof(PeriodVm), new PropertyMetadata(null));


		/// <summary>
		/// Gets or sets a bindable value that indicates StartDate
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(PeriodVm), new PropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets a bindable value that indicates EndDate
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(PeriodVm), new PropertyMetadata(DateTime.Now));

	}
}
