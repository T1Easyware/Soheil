using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Reports
{
	public class CostBarVm : ViewModelBase, IReportBarVm
	{
	    public static readonly DependencyProperty IdProperty =
	        DependencyProperty.Register("Id", typeof (CostBarInfo), typeof (CostBarVm), new PropertyMetadata(default(CostBarInfo)));

	    public CostBarInfo Info
	    {
	        get { return (CostBarInfo) GetValue(IdProperty); }
	        set { SetValue(IdProperty, value); }
	    }

	    public static readonly DependencyProperty ValueProperty =
	        DependencyProperty.Register("Value", typeof (double), typeof (CostBarVm), new PropertyMetadata(default(double)));

	    public double Value
	    {
	        get { return (double) GetValue(ValueProperty); }
	        set { SetValue(ValueProperty, value); }
	    }

	    public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register("Tip", typeof(string), typeof(CostBarVm), new PropertyMetadata(default(string)));

	    public string Tip
	    {
	        get { return (string) GetValue(TipProperty); }
	        set { SetValue(TipProperty, value); }
	    }

	    public static readonly DependencyProperty HeaderProperty =
	        DependencyProperty.Register("Header", typeof (string), typeof (CostBarVm), new PropertyMetadata(default(string)));

	    public string Header
	    {
	        get { return (string) GetValue(HeaderProperty); }
	        set { SetValue(HeaderProperty, value); }
	    }

	    public static readonly DependencyProperty HeightProperty =
	        DependencyProperty.Register("Height", typeof (double), typeof (CostBarVm), new PropertyMetadata(default(double)));

	    public double Height
	    {
	        get { return (double) GetValue(HeightProperty); }
	        set { SetValue(HeightProperty, value); }
	    }

	    public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("MainColor", typeof(LinearGradientBrush), typeof(CostBarVm), new PropertyMetadata(default(LinearGradientBrush)));

        public LinearGradientBrush MainColor
	    {
            get { return (LinearGradientBrush)GetValue(ColorProperty); }
	        set { SetValue(ColorProperty, value); DetailsColor = new SolidColorBrush(value.GradientStops[0].Color);}
	    }

	    public static readonly DependencyProperty DetailsColorProperty =
            DependencyProperty.Register("DetailsColor", typeof(SolidColorBrush), typeof(CostBarVm), new PropertyMetadata(default(SolidColorBrush)));

	    public SolidColorBrush DetailsColor
	    {
            get { return (SolidColorBrush)GetValue(DetailsColorProperty); }
	        set { SetValue(DetailsColorProperty, value); }
	    }

	    public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<CostBarInfo>), typeof(CostBarVm), new PropertyMetadata(default(ObservableCollection<CostBarInfo>)));

        public ObservableCollection<CostBarInfo> MenuItems
	    {
            get { return (ObservableCollection<CostBarInfo>)GetValue(MenuItemsProperty); }
	        set { SetValue(MenuItemsProperty, value); }
	    }

	    public CostBarVm()
	    {
            MenuItems = new ObservableCollection<CostBarInfo>();
	    }

	}
}
