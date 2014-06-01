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

        public static readonly DependencyProperty Value1Property =
    DependencyProperty.Register("Value1", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double Value1
        {
            get { return (double)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }

        public static readonly DependencyProperty Value2Property =
    DependencyProperty.Register("Value2", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double Value2
        {
            get { return (double)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }

        public static readonly DependencyProperty Value3Property =
    DependencyProperty.Register("Value3", typeof(double), typeof(OperatorBarVm), new PropertyMetadata(default(double)));

        public double Value3
        {
            get { return (double)GetValue(Value3Property); }
            set { SetValue(Value3Property, value); }
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

        public static readonly DependencyProperty Tip1Property =
    DependencyProperty.Register("Tip1", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string Tip1
        {
            get { return (string)GetValue(Tip1Property); }
            set { SetValue(Tip1Property, value); }
        }

        public static readonly DependencyProperty Tip2Property =
    DependencyProperty.Register("Tip2", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string Tip2
        {
            get { return (string)GetValue(Tip2Property); }
            set { SetValue(Tip2Property, value); }
        }

        public static readonly DependencyProperty Tip3Property =
    DependencyProperty.Register("Tip3", typeof(string), typeof(OperatorBarVm), new PropertyMetadata(default(string)));

        public string Tip3
        {
            get { return (string)GetValue(Tip3Property); }
            set { SetValue(Tip3Property, value); }
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
