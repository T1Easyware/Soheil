using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Reports
{
	public class OperatorBarVm : ViewModelBase, IReportBarVm
	{
	    public static readonly DependencyProperty IdProperty =
	        DependencyProperty.Register("Id", typeof (OperatorBarInfo), typeof (OperatorBarVm), new PropertyMetadata(default(OperatorBarInfo)));

	    public OperatorBarInfo Info
	    {
	        get { return (OperatorBarInfo) GetValue(IdProperty); }
	        set { SetValue(IdProperty, value); }
	    }

	    public static readonly DependencyProperty ValueProperty =
	        DependencyProperty.Register("Value", typeof (double), typeof (OperatorBarVm), new PropertyMetadata(default(double)));

	    public double Value
	    {
	        get { return (double) GetValue(ValueProperty); }
	        set { SetValue(ValueProperty, value); }
	    }

        public static readonly DependencyProperty ProductionValueProperty =
    DependencyProperty.Register("ProductionValue", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double ProductionValue
        {
            get { return (double)GetValue(ProductionValueProperty); }
            set { SetValue(ProductionValueProperty, value); }
        }

        public static readonly DependencyProperty DefectionValueProperty =
    DependencyProperty.Register("DefectionValue", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double DefectionValue
        {
            get { return (double)GetValue(DefectionValueProperty); }
            set { SetValue(DefectionValueProperty, value); }
        }

        public static readonly DependencyProperty StoppageValueProperty =
    DependencyProperty.Register("StoppageValue", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double StoppageValue
        {
            get { return (double)GetValue(StoppageValueProperty); }
            set { SetValue(StoppageValueProperty, value); }
        }

	    public double RemainingValue
	    {
	        get { return Value - (ProductionValue + StoppageValue + DefectionValue); }
	    }


	    public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register("Tip", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

	    public string Tip
	    {
	        get { return (string) GetValue(TipProperty); }
	        set { SetValue(TipProperty, value); }
	    }

	    public static readonly DependencyProperty HeaderProperty =
	        DependencyProperty.Register("Header", typeof (string), typeof (OperatorBarVm), new PropertyMetadata(default(string)));

	    public string Header
	    {
	        get { return (string) GetValue(HeaderProperty); }
	        set { SetValue(HeaderProperty, value); }
	    }

        public static readonly DependencyProperty ProductionTipProperty =
    DependencyProperty.Register("ProductionTip", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string ProductionTip
        {
            get { return (string)GetValue(ProductionTipProperty); }
            set { SetValue(ProductionTipProperty, value); }
        }

        public static readonly DependencyProperty DefectionTipProperty =
    DependencyProperty.Register("DefectionTip", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string DefectionTip
        {
            get { return (string)GetValue(DefectionTipProperty); }
            set { SetValue(DefectionTipProperty, value); }
        }

        public static readonly DependencyProperty StoppageTipProperty =
    DependencyProperty.Register("StoppageTip", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string StoppageTip
        {
            get { return (string)GetValue(StoppageTipProperty); }
            set { SetValue(StoppageTipProperty, value); }
        }

	    public static readonly DependencyProperty HeightProperty =
	        DependencyProperty.Register("Height", typeof (double), typeof (OperatorBarVm), new PropertyMetadata(default(double)));



	    public double Height
	    {
	        get { return (double) GetValue(HeightProperty); }
	        set { SetValue(HeightProperty, value); }
	    }

	    public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("MainColor", typeof(LinearGradientBrush), typeof(OperatorBarVm), new PropertyMetadata(default(LinearGradientBrush)));

        public LinearGradientBrush MainColor
	    {
            get { return (LinearGradientBrush)GetValue(ColorProperty); }
	        set { SetValue(ColorProperty, value); DetailsColor = new SolidColorBrush(value.GradientStops[0].Color);}
	    }

	    public static readonly DependencyProperty DetailsColorProperty =
            DependencyProperty.Register("DetailsColor", typeof(SolidColorBrush), typeof(OperatorBarVm), new PropertyMetadata(default(SolidColorBrush)));

	    public SolidColorBrush DetailsColor
	    {
            get { return (SolidColorBrush)GetValue(DetailsColorProperty); }
	        set { SetValue(DetailsColorProperty, value); }
	    }

	    public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<OperatorBarInfo>), typeof(OperatorBarVm), new PropertyMetadata(default(ObservableCollection<OperatorBarInfo>)));

        public ObservableCollection<OperatorBarInfo> MenuItems
	    {
            get { return (ObservableCollection<OperatorBarInfo>)GetValue(MenuItemsProperty); }
	        set { SetValue(MenuItemsProperty, value); }
	    }

	    public OperatorBarVm()
	    {
            MenuItems = new ObservableCollection<OperatorBarInfo>();
	    }

	}
}
