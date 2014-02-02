using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class ChangeoverCell : DurationCell
	{
		public ChangeoverCell(Rework row, Rework column, int stationId, Dal.SoheilEdmContext context)
		{
			Row = row;
			Column = column;
			RowIndex = row.RowIndex;
			ColumnIndex = column.ColumnIndex;
			_stationId = stationId;

			CrossColors = new CrossColors(row.Product.Color, column.Product.Color);
			if (Row.IsValid)
			{
				var ds = new DataServices.ChangeoverDataService();
				int fromPRId = row.ProductReworkId;
				int toPRId = column.ProductReworkId;
				//Model = await new Task<Model.Changeover>(()=>ds.GetByInfoOrAdd(stationId, fromPRId, toPRId, context));
				Model = ds.GetByInfoOrAdd(stationId, fromPRId, toPRId, context);
			}

			if (row.Product.Id == column.Product.Id)
				CellType = CellType.None;
			else
				CellType = CellType.ChangeoverCell;
		}
		int _stationId;
		//Column Dependency Property
		public Rework Column
		{
			get { return (Rework)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
		public static readonly DependencyProperty ColumnProperty =
			DependencyProperty.Register("Column", typeof(Rework), typeof(ChangeoverCell), new UIPropertyMetadata(null));

		private Model.Changeover _model;
		public Model.Changeover Model { get { return _model; } set { _model = value; if (value != null) DurationText = value.Seconds.ToString(); } }
		public override void Save(int seconds, bool involveCheckbox = true)
		{
			//apply main product for all checked rows
			if (!Row.IsRework && involveCheckbox)
				foreach (var reworkRow in Row.Product.Reworks.Where(x => x.IsRework && x.Checkbox.IsChecked))
					((ChangeoverCell)(reworkRow.Product.ProductGroup.Station.ChangeoverCells.FirstOrDefault(
						x => x.ColumnIndex == ColumnIndex && x.RowIndex == reworkRow.RowIndex))).Model 
							= new DataServices.ChangeoverDataService().SmartApply(_stationId, reworkRow.ProductReworkId, Column.ProductReworkId, seconds);
			//if it's in a valid row then save the duration data for it
			if (Model != null)
				Model = new DataServices.ChangeoverDataService().Save(Model, seconds);
		}
	}
}
