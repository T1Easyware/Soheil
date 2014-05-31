using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class DefectionStoppageCollectionBase : DependencyObject
	{
		public ProcessReportCellVm Parent { get; set; }
		public virtual void UpdateParentSumOfCount(int newValue) { }
		public virtual void Reset()
		{
			SumOfCountEquivalent = 0;
			SumOfLostCount = 0;
			SumOfLostTime = 0;
			SumOfTimeEquivalent = 0;
		}

		//SumOfLostCount Dependency Property
		public int SumOfLostCount
		{
			get { return (int)GetValue(SumOfLostCountProperty); }
			set { SetValue(SumOfLostCountProperty, value); }
		}
		public static readonly DependencyProperty SumOfLostCountProperty =
			DependencyProperty.Register("SumOfLostCount", typeof(int), typeof(DefectionStoppageCollectionBase), new UIPropertyMetadata(0));
		//SumOfLostTime Dependency Property
		public int SumOfLostTime
		{
			get { return (int)GetValue(SumOfLostTimeProperty); }
			set { SetValue(SumOfLostTimeProperty, value); }
		}
		public static readonly DependencyProperty SumOfLostTimeProperty =
			DependencyProperty.Register("SumOfLostTime", typeof(int), typeof(DefectionStoppageCollectionBase), new UIPropertyMetadata(0));
		//SumOfCountEquivalent Dependency Property
		public int SumOfCountEquivalent
		{
			get { return (int)GetValue(SumOfCountEquivalentProperty); }
			set { SetValue(SumOfCountEquivalentProperty, value); }
		}
		public static readonly DependencyProperty SumOfCountEquivalentProperty =
			DependencyProperty.Register("SumOfCountEquivalent", typeof(int), typeof(DefectionStoppageCollectionBase),
			new UIPropertyMetadata(0, (d, e) => ((DefectionStoppageCollectionBase)d).UpdateParentSumOfCount((int)e.NewValue)));
		//SumOfTimeEquivalent Dependency Property
		public int SumOfTimeEquivalent
		{
			get { return (int)GetValue(SumOfTimeEquivalentProperty); }
			set { SetValue(SumOfTimeEquivalentProperty, value); }
		}
		public static readonly DependencyProperty SumOfTimeEquivalentProperty =
			DependencyProperty.Register("SumOfTimeEquivalent", typeof(int), typeof(DefectionStoppageCollectionBase), new UIPropertyMetadata(0));

		//AddCommand Dependency Property
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(DefectionStoppageCollectionBase), new UIPropertyMetadata(null));
	}
}
