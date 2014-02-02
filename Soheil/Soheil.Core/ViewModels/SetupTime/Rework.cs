using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class Rework : Cell
	{
		public Rework(Product parentVm, Model.ProductRework model, bool isRowHeader)
		{
			ProductReworkId = model.Id;

			Name = model.Name;
			Code = model.Code;
			Product = parentVm;
			Checkbox = new CheckboxCell(this);
			Warmup = new WarmupCell(parentVm.ProductGroup.Station.Id, this, model.Warmups.First());
			IsValid = true;
			_isRowHeader = isRowHeader;
			if (model.Rework != null)
			{
				CellType = isRowHeader ? CellType.ReworkRowHeaderCell : CellType.ReworkColumnHeaderCell;
				ReworkId = model.Rework.Id;
				IsRework = true;
			}
			else
			{
				CellType = isRowHeader ? CellType.ProductRowHeaderCell : CellType.ProductColumnHeaderCell;
				ReworkId = -1;
				IsRework = false;
			}
			CopyCommand = new Commands.Command(o => _clipboard = this, () => { return IsValid || _isRowHeader; });
			PasteCommand = new Commands.Command(o =>
			{
				if (_clipboard == null) return;

				if (_isRowHeader)
				{
					//save warmup
					Warmup.Save(_clipboard.Warmup.Seconds, involveCheckbox: false);

					//load changeovers
					var copiedList = _clipboard.Product.ProductGroup.Station.ChangeoverCells.OfType<ChangeoverCell>()
						.Where(x => x.Row.ProductReworkId == _clipboard.ProductReworkId);
					var targetList = Product.ProductGroup.Station.ChangeoverCells.OfType<ChangeoverCell>()
						.Where(x => x.Row.ProductReworkId == ProductReworkId);

					//save changeovers
					foreach (var copiedC in copiedList)
					{
						var cell = targetList.FirstOrDefault(x => x.Column.ProductReworkId == copiedC.Column.ProductReworkId);
						if (cell != null)
							cell.Save(copiedC.Seconds, false);
					}
				}
				else
				{
					//load changeovers
					var copiedList = _clipboard.Product.ProductGroup.Station.ChangeoverCells.OfType<ChangeoverCell>()
						.Where(x => x.Column.ProductReworkId == _clipboard.ProductReworkId);
					var targetList = Product.ProductGroup.Station.ChangeoverCells.OfType<ChangeoverCell>()
						.Where(x => x.Column.ProductReworkId == ProductReworkId);

					//save changeovers
					foreach (var copiedC in copiedList)
					{
						var cell = targetList.FirstOrDefault(x => x.Row.ProductReworkId == copiedC.Row.ProductReworkId);
						if (cell != null)
							cell.Save(copiedC.Seconds, false);
					}
				}
			},
			() =>
			{
				if (_clipboard == null) return false;
				return _clipboard._isRowHeader == _isRowHeader;
			});
		}
		private static Rework _clipboard = null;
		private bool _isRowHeader = false;

		private Rework() { }
		/// <summary>
		/// this means this remork works differently (as a grouping tool)
		/// </summary>
		/// <param name="product"></param>
		/// <param name="isRowHeader"></param>
		/// <returns></returns>
		public static Rework InvalidMainProduct(Product product, bool isRowHeader)
		{
			Rework rework = new Rework();
			rework.ProductReworkId = new DataServices.ProductReworkDataService().GetMainForProduct(product.Id).Id;
			rework.ReworkId = -1;
			rework.CellType = isRowHeader ? CellType.ProductRowHeaderCell : CellType.ProductColumnHeaderCell;
			rework.IsRework = false;
			rework.Name = product.Name;
			rework.Code = product.Code;
			rework.Product = product;
			if (isRowHeader)
			{
				rework.Checkbox = new CheckboxCell(rework);
				rework.Warmup = new WarmupCell(rework.Product.ProductGroup.Station.Id, rework, null);
			}
			rework._isRowHeader = isRowHeader;
			rework.IsValid = false;
			rework.IsDurationsVisible = false;
			return rework;
		}
		public int ProductReworkId { get; set; }
		public int ReworkId { get; set; }
		public bool IsValid { get; set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(Rework), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(Rework), new UIPropertyMetadata(null));
		//Product Dependency Property
		public Product Product
		{
			get { return (Product)GetValue(ProductProperty); }
			set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(Product), typeof(Rework), new UIPropertyMetadata(null));
		//IsRework Dependency Property
		public bool IsRework
		{
			get { return (bool)GetValue(IsReworkProperty); }
			set { SetValue(IsReworkProperty, value); }
		}
		public static readonly DependencyProperty IsReworkProperty =
			DependencyProperty.Register("IsRework", typeof(bool), typeof(Rework), new UIPropertyMetadata(false));
		//Checkbox Dependency Property
		public CheckboxCell Checkbox
		{
			get { return (CheckboxCell)GetValue(CheckboxProperty); }
			set { SetValue(CheckboxProperty, value); }
		}
		public static readonly DependencyProperty CheckboxProperty =
			DependencyProperty.Register("Checkbox", typeof(CheckboxCell), typeof(Rework), new UIPropertyMetadata(null));
		//Warmup Dependency Property
		public WarmupCell Warmup
		{
			get { return (WarmupCell)GetValue(WarmupProperty); }
			set { SetValue(WarmupProperty, value); }
		}
		public static readonly DependencyProperty WarmupProperty =
			DependencyProperty.Register("Warmup", typeof(WarmupCell), typeof(Rework), new UIPropertyMetadata(null));
		//IsDurationsVisible Dependency Property
		public bool IsDurationsVisible
		{
			get { return (bool)GetValue(IsDurationsVisibleProperty); }
			set { SetValue(IsDurationsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsDurationsVisibleProperty =
			DependencyProperty.Register("IsDurationsVisible", typeof(bool), typeof(Rework), new UIPropertyMetadata(true));

		//CopyCommand Dependency Property
		public Commands.Command CopyCommand
		{
			get { return (Commands.Command)GetValue(CopyCommandProperty); }
			set { SetValue(CopyCommandProperty, value); }
		}
		public static readonly DependencyProperty CopyCommandProperty =
			DependencyProperty.Register("CopyCommand", typeof(Commands.Command), typeof(ProductGroup), new PropertyMetadata(null));
		//PasteCommand Dependency Property
		public Commands.Command PasteCommand
		{
			get { return (Commands.Command)GetValue(PasteCommandProperty); }
			set { SetValue(PasteCommandProperty, value); }
		}
		public static readonly DependencyProperty PasteCommandProperty =
			DependencyProperty.Register("PasteCommand", typeof(Commands.Command), typeof(ProductGroup), new PropertyMetadata(null));

	}
}
