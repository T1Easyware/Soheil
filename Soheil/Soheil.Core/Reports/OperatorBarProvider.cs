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
			10,20,50,80,100,200,500,800,1000,1600,2000,2500,3600,5000,7200,8000,10000
		};
		public int GetScale(int max)
		{
			int divisionsBy10 = 0;
			while(_scales.Last() <= max)
			{
				max /= 10;
				divisionsBy10++;
			}
			var scale = _scales.FirstOrDefault(x => x >= max);
			return (int)Math.Pow(10, divisionsBy10) * scale;
		}
		public IList<OperatorBarInfo> FetchAll()
		{
			Thread.Sleep(100);
			var bars = new List<OperatorBarInfo>();
			IList<Record> records = DataService.GetAll(DateTimeIntervals, BarInfo);
			if (!records.Any()) return bars;

			if (BarInfo.IsCountBase)
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
                    Header = records[i - startOperator].Header,
                    MainColor = GetColor(),
                    MenuItems = GetMenuItems(currentInfo)
                };
                if (BarInfo.IsCountBase)
                {
                    bar.Value = Convert.ToDouble( records[i - startOperator].Data[4])*Scale;
                    bar.ProductionValue = Convert.ToDouble( records[i - startOperator].Data[5])*Scale;
                    bar.DefectionValue = Convert.ToDouble( records[i - startOperator].Data[6])*Scale;
                    bar.StoppageValue = Convert.ToDouble( records[i - startOperator].Data[7])*Scale;
                    bar.Tip = Convert.ToString(records[i - startOperator].Data[4]);
                    bar.ProductionTip = Convert.ToString(records[i - startOperator].Data[5]);
                    bar.DefectionTip = Convert.ToString(records[i - startOperator].Data[6]);
                    bar.StoppageTip = Convert.ToString(records[i - startOperator].Data[7]);
                }
                else
                {
                    bar.Value = Convert.ToDouble(records[i - startOperator].Data[0])*Scale;
                    bar.ProductionValue = Convert.ToDouble( records[i - startOperator].Data[1])*Scale;
                    bar.DefectionValue = Convert.ToDouble( records[i - startOperator].Data[2])*Scale;
                    bar.StoppageValue = Convert.ToDouble( records[i - startOperator].Data[3])*Scale;
                    bar.Tip = Format.ConvertToHM(Convert.ToInt32(records[i - startOperator].Data[0]));
                    bar.ProductionTip = Format.ConvertToHM(Convert.ToInt32(records[i - startOperator].Data[1]));
                    bar.DefectionTip = Format.ConvertToHM(Convert.ToInt32(records[i - startOperator].Data[2]));
                    bar.StoppageTip = Format.ConvertToHM(Convert.ToInt32(records[i - startOperator].Data[3]));
                }
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
