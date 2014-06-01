using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Soheil.Common;
using Soheil.Core.DataServices;
using Soheil.Core.ViewModels.Reports;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Reports
{
    public class OperatorBarProvider : IItemsProvider<OperatorBarVm>
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

        private readonly int _count;
        public DateTimeIntervals DateTimeIntervals { get; set; }
		public Dispatcher Dispatcher { get; set; }
        public double Scale { get; set; }
        public double ScaleHeight { get; set; }
        public int MaxValue { get; set; }
        public int MaxScale { get; set; }
        public int StepScale { get; set; }
        public OperatorBarInfo BarInfo { get; set; }
        public OperatorReportDataService DataService { get; set; }

        private readonly Point _startColorPoint = new Point(0,0.5);
        private readonly Point _endColorPoint = new Point(1,0.5);

        public OperatorBarProvider(int count, double pageHeight, DateTimeIntervals dateTimeIntervals, OperatorReportDataService dataService, OperatorBarInfo barInfo)
        {
            _count = count;
            BarInfo = barInfo;
            DateTimeIntervals = dateTimeIntervals;
            DataService = dataService;

            double max = dataService.GetMax(dateTimeIntervals,barInfo,count);
            MaxValue = Convert.ToInt32(Math.Ceiling(max));

            int counter = 0;
            while (max >= 1)
            {
                max = max / 10;
                counter++;
            }
            int baseScale = Convert.ToInt32(Math.Floor(max * 10));
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

            Scale = pageHeight / MaxScale;
            ScaleHeight = StepScale * Scale;
        }

        public int FetchCount()
        {
            Thread.Sleep(1000);
            return _count;
        }

        public IList<OperatorBarVm> FetchRange(int startOperator, int count)
        {
            if (Dispatcher != null) Thread.Sleep(100);
            var bars = new List<OperatorBarVm>();
            IList<Record> records = DataService.GetInRange(DateTimeIntervals, BarInfo, startOperator, startOperator + count);

            for (int i = startOperator; i < startOperator + count; i++)
            {
                var currentInfo = new OperatorBarInfo
                    {
                        Id = records[i - startOperator].Id,
                        Text = records[i - startOperator].Header,
                        StartDate = records[i - startOperator].StartDate,
                        EndDate = records[i - startOperator].EndDate,
                        Level = BarInfo.Level,
                        IsMenuItem = false
                    };
                var bar = new OperatorBarVm
                {
                    Info = currentInfo,
                    Value = records[i - startOperator].Value * Scale,
                    Value1 = records[i - startOperator].Value1 * Scale,
                    Value2 = records[i - startOperator].Value2 * Scale,
                    Value3 = records[i - startOperator].Value3 * Scale,
                    Tip = records[i - startOperator].Value.ToString(CultureInfo.InvariantCulture),
                    Tip1 = records[i - startOperator].Value1.ToString(CultureInfo.InvariantCulture),
                    Tip2 = records[i - startOperator].Value2.ToString(CultureInfo.InvariantCulture),
                    Tip3 = records[i - startOperator].Value3.ToString(CultureInfo.InvariantCulture),
                    Header = records[i - startOperator].Header,
                    MainColor = GetColor(),
                    MenuItems = GetMenuItems(currentInfo)
                };
                bars.Add(bar);
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
