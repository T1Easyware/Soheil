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
    public class CostBarProvider : IItemsProvider<CostBarVm>
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
        public CostType CostType { get; set; }
        public DateTimeIntervals DateTimeIntervals { get; set; }
		public Dispatcher Dispatcher { get; set; }
        public double Scale { get; set; }
        public double ScaleHeight { get; set; }
        public int MaxValue { get; set; }
        public int MaxScale { get; set; }
        public int StepScale { get; set; }
        public CostBarInfo BarInfo { get; set; }
        public CostReportDataService DataService { get; set; }

        private readonly Point _startColorPoint = new Point(0,0.5);
        private readonly Point _endColorPoint = new Point(1,0.5);

        public CostBarProvider(int count, double pageHeight, CostType costType, DateTimeIntervals dateTimeIntervals, CostReportDataService dataService, CostBarInfo barInfo)
        {
            _count = count;
            BarInfo = barInfo;
            CostType = costType;
            DateTimeIntervals = dateTimeIntervals;
            DataService = dataService;

            double max = dataService.GetMax(costType,dateTimeIntervals,barInfo,count);
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

        public IList<CostBarVm> FetchRange(int startCost, int count)
        {
            if (Dispatcher != null) Thread.Sleep(100);
            var bars = new List<CostBarVm>();
            IList<Record> records = DataService.GetInRange(CostType, DateTimeIntervals, BarInfo, startCost, startCost + count);

            for (int i = startCost; i < startCost + count; i++)
            {
                var currentInfo = new CostBarInfo
                    {
                        Id = records[i - startCost].Id,
                        StartDate = records[i - startCost].StartDate,
                        EndDate = records[i - startCost].EndDate,
                        Type = CostType,
                        Level = BarInfo.Level,
                        IsMenuItem = false
                    };
                //switch (BarInfo.SourceType)
                //{
                //    case CostSourceType.Other:
                //        currentInfo.IntervalId = records[i - startCost].Id;
                //        break;
                //    case CostSourceType.Stations:
                //        currentInfo.StationId = records[i - startCost].Id;
                //        break;
                //    case CostSourceType.Operators:
                //        currentInfo.OperatorId = records[i - startCost].Id;
                //        break;
                //    case CostSourceType.Machines:
                //        currentInfo.MachineId = records[i - startCost].Id;
                //        break;
                //}
                var bar = new CostBarVm
                {
                    Info = currentInfo,
                    Value = records[i - startCost].Value * Scale,
                    Tip = records[i - startCost].Value.ToString(CultureInfo.InvariantCulture),
                    Header = records[i - startCost].Header,
                    MainColor = GetColor(),
                    MenuItems = GetMenuItems(currentInfo)
                };
                bars.Add(bar);
            }
            return bars;
        }

        private ObservableCollection<CostBarInfo> GetMenuItems(CostBarInfo currentInfo)
        {
            var items = new ObservableCollection<CostBarInfo>();

            //if ((BarInfo.SourceType & CostSourceType.Stations) != CostSourceType.Stations)
            //            items.Add(GetButtonVm(CostSourceType.Stations, currentInfo));
            //if ((BarInfo.SourceType & CostSourceType.Machines) != CostSourceType.Machines)
            //            items.Add(GetButtonVm(CostSourceType.Machines, currentInfo));
            //if ((BarInfo.SourceType & CostSourceType.Operators) != CostSourceType.Operators)
            //            items.Add(GetButtonVm(CostSourceType.Operators, currentInfo));

            return items;
        }

        private LinearGradientBrush GetColor(int levelIncrement = 0)
        {
            var level = BarInfo.Level + levelIncrement;
            switch (CostType)
            {
                case CostType.All:
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
                case CostType.Stock:
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
                case CostType.Cash:
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
            }
            return new LinearGradientBrush(Colors.Transparent, Colors.Snow, _startColorPoint,_endColorPoint);
        }
        private CostBarInfo GetButtonVm(CostSourceType filter, CostBarInfo currentInfo)
        {

            //switch (filter)
            //{
            //    case CostSourceType.ByProduct:
            //        currentInfo.Color = new LinearGradientBrush(_green, _greenLight, _startColorPoint, _endColorPoint);
            //        currentInfo.Text = "Products";
            //        currentInfo.CostSourceType |= CostSourceType.ByProduct;
            //        break;
            //    case CostSourceType.ByStation:
            //        currentInfo.Color = new LinearGradientBrush(_night, _nightLight, _startColorPoint, _endColorPoint);
            //        currentInfo.Text = "Stations";
            //        currentInfo.CostSourceType |= CostSourceType.ByStation;
            //        break;
            //    case CostSourceType.ByActivity:
            //        currentInfo.Color = new LinearGradientBrush(_yellow, _yellowLight, _startColorPoint, _endColorPoint);
            //        currentInfo.Text = "Activities";
            //        currentInfo.CostSourceType |= CostSourceType.ByActivity;
            //        break;
            //    case CostSourceType.ByOperator:
            //        currentInfo.Color = new LinearGradientBrush(_crimson, _crimsonLight, _startColorPoint, _endColorPoint);
            //        currentInfo.Text = "Operators";
            //        currentInfo.CostSourceType |= CostSourceType.ByOperator;
            //        break;
            //}
            return currentInfo;
        }
    }
}
