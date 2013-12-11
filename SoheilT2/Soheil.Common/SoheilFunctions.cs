using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Common
{
	public static class SoheilFunctions
	{
		public static int RoundFiveMinutes(int seconds)
		{
			return 300 * ((seconds + 150) / 300);
		}
		public static string GetWorkShiftTime(int seconds)
		{
			return string.Format("{0:D2} : {1:D2}{2}", (seconds / 3600) % 24, (seconds / 60) % 60, seconds > 86400 ? " Next Day" : "");
		}
	}
}
