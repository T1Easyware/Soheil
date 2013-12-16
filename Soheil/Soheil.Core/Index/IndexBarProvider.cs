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
using Soheil.Core.Reports;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.Virtualizing;

namespace Soheil.Core.Index
{
    public class IndexBarProvider : IItemsProvider<IndexBarVm>
    {
        private readonly Color _cyan = Color.FromRgb(69, 190, 168);
        private readonly Color _green = Color.FromRgb(138, 222, 123);
        private readonly Color _blue = Color.FromRgb(75, 153, 202);
        private readonly Color _purple = Color.FromRgb(169, 104, 219);
        private readonly Color _crimson = Color.FromRgb(235, 107, 120);
        private readonly Color _orange = Color.FromRgb(235, 147, 107);
        private readonly Color _yellow = Color.FromRgb(236, 226, 108);
        private readonly Color _night = Color.FromRgb(203, 209, 208);

        private readonly Color _cyanLight = Color.FromRgb(128, 221, 204);
        private readonly Color _greenLight = Color.FromRgb(178, 237, 168);
        private readonly Color _blueLight = Color.FromRgb(139, 182, 209);
        private readonly Color _purpleLight = Color.FromRgb(199, 169, 221);
        private readonly Color _crimsonLight = Color.FromRgb(237, 158, 166);
        private readonly Color _orangeLight = Color.FromRgb(238, 189, 167);
        private readonly Color _yellowLight = Color.FromRgb(240, 235, 177);
        private readonly Color _nightLight = Color.FromRgb(236, 240, 239);

        private readonly int _count;
        public IndexType IndexType { get; set; }
        public DateTimeIntervals DateTimeIntervals { get; set; }
		public Dispatcher Dispatcher { get; set; }
        public double Scale { get; set; }
        public int MaxScale { get; set; }
        public int StepScale { get; set; }
        public IndexBarInfo BarInfo { get; set; }
        public IndexDataService DataService { get; set; }

        private readonly Point _startColorPoint = new Point(0,0.5);
        private readonly Point _endColorPoint = new Point(1,0.5);

        public IndexBarProvider(int count, double pageHeight, IndexType indexType, DateTimeIntervals dateTimeIntervals, IndexDataService dataService, IndexBarInfo barInfo)
        {
            _count = count;
            BarInfo = barInfo;
            IndexType = indexType;
            DateTimeIntervals = dateTimeIntervals;
            DataService = dataService;
            //Records = DataService.GetAll(IndexType, DateTimeIntervals, count);
            //double max = Records.Max(item => item.Value);

            double max = 100; /* get it from a light query*/

            int counter = 0;
            while (max >= 1)
            {
                max = max / 10;
                counter++;
            }
            int baseScale = Convert.ToInt32(max * 10);
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

            Scale = pageHeight / (MaxScale + StepScale);
        }

        public int FetchCount()
        {
            Thread.Sleep(1000);
            return _count;
        }

        public IList<IndexBarVm> FetchRange(int startIndex, int count)
        {
            if (Dispatcher != null) Thread.Sleep(100);
            var bars = new List<IndexBarVm>();
            IList<Record> records = DataService.GetInRange(IndexType, DateTimeIntervals, BarInfo, startIndex, startIndex + count);

            for (int i = startIndex; i < startIndex + count; i++)
            {
                var currentInfo = new IndexBarInfo
                    {
                        StartDate = records[i - startIndex].StartDate,
                        EndDate = records[i - startIndex].EndDate,
                        Type = IndexType,
                        Level = BarInfo.Level,
                        IsMenuItem = false
                    };
                switch (BarInfo.Filter)
                {
                    case IndexFilter.None:
                        currentInfo.IntervalId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByProduct:
                        currentInfo.ProductId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByStation:
                        currentInfo.StationId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByActivity:
                        currentInfo.ActivityId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByOperator:
                        currentInfo.OperatorId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByMachine:
                        currentInfo.MachineId = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByCauseL1:
                        currentInfo.CauseL1Id = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByCauseL2:
                        currentInfo.CauseL2Id = records[i - startIndex].Id;
                        break;
                    case IndexFilter.ByCauseL3:
                        currentInfo.CauseL3Id = records[i - startIndex].Id;
                        break;
                }
                var bar = new IndexBarVm
                {
                    Info = currentInfo,
                    Value = records[i - startIndex].Value * Scale,
                    Tip = records[i - startIndex].Value.ToString(CultureInfo.InvariantCulture),
                    Header = records[i - startIndex].Header,
                    MainColor = GetColor(),
                    MenuItems = GetMenuItems(currentInfo)
                };
                bars.Add(bar);
            }
            return bars;
        }

        private ObservableCollection<IndexBarInfo> GetMenuItems(IndexBarInfo currentInfo)
        {
            var items = new ObservableCollection<IndexBarInfo>();
            switch (IndexType)
            {
                case IndexType.Performance:
                case IndexType.InternalPPM:
                case IndexType.RemainingCapacity:
                    if ((BarInfo.Filter & IndexFilter.ByProduct) != IndexFilter.ByProduct)
                        items.Add(GetButtonVm(IndexFilter.ByProduct, currentInfo));
                    if ((BarInfo.Filter & IndexFilter.ByStation) != IndexFilter.ByStation)
                        items.Add(GetButtonVm(IndexFilter.ByStation, currentInfo));
                    if ((BarInfo.Filter & IndexFilter.ByActivity) != IndexFilter.ByActivity)
                        items.Add(GetButtonVm(IndexFilter.ByActivity, currentInfo));
                    if ((BarInfo.Filter & IndexFilter.ByOperator) != IndexFilter.ByOperator)
                        items.Add(GetButtonVm(IndexFilter.ByOperator, currentInfo));
                    break;
            }
            return items;
        }

        private LinearGradientBrush GetColor(int levelIncrement = 0)
        {
            var level = BarInfo.Level + levelIncrement;
            switch (IndexType)
            {
                case IndexType.None:
                    break;
                case IndexType.OEE:
                    switch (level)
                    {
                        case 0:
                            return new LinearGradientBrush(_cyan, _cyanLight, _startColorPoint, _endColorPoint);
                        case 1:
                            return new LinearGradientBrush(_blue, _blueLight, _startColorPoint, _endColorPoint);
                        case 2:
                            return new LinearGradientBrush(_purple, _purpleLight, _startColorPoint, _endColorPoint);
                        case 3:
                            return new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
                        case 4:
                            return new LinearGradientBrush(_orange, _orangeLight, _startColorPoint, _endColorPoint);
                        case 5:
                            return new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
                    }
                    break;
                case IndexType.Performance:
                    if (level == 0)
                    {
                        return new LinearGradientBrush(_blue, _blueLight, _startColorPoint, _endColorPoint);
                    }
                    switch (BarInfo.Filter)
                    {
                        case IndexFilter.ByProduct:
                            return new LinearGradientBrush(_green, _greenLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByStation:
                            return new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByActivity:
                            return new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByOperator:
                            return new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
                    }
                    break;
                case IndexType.InternalPPM:
                    if (level == 0)
                    {
                        return new LinearGradientBrush(_purple, _purpleLight, _startColorPoint, _endColorPoint);
                    }
                    switch (BarInfo.Filter)
                    {
                        case IndexFilter.ByProduct:
                            return new LinearGradientBrush(_green, _greenLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByStation:
                            return new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByActivity:
                            return new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByOperator:
                            return new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
                    }
                    break;
                case IndexType.RemainingCapacity:
                    if (level == 0)
                    {
                        return new LinearGradientBrush(_orange, _orangeLight, _startColorPoint, _endColorPoint);
                    }
                    switch (BarInfo.Filter)
                    {
                        case IndexFilter.ByProduct:
                            return new LinearGradientBrush(_green, _greenLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByStation:
                            return new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByActivity:
                            return new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
                        case IndexFilter.ByOperator:
                            return new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new LinearGradientBrush(Colors.Transparent, Colors.Snow, _startColorPoint,_endColorPoint);
        }
        private IndexBarInfo GetButtonVm(IndexFilter filter, IndexBarInfo currentInfo)
        {

            switch (filter)
            {
                case IndexFilter.ByProduct:
                    currentInfo.Color = new LinearGradientBrush(_green, _greenLight, _startColorPoint, _endColorPoint);
                    currentInfo.Text = "Products";
                    currentInfo.Filter |= IndexFilter.ByProduct;
                    break;
                case IndexFilter.ByStation:
                    currentInfo.Color = new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
                    currentInfo.Text = "Stations";
                    currentInfo.Filter |= IndexFilter.ByStation;
                    break;
                case IndexFilter.ByActivity:
                    currentInfo.Color = new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
                    currentInfo.Text = "Activities";
                    currentInfo.Filter |= IndexFilter.ByActivity;
                    break;
                case IndexFilter.ByOperator:
                    currentInfo.Color = new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
                    currentInfo.Text = "Operators";
                    currentInfo.Filter |= IndexFilter.ByOperator;
                    break;
            }
            return currentInfo;
        }
    }
}
