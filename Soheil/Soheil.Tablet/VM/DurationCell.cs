using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Tablet.VM
{
	public class TimeSpanBox : DependencyObject
	{
		public event Action<int> Updated;
		private bool _firstLoad = true;
		public int Seconds { get { return _seconds; } }
		private int _seconds;

		public DateTime DateTime { get; private set; }

		public TimeSpanBox(DateTime dt)
		{
			Update(dt);
		}
		public TimeSpanBox(int durationSeconds)
		{
			DateTime = DateTime.Now;
			Update(durationSeconds);
		}
		public void Update(DateTime dt)
		{
			DateTime = dt.AddMilliseconds(-dt.TimeOfDay.Milliseconds);
			Text = DateTime.TimeOfDay.ToString(@"hh\:mm\:ss");
		}
		public void Update(int durationSeconds)
		{
			DateTime = DateTime.Date.AddSeconds(durationSeconds);
			Text = TimeSpan.FromSeconds(durationSeconds).ToString(@"hh\:mm\:ss");
		}

		#region Text
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			private set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(TimeSpanBox),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (TimeSpanBox)d;
				if (vm._firstLoad) { vm._firstLoad = false; return; }
				if (vm.Updated != null) vm.Updated(vm._seconds);
			},
			(d, v) =>
			{
				var vm = (TimeSpanBox)d;
				int val;
				var str = (string)v;
				//in minutes or hours
				if (str.Contains(':'))
				{
					var parts = str.Split(':');
					int h = 0, m = 0, s = 0;
					//in minutes and hours
					if (parts.Length == 3)
					{
						if (!int.TryParse(parts[0], out h)) h = 0;
						if (!int.TryParse(parts[1], out m)) m = 0;
						if (!int.TryParse(parts[2], out s)) s = 0;
					}
					//in minutes
					if (parts.Length == 2)
					{
						if (!int.TryParse(parts[0], out m)) m = 0;
						if (!int.TryParse(parts[1], out s)) s = 0;
					}
					val = h * 3600 + m * 60 + s;
					vm._seconds = val;
					if (val == 0) return "";
					int year = val / 3600;
					val %= 3600;
					return string.Format("{0:D2}:{1:D2}:{2:D2}", year, val / 60, val % 60);
				}
				//in military format (003000)
				if (str.Length == 6)
				{
					int h = 0, m = 0, s = 0;
					if (!int.TryParse(str.Substring(0, 2), out h)) h = 0;
					if (!int.TryParse(str.Substring(2, 2), out m)) m = 0;
					if (!int.TryParse(str.Substring(4, 2), out s)) s = 0;
					val = h * 3600 + m * 60 + s;
					vm._seconds = val;
					return string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
				}
				//not recognized
				//vm._seconds = 0;
				return v;
			}));
		#endregion

	}
}
