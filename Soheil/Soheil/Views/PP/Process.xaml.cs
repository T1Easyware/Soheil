using Soheil.Core.ViewModels.PP;
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

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for Process.xaml (one Cell in PPTable with its insides)
	/// </summary>
	public partial class Process : UserControl
	{
		public Process()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets a reference to PPTable ViewModel
		/// <para>Set this member from the containing PPTable</para>
		/// <para>This member is used to fetch some info from its parent</para>
		/// </summary>
		public PPTableVm PPTable
		{
			get { return (PPTableVm)GetValue(PPTableProperty); }
			set { SetValue(PPTableProperty, value); }
		}
		public static readonly DependencyProperty PPTableProperty =
			DependencyProperty.Register("PPTable", typeof(PPTableVm), typeof(Process), new UIPropertyMetadata(null));

	}
}
