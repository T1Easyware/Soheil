using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class DurationCell : Cell
	{
		public virtual void Save(int seconds, bool involveCheckbox = true) { }

		public int Id { get; set; }
		#region DurationText
		public string DurationText
		{
			get { return (string)GetValue(DurationTextProperty); }
			set { SetValue(DurationTextProperty, value); _firstLoad = false; }
		}
		private bool _firstLoad = true;
		private int _seconds;
		public int Seconds { get { return _seconds; } }
		public static readonly DependencyProperty DurationTextProperty =
			DependencyProperty.Register("DurationText", typeof(string), typeof(DurationCell),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (DurationCell)d;
				if (vm._firstLoad) { vm._firstLoad = false; return; }
				vm.Save(vm._seconds);
			},
			(d, v) =>
			{
				var vm = (DurationCell)d;
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
				//in seconds
				if (int.TryParse(str, out val))
				{
					vm._seconds = val;
					if (val == 0) return "";
					int year = val / 3600;
					val %= 3600;
					return string.Format("{0:D2}:{1:D2}:{2:D2}", year, val / 60, val % 60);
				}
				//not recognized
				vm._seconds = 0;
				return "";
			}));
		#endregion

		//Row Dependency Property
		public Rework Row
		{
			get { return (Rework)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public static readonly DependencyProperty RowProperty =
			DependencyProperty.Register("Row", typeof(Rework), typeof(DurationCell), new UIPropertyMetadata(null));
		//IsSaved Dependency Property
		public bool IsSaved
		{
			get { return (bool)GetValue(IsSavedProperty); }
			set { SetValue(IsSavedProperty, value); }
		}
		public static readonly DependencyProperty IsSavedProperty =
			DependencyProperty.Register("IsSaved", typeof(bool), typeof(DurationCell), new PropertyMetadata(true));

	}
}
