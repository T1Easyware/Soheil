using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Index
{
	public struct OeeType
	{
		public enum OeeInterval { Monthly, Weekly, Daily }
		public OeeInterval Interval;
		public int MonthStart;
		public TimeSpan DayStart;
	}
}
