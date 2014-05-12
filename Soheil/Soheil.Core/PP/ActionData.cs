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
		/// Specifies whether the item with the given range fits (or partially fits) in the range specifed by this ActionData
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public bool IsInRange(DateTime itemStart, DateTime itemEnd)
		{
			return !(itemEnd > Start || itemStart < End);
		}
	}
}
