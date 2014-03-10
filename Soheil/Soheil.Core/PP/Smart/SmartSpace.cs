using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.PP.Smart
{
	public class SmartSpace
	{
		public DateTime Start { get; protected set; }
		public DateTime End { get; protected set; }

		public int PrevPrId;
		public int NextPrId;

		public override string ToString()
		{
			int startSecs = (int)(Start.Ticks / TimeSpan.TicksPerSecond);
			int durationSecs = (int)End.Subtract(Start).TotalSeconds;
			return string.Format("{0} {1} {2} {3}",
				startSecs, durationSecs, PrevPrId, NextPrId
			);
		}
		public static SmartSpace operator +(SmartSpace a, SmartSpace b)
		{
			return a;
		}
	}
}
