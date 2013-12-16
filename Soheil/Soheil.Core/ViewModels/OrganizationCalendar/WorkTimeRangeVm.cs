using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class WorkTimeRangeVm : DependencyObject
	{
		public static WorkTimeRangeVm CreateAuto(object item)
		{
			if (item is WorkShift) return new WorkTimeRangeVm(item as WorkShift);
			if (item is WorkBreak) return new WorkTimeRangeVm(item as WorkBreak);
			return null;
		}
		public WorkTimeRangeVm(WorkShift shift)
		{
			StartSeconds = shift.StartSeconds;
			EndSeconds = shift.EndSeconds;
			Color = shift.WorkShiftPrototype.Color;
		}
		public WorkTimeRangeVm(WorkBreak wbreak)
		{
			StartSeconds = wbreak.StartSeconds;
			EndSeconds = wbreak.EndSeconds;
			Color = Color.FromArgb(75, 200, 50, 50);
		}

		//StartSeconds Dependency Property
		public int StartSeconds
		{
			get { return (int)GetValue(StartSecondsProperty); }
			set { SetValue(StartSecondsProperty, value); }
		}
		public static readonly DependencyProperty StartSecondsProperty =
			DependencyProperty.Register("StartSeconds", typeof(int), typeof(WorkTimeRangeVm), new UIPropertyMetadata(0));
		//EndSeconds Dependency Property
		public int EndSeconds
		{
			get { return (int)GetValue(EndSecondsProperty); }
			set { SetValue(EndSecondsProperty, value); }
		}
		public static readonly DependencyProperty EndSecondsProperty =
			DependencyProperty.Register("EndSeconds", typeof(int), typeof(WorkTimeRangeVm), new UIPropertyMetadata(0));
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(WorkTimeRangeVm), new UIPropertyMetadata(Colors.Transparent));
	}
}
