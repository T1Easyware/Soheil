using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP
{
	public static class ShiftManager
	{
		public static DateTime GetStartOfShift(DateTime dt)
		{
			if(dt.Hour<8) return dt.Date.AddHours(-7);
			if (dt.Hour >= 17) return dt.Date.AddHours(17);
			else return dt.Date.AddDays(7);
		}
	}
}
