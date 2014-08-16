using System;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Soheil.Common
{
	public static class CommonExtensions
	{
		#region Persian DateTime
		private static readonly PersianCalendar _persianCalendar = new PersianCalendar();
		public static PersianCalendar PersianCalendar { get { return _persianCalendar; } }
		public static string ToPersianDateTimeString(this DateTime dateTime)
		{
			return _persianCalendar.GetYear(dateTime).ToString("0000") + "/"
				+ _persianCalendar.GetMonth(dateTime).ToString("00") + "/"
				+ _persianCalendar.GetDayOfMonth(dateTime).ToString("00") + "  "
				+ dateTime.ToShortTimeString();
		}
		public static string ToPersianDateString(this DateTime dateTime)
		{
			return _persianCalendar.GetYear(dateTime).ToString("0000") + "/"
				+ _persianCalendar.GetMonth(dateTime).ToString("00") + "/"
				+ _persianCalendar.GetDayOfMonth(dateTime).ToString("00");
		}
		public static string ToPersianCompactDateString(this DateTime dateTime)
		{
			return _persianCalendar.GetDayOfMonth(dateTime).ToString("00") + " " + dateTime.GetPersianMonth().ToString();
		}
		public static string ToPersianCompactDateTimeString(this DateTime dateTime)
		{
			return string.Format("{0} {1} - {2}:{3}:{4}", 
				_persianCalendar.GetDayOfMonth(dateTime), 
				dateTime.GetPersianMonth(), 
				dateTime.Hour, dateTime.Minute, dateTime.Second);
		}
		public static DateTime ToPersianDate(this string dtString)
		{
			return DateTime.Now;//???
		}
		public static int GetPersianYear(this DateTime dateTime)
		{
			return _persianCalendar.GetYear(dateTime);
		}
		public static PersianMonth GetPersianMonth(this DateTime dateTime)
		{
			return (PersianMonth)_persianCalendar.GetMonth(dateTime);
		}
		public static PersianShortMonth GetPersianShortMonth(this DateTime dateTime)
		{
			return (PersianShortMonth)_persianCalendar.GetMonth(dateTime);
		}
		/// <summary>
		/// returns one-biased index
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static int GetPersianDayOfMonth(this DateTime dateTime)
		{
			return _persianCalendar.GetDayOfMonth(dateTime);
		}
		/// <summary>
		/// PersianDayOfWeek Enum is zero-biased
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static PersianDayOfWeek GetPersianDayOfWeek(this DateTime dateTime)
		{
			return (PersianDayOfWeek)(((int)dateTime.DayOfWeek + 1) % 7);
		}
		/// <summary>
		/// returns one-biased index
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static int GetPersianDayOfYear(this DateTime dateTime)
		{
			return _persianCalendar.GetDayOfYear(dateTime);
		}
		public static int GetPersianYearDays(this DateTime dateTime)
		{
			return (_persianCalendar.GetYear(dateTime) % 4 == 3) ? 366 : 365;
		}
		public static int GetPersianMonthDays(this DateTime dateTime)
		{
			return _persianCalendar.GetDaysInMonth(_persianCalendar.GetYear(dateTime), _persianCalendar.GetMonth(dateTime));
		}
		public static DateTime GetNorooz(this DateTime dateTime)
		{
			return _persianCalendar.ToDateTime(_persianCalendar.GetYear(dateTime),1,1,0,0,0,0);
		}
		/// <summary>
		/// zero biased
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static int GetPersianWeekOfYear(this DateTime dateTime)
		{
			int x = (int)dateTime.GetNorooz().GetPersianDayOfWeek();
			if (x > 0) x = 7 - x;
			return (dateTime.GetPersianDayOfYear() - x) / 7;
		} 
		public static DateTime TruncateMilliseconds(this DateTime dateTime)
		{
			return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
		}
		#endregion

        #region Color, Point
        public static bool IsDark(this System.Windows.Media.Color color)
		{
			return (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) < 128;
		}
		public static Point SubtractPoint(this Point first, Point second) { return new Point(first.X - second.X, first.Y - second.Y); }
		#endregion

		#region Collection
		public static void RemoveWhere<T>(this Collection<T> collection, Func<T, bool> where)
		{
			var list = collection.Where(where).ToList();
			foreach (var item in list)
				collection.Remove(item);
		}
		public static void RemoveWhere<T>(this IList<T> collection, Func<T, bool> where)
		{
			var list = collection.Where(where).ToList();
			foreach (var item in list)
				collection.Remove(item);
		}

		/// <summary>
		/// Returns true if no elements in collection, else return true if all specified values are equal
		/// </summary>
		/// <typeparam name="T">Type of collection items</typeparam>
		/// <typeparam name="TKey">Type of item's selector value</typeparam>
		/// <param name="collection">The collection to search</param>
		/// <param name="selector">which value to select</param>
		/// <returns></returns>
		public static bool AreAllEqual<T, TKey>(this ICollection<T> collection, Func<T, TKey> selector)
		{
			if(collection.Count == null) return true;
			TKey val = selector(collection.First());
			foreach (var item in collection)
			{
				if (!selector(item).Equals(val)) return false;
			}
			return true;
		}
		#endregion

		#region DataContext & EntityObject
		static object _disconnectedItem;
		static object DisconnectedItem
		{
			get
			{
				if (_disconnectedItem == null) _disconnectedItem = typeof(System.Windows.Data.BindingExpressionBase)
				   .GetField("DisconnectedItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
				   .GetValue(null);
				return _disconnectedItem;
			}
		}
		public static T GetDataContext<T>(this object sender)
		{
			var dc = ((System.Windows.FrameworkElement)sender).DataContext;
			if (dc == DisconnectedItem) return default(T);
			if (dc is T)
				return (T)dc;
			else return default(T);
		}
		#endregion

		#region Visual Tree
		public static System.Windows.FrameworkElement FindChild(this System.Windows.FrameworkElement root, Type childType)
		{
			System.Windows.FrameworkElement target = null;
			int c = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < c; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i) as System.Windows.FrameworkElement;
				if (child == null) continue;
				if (child.GetType() == childType)
					return child;
				target = FindChild(child, childType);
			}
			return target;
		}
		public static System.Windows.FrameworkElement FindChild(this System.Windows.FrameworkElement root, string childName)
		{
			System.Windows.FrameworkElement target = null;
			int c = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < c; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i) as System.Windows.FrameworkElement;
				if (child == null) continue;
				if (child.Name == childName)
					return child;
				target = FindChild(child, childName);
				if (target != null) break;
			}
			return target;
		}
		public static System.Windows.FrameworkElement FindParent(this System.Windows.FrameworkElement root, Type parentType)
		{
			System.Windows.FrameworkElement target = root;
			while (target != null)
			{
				target = System.Windows.Media.VisualTreeHelper.GetParent(target) as System.Windows.FrameworkElement;
				if (target == null) return null;
				if (target.GetType() == parentType) return target;
			}
			return null;
		}
		public static System.Windows.FrameworkElement FindParent(this System.Windows.FrameworkElement root, string parentName)
		{
			System.Windows.FrameworkElement target = root;
			while (target != null)
			{
				target = System.Windows.Media.VisualTreeHelper.GetParent(target) as System.Windows.FrameworkElement;
				if (target == null) return null;
				if (target.Name == parentName) return target;
			}
			return null;
		}
		public static ToolBar FindDocumentMenu(this System.Windows.FrameworkElement root)
		{
			ToolBar target = null;
			int c = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < c; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i) as System.Windows.FrameworkElement;
				if (child == null) continue;

				var menu = child as ToolBar;
				if (menu != null)
					if (menu.Items.Count == 10)
						return menu;

				target = FindDocumentMenu(child);
				if (target != null) return target;
			}
			return target;
		}
		#endregion
	}
}
