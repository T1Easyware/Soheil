using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.PP
{
	class ActionData
	{
		public ActionData(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}
		internal DateTime Start { get; private set; }
		internal DateTime End { get; private set; }

		/// <summary>
		/// Specifies whether the PPItemBase fits (or partially fits) in the range specifed by this ActionData
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public bool IsInRange(PPItemBase item)
		{
			return !(item.End < Start || item.Start > End);
		}
		public bool IsValidRange()
		{
			return !(Start == DateTime.MinValue 
				|| Start == DateTime.MaxValue 
				|| End == DateTime.MinValue 
				|| End == DateTime.MaxValue);
		}
	}
}
