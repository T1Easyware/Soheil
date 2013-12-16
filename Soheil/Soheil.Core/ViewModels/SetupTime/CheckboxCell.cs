using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class CheckboxCell : Cell
	{
		public CheckboxCell(Rework parentVm)
		{
			CellType = SetupTime.CellType.CheckBoxCell;
			Row = parentVm;
		}

		//IsChecked Dependency Property
		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckboxCell),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (CheckboxCell)d;
				var val = (bool)e.NewValue;
				if (vm.Row.IsRework)
				{
					//if row is a rework : checked = invisible
					vm.Row.IsDurationsVisible = !val;
					//Save rework data according to mainProductRework
					var mainPR = vm.Row.Product.Reworks.FirstOrDefault(x => !x.IsRework);
					if(mainPR!=null) vm.Row.Warmup.Save(mainPR.Warmup.Seconds);
					var allChangeovers = vm.Row.Product.ProductGroup.Station.ChangeoverCells.OfType<ChangeoverCell>();
					foreach (ChangeoverCell reworkChangeover in allChangeovers.Where(
						x => ((ChangeoverCell)x).Row.ProductReworkId == vm.Row.ProductReworkId))
					{
						var mainChangeover = allChangeovers.FirstOrDefault(
							x => ((ChangeoverCell)x).Row.ProductReworkId == mainPR.ProductReworkId
							&& ((ChangeoverCell)x).Column.ProductReworkId == reworkChangeover.Column.ProductReworkId);
						if (mainChangeover is ChangeoverCell)
							reworkChangeover.Save((mainChangeover as ChangeoverCell).Seconds);
					}
				}
				else if (!vm.Row.IsRework)
				{
					//if row is main product but it's just a header (not present in fpc) : checked = visible
					if (!vm.Row.IsValid)
						vm.Row.IsDurationsVisible = val;
					//change other checkboxes
					foreach (var rework in vm.Row.Product.Reworks.Where(x => x.IsRework))
					{
						rework.Checkbox.IsChecked = val;
					}
				}
			}));
		//Row Dependency Property
		public Rework Row
		{
			get { return (Rework)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public static readonly DependencyProperty RowProperty =
			DependencyProperty.Register("Row", typeof(Rework), typeof(CheckboxCell), new UIPropertyMetadata(null));
	}
}
