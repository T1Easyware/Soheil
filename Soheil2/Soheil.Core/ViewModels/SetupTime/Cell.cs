using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class Cell : DependencyObject
	{
		public static Cell Blank(Cell row, Cell column, SolidColorBrush rowColor = null, SolidColorBrush columnColor = null)
		{
			return new Cell
			{
				RowIndex = row.RowIndex,
				ColumnIndex = column.ColumnIndex,
				CrossColors = new CrossColors(rowColor, columnColor),
				CellType = CellType.None
			};
		}
		//RowIndex Dependency Property
		public int RowIndex
		{
			get { return (int)GetValue(RowIndexProperty); }
			set { SetValue(RowIndexProperty, value); }
		}
		public static readonly DependencyProperty RowIndexProperty =
			DependencyProperty.Register("RowIndex", typeof(int), typeof(Cell), new UIPropertyMetadata(0));
		//ColumnIndex Dependency Property
		public int ColumnIndex
		{
			get { return (int)GetValue(ColumnIndexProperty); }
			set { SetValue(ColumnIndexProperty, value); }
		}
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyProperty.Register("ColumnIndex", typeof(int), typeof(Cell), new UIPropertyMetadata(0));
		//CellType Dependency Property
		public CellType CellType
		{
			get { return (CellType)GetValue(CellTypeProperty); }
			set { SetValue(CellTypeProperty, value); }
		}
		public static readonly DependencyProperty CellTypeProperty =
			DependencyProperty.Register("CellType", typeof(CellType), typeof(Cell), new UIPropertyMetadata(CellType.TextCell));
		//CrossColors Dependency Property
		public CrossColors CrossColors
		{
			get { return (CrossColors)GetValue(CrossColorsProperty); }
			set { SetValue(CrossColorsProperty, value); }
		}
		public static readonly DependencyProperty CrossColorsProperty =
			DependencyProperty.Register("CrossColors", typeof(CrossColors), typeof(Cell), new UIPropertyMetadata(null));
	}
}
