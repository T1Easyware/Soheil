using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Reports
{
    public class OperatorBarProvider
    {
        private readonly Color _cyan = Color.FromRgb(69, 190, 168);
        private readonly Color _blue = Color.FromRgb(75, 153, 202);
        private readonly Color _purple = Color.FromRgb(169, 104, 219);
        private readonly Color _crimson = Color.FromRgb(235, 107, 120);
        private readonly Color _yellow = Color.FromRgb(236, 226, 108);
        private readonly Color _night = Color.FromRgb(203, 209, 208);

        private readonly Color _cyanLight = Color.FromRgb(128, 221, 204);
        private readonly Color _blueLight = Color.FromRgb(139, 182, 209);
        private readonly Color _purpleLight = Color.FromRgb(199, 169, 221);
        private readonly Color _crimsonLight = Color.FromRgb(237, 158, 166);
        private readonly Color _yellowLight = Color.FromRgb(240, 235, 177);
        private readonly Color _nightLight = Color.FromRgb(236, 240, 239);

        public DateTimeIntervals DateTimeIntervals { get; set; }
		public Dispatcher Dispatcher { get; set; }
        public double Scale { get; set; }
		public int MaxValue { get; set; }
		public int MaxScale { get; set; }
		public double ScaleHeight { get; set; }
		public int StepScale { get; set; }
        public OperatorBarInfo BarInfo { get; set; }
        public OperatorReportDataService DataService { get; set; }

        private readonly Point _startColorPoint = new Point(0,0.5);
        private readonly Point _endColorPoint = new Point(1,0.5);

        public OperatorBarProvider(DateTimeIntervals dateTimeIntervals, OperatorReportDataService dataService, OperatorBarInfo barInfo)
        {
            BarInfo = barInfo;
            DateTimeIntervals = dateTimeIntervals;
            DataService = dataService;

			//double max = dataService.GetMax(dateTimeIntervals, barInfo);
			//MaxValue = Convert.ToInt32(Math.Ceiling(max));
			//MaxScale = GetScale(MaxValue);
			//scale = windowHeight/maxScale
        }
		static int[] _scales = new int[]
		{
			1,2,5,10,20,50,80,100,150,200,400,500,750,1000,1250,1500,2000,2500,3000,4000,5000,6000,7500,8000,10000
		};
		public int GetMaxScale()
		{
			/*int counter = 0;
			var max = MaxValue;
			while (max >= 1)
			{
				max = max / 10;
				counter++;
			}
			int baseScale = Convert.ToInt32(Math.Floor(max * 10d));
			MaxScale = (int)(baseScale * Math.Pow(10, counter - 1));
			var baseStep = (int)(Math.Pow(10, counter - 2));

			if (baseScale > 7)
			{
				StepScale = 20 * baseStep;
			}
			else if (baseScale > 5)
			{
				StepScale = 15 * baseStep;
			}
			else if (baseScale > 3)
			{
				StepScale = 10 * baseStep;
			}
			else if (baseScale > 1)
			{
				StepScale = 5 * baseStep;
			}
			else if (baseScale > 0)
			{
				StepScale = 2 * baseStep;
			}
			else
			{
				StepScale = 2;
				MaxScale = 10;
			}

			while (MaxScale < MaxValue)
			{
				MaxScale += StepScale;
			}
			MaxScale += StepScale;

			Scale = 1 / MaxScale;
			ScaleHeight = StepScale * Scale;

			*/

			int divisionsBy10 = 0;
			int max = MaxValue;
			if (BarInfo.CurrentType != OEType.CountBased)
				max /= 3600;
			while(_scales.Last() <= max)
			{
				max /= 10;
				divisionsBy10++;
			}
			var scale = _scales.FirstOrDefault(x => x >= max);
			if (BarInfo.CurrentType != OEType.CountBased)
				scale *= 3600;
			return (int)Math.Pow(10, divisionsBy10) * scale;
		}
		public IList<OperatorBarInfo> FetchAll()
		{
			Thread.Sleep(100);
			var bars = new List<OperatorBarInfo>();
			IList<Record> records = DataService.GetAll(DateTimeIntervals, BarInfo);
			if (!records.Any()) return bars;

			if (BarInfo.CurrentType == OEType.CountBased)
				MaxValue = records.Max(x => (int)x.Data[4]);
			else 
				MaxValue = (int)records.Max(x => (float)x.Data[0]);
			foreach (var record in records)
			{
				var currentInfo = new OperatorBarInfo
				{
					Id = record.Id,
					Text = record.Header,
					StartDate = record.StartDate,
					EndDate = record.EndDate,
					Level = BarInfo.Level,
					Data = record.Data,
					IsMenuItem = false,
				};
				bars.Add(currentInfo);
			}
			return bars;
		}

        private ObservableCollection<OperatorBarInfo> GetMenuItems(OperatorBarInfo currentInfo)
        {
            var items = new ObservableCollection<OperatorBarInfo>();
            return items;
        }

        private LinearGradientBrush GetColor(int levelIncrement = 0)
        {
            return new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
        }

    }
}
