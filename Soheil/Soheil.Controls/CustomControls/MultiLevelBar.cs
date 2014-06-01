using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Soheil.Controls.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls;assembly=Soheil.Controls.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ImageButton/>
    ///
    /// </summary>
    public class MultiLevelBar : ToggleButton
    {
        static MultiLevelBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiLevelBar), new FrameworkPropertyMetadata(typeof(MultiLevelBar)));
        }
        public double Level1
        {
            get { return (double)GetValue(Level1Property); }
            set { SetValue(Level1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Level1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Level1Property =
            DependencyProperty.Register("Level1", typeof(double), typeof(MultiLevelBar), new PropertyMetadata(default(double)));

        public double Level2
        {
            get { return (double)GetValue(Level2Property); }
            set { SetValue(Level2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Level2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Level2Property =
            DependencyProperty.Register("Level2", typeof(double), typeof(MultiLevelBar), new PropertyMetadata(default(double)));

        public double Level3
        {
            get { return (double)GetValue(Level3Property); }
            set { SetValue(Level3Property, value); }
        }

        // Using a DependencyProperty as the backing store for Level3.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Level3Property =
            DependencyProperty.Register("Level3", typeof(double), typeof(MultiLevelBar), new PropertyMetadata(default(double)));

        public double Total
        {
            get { return (double)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Total.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(double), typeof(MultiLevelBar), new PropertyMetadata(default(double)));

        public SolidColorBrush BackColor1
        {
            get { return (SolidColorBrush)GetValue(BackColor1Property); }
            set { SetValue(BackColor1Property, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColor1Property =
            DependencyProperty.Register("BackColor1", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush BackColor2
        {
            get { return (SolidColorBrush)GetValue(BackColor2Property); }
            set { SetValue(BackColor2Property, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColor2Property =
            DependencyProperty.Register("BackColor2", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush BackColor3
        {
            get { return (SolidColorBrush)GetValue(BackColor3Property); }
            set { SetValue(BackColor3Property, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor3.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColor3Property =
            DependencyProperty.Register("BackColor3", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));


        public SolidColorBrush EffectColor1
        {
            get { return (SolidColorBrush)GetValue(EffectColor1Property); }
            set { SetValue(EffectColor1Property, value); }
        }

        // Using a DependencyProperty as the backing store for EffectColor1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EffectColor1Property =
            DependencyProperty.Register("EffectColor1", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush EffectColor2
        {
            get { return (SolidColorBrush)GetValue(EffectColor2Property); }
            set { SetValue(EffectColor2Property, value); }
        }

        // Using a DependencyProperty as the backing store for EffectColor2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EffectColor2Property =
            DependencyProperty.Register("EffectColor2", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush EffectColor3
        {
            get { return (SolidColorBrush)GetValue(EffectColor3Property); }
            set { SetValue(EffectColor3Property, value); }
        }

        // Using a DependencyProperty as the backing store for EffectColor3.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EffectColor3Property =
            DependencyProperty.Register("EffectColor3", typeof(SolidColorBrush), typeof(MultiLevelBar), new PropertyMetadata(default(SolidColorBrush)));
    }
}
