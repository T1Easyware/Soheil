using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Core.PP.PricingAI;
using Soheil.Core.Commands;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class ProductPeriodVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance from the given ProductPeriod
		/// </summary>
		public ProductPeriodVm(ProductPeriod pp, string periodName, ProductVm product)
		{
			_product = product;
			MaxProduction = pp.MaxProduction;
			PeriodIndex = pp.PeriodIndex;
			Name = periodName;
			foreach (var price in pp.Prices)
			{
				Prices.Add(new PriceVm(price, this));
			}
			initializeCommand();
		}

		/// <summary>
		/// Creates a default instance with 1 Price in it
		/// </summary>
		/// <param name="period"></param>
		public ProductPeriodVm(PeriodVm period, ProductVm product)
		{
			_product = product;
			PeriodIndex = period.Index;
			Name = period.Name;
			MaxProduction = 0;
			Prices.Add(new PriceVm(new Price(), this));
			initializeCommand();
		}

		ProductVm _product;
		public int PeriodIndex { get; set; }
		/// <summary>
		/// Period Title
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a bindable collection that indicates Prices
		/// </summary>
		public ObservableCollection<PriceVm> Prices { get { return _prices; } }
		private ObservableCollection<PriceVm> _prices = new ObservableCollection<PriceVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates MaxProduction
		/// </summary>
		public int MaxProduction
		{
			get { return (int)GetValue(MaxProductionProperty); }
			set { SetValue(MaxProductionProperty, value); }
		}
		public static readonly DependencyProperty MaxProductionProperty =
			DependencyProperty.Register("MaxProduction", typeof(int), typeof(ProductPeriodVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = (ProductPeriodVm)d;
				var val = (int)e.NewValue;
				if (val != 0)
					vm._product.IsActive = true;
				else if (vm._product.Periods.All(x => x.MaxProduction == 0))
					vm._product.IsActive = false;
			}));

		#region Command
		void initializeCommand()
		{
			AddNewCommand = new Command(t =>
			{
				var newPriceVm = new PriceVm(new Price(), this);
				var priceVm = t as PriceVm;
				if (priceVm == null)
					//pp
					Prices.Add(newPriceVm);
				else
				{
					//price
					int index = Prices.IndexOf(priceVm);
					if (index == -1) index = 0;
					Prices.Insert(index, newPriceVm);
				}
			});

			CopyCommand = new Command(t =>
			{
				var priceVm = t as PriceVm;
				if (priceVm == null)
					//pp
					ClipboardHelper.SetData(new ProductPeriod(this));
				else
					//price
					ClipboardHelper.SetData(new Price(priceVm));
			});
			
			AppendCommand = new Command(t =>
			{
				var prices = ClipboardHelper.GetPrices();
				if (prices == null) MessageBox.Show("Copy a price or a period first.");
				else
				{
					//pastable
					var priceVm = t as PriceVm;
					if (priceVm == null)
					{
						//pp
						foreach (var price in prices)
							Prices.Add(new PriceVm(price, this));
					}
					else
					{
						//price
						int index = Prices.IndexOf(priceVm);
						if (index == -1) index = 0;
						for (int i = 0; i < prices.Count; i++)
						{
							Prices.Insert(index + i, new PriceVm(prices[i], this));
						}
					}
				}
			}, () => ClipboardHelper.HasPrice());
			
			OverwriteCommand = new Command(t =>
			{
				var prices = ClipboardHelper.GetPrices();
				if (prices == null) MessageBox.Show("Copy a price or a period first.");
				else
				{
					//pastable
					var priceVm = t as PriceVm;
					if (priceVm == null)
					{
						//pp
						Prices.Clear();
						foreach (var price in prices)
							Prices.Add(new PriceVm(price, this));
					}
					else
					{
						//price
						int index = Prices.IndexOf(priceVm);
						if (index == -1) index = 0;
						Prices.Remove(priceVm);
						for (int i = 0; i < prices.Count; i++)
						{
							Prices.Insert(index + i, new PriceVm(prices[i], this));
						}
					}
				}
			}, () => ClipboardHelper.HasPrice());

			DeleteCommand = new Command(t => 
			{
				var priceVm = t as PriceVm;
				if (priceVm == null)
					Prices.Clear();
				else
					Prices.Remove(priceVm);
			});
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates AddNewCommand
		/// </summary>
		public Command AddNewCommand
		{
			get { return (Command)GetValue(AddNewCommandProperty); }
			set { SetValue(AddNewCommandProperty, value); }
		}
		public static readonly DependencyProperty AddNewCommandProperty =
			DependencyProperty.Register("AddNewCommand", typeof(Command), typeof(ProductPeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates CopyCommand
		/// </summary>
		public Command CopyCommand
		{
			get { return (Command)GetValue(CopyCommandProperty); }
			set { SetValue(CopyCommandProperty, value); }
		}
		public static readonly DependencyProperty CopyCommandProperty =
			DependencyProperty.Register("CopyCommand", typeof(Command), typeof(ProductPeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates AppendCommand
		/// </summary>
		public Command AppendCommand
		{
			get { return (Command)GetValue(AppendCommandProperty); }
			set { SetValue(AppendCommandProperty, value); }
		}
		public static readonly DependencyProperty AppendCommandProperty =
			DependencyProperty.Register("AppendCommand", typeof(Command), typeof(ProductPeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates OverwriteCommand
		/// </summary>
		public Command OverwriteCommand
		{
			get { return (Command)GetValue(OverwriteCommandProperty); }
			set { SetValue(OverwriteCommandProperty, value); }
		}
		public static readonly DependencyProperty OverwriteCommandProperty =
			DependencyProperty.Register("OverwriteCommand", typeof(Command), typeof(ProductPeriodVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Command DeleteCommand
		{
			get { return (Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Command), typeof(ProductPeriodVm), new PropertyMetadata(null));

		#endregion


		internal void MoveUp(PriceVm priceVm)
		{
			if (priceVm != null)
			{
				int index = Prices.IndexOf(priceVm);
				if (index < 1) return;
				Prices.Move(index, index - 1);
			}
		}

		internal void MoveDown(PriceVm priceVm)
		{
			if (priceVm != null)
			{
				int index = Prices.IndexOf(priceVm);
				if (index > Prices.Count - 2) return;
				Prices.Move(index, index + 1);
			}
		}

		internal bool CanMoveUp(PriceVm priceVm)
		{
			return Prices.IndexOf(priceVm) > 0;
		}

		internal bool CanMoveDown(PriceVm priceVm)
		{
			return Prices.IndexOf(priceVm) < Prices.Count - 1;
		}
	}
}
