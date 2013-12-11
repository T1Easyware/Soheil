using System.Windows.Media;

namespace Soheil.Core.Interfaces
{
    public interface IReportBarVm
    {
        double Value { get; set; }
        string Tip { get; set; }
        string Header { get; set; }
        double Height { get; set; }
        LinearGradientBrush MainColor { get; set; }
        SolidColorBrush DetailsColor { get; set; }
    }
}
