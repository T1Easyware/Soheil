using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Soheil.Views.OrganizationCalendar
{
	/// <summary>
	/// Interaction logic for ShiftBoundary.xaml
	/// </summary>
	public partial class ShiftBoundary : UserControl
	{
		public ShiftBoundary()
		{
			InitializeComponent(); 
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates whether this ShiftBoundary is start of a shift
		/// </summary>
		public bool IsStart
		{
			get { return (bool)GetValue(IsStartProperty); }
			set { SetValue(IsStartProperty, value); }
		}
		public static readonly DependencyProperty IsStartProperty =
			DependencyProperty.Register("IsStart", typeof(bool), typeof(ShiftBoundary), new UIPropertyMetadata(true));
	}
}
