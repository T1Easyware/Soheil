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
		public int Id { get; protected set; }
		public bool IsShift { get; protected set; }
		public ICollection<WorkTimeRangeVm> Children { get; protected set; }

		/// <summary>
		/// Creates multiple instances of <see cref="PPItemWorkTime"/> for given shift and its breaks
		/// </summary>
		/// <param name="item"></param>
		/// <returns>
		/// a collection of vms for this shift and its breaks
		/// <para>returns empty if current shift is not open</para></returns>
		public static IEnumerable<WorkTimeRangeVm> CreateAuto(Soheil.Core.PP.PPItemWorkTime item)
		{
			List<WorkTimeRangeVm> list = new List<WorkTimeRangeVm>();
			
			if (item.Model.IsOpen)
			{
				var shiftVm = new WorkTimeRangeVm(item.Model, item.DayStart);
				list.Add(shiftVm);
				foreach (var wbreak in item.Model.WorkBreaks)
				{
					var breakVm = new WorkTimeRangeVm(wbreak, item.DayStart);
					shiftVm.Children.Add(breakVm);
					list.Add(breakVm);
				}
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
			Id = shift.Id;
			Children = new List<WorkTimeRangeVm>();
			IsShift = true;
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
			Id = wbreak.Id;
			IsShift = false;
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
	}
}
