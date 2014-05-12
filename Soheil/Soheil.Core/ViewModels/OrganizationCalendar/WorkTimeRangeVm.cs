using Soheil.Common;
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
		/// Creates multiple instances of <see cref="PPItemWorkTime"/> for given shift and its breaks
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IEnumerable<WorkTimeRangeVm> CreateAuto(Soheil.Core.PP.PPItemWorkTime item)
		{
			List<WorkTimeRangeVm> list = new List<WorkTimeRangeVm>();
			list.Add(new WorkTimeRangeVm(item.Model, item.DayStart));
			foreach (var wbreak in item.Model.WorkBreaks)
			{
				list.Add(new WorkTimeRangeVm(wbreak, item.DayStart));
			}
			return list;
		}
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for the given <see cref="Soheil.Model.WorkShift"/>
		/// </summary>
		/// <param name="shift"></param>
		protected WorkTimeRangeVm(WorkShift shift, DateTime offset)
		{
			Start = offset.AddSeconds(shift.StartSeconds);
			End = offset.AddSeconds(shift.EndSeconds);
			Color = shift.WorkShiftPrototype.Color;
			IsOpen = shift.IsOpen;
		}
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for the given <see cref="Soheil.Model.WorkBreak"/>
		/// </summary>
		/// <param name="wbreak"></param>
		protected WorkTimeRangeVm(WorkBreak wbreak, DateTime offset)
		{
			Start = offset.AddSeconds(wbreak.StartSeconds);
			End = offset.AddSeconds(wbreak.EndSeconds);
			Color = DefaultColors.WorkBreak;
			IsOpen = false;
		}


		/// <summary>
		/// Gets or sets a bindable value for start of this time range
		/// </summary>
		public DateTime Start
		{
			get { return (DateTime)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}
		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(DateTime), typeof(WorkTimeRangeVm), new UIPropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets a bindable value for end of this time range
		/// </summary>
		public DateTime End
		{
			get { return (DateTime)GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}
		public static readonly DependencyProperty EndProperty =
			DependencyProperty.Register("End", typeof(DateTime), typeof(WorkTimeRangeVm), new UIPropertyMetadata(DateTime.Now));

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
		/// <summary>
		/// Gets or sets a bindable value that indicates whether this range is open for business
		/// </summary>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(WorkTimeRangeVm), new UIPropertyMetadata(true));
	}
}
