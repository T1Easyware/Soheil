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
using Soheil.Common;

namespace Soheil.Views.PP
{
	/// <summary>
	/// Interaction logic for Block.xaml (one Block in PPTable with its Tasks and Buttons)
	/// </summary>
	public partial class Block : UserControl
	{
		public Block()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Set this member from the containing PPTable
		/// <para>This member is used to fetch some info from its parent</para>
		/// </summary>
		public PPTableVm PPTable
		{
			get { return (PPTableVm)GetValue(PPTableProperty); }
			set { SetValue(PPTableProperty, value); }
		}
		public static readonly DependencyProperty PPTableProperty =
			DependencyProperty.Register("PPTable", typeof(PPTableVm), typeof(Block), new UIPropertyMetadata(null));

		private void MouseEnters(object sender, MouseEventArgs e)
		{
			var block = sender.GetDataContext<BlockVm>();
			if(block!=null)
			{
				block.ShowTasks = true;
			}
		}
		private void MouseLeaves(object sender, MouseEventArgs e)
		{
			var block = sender.GetDataContext<BlockVm>();
			if(block!=null)
			{
				if (block.Parent.PPTable.SelectedBlock != block)
					block.ShowTasks = false;
			}
		}
	}
}
