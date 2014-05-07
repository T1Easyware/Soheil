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
	/// <summary>
	/// ViewModel for a <see cref="Soheil.Model.WorkShift"/> or a <see cref="Soheil.Model.WorkBreak"/> presented as a range of time 
	/// </summary>
	public class WorkTimeRangeVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for an object which is either a <see cref="Soheil.Model.WorkShift"/> or a <see cref="Soheil.Model.WorkBreak"/>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static WorkTimeRangeVm CreateAuto(object item)
		{
			if (item is WorkShift) return new WorkTimeRangeVm(item as WorkShift);
			if (item is WorkBreak) return new WorkTimeRangeVm(item as WorkBreak);
			return null;
		}
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for the given <see cref="Soheil.Model.WorkShift"/>
		/// </summary>
		/// <param name="shift"></param>
		public WorkTimeRangeVm(WorkShift shift)
		{
			StartSeconds = shift.StartSeconds;
			EndSeconds = shift.EndSeconds;
			Color = shift.WorkShiftPrototype.Color;
		}
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for the given <see cref="Soheil.Model.WorkBreak"/>
		/// </summary>
		/// <param name="wbreak"></param>
		public WorkTimeRangeVm(WorkBreak wbreak)
		{
			StartSeconds = wbreak.StartSeconds;
			EndSeconds = wbreak.EndSeconds;
			Color = Color.FromArgb(75, 200, 50, 50);
		}

		/// <summary>
		/// Gets or sets a bindable value for start of this time range
		/// <para>The number of seconds after 0:00AM</para>
		/// </summary>
		public int StartSeconds
		{
			get { return (int)GetValue(StartSecondsProperty); }
			set { SetValue(StartSecondsProperty, value); }
		}
		public static readonly DependencyProperty StartSecondsProperty =
			DependencyProperty.Register("StartSeconds", typeof(int), typeof(WorkTimeRangeVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value for end of this time range
		/// <para>The number of seconds after 0:00AM</para>
		/// </summary>
		public int EndSeconds
		{
			get { return (int)GetValue(EndSecondsProperty); }
			set { SetValue(EndSecondsProperty, value); }
		}
		public static readonly DependencyProperty EndSecondsProperty =
			DependencyProperty.Register("EndSeconds", typeof(int), typeof(WorkTimeRangeVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value for color of this range
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(WorkTimeRangeVm), new UIPropertyMetadata(Colors.Transparent));
	}
}
