using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections;
using Soheil.Core.PP.PricingAI;
using Soheil.Core.Commands;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class ProductVm : DependencyObject
	{
		//public event Action<bool> ExclusionChanged;
		public ProductVm(Product data, IEnumerable<PeriodVm> periods)
		{
			_periods = periods;
			Id = data.Id;
			//MaxProductions.CollectionChanged += MaxProductions_CollectionChanged;

			Name = data.Name;//Recheck with DATABASE
			ProductGroupName = data.GroupName;

			//basic props
			FinishedCost = data.FinishedCost;
			InventoryCost = data.InventoryCost;
			LostSaleCost = data.LostSaleCost;
			Inventory = data.Inventory;
			SpaceCoef = data.SpaceCoef;

			//max productions
			foreach (var period in data.Periods)
			{
				var periodVm = _periods.FirstOrDefault(x => x.Index == period.PeriodIndex);
				Periods.Add(new ProductPeriodVm(period, periodVm.Name, this));
			}
			//pricing
			SetPricings(data);

			initializeCommands();
		}

		/*void MaxProductions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				if (ExclusionChanged != null)
					ExclusionChanged(MaxProductions.All(x => x == 0));
			}
		}*/

		IEnumerable<PeriodVm> _periods;

		/// <summary>
		/// Sets/Resets Prices according to the values in data.Periods
		/// </summary>
		/// <param name="data">Product data containing periods containing prices (set null to reset)</param>
		/// <param name="append">Default is false</param>
		void SetPricings(Product data, bool append = false)
		{
			//delete old Prices
			if(!append)
			{
				foreach (var period in Periods)
				{
					period.Prices.Clear();
				}
			}
			if (data == null)
			{
				//reset
				foreach (var period in _periods)
				{
					Periods.Add(new ProductPeriodVm(period, this));
				}
			}
			else
			{
				//add new Prices
				foreach (var period in data.Periods)
				{
					var pperiodVm = Periods.FirstOrDefault(x => x.PeriodIndex == period.PeriodIndex);
					foreach (var price in period.Prices)
					{
						pperiodVm.Prices.Add(new PriceVm(price, pperiodVm));
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets a bindable collection that indicates Periods
		/// </summary>
		public ObservableCollection<ProductPeriodVm> Periods { get { return _pperiods; } }
		private ObservableCollection<ProductPeriodVm> _pperiods = new ObservableCollection<ProductPeriodVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates IsActive
		/// </summary>
		public bool IsActive
		{
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.Register("IsActive", typeof(bool), typeof(ProductVm), new PropertyMetadata(false));

		#region Props
		public int Id { get; protected set; }

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates ProductGroupName
		/// </summary>
		public string ProductGroupName
		{
			get { return (string)GetValue(ProductGroupNameProperty); }
			set { SetValue(ProductGroupNameProperty, value); }
		}
		public static readonly DependencyProperty ProductGroupNameProperty =
			DependencyProperty.Register("ProductGroupName", typeof(string), typeof(ProductVm), new PropertyMetadata("-"));
		/// <summary>
		/// Gets or sets a bindable value that indicates FinishedCost
		/// </summary>
		public int FinishedCost
		{
			get { return (int)GetValue(FinishedCostProperty); }
			set { SetValue(FinishedCostProperty, value); }
		}
		public static readonly DependencyProperty FinishedCostProperty =
			DependencyProperty.Register("FinishedCost", typeof(int), typeof(ProductVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates InventoryCost
		/// </summary>
		public int InventoryCost
		{
			get { return (int)GetValue(InventoryCostProperty); }
			set { SetValue(InventoryCostProperty, value); }
		}
		public static readonly DependencyProperty InventoryCostProperty =
			DependencyProperty.Register("InventoryCost", typeof(int), typeof(ProductVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates LostSaleCost
		/// </summary>
		public int LostSaleCost
		{
			get { return (int)GetValue(LostSaleCostProperty); }
			set { SetValue(LostSaleCostProperty, value); }
		}
		public static readonly DependencyProperty LostSaleCostProperty =
			DependencyProperty.Register("LostSaleCost", typeof(int), typeof(ProductVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates Inventory
		/// </summary>
		public int Inventory
		{
			get { return (int)GetValue(InventoryProperty); }
			set { SetValue(InventoryProperty, value); }
		}
		public static readonly DependencyProperty InventoryProperty =
			DependencyProperty.Register("Inventory", typeof(int), typeof(ProductVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates SpaceCoef
		/// </summary>
		public double SpaceCoef
		{
			get { return (double)GetValue(SpaceCoefProperty); }
			set { SetValue(SpaceCoefProperty, value); }
		}
		public static readonly DependencyProperty SpaceCoefProperty =
			DependencyProperty.Register("SpaceCoef", typeof(double), typeof(ProductVm), new PropertyMetadata(0d));

		#endregion

		#region Commands
		void initializeCommands()
		{
			CopyCommand = new Command(o => ClipboardHelper.SetData(new Product(this)));
			
			PasteCommand = new Command(o =>
			{
				var clip = ClipboardHelper.GetProduct();
				if (clip == null) MessageBox.Show("Copy a product first.");
				else
				{
					FinishedCost = clip.FinishedCost;
					Inventory = clip.Inventory;
					InventoryCost = clip.InventoryCost;
					LostSaleCost = clip.LostSaleCost;
					SpaceCoef = clip.SpaceCoef;
					foreach (var period in clip.Periods)
					{
						var periodVm = Periods.FirstOrDefault(x => x.PeriodIndex == period.PeriodIndex);
						periodVm.MaxProduction = period.MaxProduction;
					}
				}
			}, () => Clipboard.ContainsData(ClipboardHelper.Product));
			
			AppendPricingCommand = new Command(o =>
			{
				var clip = ClipboardHelper.GetProduct();
				if (clip == null) MessageBox.Show("Copy a product first.");
				else SetPricings(clip, true);
			}, () => Clipboard.ContainsData(ClipboardHelper.Product));

			OverwritePricingCommand = new Command(o =>
			{
				var clip = ClipboardHelper.GetProduct();
				if (clip == null) MessageBox.Show("Copy a product first.");
				else SetPricings(clip);
			}, () => Clipboard.ContainsData(ClipboardHelper.Product));
			
			DeleteCommand = new Command(o => SetPricings(null));

			ExcludeCommand = new Command(o => IsActive = false);
		}

		/// <summary>
		/// Gets or sets a bindable value that indicates CopyCommand
		/// </summary>
		public Command CopyCommand
		{
			get { return (Command)GetValue(CopyCommandProperty); }
			set { SetValue(CopyCommandProperty, value); }
		}
		public static readonly DependencyProperty CopyCommandProperty =
			DependencyProperty.Register("CopyCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates AppendPricingCommand
		/// </summary>
		public Command AppendPricingCommand
		{
			get { return (Command)GetValue(AppendPricingCommandProperty); }
			set { SetValue(AppendPricingCommandProperty, value); }
		}
		public static readonly DependencyProperty AppendPricingCommandProperty =
			DependencyProperty.Register("AppendPricingCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates OverwritePricingCommand
		/// </summary>
		public Command OverwritePricingCommand
		{
			get { return (Command)GetValue(OverwritePricingCommandProperty); }
			set { SetValue(OverwritePricingCommandProperty, value); }
		}
		public static readonly DependencyProperty OverwritePricingCommandProperty =
			DependencyProperty.Register("OverwritePricingCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates PasteCommand
		/// </summary>
		public Command PasteCommand
		{
			get { return (Command)GetValue(PasteCommandProperty); }
			set { SetValue(PasteCommandProperty, value); }
		}
		public static readonly DependencyProperty PasteCommandProperty =
			DependencyProperty.Register("PasteCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Command DeleteCommand
		{
			get { return (Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates ExcludeCommand
		/// </summary>
		public Command ExcludeCommand
		{
			get { return (Command)GetValue(ExcludeCommandProperty); }
			set { SetValue(ExcludeCommandProperty, value); }
		}
		public static readonly DependencyProperty ExcludeCommandProperty =
			DependencyProperty.Register("ExcludeCommand", typeof(Command), typeof(ProductVm), new PropertyMetadata(null));

		#endregion

	}
}
