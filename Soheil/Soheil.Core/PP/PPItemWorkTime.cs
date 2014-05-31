using Soheil.Common;
using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.PP
{
	/// <summary>
	/// WorkShift
	/// </summary>
	public class PPItemWorkTime : PPItemBase
	{
		public DateTime DayStart { get; set; }
		public Model.WorkShift Model { get; set; }
		/// <summary>
		/// Creates an instance of WorkTimeRangeVm for the given <see cref="Soheil.Model.WorkShift"/>
		/// </summary>
		/// <param name="shift"></param>
		public PPItemWorkTime(WorkShift shift, DateTime dayStart)
		{
			DayStart = dayStart;
			Start = dayStart.AddSeconds(shift.StartSeconds);
			End = dayStart.AddSeconds(shift.EndSeconds);
			Id = shift.Id;
			Model = shift;
		}
	}
}
