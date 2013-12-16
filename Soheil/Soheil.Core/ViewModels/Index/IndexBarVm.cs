using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Soheil.Core.Base;
using Soheil.Core.Index;
using Soheil.Core.Interfaces;
using Soheil.Core.Reports;

namespace Soheil.Core.ViewModels.Index
{
	public class IndexBarVm : ViewModelBase, IReportBarVm
	{
	    public static readonly DependencyProperty IdProperty =
	        DependencyProperty.Register("Id", typeof (IndexBarInfo), typeof (IndexBarVm), new PropertyMetadata(default(IndexBarInfo)));

	    public IndexBarInfo Info
	    {
	        get { return (IndexBarInfo) GetValue(IdProperty); }
	        set { SetValue(IdProperty, value); }
	    }

	    public static readonly DependencyProperty ValueProperty =
	        DependencyProperty.Register("Value", typeof (double), typeof (IndexBarVm), new PropertyMetadata(default(double)));

	    public double Value
	    {
	        get { return (double) GetValue(ValueProperty); }
	        set { SetValue(ValueProperty, value); }
	    }

	    public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register("Tip", typeof(string), typeof(IndexBarVm), new PropertyMetadata(default(string)));

	    public string Tip
	    {
	        get { return (string) GetValue(TipProperty); }
	        set { SetValue(TipProperty, value); }
	    }

	    public static readonly DependencyProperty HeaderProperty =
	        DependencyProperty.Register("Header", typeof (string), typeof (IndexBarVm), new PropertyMetadata(default(string)));

	    public string Header
	    {
	        get { return (string) GetValue(HeaderProperty); }
	        set { SetValue(HeaderProperty, value); }
	    }

	    public static readonly DependencyProperty HeightProperty =
	        DependencyProperty.Register("Height", typeof (double), typeof (IndexBarVm), new PropertyMetadata(default(double)));

	    public double Height
	    {
	        get { return (double) GetValue(HeightProperty); }
	        set { SetValue(HeightProperty, value); }
	    }

	    public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("MainColor", typeof(LinearGradientBrush), typeof(IndexBarVm), new PropertyMetadata(default(LinearGradientBrush)));

        public LinearGradientBrush MainColor
	    {
            get { return (LinearGradientBrush)GetValue(ColorProperty); }
	        set { SetValue(ColorProperty, value); DetailsColor = new SolidColorBrush(value.GradientStops[0].Color);}
	    }

	    public static readonly DependencyProperty DetailsColorProperty =
            DependencyProperty.Register("DetailsColor", typeof(SolidColorBrush), typeof(IndexBarVm), new PropertyMetadata(default(SolidColorBrush)));

	    public SolidColorBrush DetailsColor
	    {
            get { return (SolidColorBrush)GetValue(DetailsColorProperty); }
	        set { SetValue(DetailsColorProperty, value); }
	    }

	    public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<IndexBarInfo>), typeof(IndexBarVm), new PropertyMetadata(default(ObservableCollection<IndexBarInfo>)));

        public ObservableCollection<IndexBarInfo> MenuItems
	    {
            get { return (ObservableCollection<IndexBarInfo>)GetValue(MenuItemsProperty); }
	        set { SetValue(MenuItemsProperty, value); }
	    }

	    public IndexBarVm()
	    {
            MenuItems = new ObservableCollection<IndexBarInfo>();
	    }

	}
}
