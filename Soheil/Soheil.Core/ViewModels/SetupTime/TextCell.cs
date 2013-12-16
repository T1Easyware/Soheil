using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class TextCell : Cell
	{
		public TextCell(string text)
		{
			CellType = CellType.TextCell;
			Text = text;
		}
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(TextCell), new UIPropertyMetadata(null));
	}
}
