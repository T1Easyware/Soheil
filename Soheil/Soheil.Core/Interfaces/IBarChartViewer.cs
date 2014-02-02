using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IBarChartViewer
    {
        ObservableCollection<int> Scales { get; set; }
        ObservableCollection<int> ScaleLines { get; set; }
        double ScaleHeight { get; set; }

        void MoveCenterBy(double offset, double scrollableRange);
        double CenterPoint { get; set; }
        double BarWidth { get; set; }

        Command InitializeProviderCommand { get; set; }
        Command NavigateInsideCommand { get; set; }
        Command NavigateBackCommand { get; set; }
    }
}
