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
		public static readonly Color WorkBreak = Color.FromArgb(50, 200, 40, 40);
		public static class Day
		{
			public readonly static Color Open = Color.FromArgb(50, 255, 255, 255);
			public readonly static Color HalfClosed = Color.FromArgb(50, 200, 200, 127);
			public readonly static Color Closed = Color.FromArgb(50, 255, 127, 127);
			public readonly static Color SpecialDay1 = Color.FromArgb(50, 255, 127, 255);
			public readonly static Color SpecialDay2 = Color.FromArgb(50, 127, 150, 255);
			public readonly static Color SpecialDay3 = Color.FromArgb(50, 127, 255, 150);
			public readonly static Color Reserve1 = Color.FromArgb(50, 255, 192, 192);
			public readonly static Color Reserve2 = Color.FromArgb(50, 192, 255, 192);
			public readonly static Color Reserve3 = Color.FromArgb(50, 192, 192, 255);
		}
		public static class Shift
		{
			public static readonly Color Day = Color.FromArgb(50, 255, 255, 255);
			public static readonly Color Evening = Color.FromArgb(60, 0, 128, 255);
			public static readonly Color Night = Color.FromArgb(50, 0, 0, 0);
			public static readonly Color Reserve1 = Color.FromArgb(50, 255, 0, 0);
			public static readonly Color Reserve2 = Color.FromArgb(50, 255, 0, 255);
			public static readonly Color Reserve3 = Color.FromArgb(50, 128, 255, 0);
		}
	}
}
