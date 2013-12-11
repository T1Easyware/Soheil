using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Soheil.Common
{
	public static class DefaultColors
	{
		public static class Day
		{
			public readonly static Color Open = Color.FromArgb(31, 0, 0, 0);
			public readonly static Color HalfClosed = Color.FromArgb(31, 200, 200, 127);
			public readonly static Color Closed = Color.FromArgb(31, 255, 127, 127);
			public readonly static Color SpecialDay1 = Color.FromArgb(31, 255, 127, 255);
			public readonly static Color SpecialDay2 = Color.FromArgb(31, 127, 150, 255);
			public readonly static Color SpecialDay3 = Color.FromArgb(31, 127, 255, 150);
			public readonly static Color Reserve1 = Color.FromArgb(31, 255, 192, 192);
			public readonly static Color Reserve2 = Color.FromArgb(31, 192, 255, 192);
			public readonly static Color Reserve3 = Color.FromArgb(31, 192, 192, 255);
		}
		public static class Shift
		{
			public static readonly Color Day = Color.FromArgb(31, 255, 255, 255);
			public static readonly Color Evening = Color.FromArgb(45, 0, 128, 255);
			public static readonly Color Night = Color.FromArgb(31, 0, 0, 0);
			public static readonly Color Reserve1 = Color.FromArgb(31, 255, 0, 0);
			public static readonly Color Reserve2 = Color.FromArgb(31, 255, 0, 255);
			public static readonly Color Reserve3 = Color.FromArgb(31, 128, 255, 0);
		}
	}
}
