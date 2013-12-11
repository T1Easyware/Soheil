using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class WarmupCell : DurationCell
	{
		public WarmupCell(int stationId, Rework parentVm, Model.Warmup model)
		{
			CellType = CellType.WarmupCell;
			Row = parentVm;
			Model = model;
			_stationId = stationId;
		}
		int _stationId;
		private Model.Warmup _model;
		public Model.Warmup Model
		{
			get { return _model; }
			set
			{
				_model = value;
				if (value != null) DurationText = value.Seconds.ToString();
				else DurationText = "";
			}
		}
		public override void Save(int seconds, bool involveCheckbox = true)
		{
			//apply main product for all checked rows
			if (!Row.IsRework && involveCheckbox)
				foreach (var reworkRow in Row.Product.Reworks.Where(x => x.IsRework && x.Checkbox.IsChecked))
					reworkRow.Warmup.Model = new DataServices.WarmupDataService().SmartApply(_stationId, reworkRow.ProductReworkId, seconds);
			//if it's in a valid row then save the duration data for it
			if (Model != null)
				Model = new DataServices.WarmupDataService().Save(Model, seconds);
		}
	}
}
